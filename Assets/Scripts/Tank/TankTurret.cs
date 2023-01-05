using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankTurret : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Input Porperties")]
    public Camera camera;
    public GameObject pointer;

    private Vector3 reticlePosition;


    public Vector3 ReticlePosition
    {
        get { return reticlePosition; }
    }


    private Vector3 reticleNorm;
    public Vector3 ReticleNorm
    {
        get { return reticleNorm; }
    }

    private float fwdIp;
    public float FwdIp
    {
        get { return fwdIp; }
    }

    private float rotIp;

    public float RotIp
    {
        get { return RotIp; }
    }


    void Start()
    {
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (camera)
        {
            // FollowMouse(); 
        }
        HandleTurret();
        
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(reticlePosition, 0.5f);
    }
    

    protected virtual void FollowMouse()
    {
        Ray screenRay = camera.ScreenPointToRay(Input.mousePosition); // Ray hits scene 
        RaycastHit hit;

        // If we hit something 
        if (Physics.Raycast(screenRay, out hit))
        {
            reticlePosition = hit.point;
            reticleNorm = hit.normal;
            pointer.transform.position = reticlePosition;
        }

    }


    protected virtual void HandleTurret()
    {
            Vector3 turretLookDir = reticlePosition - gameObject.transform.position;
            turretLookDir.y = 0f;
            gameObject.transform.rotation = Quaternion.LookRotation(turretLookDir);
    }



}
