using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankFiringSystem : MonoBehaviour
{

    [Header("OjectPool")]
    public int amountToPool;

    [Header("Bullets")]
    public GameObject[] bulletList;

    protected Bullet _bullet;
    private GameObject _currentBullet;
    public GameObject _normalBullet;
    private int _currentAmmo;
    private int maxAmmo;
    public bool outOfAmmo;



    [Header("CoolDownHandling")]

    protected float mCooldownCounter;
    public float _cooldown;
    
    
    public Transform _spawnPoint;


    public enum State
    {
        ReadyToFire,
        OnCooldown
    }
    protected State mState = State.ReadyToFire;

    public void EquipBullet(GameObject bulletPrefab)
    {
        // 
        reload();

        _currentBullet = bulletPrefab;
        _bullet = _currentBullet.GetComponent<Bullet>();

        maxAmmo = _bullet._ammo;
        _currentAmmo = maxAmmo;

        // TODO : handle player attack speed
        _cooldown = 1.0f * _bullet._coolDownModifier;
        mCooldownCounter = 1.0f * _bullet._coolDownModifier;


        outOfAmmo = false;
    }

    private void reload()
    {
        StartCoroutine(ReloadDelay());
        

    }

     IEnumerator ReloadDelay()
    {
        yield return new WaitForSeconds(1.4f);
        //Debug.Log("waiting");
        // 
        //StartCoroutine (ReloadDelay());

    }

    void Awake()
    {
    }

    private void Start()
    {
        EquipBullet(_normalBullet);
        //EquipBullet(bulletList[0]);
    }


    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.ReadyToFire:

                break;

            case State.OnCooldown:
                mCooldownCounter -= Time.deltaTime;
                if (mCooldownCounter <= 0)
                    state = State.ReadyToFire;
                break;

            default:
                break;
        }
    }

    public GameObject Fire()
    {
        GameObject shell = null;
        if (state == State.ReadyToFire)
        {
            //change state
            state = State.OnCooldown;

            //spawn shell
             shell = Instantiate(_currentBullet, _spawnPoint.position, _spawnPoint.rotation);
    
            
        }

        return shell;
    }

    public Transform getSpawnPoint()
    {
        return this._spawnPoint;
    }

    public State state
    {
        get { return mState; }
        set
        {
            if(mState != value)
            {
                switch (mState)
                {
                    case State.ReadyToFire:
                        break;

                    case State.OnCooldown:
                       if (gameObject.CompareTag("EnemyTank"))
                        {
                             mCooldownCounter = _cooldown;
                        }
                        mCooldownCounter = _cooldown;
                        break;

                    default:
                        break;
                }

                mState = value;
            }
        }
    }
}
