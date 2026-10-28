using UnityEngine;

public class CameraFollowLook : MonoBehaviour
{
    public Transform target;

    public float height = 10f; // プレイヤーから上にどれくらい離れるか
    public float distance = 20f; // プレイヤーから後ろにどれくらい離れるか
    public float sideOffset = 0f; // プレイヤーから横にどれくらい離れるか
    public float smoothSpeed = 6f;

    void LateUpdate()
    {
        Quaternion yRotation = Quaternion.Euler(0, target.eulerAngles.y, 0);

        Vector3 offset = new Vector3(sideOffset, height, -distance);

        Vector3 targetPosition = target.position + yRotation * offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        Vector3 lookTarget = target.position;
        lookTarget.y += 1.5f;

        transform.LookAt(lookTarget);
    }
}
