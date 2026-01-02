using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravityScale = 2f;
    [SerializeField] private float turnSpeed = 60f;
    private Vector2 MoveInput;
    private bool jumpInput;
    bool wasGrounded = false;

    [Header("Component References ")]
    [SerializeField] CharacterController characterController;
    [SerializeField] Animator animator;

    [Header("Unity Events ")]
    public UnityEvent jumped;
    public UnityEvent coinCollected;
    public UnityEvent landed;


    /// <summary>
    /// Kiểm tra xem người chơi có đang ở trên mặt đất hay không
    /// </summary>
    public bool Grounded
    {
        get { return characterController.isGrounded; }
    }

    /// <summary>
    /// Vận tốc theo trục Y (chiều dọc) của người chơi
    /// </summary>
    private float verticalVelocity = 0f;

    #region Input Handing Melthods
    /// <summary>
    /// Xử lý input di chuyển từ người chơi
    /// </summary>
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Xử lý input tấn công từ người chơi
    /// </summary>
    public void OnAttack(InputAction.CallbackContext context)
    {

    }

    /// <summary>
    /// Xử lý input nhảy từ người chơi
    /// </summary>
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpInput = true;
        }
        jumpInput = context.performed;
    }
    #endregion

    #region Unity Callback Methods (start, update, etc)
    /// <summary>
    /// Được gọi mỗi frame để cập nhật di chuyển và animation
    /// </summary>
    private void Update()
    {
        UpdateMovement();
        UpdateAnimator();
    }
    #endregion

    #region Charactor Control Method
    /// <summary>
    /// Cập nhật di chuyển và nhảy của nhân vật
    /// </summary>
    void UpdateMovement()
    {
        // Chuyển đổi input 2D thành vector 3D
        Vector3 moveInput3D = new Vector3(MoveInput.x, 0f, MoveInput.y);
        Vector3 motion = moveInput3D * speed * Time.deltaTime;

        // Cập nhật hướng quay của nhân vật
        UpdatePlayerRotation(moveInput3D);

        // Cập nhật vận tốc dọc (trọng lực)
        if (Grounded)
        {
            verticalVelocity = -3f;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime * gravityScale;
        }

        // Xử lý nhảy
        if (jumpInput && Grounded)
        {
            verticalVelocity = Mathf.Sqrt(2f * jumpHeight * Mathf.Abs(Physics.gravity.y * gravityScale));
            jumpInput = false;
            jumped.Invoke();
        }

        // Áp dụng vận tốc dọc vào chuyển động
        motion.y = verticalVelocity * Time.deltaTime;
        characterController.Move(motion);

        // Phát sự kiện khi nhân vật tiếp đất
        if (!wasGrounded && Grounded)
        {
            landed.Invoke();
        }

        wasGrounded = Grounded;
    }

    /// <summary>
    /// Cập nhật hướng quay của nhân vật dựa trên input di chuyển
    /// </summary>
    void UpdatePlayerRotation(Vector3 moveInput)
    {
        // Nếu không có input di chuyển, không quay
        if (moveInput.sqrMagnitude < 0.01f) return;

        // Tính toán góc quay mục tiêu
        Vector3 playerRotation = transform.rotation.eulerAngles;
        playerRotation.y = GetAngleFromVector(moveInput);
        Quaternion targetRotation = Quaternion.Euler(playerRotation);
        float maxDegreesDelta = turnSpeed * Time.deltaTime;

        // Quay nhân vật mượt mà hướng tới mục tiêu
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxDegreesDelta);
    }

    /// <summary>
    /// Tính toán góc quay từ một vector hướng
    /// </summary>
    float GetAngleFromVector(Vector3 direction)
    {
        Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
        return rotation.eulerAngles.y;
    }
    #endregion

    #region Other Methods
    /// <summary>
    /// Cập nhật các tham số animation dựa trên trạng thái hiện tại
    /// </summary>
    void UpdateAnimator()
    {
        bool jump = false;
        bool fall = false;

        // Xác định trạng thái nhảy hoặc rơi
        if (characterController.isGrounded)
        {
            jump = false;
            fall = false;
        }
        else
        {
            if (verticalVelocity >= 0)
            {
                jump = true;
            }
            else
            {
                fall = true;
            }
        }

        // Tính tốc độ ngang của nhân vật
        Vector3 horizontalVelocity = characterController.velocity;
        horizontalVelocity.y = 0f;
        float speed = horizontalVelocity.magnitude;

        // Cập nhật animator
        animator.SetFloat("Speed", speed);
        animator.SetBool("Jump", jump);
        animator.SetBool("Fall", fall);
    }
    #endregion
}
