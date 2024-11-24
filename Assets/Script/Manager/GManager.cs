using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class GManager : MonoBehaviour
{
    public static GManager Instance;

    [SerializeField] private SlimeData[] slimeDataList; // 슬라임 데이터 리스트
    [SerializeField] private int maxResources = 200; // 최대 자원
    [SerializeField] private int resourcesPerSec = 10; // 초당 자원 증가량
    [SerializeField] private float resourceInterval = 2.0f; // 자원 증가 간격
    [SerializeField] private Slider costSlider; // 코스트 슬라이더 UI
    [SerializeField] private Text costText; // 코스트 텍스트
    private int currentResources = 0; // 현재 자원

    private bool isDragging = false;
    private Vector2 startpoint, endpoint, direction, force;
    private float distance;

    [SerializeField] private float pushForce = 4f; // 발사 힘
    [SerializeField] private Vector3 launchPosition = new Vector3(-2.6f, -1.8f, 0f); // 발사 위치

    public SummonController Slime { get; private set; } // 현재 소환된 슬라임
    public bool isSlimeReady = false;
    public Trajectory trajectory; // 궤적 표시

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
            Debug.LogError("잘못된 슬라임 인덱스입니다!");
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
                            Debug.Log($"슬라임 {data.m_name} 소환 완료!");
                        }
                        else
                        {
                            Debug.LogError("SummonController가 프리팹에 연결되지 않았습니다.");
                        }
                    }
                    else
                    {
                        Debug.LogError("슬라임 소환 실패!");
                    }
                };
            }
            else
            {
                Debug.LogError("슬라임 프리팹이 설정되지 않았습니다.");
            }
        }
        else
        {
            Debug.Log("자원이 부족합니다!");
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
