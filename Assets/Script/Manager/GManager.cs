using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GManager : MonoBehaviour
{
    public static GManager Instance;

    [Header("슬라임 관리")]
    [SerializeField] private List<SlimeBtn> unitButtons;
    [SerializeField] private SlimeData[] slimeDataList;
    [SerializeField] private Transform unitButtonParent;

    [Header("성 관리")]
    [SerializeField] private int castleMaxHp = 1000;
    [SerializeField] private HealthBarController castleHealthBar;

    [Header("자원 관리")]
    [SerializeField] private int maxResources = 200;
    [SerializeField] private int resourcesPerSec = 10;
    [SerializeField] private float resourceInterval = 2.0f;
    [SerializeField] private Text costText;
    [SerializeField] private Slider costSlider;

    [Header("발사대 관리")]
    [SerializeField] private float pushForce = 4f;
    [SerializeField] private Vector3 launchPosition = new Vector3(-2.6f, -1.8f, 0f);
    public Trajectory trajectory;

    [Header("슬라임 공격 관리")]
    [SerializeField] private float slimeAttackDelay = 1.5f; // 슬라임 공격 대기 시간

    // 슬라임 공격 대기 시간 프로퍼티 추가
    public float SlimeAttackDelay => slimeAttackDelay;

    private bool isSpawning = false;
    public GameObject DefeatUI;
    private int castleCurrentHp;
    private int currentResources = 0;
    private bool isDragging = false;
    private bool isGameOver = false;
    private SummonController currentSlime;

    private Vector2 startpoint, endpoint, direction, force;
    private float distance;
    [SerializeField] private float dragThreshold = 0.5f;  // 드래그 민감도 설정

    public bool IsGameOver => isGameOver;
    public int CurrentCastleHp => castleCurrentHp;


    public GameObject defeatUI;
    public GameObject winUI;
    public GameObject pauseUI;



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            castleCurrentHp = castleMaxHp;
            UpdateCastleHealthBar();
        }
        InitializeUI();

    }
    private void InitializeUI()
    {
        if (defeatUI) defeatUI.SetActive(false);
        if (winUI) winUI.SetActive(false);
        if (pauseUI) pauseUI.SetActive(false);
    }

    private void Start()
    {
        costSlider.maxValue = maxResources;
        UpdateCostUI();
        StartCoroutine(GenerateResources());

        StageData currentStageData = StageManager.Instance.GetCurrentStageData();
        if (currentStageData != null)
        {
            InitializeUnitButtonsFromStageData(currentStageData);
        }
        else
        {
            Debug.LogError("스테이지 데이터를 가져올 수 없습니다!");
        }
    }

    private void Update()
    {
        HandleDragging();
    }

    /// <summary>
    /// 성이 피해를 받을 때 호출
    /// </summary>
    public void TakeDamageToCastle(int damage)
    {
        if (isGameOver) return;

        castleCurrentHp -= damage;
        UpdateCastleHealthBar();

        if (castleCurrentHp <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        isGameOver = true;
        DefeatUI.SetActive(true);
    }

    private void UpdateCastleHealthBar()
    {
        if (castleHealthBar == null) return;
        castleHealthBar.UpdateHealthBar(castleCurrentHp, castleMaxHp);
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
        if (costSlider == null || costText == null) return;

        costSlider.value = currentResources;
        costText.text = $"{currentResources}/{maxResources}";
    }


    /// <summary>
    /// 스테이지 데이터 기반 슬라임 버튼 초기화
    /// </summary>
    public void InitializeUnitButtonsFromStageData(StageData currentStageData)
    {
        if (unitButtonParent == null)
        {
            Debug.LogError("유닛 버튼 부모 오브젝트가 설정되지 않았습니다!");
            return;
        }

        unitButtons = new List<SlimeBtn>();

        foreach (Transform child in unitButtonParent)
        {
            SlimeBtn slimeBtn = child.GetComponent<SlimeBtn>();
            if (slimeBtn != null)
            {
                unitButtons.Add(slimeBtn);
            }
        }

        if (unitButtons.Count == 0)
        {
            Debug.LogError("슬라임 버튼이 유닛 버튼 부모 오브젝트에 없습니다!");
            return;
        }

        // **슬라임 버튼 초기화 수행**
        foreach (var button in unitButtons)
        {
            bool isUnlocked = false;

            foreach (var unlockedSlime in currentStageData.unlockedSlime)
            {
                if (unlockedSlime.slimeType == button.SlimeType)
                {
                    isUnlocked = unlockedSlime.isUnlocked;
                    break;
                }
            }

            button.Initialize(isUnlocked);
        }
    }


    private void HandleDragging()
    {
        if (currentSlime == null || currentSlime.IsLaunched || EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            startpoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isDragging = false;
        }

        if (Input.GetMouseButton(0))
        {
            endpoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            distance = Vector2.Distance(startpoint, endpoint);

            if (distance >= dragThreshold && !isDragging)
            {
                isDragging = true;
                OnDragStart();
            }

            if (isDragging)
            {
                OnDrag();
            }
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            isDragging = false;
            OnDragEnd();
        }
    }

    private void OnDragStart()
    {
        currentSlime.DeActivateRb();
        trajectory.show();
    }

    private void OnDrag()
    {
        endpoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        direction = (startpoint - endpoint).normalized;
        force = direction * distance * pushForce;

        trajectory.UpdateDots(currentSlime.transform.position, force);
    }

    private void OnDragEnd()
    {
        if (currentSlime == null || currentSlime.IsLaunched) return;

        // 슬라임 발사 처리
        currentSlime.ActivateRb();
        currentSlime.Push(force);
        trajectory.hide();

        // 발사 상태 설정
        currentSlime.IsLaunched = true;
    }
    public void Spawn_Launching_Slime(int slimeIndex)
    {
        if (slimeIndex < 0 || slimeIndex >= slimeDataList.Length)
        {
            Debug.LogError("잘못된 슬라임 인덱스입니다!");
            return;
        }

        SlimeData newSlimeData = slimeDataList[slimeIndex];

        // 소환 대기 상태 확인
        if (isSpawning)
        {
            Debug.LogWarning("슬라임 소환 중입니다!");
            return;
        }

        // 자원 확인
        if (currentResources < newSlimeData.Cost)
        {
            Debug.Log("자원이 부족합니다!");
            return;
        }

        // 기존 슬라임이 발사대에 남아있으면 초기화 및 자원 환급
        if (currentSlime != null && !currentSlime.IsLaunched)
        {
            RefundCurrentSlime();
        }

        // 중복 소환 방지
        if (currentSlime != null && currentSlime.SlimeData == newSlimeData && !currentSlime.IsLaunched)
        {
            Debug.Log("같은 슬라임이 이미 발사대에 있습니다!");
            return;
        }

        // 자원 차감 및 업데이트
        currentResources -= Mathf.RoundToInt(newSlimeData.Cost);
        UpdateCostUI();

        // 슬라임 소환 시작
        isSpawning = true;
        AssetReferenceGameObject prefabToSpawn = newSlimeData.GetPrefab();

        prefabToSpawn.InstantiateAsync(launchPosition, Quaternion.identity).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject spawnedSlime = handle.Result;
                currentSlime = spawnedSlime.GetComponent<SummonController>();

                if (currentSlime != null)
                {
                    currentSlime.Initialize(newSlimeData);
                    Debug.Log($"{newSlimeData.slimeType} 소환 완료!");
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

            isSpawning = false; // 소환 완료
        };
    }


    /// <summary>
    /// 슬라임 잠금 상태 확인
    /// </summary>
    private bool IsSlimeUnlocked(SlimeData slimeData)
    {
        StageData currentStageData = StageManager.Instance.GetCurrentStageData();

        foreach (var unlockedSlime in currentStageData.unlockedSlime)
        {
            if (unlockedSlime.slimeType == slimeData.slimeType)
            {
                return unlockedSlime.isUnlocked;
            }
        }
        return false;
    }

    /// <summary>
    /// 기존 슬라임 환급 및 발사대 초기화
    /// </summary>
    private void RefundCurrentSlime()
    {
        if (currentSlime == null || currentSlime.IsLaunched) return;

        // 자원 환급
        currentResources = Mathf.Min(currentResources + Mathf.RoundToInt(currentSlime.SlimeData.Cost), maxResources);
        UpdateCostUI();

        // 기존 슬라임 제거
        Destroy(currentSlime.gameObject);
        currentSlime = null;
    }

}

