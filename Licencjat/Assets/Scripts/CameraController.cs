using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float cameraSpeed = 50f;
    public float minZoom = 30f;
    public float maxZoom = 160f;
    public float zoomSpeed = 10f;

    private Vector3 initialPosition;
    private Camera mainCamera;

    private void Start()
    {
        initialPosition = transform.position;
        mainCamera = this.GetComponent<Camera>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, verticalInput, 0) * cameraSpeed * Time.deltaTime;
        transform.Translate(movement);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = initialPosition;
        }

        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if (mainCamera != null)
        {
            float newZoom = mainCamera.fieldOfView - scrollWheel * zoomSpeed;
            newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);
            mainCamera.fieldOfView = newZoom;
        }else
        {
            Debug.Log("Brak kamery");
        }
    }
}
