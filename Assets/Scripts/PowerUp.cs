using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    public GameObject[] bulletList;
    public AudioClip[] sounds;
    public GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

    protected virtual void OnTriggerEnter(Collider other)
    {

        TankFiringSystem tankFiringSystem = other.gameObject.GetComponent<TankFiringSystem>();
        if (tankFiringSystem != null && other.gameObject.CompareTag("Player"))
        {
            
            bulletList = tankFiringSystem.bulletList;
            int weaponPtr = Random.Range(0, bulletList.Length);
            AudioClip audioClip = sounds[weaponPtr];
            SoundManager.Instance.Play(audioClip);
            bullet = bulletList[weaponPtr];
            tankFiringSystem.EquipBullet(bullet);
            Destroy(gameObject);
        }


       
    }

}
