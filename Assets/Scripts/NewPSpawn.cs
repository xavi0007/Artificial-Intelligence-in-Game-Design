using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPSpawn : MonoBehaviour
{
    public float spawnTime;
    public float despawnTime;
    // Start is called before the first frame update

    [SerializeField]
    public GameObject cratePrefab;
    [SerializeField]
    public Transform spawnPoint;
    [SerializeField]
    public float spawnDelay = 15;
    [SerializeField]
    public float despawnDelay = 10;

    // Update is called once per frame
    void Update()
    {

        if (ShouldSpawn())
        {
            Spawn();
        }

        if (ShouldDespawn())
        {
            Despawn();
        }
        
    }

    private void Spawn()
    {
        spawnTime = Time.time + spawnDelay;
        despawnTime = Time.time + spawnDelay + despawnDelay;
        Instantiate(cratePrefab, spawnPoint.position, spawnPoint.rotation);
    }

    private void Despawn()
    {
        GameObject[] crates = GameObject.FindGameObjectsWithTag("powercrate");
        foreach (GameObject crate in crates)
        GameObject.Destroy(crate);

    }

    private bool ShouldSpawn()
    {
        //here we can insert the tag check
        // need to make sure the object always has a tag
        if ((GameObject.FindGameObjectsWithTag("powercrate").Length == 0) && (Time.time > spawnTime))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool ShouldDespawn()
    {
        if ((GameObject.FindGameObjectsWithTag("powercrate").Length > 0) && (Time.time > despawnTime))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
