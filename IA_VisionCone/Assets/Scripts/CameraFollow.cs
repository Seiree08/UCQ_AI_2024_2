using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // El personaje que la c�mara debe seguir
    public Vector3 offset;          // La distancia desde la c�mara al personaje
    public float smoothSpeed = 0.125f; // Velocidad de suavizado del movimiento de la c�mara

    public float mouseSensitivity = 10f; // Sensibilidad del mouse
    public float verticalLookLimit = 80f; // L�mite de visi�n vertical

    private float currentYaw = 0f; // Rotaci�n horizontal actual
    private float currentPitch = 0f; // Rotaci�n vertical actual

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor en el centro de la pantalla
        Cursor.visible = false; // Oculta el cursor
    }

    void LateUpdate()
    {
        // Calcula la posici�n deseada de la c�mara
        Vector3 desiredPosition = target.position + offset;
        // Suaviza el movimiento de la c�mara
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        // Actualiza la posici�n de la c�mara
        transform.position = smoothedPosition;

        // Obtener entrada del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Actualizar rotaci�n actual en base a la entrada del mouse
        currentYaw += mouseX;
        currentPitch -= mouseY;
        currentPitch = Mathf.Clamp(currentPitch, -verticalLookLimit, verticalLookLimit);

        // Aplicar rotaci�n a la c�mara
        transform.eulerAngles = new Vector3(currentPitch, currentYaw, 0);
    }
}