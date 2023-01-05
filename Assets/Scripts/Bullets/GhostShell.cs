using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostShell : Bullet
{
    // Start is called before the first frame update
    private float degeneration; 


    void Start()
    {
        degeneration = 0.99f;
        StartCoroutine(SelfDestruct());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void OnTriggerEnter(Collider other)
    {
        //base.OnTriggerEnter(other);
        _baseDamage *= degeneration;
        Debug.Log("Collided, new damage  "+ _baseDamage);

        IDamageable damageable = other.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Debug.Log("Dealing Damage");
            damageable.Damage(_baseDamage);
        }


    }

    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(4f);
       
        Debug.Log("Destroyed");
        state = State.Destroy;
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
                //Destroy(gameObject);
                gameObject.SetActive(false);
                break;
            default:
                break;
        }
    }
}
