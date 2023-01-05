using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryScene : MonoBehaviour
{   
    

    void OnTriggerEnter (Collider other)
    {
        if (gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(3);
        }
 
    }
}
