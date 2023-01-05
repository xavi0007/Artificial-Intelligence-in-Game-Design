using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelManager : MonoBehaviour

{

    [Header("Enermy Prefabs")]
    public List<GameObject> enemyTankPrefabList = new List<GameObject>();

    [Header("Enemy and Friendly Tanks")]
    private GameObject[] numFriendly;
    private GameObject[] numEnemy;
    private List<GameObject> tankList = new List<GameObject>();
    public int maxTanks = 100;

    [Header("Spawn rate")]
    bool isSpawning = false;
    public float minTime = 1.0f;
    public float maxTime = 3.0f;

    public enum State
    {
        GameStart,
        GameEnds,
        PlayerDead,
    };

    
    private State mState;

    protected List<Transform> mSpawnPoints = new List<Transform>();

    private void Awake()
    {
        GameObject playerTank =  GameObject.FindGameObjectWithTag("Player");
        // playerTank.GetComponent<Tank>().myNumberChange = OnTankDeath;
    }


    // Start is called before the first frame update
    void Start()
    { 
        state = State.GameStart;
    }

    // Spawn Tank and store it
    // public void SpawnTanks()
    // {
    //     var tankSpawnPosition = new Vector3(Random.Range(-40.0f, 40.0f), 0.5f, Random.Range(-40.0f, 40.0f));            
    //     int prefabidx = Random.Range(0, enemyTankPrefabList.Count);
    //     Instantiate(enemyTankPrefabList[prefabidx], tankSpawnPosition, Quaternion.identity);
    //     // EnemyAI enemyAI = tank.GetComponent<EnemyAI>();
    //     // enemyAI.checkpoint_name = _checkpoint_name;
    //     // tank.GetComponent<Tank>().myNumberChange = OnTankDeath;
    // }
    IEnumerator SpawnTank(float seconds)
    {
        //Debug.Log ("Waiting for " + seconds + " seconds");

        yield return new WaitForSeconds(seconds);
        //random position
        var tankSpawnPosition = new Vector3(Random.Range(-40.0f, 40.0f), 0.5f, Random.Range(-40.0f, 40.0f));            
        int prefabidx = Random.Range(0, enemyTankPrefabList.Count);
        Instantiate(enemyTankPrefabList[prefabidx], tankSpawnPosition, Quaternion.identity);

        //We've spawned, so now we could start another spawn     
        isSpawning = false;
    }


    private void Update(){
        numFriendly = GameObject.FindGameObjectsWithTag("Friendly");
        numEnemy = GameObject.FindGameObjectsWithTag("EnemyTank");
        if(numFriendly.Length != 0)
            tankList.AddRange(numFriendly);
        if(numEnemy.Length != 0){
            tankList.AddRange(numEnemy);
            //Debug.Log(tankList[0]);
        }
            
        if(!isSpawning)
            {
                isSpawning = true; 
                StartCoroutine(SpawnTank(Random.Range(minTime, maxTime)));
            }
        

    }
    
    private void resetArea(){
        if(tankList.Count != 0)
        {
            foreach (GameObject tank in tankList)
            {
                Destroy(tank);
            }
        }
        
    }

    public void setGameEndState(){
        state = State.GameEnds;
        resetArea();
    }
    
    
    public List<GameObject> getTankList(){
        return this.tankList;
    }
  


    public State state
    {
        get { return mState; }
        set
        {
            if (mState != value)
            {
                mState = value;

                switch (value)
                {
                    case State.GameStart:
                        break;

                    case State.GameEnds:
                        //StartCoroutine(InitGameEnd());
                        SoundManager.Instance.PlayName("MissionComplete");
                        Debug.Log("Win Game");
                        //TODO : Victory Screen

                        break;
                    case State.PlayerDead:
                        // TODO : Game Over Screen
                        // TODO : Play the Game Over Music
                        SoundManager.Instance.PlayName("GameOver");
                        break;

                    default:
                        break;
                }
            }
        }
    }


}
