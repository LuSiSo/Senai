using UnityEngine;

public class CameraFollow : MonoBehaviour
{
 
    public Transform target;
  
    public float smoothSpeed = 5f;
 
    public Vector3 offset;

    [Header("Eixos que a câmera acompanha")]
 
    public bool followX = true;
    public bool followY = true;

    private void LateUpdate()
    {
        if (target == null) return;
        Vector3 desiredPosition = transform.position;

        if (followX)
        {
            desiredPosition.x = target.position.x + offset.x;
        }

        if (followY)
        {
            desiredPosition.y = target.position.y + offset.y;
        }

        desiredPosition.z = offset.z; // A posição Z da câmera é definida 
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
