using UnityEngine;

public static class Actions
{
    public static InputActions actions
    {
        get; private set;
    }
   
    static Actions()
    {
        actions = new InputActions();
        actions.Enable();
    }
}

public class CameraControl : MonoBehaviour
{
    float speed = 10f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    void Update()
    {
        Vector2 move = Actions.actions.Player.Move.ReadValue<Vector2>();

        Vector3 delta = Vector3.zero;
        delta += move.y * transform.forward;
        delta += move.x * transform.right;

        delta += transform.up * Actions.actions.Player.Jump.ReadValue<float>();
        delta -= transform.up * Actions.actions.Player.Crouch.ReadValue<float>();

        transform.position += speed * delta * Time.deltaTime;

        Vector2 look = Actions.actions.Player.Look.ReadValue<Vector2>() / 10f;
        transform.Rotate(0, look.x, 0, relativeTo: Space.World);
        transform.Rotate(-look.y, 0, 0, relativeTo: Space.Self);

        float scroll = Actions.actions.Player.ScrollDelta.ReadValue<float>();
        if (scroll > 0.5f) speed *= 2f;
        if (scroll < -0.5f) speed /= 2f;
    }
}
