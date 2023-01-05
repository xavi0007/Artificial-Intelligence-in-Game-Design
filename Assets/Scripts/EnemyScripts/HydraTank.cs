using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HydraTank : Tank, IDamageable
{
    [SerializeField] private float health;
    // Start is called before the first frame update
    public Transform Spawn1;
    public Transform Spawn2;
    public Transform Spawn3;
    public GameObject tankPrefab;
    public int splitCount;


    // public delegate int numberChange(int numberChange);
    // public numberChange myNumberChange;


    void Awake()
    {
        _maxHealth = 150;
        splitCount = 1;
        mRigidbody = GetComponent<Rigidbody>();
        mTankShot = GetComponent<TankFiringSystem>();
        mHealth = _maxHealth;

    }
    
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }



    void setSplit(int split)
    {
        splitCount = split;
    }
    void setHealth(int newHealth)
    {
        health = newHealth;
    }

    // public void Damage(float damageAmount)
    // {
    //     TakeDamage(damageAmount);
    // }

    public override void Death()
    {
        //PlaySFX(_clipTankExplode);
        //StartCoroutine(ChangeState(State.Inactive, 1f));
        Debug.Log("Hydra Dead");
        if(splitCount == 2)
        {
            myNumberChange.Invoke(score);
            Vector3 offset = new Vector3(0, 1, 0);
            if (Random.value > 0.5) //%30 percent chance (1 - 0.7 is 0.3)
            {
                Instantiate(powerUp, transform.position + offset, transform.rotation);
            }

        }
        
        Debug.Log(splitCount >= 1);
        if (splitCount >= 1)
        {

            splitCount -= 1;
            GameObject s1 = Instantiate(tankPrefab, Spawn1.position, Spawn1.rotation);
            s1.GetComponent<HydraTank>().setSplit(splitCount);
            s1.GetComponent<HydraTank>().myNumberChange = this.myNumberChange;
            

            GameObject s2 = Instantiate(tankPrefab, Spawn2.position, Spawn2.rotation);
            s2.GetComponent<HydraTank>().setSplit(splitCount);
            s2.GetComponent<HydraTank>().myNumberChange = this.myNumberChange;
            
            // myNumberChange.Invoke(2);


        }
        Destroy(gameObject);
    }

}
