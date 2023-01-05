using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class TankML : Agent, IDamageable
{
    
    //audio clip
    [Header("Tank Stats")]
    public float _moveSpeed;
    public float _rotationSpeed ;
    public bool _inputIsEnabled = true;
    public float _maxHealth;
    public float mHealth;

    //audio source
    [Header("Audio Properties")]
    public AudioSource _movementSFX;
    public AudioSource _tankSFX;
    public AudioClip _clipIdle;
    public AudioClip _clipMoving;
    public AudioClip _clipShotFired;
    public AudioClip _clipNoAmmo;
    public AudioClip _clipTankExplode;
    public float _pitchRange = 0.2f;
    protected float mOriginalPitch;

    
    protected TankFiringSystem mTankShot;
    
    [Header("Tank Controller")]
    protected Rigidbody mRigidbody;
    protected string mVerticalAxisInputName = "Vertical";
    protected string mHorizontalAxisInputName = "Horizontal";
    protected string mFireInputName = "Fire";
    protected float mVerticalInputValue = 0f;
    protected float mHorizontalInputValue = 0f;

    [Header("FX")]
    public GameObject ExplosionEffect;


    [Header("ML var")]
    public int maxKillCount = 20;
    public int maxSafeCount = 10;
    private int killCount = 0;
    private int saveFriendlyCount = 0;
    public GameObject levelManager;
    private bool isFrozen = false;
    public bool trainingMode = true;
    private GameObject nearestTank;
    private const float MaxRotSpeed = 90;
    private Quaternion currentRot;
    private Vector3 currentPos;
    private float m_Existential;
    private bool canShoot = true;
    private float rayCastShootDist = 40.0f;
    protected float canShootTimer = 1f;

    public enum State
    {
        Idle = 0,
        Moving,
        TakingDamage,
        Death,
        Inactive
    };
    protected State mState;




    // Awake is called right at the beginning if the object is active
    // private void Awake()
    // {

    //     if (!gameObject.CompareTag("Player"))
    //         _inputIsEnabled = false;


    //     mRigidbody = GetComponent<Rigidbody>();
    //     mTankShot = GetComponent<TankFiringSystem>();
    //     mHealth = _maxHealth;
    // }
    // Start is called before the first frame update

    void Start()
    {
        mOriginalPitch = _movementSFX.pitch;
        
    }


    public override void Initialize()
    {
        currentRot = GetComponent<Transform>().rotation;
        currentPos = GetComponent<Transform>().position;
        mRigidbody = GetComponent<Rigidbody>();
        mTankShot = GetComponent<TankFiringSystem>();
        mHealth = _maxHealth;

        if(!trainingMode){
            MaxStep =  0;
            _inputIsEnabled = true;
        }
        else{
            _inputIsEnabled = false;
        }
        m_Existential = 1f / MaxStep;
    }

    public override void OnEpisodeBegin(){
        //restart agent tank and remove all other tank
        Restart(currentPos, currentRot);
    }   

    private void RayCastShooting()
    {
        if (!canShoot)
        {
            //shoot wrong timing
            AddReward(-0.03f);
            return;
        }
        var tankDirection = transform.forward * 42;
        //mTankShot.Fire();
        //Debug.Log("shooting with raycast");
        Debug.DrawRay(currentPos, tankDirection, Color.green, 1f);

        //if (Physics.Raycast(mTankShot.getSpawnPoint().position, tankDirection, out var hit, rayCastShootDist))
        if (Physics.SphereCast(mTankShot.getSpawnPoint().position, 1f, tankDirection, out var hit, rayCastShootDist))
        {
            Debug.Log("hit");
            Debug.DrawRay(transform.position, tankDirection, Color.blue, 1f);

            if (hit.collider.gameObject.CompareTag("Friendly"))
            {
                //Cannot hit friendly
                AddReward(-0.3f);
                // 1 shot kill
                hit.transform.GetComponent<EnemyTank>().Death();
            }
            else if (hit.collider.gameObject.CompareTag("EnemyTank"))
            {
                //can hit friendly
                AddReward(0.5f);
                //1 shot kill
                hit.transform.GetComponent<EnemyTank>().Death();
            }
            
        }
        else
        {
            //shooting random stuff
            AddReward(-0.2f);
        }
        canShoot = false;
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        //shooting raycast (2 observation)
        //Debug.Log(canShoot);
        sensor.AddObservation(canShoot);
        sensor.AddObservation(canShootTimer);

        //Agent's local rot (3 observation)
        sensor.AddObservation(transform.localRotation.normalized);

        ////Get vector from agent to nearest tank
        //Vector3 toTank = nearestTank.transform.position - currentPos;
        ////get unit vector (3 observation)
        //sensor.AddObservation(toTank.normalized);
    }

    //prevent agent from doing anything
    public void FreezeAgent(){
        Debug.Assert(trainingMode == false, "Freeze/UnFreeze not supported in Training");
        isFrozen = true;
        mRigidbody.Sleep();
    }
    //resume agent actions
    public void UnfreezeAgent(){
        Debug.Assert(trainingMode == false, "Freeze/UnFreeze not supported in Training");
        isFrozen = false;
        mRigidbody.WakeUp();
    }
    //triggered when NN or player inputs
    // Idx 0 : rotate (+1 = rotate clockwise  
    // Idx 1 : rotate anticlockwise
    //idx 2:   fire
    public override void OnActionReceived(ActionBuffers actionbuffers)
    {
        //Do not take any action if frozen
        if (isFrozen) return;

        //All actions that agent can do

        //Rotate and Shoot
        MoveNShootAgent(actionbuffers.DiscreteActions, actionbuffers.ContinuousActions);


        //End condition
        if (saveFriendlyCount >= 10 || killCount >= 20)
        {
            levelManager.GetComponent<LevelManager>().setGameEndState();
            //Major reward because end goal is this
            AddReward(1f);
            EndEpisode();
        }

        // Penalty given each step to encourage agent to finish task quickly.
        AddReward(-1f / MaxStep);
    }

    public void MoveNShootAgent(ActionSegment<int> actD, ActionSegment<float> actC)
    {
        //Actions size = 1
        //Vector3 contolSignal = Vector3.zero;
        //contolSignal.x = actC[0];
        //contolSignal.z = actC[0];
        //Debug.Log(contolSignal.x);
        //Debug.Log(contolSignal.y);
        //Rotate(contolSignal.x);

        var rotateAxis = actC[0];
        //Debug.Log(rotateAxis);
        Vector3 rotateDir = Vector3.zero;
        if (rotateAxis >= 0)
        {
            rotateDir = transform.up * 1f;
        }
        else
        {
            rotateDir = transform.up * -1f;
        }

        var shootAction = actD[0];
        
        switch (shootAction)
        {
            case 1:
                RayCastShooting();
                break;
        }

        Rotate(rotateAxis);
        //transform.Rotate(rotateDir, Time.deltaTime * _rotationSpeed);
    }



    public override void Heuristic(in ActionBuffers actionsOut){

        var discreteActionsOut = actionsOut.DiscreteActions;

        var continuousActionsOut = actionsOut.ContinuousActions;

        //Keycode A,D , for rotation
        continuousActionsOut[0] = Input.GetAxis(mHorizontalAxisInputName);

        //Debug.Log(continuousActionsOut[0]);
        if (Input.GetButton(mFireInputName))
        {
            discreteActionsOut[0] = 1;
        }
        //Debug.Log(discreteActionsOut[0]);
    }

   

    //private void updateNearestTank(List<GameObject> tankList)
    //{
    //    foreach (GameObject tank in tankList)
    //    {
    //        if (nearestTank == null)
    //        {
    //            nearestTank = tank;
    //        }
    //        float currentDis = Vector3.Distance(nearestTank.transform.position, currentPos);
    //        float distanceToTank = Vector3.Distance(tank.transform.position, currentPos);
    //        if (distanceToTank < currentDis)
    //        {
    //            nearestTank = tank;
    //        }
    //    }
    //}

    // Update is called once per frame
    void Update()
    {

        if (canShootTimer > 0)
        {
            canShootTimer -= Time.deltaTime;
        }
        if (canShootTimer <= 0)
        {
            canShoot = true;
            canShootTimer = 0;
        }
        //switch (state)
        //{
        //    case State.Idle:
        //    case State.Moving:
        //        if (_inputIsEnabled)
        //        {
        //            MovementInput();
        //            FireInput();
        //        }
        //        break;

        //    case State.TakingDamage:
        //        break;
        //    case State.Death:

        //        break;
        //    case State.Inactive:
        //        break;
        //    default:
        //        break;
        //}
        //updateNearestTank(levelManager.GetComponent<LevelManager>().getTankList());
        //if(nearestTank != null){
        //    Debug.DrawLine(currentPos, nearestTank.transform.position, Color.green);
        //}
    }
     // Physic update. Update regardless of FPS
    //void FixedUpdate()
    //{
    //    Move();
    //    Rotate(mHorizontalInputValue);
    //    mRigidbody.velocity = Vector3.ClampMagnitude(mRigidbody.velocity, _moveSpeed);
    //}
    // Rotate the tank
    public void Rotate(float rotChange)
    {
        float rotationDegree = _rotationSpeed * Time.deltaTime * rotChange;
        Quaternion rotQuat = Quaternion.Euler(0f, rotationDegree, 0f);
        mRigidbody.MoveRotation(mRigidbody.rotation * rotQuat);
    }
    protected void MovementInput()
    {
        // Update input
        mVerticalInputValue = Input.GetAxis(mVerticalAxisInputName);
        mHorizontalInputValue = Input.GetAxis(mHorizontalAxisInputName);

        // Check movement and change states according to it
        if (Mathf.Abs(mVerticalInputValue) > 0.1f || Mathf.Abs(mHorizontalInputValue) > 0.1f)
            state = State.Moving;
        else state = State.Idle;
    }
    protected void FireInput()
    {
        //fire shots
        if (Input.GetButton(mFireInputName))
        {

            if (mTankShot.Fire() != null)
            {
                Debug.Log("Playing Fire Audio");
                PlaySFX(_clipShotFired);
            }
            else if (mTankShot.outOfAmmo)
            {
                AudioClip reload = SoundManager.Instance.getIndex("Reload");
                PlaySFX(reload);
            }

        }
    }
    // Move the tank based on speed
    public void Move()
    {
        // Debug.Log(mRigidbody);
        //Vector3 moveVect = transform.forward * _moveSpeed * Time.deltaTime * mVerticalInputValue;
        Vector3 moveVect = transform.forward * _moveSpeed * mVerticalInputValue;

        if (mVerticalInputValue == 0)
        {

            mRigidbody.velocity *= 0.9f;
            //moveVect = Vector3.zero;
            //mRigidbody.MovePosition(mRigidbody.position - moveVect);

        }
        else
        {
            // mRigidbody.MovePosition(mRigidbody.position + moveVect);
            mRigidbody.velocity =  moveVect;

        }
    }
    public void TakeDamage(float damage)
    {
        if (mState != State.Inactive || mState != State.Death)
        {
            mHealth -= damage;  

            if (mHealth > 0)
                state = State.TakingDamage;
            else
            {
                state = State.Death;
                if (gameObject.CompareTag("Player"))
                {
                    StartCoroutine(SceneChanges());
                    SceneManager.LoadScene(2);
                }
            }            
        }
    }
    public virtual void Death()
    {
        
        //PlaySFX(_clipTankExplode);
        // myNumberChange.Invoke(score);
        //Instantiate(powerUp, transform.position, transform.rotation);
        Instantiate(ExplosionEffect, transform.position, transform.rotation);
        StartCoroutine(ChangeState(State.Inactive, _clipTankExplode.length));
        //yield return new WaitForSeconds(1);

    }

    public void Restart(Vector3 pos, Quaternion rot)
    {
        // Reset position, rotation and health
        transform.position = pos;
        transform.rotation = rot;
        mHealth = _maxHealth;

        // Diable kinematic and activate the gameobject and input
        // mRigidbody.isKinematic = false;
        // gameObject.SetActive(true);
        // _inputIsEnabled = true;

        // Change state
        //state = State.Idle;
        killCount = 0;
        saveFriendlyCount = 0;
        canShoot = true;
        
        
    }

    protected IEnumerator ChangeState(State _state, float delay)
    {
        // Delay
        yield return new WaitForSeconds(delay);
        // Change state
        this.state = _state;
    }
    public void Damage(float damageAmount)
    {
        TakeDamage(damageAmount);
    }
    protected void ChangeMovementAudio(AudioClip clip)
    {
        if(_movementSFX.clip != clip)
        {
            _movementSFX.clip = clip;
            _movementSFX.pitch = mOriginalPitch + Random.Range(-_pitchRange, _pitchRange);
            _movementSFX.Play();
        }
    }
    protected void PlaySFX(AudioClip clip)
    {
        _tankSFX.clip = clip;
        _tankSFX.pitch = mOriginalPitch + Random.Range(-_pitchRange, _pitchRange);
        _tankSFX.Play();
    }
    public State state
    {
        get { return mState; }
        set
        {
            if (mState != value)
            {
                switch (value)
                {
                    case State.Idle:
                        ChangeMovementAudio(_clipIdle);
                        break;

                    case State.Moving:
                        ChangeMovementAudio(_clipMoving);
                        break;

                    case State.TakingDamage:
                        StartCoroutine(ChangeState(State.Idle, 0.5f));
                        break;

                    case State.Death:
                        Death();

                        break;

                    case State.Inactive:
                        gameObject.SetActive(false);
                        
                        mRigidbody.isKinematic = true;
                        _inputIsEnabled = false;
                        break;
                    default:
                        break;
                }

                mState = value;
            }
        }
    }

    private void TriggerEnterOrStay(Collider collision)
    {
        if (collision.CompareTag("Friendly"))
        {
            Vector3 closestToPlayer = GetComponent<Collider>().ClosestPoint(currentPos);
            Destroy(collision.gameObject);
            saveFriendlyCount++;
            AddReward(0.08f);
        }
        else if(collision.CompareTag("EnemyTank")){
            Destroy(collision.gameObject);
            AddReward(-0.8f);
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        TriggerEnterOrStay(collision);
    }
    private void OnTriggerStay(Collider collision)
    {
        TriggerEnterOrStay(collision);
    }

   
    IEnumerator SceneChanges()
    {
        yield return new WaitForSeconds(2);
    }
}
