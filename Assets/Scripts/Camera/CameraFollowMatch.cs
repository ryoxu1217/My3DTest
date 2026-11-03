using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollowMatch : MonoBehaviour
{
    public Transform target;

    public float height = 10f;
    public float distance = 20f;
    public float sideOffset = 0f;
    public float smoothSpeed = 6f;

    public float sensitivity = 0.08f;
    public float minPitch = -30f;
    public float maxPitch = 50f;

    private float pitch = 0f;

    void LateUpdate()
    {
        // マウスをY方向にどれだけ移動させたか
        float mouseY = Mouse.current.delta.ReadValue().y;

        float factor =
            mouseY < 0
            ? (maxPitch - pitch) / (maxPitch - minPitch)
            : (pitch - minPitch) / (maxPitch - minPitch);

        pitch -= mouseY * sensitivity * factor;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // プレイヤーの向きを取得。その後向きを設定。
        Quaternion yRotation = Quaternion.Euler(0, target.eulerAngles.y, 0);
        Quaternion cameraRotation = Quaternion.Euler(pitch, target.eulerAngles.y, 0);

        // 位置を調整
        Vector3 offset = yRotation * new Vector3(sideOffset, height, -distance);
        Vector3 targetPosition = target.position + offset;

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );

        transform.rotation = cameraRotation;
    }
}