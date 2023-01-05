using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTank : Tank, IDamageable
{

    //public delegate int numberChange(int numberChange);
    //public numberChange myNumberChange;

    // Start is called before the first frame update
    void Awake()
    {
        _maxHealth = 100;
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


    //public void Damage(float damageAmount)
    //{
    //    TakeDamage(damageAmount);
    //    //Instantiate(powerUp, transform.position, transform.rotation);
    //}

    public override void Death()
    {
        //PlaySFX(_clipTankExplode);
        //StartCoroutine(ChangeState(State.Inactive, 1f));
        // Vector3 offset = new Vector3(0, 1, 0);

        // if (Random.value > 0.7) //%30 percent chance (1 - 0.7 is 0.3)
        // {
        //     Instantiate(powerUp, transform.position + offset, transform.rotation);
        // }
        
        Destroy(this.gameObject);
    }
}
