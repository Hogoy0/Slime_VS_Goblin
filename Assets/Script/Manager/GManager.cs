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

    [Header("������ ����")]
    [SerializeField] private List<SlimeBtn> unitButtons;
    [SerializeField] private SlimeData[] slimeDataList;
    [SerializeField] private Transform unitButtonParent;

    [Header("�� ����")]
    [SerializeField] private int castleMaxHp = 1000;
    [SerializeField] private HealthBarController castleHealthBar;

    [Header("�ڿ� ����")]
    [SerializeField] private int maxResources = 200;
    [SerializeField] private int resourcesPerSec = 10;
    [SerializeField] private float resourceInterval = 2.0f;
    [SerializeField] private Text costText;
    [SerializeField] private Slider costSlider;

    [Header("�߻�� ����")]
    [SerializeField] private float pushForce = 4f;
    [SerializeField] private Vector3 launchPosition = new Vector3(-2.6f, -1.8f, 0f);
    public Trajectory trajectory;

    [Header("������ ���� ����")]
    [SerializeField] private float slimeAttackDelay = 1.5f; // ������ ���� ��� �ð�

    // ������ ���� ��� �ð� ������Ƽ �߰�
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
    [SerializeField] private float dragThreshold = 0.5f;  // �巡�� �ΰ��� ����

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
            Debug.LogError("�������� �����͸� ������ �� �����ϴ�!");
        }
    }

    private void Update()
    {
        HandleDragging();
    }

    /// <summary>
    /// ���� ���ظ� ���� �� ȣ��
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
    /// �������� ������ ��� ������ ��ư �ʱ�ȭ
    /// </summary>
    public void InitializeUnitButtonsFromStageData(StageData currentStageData)
    {
        if (unitButtonParent == null)
        {
            Debug.LogError("���� ��ư �θ� ������Ʈ�� �������� �ʾҽ��ϴ�!");
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
            Debug.LogError("������ ��ư�� ���� ��ư �θ� ������Ʈ�� �����ϴ�!");
            return;
        }

        // **������ ��ư �ʱ�ȭ ����**
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

        // ������ �߻� ó��
        currentSlime.ActivateRb();
        currentSlime.Push(force);
        trajectory.hide();

        // �߻� ���� ����
        currentSlime.IsLaunched = true;
    }
    public void Spawn_Launching_Slime(int slimeIndex)
    {
        if (slimeIndex < 0 || slimeIndex >= slimeDataList.Length)
        {
            Debug.LogError("�߸��� ������ �ε����Դϴ�!");
            return;
        }

        SlimeData newSlimeData = slimeDataList[slimeIndex];

        // ��ȯ ��� ���� Ȯ��
        if (isSpawning)
        {
            Debug.LogWarning("������ ��ȯ ���Դϴ�!");
            return;
        }

        // �ڿ� Ȯ��
        if (currentResources < newSlimeData.Cost)
        {
            Debug.Log("�ڿ��� �����մϴ�!");
            return;
        }

        // ���� �������� �߻�뿡 ���������� �ʱ�ȭ �� �ڿ� ȯ��
        if (currentSlime != null && !currentSlime.IsLaunched)
        {
            RefundCurrentSlime();
        }

        // �ߺ� ��ȯ ����
        if (currentSlime != null && currentSlime.SlimeData == newSlimeData && !currentSlime.IsLaunched)
        {
            Debug.Log("���� �������� �̹� �߻�뿡 �ֽ��ϴ�!");
            return;
        }

        // �ڿ� ���� �� ������Ʈ
        currentResources -= Mathf.RoundToInt(newSlimeData.Cost);
        UpdateCostUI();

        // ������ ��ȯ ����
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
                    Debug.Log($"{newSlimeData.slimeType} ��ȯ �Ϸ�!");
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

            isSpawning = false; // ��ȯ �Ϸ�
        };
    }


    /// <summary>
    /// ������ ��� ���� Ȯ��
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
    /// ���� ������ ȯ�� �� �߻�� �ʱ�ȭ
    /// </summary>
    private void RefundCurrentSlime()
    {
        if (currentSlime == null || currentSlime.IsLaunched) return;

        // �ڿ� ȯ��
        currentResources = Mathf.Min(currentResources + Mathf.RoundToInt(currentSlime.SlimeData.Cost), maxResources);
        UpdateCostUI();

        // ���� ������ ����
        Destroy(currentSlime.gameObject);
        currentSlime = null;
    }

}

