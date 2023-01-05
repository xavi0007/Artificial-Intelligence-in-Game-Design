using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class State
{

    // 'States' that the enemy could be in.
    public enum STATE
    {
        IDLE, PATROL, PURSUE, ATTACK, TAKINGDAMAGE
    };

    // 'Events' - where we are in the running of a STATE.
    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    public STATE name;
    protected EVENT stage;
    protected GameObject enemy;
    protected Transform player;
    protected State nextState;
    protected NavMeshAgent agent; // To store the enemy NavMeshAgent component.


    float visDist = 100.0f; // When the player is within a distance of 10 from the enemy, then the enemy should be able to see it...
    float visAngle = 90.0f; // arc of fire 
    float shootDist = 90.0f; // When the player is within a distance of 7 from the enemy, then the enemy can go into an ATTACK state.

    public string getName()
    {

        switch (name)
        {
            case STATE.IDLE:
                return "Idle";
                break;
            case STATE.PURSUE:
                return "Pursue";
                break;
            case STATE.ATTACK:
                return "Attack";
                break;
            default:
                return "Idle";
                break;

        }

    }

    // Constructor for State
    public State(GameObject _enemy, NavMeshAgent _agent, Transform _player)
    {
        enemy = _enemy;
        agent = _agent;
        stage = EVENT.ENTER;
        player = _player;
        
    }

    // Phases as you go through the state.
    public virtual void Enter() { stage = EVENT.UPDATE; } // Runs first whenever you come into a state and sets the stage to whatever is next, so it will know later on in the process where it's going.
    public virtual void Update() { stage = EVENT.UPDATE; } // Once you are in UPDATE, you want to stay in UPDATE until it throws you out.
    public virtual void Exit() { stage = EVENT.EXIT; } // Uses EXIT so it knows what to run and clean up after itself.

    // The method that will get run from outside and progress the state through each of the different stages.
    public State Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)
        {
            Exit();
            return nextState; //returns a newly updated 'state'.
        }
        return this; // return the same state.
    }

    // using a simple Line Of Sight calculation
    public bool CanSeeTank()
    {
        Vector3 direction = player.position - enemy.transform.position; // Provides the vector from the enemy to the player.
        float angle = Vector3.Angle(direction, enemy.transform.forward); // Provide angle of sight.

        // If player tank is near enough AND within the arc of fire 
        if (direction.magnitude < visDist && angle < visAngle)
        {
            return true; // enemy CAN see the player.
        }
        return false;
    }

    public bool CanAttackTank()
    {
        // direction vector from the enemy to the player.
        Vector3 direction = player.position - enemy.transform.position;



        if (direction.magnitude < shootDist)
        {

            //RaycastHit hit;
            //LayerMask mask = LayerMask.GetMask("Tanks");
            //Debug.Log(" ENEMY CAN TANK ATTACK");
            // Does the ray intersect any objects excluding the player layer
            // Transform bulletSpawn = enemy.GetComponent<TankFiringSystem>()._spawnPoint;
            // if (Physics.Raycast(bulletSpawn.position, direction, out hit, shootDist)) //mask
            // {
            //     Debug.Log(" CAN TANK ATTACK 2nd IF");
            //     Debug.DrawRay(bulletSpawn.position,direction * hit.distance, Color.yellow);
            //     Debug.Log("Did Hit");
            //     bool playerTank = hit.collider.gameObject.CompareTag("Player");
            //     if (playerTank)
            //     {
            //         Debug.Log("No friendly fire detected");
            //         return true;
            //     }
            // }

        }

        return false;
    }
}


public class Idle : State
{
    public Idle(GameObject _enemy, NavMeshAgent _agent, Transform _player)
                : base(_enemy, _agent, _player)
    {
        name = STATE.IDLE; // Set name of current state.
    }

    public override void Enter()
    {
        base.Enter(); // Sets stage to UPDATE.
    }
    public override void Update()
    {
        if (CanSeeTank())
        {
            nextState = new Pursue(enemy, agent, player);
            stage = EVENT.EXIT; // Trigger the return of the nextState.
        }
        // The only place where Update can break out of itself. Set probability breaking out at 60%.
        else if (Random.Range(0, 100) < 60)
        {
            nextState = new Patrol(enemy, agent, player);
            stage = EVENT.EXIT; // The next time 'Process' runs, the EXIT stage will run instead, which will then return the nextState.
        }
    }

    public override void Exit()
    {

        base.Exit();
    }
}

// Constructor for Patrol state.
public class Patrol : State
{
    int currentIndex = -1;
    public Patrol(GameObject _enemy, NavMeshAgent _agent, Transform _player)
                : base(_enemy, _agent, _player)
    {
        name = STATE.PATROL;
        agent.speed = 15; // Tank speed ONLY if path is found.
        agent.isStopped = false; // Start and stop agent on current path using this bool.
    }

    public override void Enter()
    {
        float lastDist = Mathf.Infinity; // Store distance between enemy and waypoints.
        for (int i = 0; i < GameEnvironment.Singleton.Checkpointsec1.Count; i++)
            {
                GameObject thisWP = GameEnvironment.Singleton.Checkpointsec1[i];
                float distance = Vector3.Distance(enemy.transform.position, thisWP.transform.position);
                if (distance < lastDist)
                {
                    // Need to subtract 1 because in Update, we add 1 to i before setting the destination.
                    currentIndex = i - 1;
                    lastDist = distance;
                }
            }

        base.Enter();
    }

    public override void Update()
    {
        // Check if agent hasn't finished walking between waypoints.
        if (agent.remainingDistance < 1)
        {
            
            // If agent has reached end of waypoint list, go back to the first one, otherwise move to the next one.
            if (currentIndex >= GameEnvironment.Singleton.Checkpointsec1.Count - 1)
                currentIndex = 0;
            else
                currentIndex++;
            agent.SetDestination(GameEnvironment.Singleton.Checkpointsec1[currentIndex].transform.position);
            
        }

        if (CanSeeTank())
        {
            nextState = new Pursue(enemy, agent, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class Pursue : State
{
    public Pursue(GameObject _enemy, NavMeshAgent _agent, Transform _player )
                : base(_enemy, _agent, _player)
    {
        name = STATE.PURSUE; // State set to match what enemy is doing.
        agent.speed = 5; // Speed set to make sure enemy appears to be running.
        agent.isStopped = false; // Set bool to determine enemy is moving.
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        // Set goal for enemy to reach but navmesh processing might not have taken place, so
        agent.SetDestination(player.position);
        if (agent.hasPath)                       // Sanity check
        {
            if (CanAttackTank())
            {
                //Time to attack player
                nextState = new Attack(enemy, agent, player);
                stage = EVENT.EXIT; // End pursue state
            }
            // If enemy can't see the player, switch back to Patrol state.
            else if (!CanSeeTank())
            {
                nextState = new Idle(enemy, agent, player); // If enemy can't see player, set correct nextState.
                stage = EVENT.EXIT; // Set stage correctly as we are finished with Pursue state.
            }
            // If player tank is near enough, strife
            //if ( (player.position - enemy.transform.position).magnitude  < 15)
            //{
            //    Debug.Log("strifing");
            //    enemy.transform.position = enemy.transform.position + new Vector3(0, 1 * 10 * Time.deltaTime,0);
            //    enemy.transform.position = enemy.transform.position + new Vector3(0,-1 * 10 * Time.deltaTime,0);
            //}
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}

//Deprecated
public class Attack : State
{
    // Set speed that enemy will rotate around to face player.
    float rotationSpeed = 3.0f;
    AudioSource firingNoise;
    protected TankFiringSystem eTankShoot; //Shooting system for the enemy tanks

    public Attack(GameObject _enemy, NavMeshAgent _agent, Transform _player)
                : base(_enemy, _agent, _player)
    {
        name = STATE.ATTACK;
        Debug.Log("Get Firing System and Audio Noise");
        //firingNoise = _enemy.GetComponent<AudioSource>();
        eTankShoot = _enemy.GetComponent<TankFiringSystem>();
    }

    public override void Enter()
    {
        agent.isStopped = true;
        base.Enter();
    }

    public override void Update()
    {
        // Calculate direction and angle to player.
        Vector3 direction = player.position - enemy.transform.position;
        // Arc of fire
        float angle = Vector3.Angle(direction, enemy.transform.forward);
        direction.y = 0; // Prevent character from tilting.

        // Rotate enemy to always face the player that he's attacking.
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation,
                                            Quaternion.LookRotation(direction),
                                            Time.deltaTime * rotationSpeed);

        if (!CanAttackTank())
        {
            // If enemy can't attack player, go back to idle
            nextState = new Idle(enemy, agent, player);
            stage = EVENT.EXIT;
        }
        else
        {
            // if(eTankShoot.get)

            Debug.Log("Fire Bullet");
            eTankShoot.Fire();
            //firingNoise.PlayDelayed(5.0f);

        }
    }

    public void tankshootNoise()
    {
        Debug.Log("Play Firing Noise");
        //firingNoise.Play();
    }

    public override void Exit()
    {
        Debug.Log("Stop Firing Noise");
        //firingNoise.Stop();
        base.Exit();
    }
}