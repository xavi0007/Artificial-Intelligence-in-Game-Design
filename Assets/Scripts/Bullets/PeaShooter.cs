using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeaShooter : Bullet
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Move()
    {
        Vector3 moveVect = transform.forward * _moveSpeed * Time.deltaTime * 999999;
        mRigidBody.MovePosition(mRigidBody.position + moveVect);
        _currentRange -= (moveVect).magnitude;
        if(_baseRange <= 0)
        {
            state = State.Destroy;
        }
    }

    private void FixedUpdate()
    {
        switch (mState)
        {
            case State.Moving:
                Move();
                break;

            case State.Collide:
                
                break;

            case State.Destroy:
                Destroy(gameObject);
                //state = State.Moving;
                //_currentRange = _baseRange;
                //gameObject.SetActive(false);

                break;
            default:
                break;
        }
    }
    protected virtual void OnTriggerEnter(Collider other)
    {

        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log("Dealing Damage");
            damageable.Damage(_baseDamage);
        }

        if (state == State.Moving)
        {
            Debug.Log("Exploding");
            state = State.Destroy;
        }

    }


}
