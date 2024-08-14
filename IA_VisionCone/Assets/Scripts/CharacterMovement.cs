using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float crouchSpeed = 2.5f;
    public float gravity = -9.81f;
    public float jumpForce = 5f;
    public Transform cameraTransform; // Referencia a la c�mara

    private float currentSpeed;
    private Vector3 velocity;
    private CharacterController controller;
    private Animator animator;

    private bool isCrouching = false;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        currentSpeed = walkSpeed;
    }

    void Update()
    {
        // Verificar si est� en el suelo
        isGrounded = controller.isGrounded;

        // Aplicar la gravedad si no est� en el suelo
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Mantener al personaje en el suelo
        }

        // Movimiento b�sico con WASD
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Calcular direcci�n de movimiento en relaci�n con la c�mara
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Ajustar la direcci�n de movimiento para que siga la orientaci�n de la c�mara
        forward.y = 0; // Evitar que la direcci�n de movimiento tenga componente vertical
        right.y = 0;   // Evitar que la direcci�n de movimiento tenga componente vertical
        forward.Normalize();
        right.Normalize();

        // Direcci�n de movimiento en el espacio del mundo
        Vector3 move = (forward * moveZ + right * moveX).normalized;

        // Aplicar movimiento
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Aplicar gravedad
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // Correr con Left Shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = isCrouching ? crouchSpeed * 1.5f : runSpeed;
            animator.SetBool("IsRunning", true);
        }
        else
        {
            currentSpeed = isCrouching ? crouchSpeed : walkSpeed;
            animator.SetBool("IsRunning", false);
        }

        // Agacharse con C
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouching = !isCrouching;
            if (isCrouching)
            {
                controller.height = 1f; // Reducir la altura al agacharse
                currentSpeed = crouchSpeed;
                animator.SetBool("IsCrouching", true);
            }
            else
            {
                controller.height = 2f; // Restaurar la altura al levantarse
                currentSpeed = walkSpeed;
                animator.SetBool("IsCrouching", false);
            }
        }

        // Salto con la barra espaciadora
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity); // Calcula la velocidad de salto
            animator.SetBool("IsJumping", true);
        }
        else if (isGrounded)
        {
            animator.SetBool("IsJumping", false);
        }

        // Configurar animaci�n de caminar
        if (moveX != 0 || moveZ != 0)
        {
            animator.SetBool("IsWalking", true);
            // Rotar el personaje hacia la direcci�n del movimiento
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f); // Ajustar velocidad de rotaci�n seg�n necesidad
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }
}