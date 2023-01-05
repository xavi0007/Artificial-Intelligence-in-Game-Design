using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Attributes")]
    public float _moveSpeed;
    public float _baseDamage;
    public float _coolDownModifier;
    public float _baseRange;
    protected float _currentRange;
    public int _ammo;
    protected Rigidbody mRigidBody;


    [Header("State Attributes")]
    protected State mState;

    public enum State
    {
        Moving,
        Collide,
        Destroy
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
                    case State.Moving:
                        break;

                    case State.Collide:
                        
                        break;

                    case State.Destroy:
                        
                        break;

                    default:
                        break;
                }

                mState = value;
            }
        }
    }

    

    // Start is called before the first frame update
    void Awake()
    {
        mRigidBody = GetComponent<Rigidbody>();
        _currentRange = _baseRange;
    }


    public virtual void Move()
    {
        Vector3 moveVect = transform.forward * _moveSpeed * Time.deltaTime;
        mRigidBody.MovePosition(mRigidBody.position + moveVect);
    }




}
