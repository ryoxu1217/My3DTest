using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2, -5);

    public float radius = 0.3f;
    public LayerMask wallLayer;

    void LateUpdate()
    {
        Vector3 targetPos = target.position;
        Vector3 desiredPos = targetPos + target.TransformDirection(offset);

        Vector3 dir = desiredPos - targetPos;
        float distance = dir.magnitude;

        if (
            Physics.SphereCast(
                targetPos,
                radius,
                dir.normalized,
                out RaycastHit hit,
                distance,
                wallLayer
            )
        )
        {
            transform.position = hit.point - dir.normalized * radius;
        }
        else
        {
            transform.position = desiredPos;
        }

        transform.LookAt(target);
    }
}
