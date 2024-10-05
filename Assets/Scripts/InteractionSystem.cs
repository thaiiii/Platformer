using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionSystem : MonoBehaviour
{
    [Header("Interaction Point")]
    public Transform detectionPoint;
    public LayerMask detectionLayer;
    [SerializeField] float detectionRadius = 0.1f;
    
    void Update()
    {
        if(DetectObject())
        {
            if(InteractInput())
            {
                Debug.Log("press E");
            }
        }    
    }

    bool InteractInput()
    {
        return Input.GetKeyDown(KeyCode.E);
        
    }

    bool DetectObject()
    {
        return Physics2D.OverlapCircle(detectionPoint.position, detectionRadius, detectionLayer); ;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(detectionPoint.position, detectionRadius);

    }

}
