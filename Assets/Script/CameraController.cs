using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float cameraMoveSpeed = 5.0f; // ī�޶� �̵� �ӵ�
    [SerializeField] private float minX = 0.0f; // ī�޶� X�� �ּҰ� ����
    [SerializeField] private float maxX = 19.0f; // ī�޶� X�� �ִ밪 ����

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

        // ���� �̵� (A Ű)
        if (Input.GetKey(KeyCode.A) && cameraPosition.x > minX)
        {
            cam.transform.position += Vector3.left * cameraMoveSpeed * Time.deltaTime;
        }

        // ������ �̵� (D Ű)
        if (Input.GetKey(KeyCode.D) && cameraPosition.x < maxX) // ��ȣ ����
        {
            cam.transform.position += Vector3.right * cameraMoveSpeed * Time.deltaTime;
        }

        // X ��ǥ�� minX �̻�, maxX ���Ϸ� ����
        cameraPosition = cam.transform.position;
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, minX, maxX); // Clamp ���
        cam.transform.position = cameraPosition;
    }
}
