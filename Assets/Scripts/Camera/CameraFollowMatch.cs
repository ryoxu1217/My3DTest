using UnityEngine;

public class CameraFollowMatch : MonoBehaviour
{
    public Transform target;

    public float height = 10f; // 高さずれ
    public float distance = 20f; // 後ろずれ
    public float sideOffset = 0f; // 横ずれ
    public float smoothSpeed = 6f; // 移動速度

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

        // プレイヤーと同じ向きにする
        transform.rotation = yRotation;
    }
}
