using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour{
    public GameObject[] enemyTanksList;

    EnemyAI eAI;


    // Start is called before the first frame update
    void Start()
    {
        enemyTanksList = GameObject.FindGameObjectsWithTag("EnemyTank");
        if(enemyTanksList.Length > 0)
        {
            // Debug.Log("Enemy tank at" + enemyTanksList[0].name);
        }
    }

    bool CheckPursue(GameObject[] enemyTanksList){
        foreach(GameObject enemyTank in enemyTanksList)
        {  
            eAI = enemyTank.GetComponent<EnemyAI>();
          
            if (eAI.getState() == "Pursue"){
                // Debug.Log("from EM" + eAI.getState());
                return true;
            }
               
        }
        
        return false;
    }

    void allPursue(GameObject[] enemyTanksList){
        foreach(GameObject enemyTank in enemyTanksList)
        {
            eAI = enemyTank.GetComponent<EnemyAI>();
            eAI.setState("Pursue");
        }
    }

    
    // Update is called once per frame
    void FixedUpdate()
    {
        enemyTanksList = GameObject.FindGameObjectsWithTag("EnemyTank");
        if (enemyTanksList.Length >= 1)
        {  
            if (CheckPursue(enemyTanksList)){
                allPursue(enemyTanksList);
            }

        }

        else
        {
            //Debug.Log("All enemies dead");
        }
       

       
    }
}
