using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    void LateUpdate()
    {
        Vector3 desiredPosition = new Vector3(target.position.x, target.position.y, transform.position.z) + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        // Piksel hassasiyetli pozisyon düzeltmesi
        smoothedPosition.x = Mathf.Round(smoothedPosition.x * 100) / 100f;
        smoothedPosition.y = Mathf.Round(smoothedPosition.y * 100) / 100f;

        transform.position = smoothedPosition;
    }
}

