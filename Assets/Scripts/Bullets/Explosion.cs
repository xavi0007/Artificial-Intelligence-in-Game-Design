using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : Bullet
{

    // public LayerMask _explosionMask;
    // public float _explosionRadius = 5f;
    // public float _explosionForce = 1000f;
    // public float _maxDamage;
    // public GameObject ExplosionEffect;





    // // Start is called before the first frame update
    // void Awake()
    // {
    //     mRigidBody = GetComponent<Rigidbody>();
    //     _maxDamage = _baseDamage;


       

    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    // // Called when collide with other collider
    // protected virtual void OnTriggerEnter(Collider other)
    // {

        

    //     if (state == State.Moving)
    //     {
    //         state = State.Destroy;
    //     }


            
    //         //
    // }

    // private void FixedUpdate()
    // {
    //     switch (mState)
    //     {
    //         case State.Moving:
    //             Move();
    //             break;

    //         case State.Destroy:
    //             Explode();
    //             GameObject ex2 = Instantiate(ExplosionEffect, transform.position, transform.rotation);
    //             //ex2.GetComponent<ParticleSystem>().Play();
    //             //StartCoroutine(effectsDelay());
    //             Destroy(gameObject);


    //             break;
    //         default:
    //             break;
    //     }
    // }


    // public void Move()
    // {
    //     Vector3 moveVect = transform.forward * _moveSpeed * Time.deltaTime;
    //     mRigidBody.MovePosition(mRigidBody.position + moveVect);
    //     _currentRange -= (moveVect).magnitude;
    //     if (_baseRange <= 0)
    //     {
    //         state = State.Destroy;
    //     }
    // }


    // public void Explode()
    // {
    //     Debug.Log("Exploding");
        
    //     // Get all the tanks caught in the explosion
    //     Collider[] tanksCollider = Physics.OverlapSphere(transform.position, _explosionRadius, _explosionMask);

    //     // Loop through the collider to apply force and damage
    //     foreach( Collider collider in tanksCollider)
    //     {
    //         // Apply physics to the tank
    //         Rigidbody trbody = collider.GetComponent<Rigidbody>();
    //         if (trbody == null)
    //             continue;

    //         trbody.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);

    //         // Apply effects to tanks

    //         IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();
    //         if (damageable != null)
    //         {
    //             Debug.Log("Dealing Damage");
    //             float exDamage = Mathf.Abs(CalculateDamage(collider.transform.position));
    //             damageable.Damage(exDamage);
    //         }
    //     }

    //     // Destroy the shell
        

    // }

    // public float CalculateDamage(Vector3 targetPos)
    // {
    //     // Create a vector from the shell to the target
    //     Vector3 explosionToTarget = targetPos - transform.position;

    //     // Get the distance between shell and the target
    //     float distance = explosionToTarget.magnitude;

    //     // Calculate the proportion of the maximum distance (the explosionRadius) the target is away
    //     float relativeDistance = (_explosionRadius - distance) / _explosionRadius;

    //     // Damage is proportional to the distance
    //     float damage = relativeDistance * _maxDamage;

    //     Debug.Log("Explosion Damage " + damage);
    //     Debug.Log("Rel Dist " + relativeDistance);
    //     Debug.Log("Dist " + distance);
    //     return Mathf.Max(0, damage);
    // }
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
    //     Gizmos.DrawWireSphere(transform.position, 5.0f);
    // }

}
