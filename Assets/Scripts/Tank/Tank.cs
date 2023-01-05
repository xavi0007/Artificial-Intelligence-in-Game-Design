using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Tank : MonoBehaviour, IDamageable
{
    
    //audio clip
    [Header("Tank Stats")]
    public float _moveSpeed;
    public float _rotationSpeed ;
    public bool _inputIsEnabled = true;
    public int _playerNum;
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

    [Header("FX suff")]
    public GameObject ExplosionEffect;


    //delegate
    public delegate void TankDestroyed(Tank target);
    public TankDestroyed dTankDestroyed;
    public delegate int numberChange(int numberChange);
    public numberChange myNumberChange;
    public GameObject powerUp;
    public int score;
    

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
    private void Awake()
    {

        if (!gameObject.CompareTag("Player"))
            _inputIsEnabled = false;


        mRigidbody = GetComponent<Rigidbody>();
        //mTankShot = GetComponent<TankFiringSystem>();
        mHealth = _maxHealth;
        

    }

    // Start is called before the first frame update
    void Start()
    {
        mOriginalPitch = _movementSFX.pitch;
        
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Idle:
            case State.Moving:
                if (_inputIsEnabled)
                {
                    MovementInput();
                    FireInput();
                }
                break;

            case State.TakingDamage:
                break;
            case State.Death:

                break;
            case State.Inactive:
                break;
            default:
                break;
        }

    }

    protected void MovementInput()
    {
        // Update input
        mVerticalInputValue = Input.GetAxis(mVerticalAxisInputName);
        mHorizontalInputValue = Input.GetAxis(mHorizontalAxisInputName );

        // Check movement and change states according to it
        if (Mathf.Abs(mVerticalInputValue) > 0.1f || Mathf.Abs(mHorizontalInputValue) > 0.1f)
            state = State.Moving;
        else state = State.Idle;
    }
    

    protected void FireInput()
    {
        //fire shots
        if (Input.GetButton(mFireInputName + (_playerNum + 1)))
        {

            if (mTankShot.Fire() != null)
            {
                // Debug.Log("Playing Fire Audio");
                PlaySFX(_clipShotFired);
            }
            else if(mTankShot.outOfAmmo)
            {
                AudioClip reload = SoundManager.Instance.getIndex("Reload");
                PlaySFX(reload);
            }

        }
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

    // Physic update. Update regardless of FPS
    void FixedUpdate()
    {
        Move();
        Rotate();
        mRigidbody.velocity = Vector3.ClampMagnitude(mRigidbody.velocity, _moveSpeed);
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

    // Rotate the tank
    public void Rotate()
    {
        float rotationDegree = _rotationSpeed * Time.deltaTime * mHorizontalInputValue;
        Quaternion rotQuat = Quaternion.Euler(0f, rotationDegree, 0f);
        mRigidbody.MoveRotation(mRigidbody.rotation * rotQuat);
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
                    StartCoroutine(SceneChange());
                    SceneManager.LoadScene(2);
                }
            }
                
                
        }


    }

    public virtual void Death()
    {
        
        //PlaySFX(_clipTankExplode);
        myNumberChange.Invoke(score);
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
        mRigidbody.isKinematic = false;
        gameObject.SetActive(true);
        _inputIsEnabled = true;

        // Change state
        state = State.Idle;
    }

    protected IEnumerator ChangeState(State state, float delay)
    {
        // Delay
        yield return new WaitForSeconds(delay);

        // Change state
        this.state = state;
    }

    public void Damage(float damageAmount)
    {
        TakeDamage(damageAmount);
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
    //  public void OnTriggerEnter(Collider collision)
    // {
    //     if (collision.CompareTag("Friendly"))
    //     {
    //         Destroy(collision.gameObject);
    //         saveFriendlyCount++;
    //         Debug.Log("save");
    //     }
    //     else if(collision.CompareTag("EnemyTank")){
    //         Destroy(collision.gameObject);
    //         killCount++;
    //     }
        
    // }

    IEnumerator SceneChange()
    {
        yield return new WaitForSeconds(2);
    }
}
