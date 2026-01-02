using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] float activeDuration = 2f; 
    [SerializeField] float transitionDuration = 0.1f;

    [SerializeField] Vector3 SpikeActivePosition = Vector3.zero;
    [SerializeField] Vector3 SpikeIdlePosition = new Vector3(0, -.5f, 0);

    [Header("Sounds")]
    [SerializeField] AudioClip spikeActivateSound;
    [SerializeField] AudioSource audioSource;

    [Header("Components")]
    [SerializeField] ParticleSystem ActivationEffect;
    [SerializeField] GameObject spikeMesh;

    private float timer = 0f;
    
    // Các trạng thái của spike trap
    enum EState
    {
        Idle,              // Đang ẩn
        TransitionToActive, // Đang chuyển từ ẩn sang hiển thị
        Active,            // Đang hiển thị
        TransitionToIdle   // Đang chuyển từ hiển thị sang ẩn
    }

    private EState state = EState.Idle;
    
    /// <summary>
    /// Thay đổi trạng thái hiện tại của spike trap
    /// </summary>
    void ChangeState(EState newState)
    {
        state = newState;
        timer = 0f;

        // Phát âm thanh kích hoạt khi chuyển sang trạng thái TransitionToActive
        if(state == EState.TransitionToActive)
        {
            audioSource.PlayOneShot(spikeActivateSound);
        }
    }
    
    private void Start()
    {
        // Khởi tạo trạng thái ban đầu
    }
    
    private void Update()
    {
        // Xử lý chuyển đổi từ ẩn sang hiển thị
        if (state == EState.TransitionToActive)
        {
            // Nội suy vị trí spike từ vị trí ẩn đến vị trí hoạt động
            Vector3 p = Vector3.Lerp(SpikeIdlePosition, SpikeActivePosition, timer / transitionDuration);
            spikeMesh.transform.localPosition = p;
            
            // Phát hiệu ứng hạt
            ActivationEffect.Play();
            
            // Nếu thời gian chuyển đổi kết thúc, chuyển sang trạng thái Active
            if (timer >= transitionDuration)
            {
                ChangeState(EState.Active); 
            }
        }
        // Xử lý chuyển đổi từ hiển thị sang ẩn
        else if (state == EState.TransitionToIdle)
        {
            // Nội suy vị trí spike từ vị trí hoạt động đến vị trí ẩn
            Vector3 p = Vector3.Lerp(SpikeActivePosition, SpikeIdlePosition, timer / transitionDuration);
            spikeMesh.transform.localPosition = p;
            
            // Nếu thời gian chuyển đổi kết thúc, chuyển sang trạng thái Idle
            if (timer >= transitionDuration)
            {
                ChangeState(EState.Idle);
            }
        }
        // Xử lý khi spike đang ở trạng thái hoạt động
        else if(state == EState.Active)
        {
            // Nếu thời gian hoạt động kết thúc, bắt đầu chuyển về ẩn
            if (timer >= activeDuration)
            {
                ChangeState(EState.TransitionToIdle);
            }
        }

        // Cập nhật bộ đếm thời gian
        timer += Time.deltaTime;
    }

    /// <summary>
    /// Kích hoạt spike trap (từ context menu)
    /// </summary>
    [ContextMenu("Activate Spike Trap")]
    void Activate()
    {
        // Chỉ kích hoạt nếu spike đang ở trạng thái Idle
        if (state == EState.Idle)
            ChangeState(EState.TransitionToActive);
    }
    
    /// <summary>
    /// Ẩn model spike (không sử dụng hiện tại)
    /// </summary>
    void HideSpikes()
    {
        spikeMesh.SetActive(false);
    }       
}
