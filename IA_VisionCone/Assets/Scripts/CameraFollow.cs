using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;        // El personaje que la cámara debe seguir
    public Vector3 offset;          // La distancia desde la cámara al personaje
    public float smoothSpeed = 0.125f; // Velocidad de suavizado del movimiento de la cámara

    public float mouseSensitivity = 10f; // Sensibilidad del mouse
    public float verticalLookLimit = 80f; // Límite de visión vertical

    private float currentYaw = 0f; // Rotación horizontal actual
    private float currentPitch = 0f; // Rotación vertical actual

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor en el centro de la pantalla
        Cursor.visible = false; // Oculta el cursor
    }

    void LateUpdate()
    {
        // Calcula la posición deseada de la cámara
        Vector3 desiredPosition = target.position + offset;
        // Suaviza el movimiento de la cámara
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        // Actualiza la posición de la cámara
        transform.position = smoothedPosition;

        // Obtener entrada del mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Actualizar rotación actual en base a la entrada del mouse
        currentYaw += mouseX;
        currentPitch -= mouseY;
        currentPitch = Mathf.Clamp(currentPitch, -verticalLookLimit, verticalLookLimit);

        // Aplicar rotación a la cámara
        transform.eulerAngles = new Vector3(currentPitch, currentYaw, 0);
    }
}