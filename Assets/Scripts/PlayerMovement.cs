using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovment : MonoBehaviour
{
    public static readonly int HashMove = Animator.StringToHash("Move");
    public float moveSpeed = 5f;
    public float rotateSpeed = 180f;
    public Camera mainCamera;
    private Animator playerAnimator;
    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;

    private void Awake()
    // 아래 입력 내용을 알아서 가져와라.
    {
        playerAnimator = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }
    private void Start()
    {

    }

    private void Update() // 매 프레임 마다 업데이트.
    {
        
    }

    private void FixedUpdate() // 매 프레임마다 물리 관련 업데이트.
    {
        float moveX = playerInput.Rotate;
        float moveZ = playerInput.Move;

        Vector3 moveDir = new Vector3(moveX, 0f, moveZ).normalized;
        playerRigidbody.linearVelocity = new Vector3(moveDir.x * moveSpeed, playerRigidbody.linearVelocity.y, moveDir.z * moveSpeed);
        bool Moving = moveDir.magnitude > 0.1f;
        playerAnimator.SetBool(HashMove, Moving);
        RotateTowardsMouse();
    }

    void RotateTowardsMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 targetPoint = ray.GetPoint(rayDistance);
            Vector3 targetDirection = targetPoint - transform.position;
            targetDirection.y = 0f; 

            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion newRotation = Quaternion.Slerp(playerRigidbody.rotation, targetRotation, 20f * Time.deltaTime);
                playerRigidbody.MoveRotation(newRotation);
            }
        }
    }
}