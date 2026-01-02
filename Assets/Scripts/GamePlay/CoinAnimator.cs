using UnityEngine;

public class CoinAnimator : MonoBehaviour
{
    [SerializeField] float argularSpeed = 50f;
    [SerializeField] float coinHeight = 1f;
    [SerializeField] float movementAmplitude = 0.5f;
    [SerializeField] float movementFrequency = 1f;
    [SerializeField] Transform coinMesh;

    // Được gọi mỗi frame
    void Update()
    {
        // Xoay đồng xu quanh trục Y
        coinMesh.Rotate(0f, argularSpeed * Time.deltaTime, 0f);
        
        // Tính toán độ thay đổi chiều cao dựa trên hàm sin (chuyển động lên xuống)
        float deltaY = movementAmplitude * Mathf.Sin(movementFrequency * Time.time);

        // Cập nhật vị trí của đồng xu (X = 0, Y = chiều cao + chuyển động, Z = 0)
        coinMesh.localPosition = new Vector3(0f, coinHeight + deltaY, 0f);
    }
}
