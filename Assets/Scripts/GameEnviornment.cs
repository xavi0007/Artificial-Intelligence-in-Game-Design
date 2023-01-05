using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public sealed class GameEnvironment
{

    private static GameEnvironment instance;
    private List<GameObject> checkpoints1 = new List<GameObject>();
    public List<GameObject> Checkpointsec1 { get { return checkpoints1; } }
    
    

    public static GameEnvironment Singleton
    {
        get
        {
            if (instance == null)
            {
                
                instance = new GameEnvironment();
                instance.checkpoints1.AddRange(GameObject.FindGameObjectsWithTag("sector1"));
                //Debug.Log(instance.checkpoints1.Count);
                instance.checkpoints1 = instance.checkpoints1.OrderBy(waypoint => waypoint.name).ToList(); // Order waypoints in ascending alphabetical order by name, so that the NPC follows them correctly.
            }
            return instance;
        }
    }

}
