using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public static readonly string MoveAxis = "Vertical";
    public static readonly string RotateAxis = "Horizontal";
    public static readonly string FireAxis = "Fire1";

    public float Move { get; private set; }
    public float Rotate { get; private set; }
    public bool Fire { get; private set; }

    private void OnDisable()  // 플레이어가 Die 될때 사용
    {
        Move = 0f;
        Rotate = 0f;
        Fire = false;
    }

    public void Update()
    {
        Move = Input.GetAxisRaw(MoveAxis);
        Rotate = Input.GetAxisRaw(RotateAxis);
        Fire = Input.GetButton(FireAxis);
    }
}