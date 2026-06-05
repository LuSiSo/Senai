using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxEffect = 0.5f;

    private float startPosX;

    void Start()
    {
        startPosX = transform.position.x;
    }

    void Update()
    {
        float distance = cameraTransform.position.x * parallaxEffect;

        transform.position = new Vector3(
            startPosX + distance,
            transform.position.y,
            transform.position.z
        );
    }
}
