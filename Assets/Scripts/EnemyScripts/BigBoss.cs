using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBoss : Tank, IDamageable
{

    IDamageable selfDamage;
    public GameObject Warning;
    public GameObject Explosion;
    ParticleSystem em;
    ParticleSystem ex;
    public LayerMask _explosionMask;

    // Start is called before the first frame update
    //public Transform Spawn1;
    //public Transform Spawn2;
    //public Transform Spawn3;
    //public GameObject tankPrefab;


    //public delegate int numberChange(int numberChange);
    //public numberChange myNumberChange;

    [Header("CoolDownHandling")]

    protected float mCooldownCounter;
    public float _cooldown;

    void Awake()
    {
        _maxHealth = 1000;
        em = Warning.GetComponent<ParticleSystem>();
        ex = Explosion.GetComponent<ParticleSystem>();
        mRigidbody = GetComponent<Rigidbody>();
        em.Pause();
        ex.Pause();
        selfDamage = GetComponent<IDamageable>();
 

        mRigidbody = GetComponent<Rigidbody>();
        mTankShot = GetComponent<TankFiringSystem>();
        mHealth = _maxHealth;
    }


    void Start()
    {
        StartCoroutine("AttackSequence");
        _cooldown = 2.5f;
        mCooldownCounter = _cooldown;
    }



    protected void Jump()
    {
        //rb.AddForce(Physics.gravity * (gravityScale - 1) * rb.mass);
    }
    void FixedUpdate()
    {
        //rb.AddForce(10 * Vector3.up, ForceMode.VelocityChange);
        //StartCoroutine("AttackSequence");
        
    }

    // Update is called once per frame
    void Update()
    {
        mCooldownCounter -= Time.deltaTime;
        if (mCooldownCounter <= 0)
        {
            StartCoroutine("AttackSequence");
            mCooldownCounter = _cooldown;
        }
    }
            


    IEnumerator AttackSequence()
    {

        Debug.Log("Attack");
        em.Play();
        AudioClip uhOh = SoundManager.Instance.getIndex("CLassicAlarm");
        PlaySFX(uhOh);
        yield return new WaitForSeconds(0.5f);
        em.Pause();
        ex.Play();
        // Glow 
        CheckForDestructibles();
        yield return new WaitForSeconds(1.5f);
        // AOE
        
        ex.Pause();
        
    }

    public override void Death()
    {
        //PlaySFX(_clipTankExplode);
        //StartCoroutine(ChangeState(State.Inactive, 1f));
        myNumberChange.Invoke(score);
        em.Stop();
        ex.Stop();
        // myNumberChange.Invoke(-1);
        StartCoroutine("AttackSequence");
        Destroy(gameObject);
    }


    // public void Damage(float damageAmount)
    // {
    //     base.Damage(damageAmount);
    //     // TakeDamage(damageAmount);
    // }

    private void CheckForDestructibles()
    {

        Vector3 offset = gameObject.transform.up * 2;
        Collider[] colliders = Physics.OverlapSphere(transform.position + offset, 6.0f, _explosionMask);
        foreach(Collider c in colliders)
        {
            IDamageable damageable = c.gameObject.GetComponent<IDamageable>();
            Debug.Log(c.gameObject.tag);

            if (damageable != null && c.gameObject.tag != "BigBoss")
            {
                Debug.Log("Dealing Damage to " + c.gameObject.tag);
                damageable.Damage(25f);
            }
            Rigidbody trbody = c.gameObject.GetComponent<Rigidbody>();
            if (trbody == null)
                continue;

            trbody.AddExplosionForce(10f, transform.position, 6.0f);

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Vector3 offset = gameObject.transform.up * 2;
        Gizmos.DrawWireSphere(transform.position + offset, 6.0f);
    }
}
