using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class GManager : MonoBehaviour
{
    public static GManager Instance;

    [SerializeField] private SlimeData[] slimeDataList; // ������ ������ ����Ʈ
    [SerializeField] private int maxResources = 200; // �ִ� �ڿ�
    [SerializeField] private int resourcesPerSec = 10; // �ʴ� �ڿ� ������
    [SerializeField] private float resourceInterval = 2.0f; // �ڿ� ���� ����
    [SerializeField] private Slider costSlider; // �ڽ�Ʈ �����̴� UI
    [SerializeField] private Text costText; // �ڽ�Ʈ �ؽ�Ʈ
    private int currentResources = 0; // ���� �ڿ�

    private bool isDragging = false;
    private Vector2 startpoint, endpoint, direction, force;
    private float distance;

    [SerializeField] private float pushForce = 4f; // �߻� ��
    [SerializeField] private Vector3 launchPosition = new Vector3(-2.6f, -1.8f, 0f); // �߻� ��ġ

    public SummonController Slime { get; private set; } // ���� ��ȯ�� ������
    public bool isSlimeReady = false;
    public Trajectory trajectory; // ���� ǥ��

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        costSlider.maxValue = maxResources;
        costSlider.value = currentResources;
        UpdateCostUI();
        StartCoroutine(GenerateResources());
    }

    void Update()
    {
        HandleDragging();
    }

    private void HandleDragging()
    {
        if (Slime != null && isSlimeReady)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                OnDragStart();
            }
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                OnDragEnd();
            }

            if (isDragging) OnDrag();
        }
    }

    private void OnDragStart()
    {
        Slime.DeActivateRb();
        startpoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        trajectory.show();
    }

    private void OnDrag()
    {
        endpoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        distance = Vector2.Distance(startpoint, endpoint);
        direction = (startpoint - endpoint).normalized;
        force = direction * distance * pushForce;

        Debug.DrawLine(startpoint, endpoint);
        trajectory.UpdateDots(Slime.transform.position, force);
    }

    private void OnDragEnd()
    {
        Slime.ActivateRb();
        Slime.Push(force);
        trajectory.hide();
        isSlimeReady = false;
    }

    public void Spawn_Launching_Slime(int slimeIndex)
    {
        if (slimeIndex < 0 || slimeIndex >= slimeDataList.Length)
        {
            Debug.LogError("�߸��� ������ �ε����Դϴ�!");
            return;
        }

        SlimeData data = slimeDataList[slimeIndex];

        if (currentResources >= data.Cost)
        {
            currentResources -= Mathf.RoundToInt(data.Cost);
            UpdateCostUI();

            AssetReferenceGameObject prefabToSpawn = data.GetPrefab();
            if (prefabToSpawn != null)
            {
                prefabToSpawn.InstantiateAsync(launchPosition, Quaternion.identity).Completed += handle =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        GameObject spawnedSlime = handle.Result;
                        Slime = spawnedSlime.GetComponent<SummonController>();
                        if (Slime != null)
                        {
                            Slime.Initialize(data);
                            isSlimeReady = true;
                            Debug.Log($"������ {data.m_name} ��ȯ �Ϸ�!");
                        }
                        else
                        {
                            Debug.LogError("SummonController�� �����տ� ������� �ʾҽ��ϴ�.");
                        }
                    }
                    else
                    {
                        Debug.LogError("������ ��ȯ ����!");
                    }
                };
            }
            else
            {
                Debug.LogError("������ �������� �������� �ʾҽ��ϴ�.");
            }
        }
        else
        {
            Debug.Log("�ڿ��� �����մϴ�!");
        }
    }

    private IEnumerator GenerateResources()
    {
        while (true)
        {
            yield return new WaitForSeconds(resourceInterval);
            AddResources(resourcesPerSec);
        }
    }

    private void AddResources(int amount)
    {
        currentResources = Mathf.Min(currentResources + amount, maxResources);
        UpdateCostUI();
    }

    private void UpdateCostUI()
    {
        if (costSlider != null)
        {
            costSlider.value = currentResources;
        }

        if (costText != null)
        {
            costText.text = $"{currentResources}/{maxResources}";
        }
    }
}
