using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Controles (Input System)")]
    [SerializeField] private InputActionProperty moveAction;

    [Header("Configuración de Movimiento")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;
    private Vector3 velocity;

    void Start()
    {
        if (!IsOwner) return;

        characterController = GetComponent<CharacterController>();

        // TRUCO ANTICLIPPING
        if (characterController != null)
        {
            characterController.enabled = false;
            transform.position = new Vector3(transform.position.x, 2f, transform.position.z);
            characterController.enabled = true;
        }

        moveAction.action.Enable();
    }

    void Update()
    {
        if (!IsOwner) return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = transform.right * input.x + transform.forward * input.y;

        if (characterController != null && characterController.enabled)
        {
            characterController.Move(move * speed * Time.deltaTime);

            if (characterController.isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
            }
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);
        }
    }
}