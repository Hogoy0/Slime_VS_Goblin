using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float cameraMoveSpeed = 5.0f; // 카메라 이동 속도
    [SerializeField] private float minX = 0.0f; // 카메라 X축 최소값 제한
    [SerializeField] private float maxX = 19.0f; // 카메라 X축 최대값 제한

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        HandleCameraMovement();
    }

    private void HandleCameraMovement()
    {
        Vector3 cameraPosition = cam.transform.position;

        // 왼쪽 이동 (A 키)
        if (Input.GetKey(KeyCode.A) && cameraPosition.x > minX)
        {
            cam.transform.position += Vector3.left * cameraMoveSpeed * Time.deltaTime;
        }

        // 오른쪽 이동 (D 키)
        if (Input.GetKey(KeyCode.D) && cameraPosition.x < maxX) // 괄호 수정
        {
            cam.transform.position += Vector3.right * cameraMoveSpeed * Time.deltaTime;
        }

        // X 좌표를 minX 이상, maxX 이하로 제한
        cameraPosition = cam.transform.position;
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, minX, maxX); // Clamp 사용
        cam.transform.position = cameraPosition;
    }
}
