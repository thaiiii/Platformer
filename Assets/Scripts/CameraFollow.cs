
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    Vector3 offset = new Vector3(0,0,-10);
    [Range(1, 10)]
    public float smoothFactor;
    public Vector3 minValues, maxValues;

    private void FixedUpdate()
    {
        Follow(); 
    }

    void Follow()
    {
        Vector3 targetPosition = target.position + offset;
        //Check if target position is out of bound or not
        //Limit it in the visible range
        Vector3 boundPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, minValues.x, maxValues.x),
            Mathf.Clamp(targetPosition.y, minValues.y, maxValues.y),
            Mathf.Clamp(targetPosition.z, minValues.z, maxValues.z));
        Vector3 smoothPosition = Vector3.Lerp(transform.position, boundPosition, smoothFactor*Time.fixedDeltaTime);
        transform.position = smoothPosition;
    }
}
