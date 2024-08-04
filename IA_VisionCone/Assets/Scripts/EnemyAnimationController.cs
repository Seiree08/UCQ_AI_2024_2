using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationController : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;
    int isRunningHash;
    int isAlertHash;
    public float walkSpeed = 2.0f;
    public float runSpeed = 4.0f; // Velocidad de correr
    public float rotationSpeed = 360.0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isRunningHash = Animator.StringToHash("isRunning");
        isAlertHash = Animator.StringToHash("isAlert");
    }

    // Update is called once per frame
    void Update()
    {
        bool isRunning = animator.GetBool(isRunningHash);
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isAlert = animator.GetBool(isAlertHash);
        bool forwardPressed = Input.GetKey(KeyCode.UpArrow);
        bool backwardPressed = Input.GetKey(KeyCode.DownArrow);
        bool leftPressed = Input.GetKey(KeyCode.LeftArrow);
        bool rightPressed = Input.GetKey(KeyCode.RightArrow);
        bool runPressed = Input.GetKey(KeyCode.RightShift);
        bool alertPressed = Input.GetKeyDown(KeyCode.E);

        Vector3 moveDirection = Vector3.zero;

        if (forwardPressed)
        {
            moveDirection += Vector3.forward;
        }
        if (backwardPressed)
        {
            moveDirection += Vector3.back;
        }
        if (leftPressed)
        {
            moveDirection += Vector3.left;
        }
        if (rightPressed)
        {
            moveDirection += Vector3.right;
        }

        float currentSpeed = walkSpeed;
        if (runPressed && moveDirection != Vector3.zero)
        {
            currentSpeed = runSpeed;
        }

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.Translate(moveDirection.normalized * currentSpeed * Time.deltaTime, Space.World);

            if (!isWalking)
            {
                animator.SetBool(isWalkingHash, true);
            }
            if (runPressed && !isRunning)
            {
                animator.SetBool(isRunningHash, true);
            }
            else if (!runPressed && isRunning)
            {
                animator.SetBool(isRunningHash, false);
            }
        }
        else
        {
            if (isWalking)
            {
                animator.SetBool(isWalkingHash, false);
            }
            if (isRunning)
            {
                animator.SetBool(isRunningHash, false);
            }
        }

        // Lógica para cambiar entre estado "idle" y "isAlert"
        if (alertPressed)
        {
            animator.SetBool(isAlertHash, !isAlert);
        }
    }
}
