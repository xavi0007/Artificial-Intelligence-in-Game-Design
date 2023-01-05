using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingShot : Bullet
{
    // private Transform target;
    // private bool targetAcquired;
    // public LayerMask homingMask;
    // private bool isLookingAtObject = true;
    // public float homingRadius;

    // // Start is called before the first frame update
    // void Start()
    // {
    //     targetAcquired = false;
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }





    // public void Seeking()
    // {
    //     Debug.Log("Homing");

    //     // Get all the tanks caught in the explosion
    //     Collider[] tanksCollider = Physics.OverlapSphere(transform.position, homingRadius, homingMask);

    //     // Loop through the collider to apply force and damage
    //     foreach (Collider collider in tanksCollider)
    //     {


    //         IDamageable damageable = collider.gameObject.GetComponent<IDamageable>();
    //         Debug.Log(damageable);
    //         if (damageable != null && !collider.gameObject.CompareTag("Player"))
    //         {
    //             Debug.Log("Target Acquired");
    //             target = collider.transform;
                
    //             targetAcquired = true;
    //         }
    //     }

    //     // Destroy the shell


    // }

    // public void Move()
    // {
    //     Vector3 moveVect = transform.forward * _moveSpeed * Time.deltaTime;
        
    //     if (!targetAcquired)
    //     {
    //         mRigidBody.MovePosition(mRigidBody.position + moveVect);
    //     }
    //     else
    //     {
    //         // https://answers.unity.com/questions/1452516/dodgeable-homing-missiles-in-unity3d.html
    //         if (target != null)
    //         {
    //             Vector3 targetDirection = target.position - transform.position;
    //             Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 1000 * Time.deltaTime, 0.0F);

    //             moveVect = Vector3.forward * Time.deltaTime * _moveSpeed;
    //             //mRigidBody.MovePosition(mRigidBody.position + moveVect);
    //             transform.Translate(Vector3.forward * Time.deltaTime * _moveSpeed, Space.Self);

    //             if (Vector3.Distance(transform.position, target.position) < 5)
    //             {
    //                 isLookingAtObject = false;
    //             }

    //             if (isLookingAtObject)
    //             {
    //                 transform.rotation = Quaternion.LookRotation(newDirection);
    //             }
    //         }
    //         else
    //         {
    //             targetAcquired = false;
    //         }

            





    //         _currentRange -= (moveVect).magnitude;
    //     }


        
       
    //     if (_baseRange <= 0)
    //     {
    //         state = State.Destroy;
    //     }
    // }

    // protected virtual void OnTriggerEnter(Collider other)
    // {

    //     IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
    //     if (damageable != null)
    //     {
    //         Debug.Log("Dealing Damage");
    //         damageable.Damage(_baseDamage);
    //     }

    //     if (state == State.Moving)
    //     {
    //         Debug.Log("Exploding");
    //         state = State.Destroy;
    //     }

    // }

    // private void FixedUpdate()
    // {
    //     switch (mState)
    //     {
    //         case State.Moving:
    //             Move();
    //             if (!targetAcquired) 
    //                 Seeking();
    //             break;



    //         case State.Destroy:
    //             Destroy(gameObject);


    //             break;
    //         default:
    //             break;
    //     }
    //  }
}
