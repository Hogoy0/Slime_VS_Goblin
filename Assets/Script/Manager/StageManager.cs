using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using TMPro;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    private TMP_Text WaveText;                          // ���̺� �ؽ�Ʈ ����
    [SerializeField] private StageData currentStageData;  // ���� �������� ������
    [SerializeField] private Transform[] spawnPoints;     // ��ȯ ��ġ �迭
    [SerializeField] private float goblinInterval = 0.5f; // ��� ���� ����
    [SerializeField] private List<GoblinData> goblinDataList; // ��� ������ ����Ʈ

    private int currentWaveIndex = 0;                  // ���� ���̺� �ε���
    private int remainingGoblinsInWave = 0;            // ���� ���̺���� ���� ��� ��
    private List<GameObject> activeGoblins = new();    // Ȱ��ȭ�� ��� ����Ʈ

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        // ���̾��Ű�� �ִ� WaveText �ڵ� ����
        WaveText = GameObject.Find("WaveText").GetComponent<TMP_Text>();

        LoadStageData();
    }

    private void LoadStageData()
    {
        currentStageData = StageBtnManager.GetSelectedStageData();

        if (currentStageData == null)
        {
            Debug.LogError("���õ� �������� �����Ͱ� �����ϴ�!");
            return;
        }

        Debug.Log($"�������� ������ �ε� �Ϸ�: {currentStageData.stageName}");
    }

    void Start()
    {
        StartStage();
    }

    public void StartStage()
    {
        if (currentStageData == null || currentStageData.waves.Count == 0)
        {
            Debug.LogError("�������� �����Ͱ� ��ȿ���� �ʽ��ϴ�.");
            return;
        }
        StartCoroutine(RunWaves());
    }

    /// <summary>
    /// ���̺� ����
    /// </summary>
    private IEnumerator RunWaves()
    {
        foreach (var wave in currentStageData.waves)
        {
            currentWaveIndex++;
            remainingGoblinsInWave = 0;

            // ���̺� ���� �ؽ�Ʈ ǥ��
            ShowWaveText($"Wave {currentWaveIndex}");

            yield return new WaitForSeconds(1f); // 1�� ���

            Debug.Log($"=== ���̺� {currentWaveIndex} ���� ===");

            int spawnIndex = 0;

            foreach (var goblin in wave.goblinSpawnList)
            {
                remainingGoblinsInWave += goblin.count;
                Debug.Log($"���� ���̺���� ���� ��� ��: {remainingGoblinsInWave}");

                for (int i = 0; i < goblin.count; i++)
                {
                    Transform spawnPoint = GetNextSpawnPoint(ref spawnIndex);
                    SpawnGoblin(goblin.goblinType, spawnPoint.position);
                    yield return new WaitForSeconds(goblinInterval);
                }
            }

            // ����� ��� ����� ������ ���
            yield return new WaitUntil(() => remainingGoblinsInWave == 0);

            Debug.Log($"=== ���̺� {currentWaveIndex} �Ϸ� ===");
        }

        Debug.Log("��� ���̺갡 �Ϸ�Ǿ����ϴ�!");
    }

    /// <summary>
    /// ���̺� �ؽ�Ʈ ǥ��
    /// </summary>
    private void ShowWaveText(string text)
    {
        if (WaveText == null) return;

        WaveText.text = text;
        WaveText.gameObject.SetActive(true);

        // 1�� �� �ؽ�Ʈ ��Ȱ��ȭ
        Invoke(nameof(HideWaveText), 1f);
    }

    /// <summary>
    /// ���̺� �ؽ�Ʈ �����
    /// </summary>
    private void HideWaveText()
    {
        if (WaveText != null)
        {
            WaveText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// ��ȯ ��ġ ����
    /// </summary>
    private Transform GetNextSpawnPoint(ref int spawnIndex)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("��ȯ ��ġ�� �������� �ʾҽ��ϴ�.");
            return null;
        }

        Transform spawnPoint = spawnPoints[spawnIndex];
        spawnIndex = (spawnIndex + 1) % spawnPoints.Length;
        return spawnPoint;
    }

    /// <summary>
    /// ��� ��ȯ
    /// </summary>
    private void SpawnGoblin(GType.GoblinType goblinType, Vector3 spawnPosition)
    {
        GoblinData goblinData = GetGoblinData(goblinType);
        if (goblinData == null)
        {
            Debug.LogError($"GoblinData�� ã�� �� �����ϴ�: {goblinType}");
            return;
        }

        AssetReferenceGameObject prefab = goblinData.GetPrefab();
        if (prefab != null)
        {
            prefab.InstantiateAsync(spawnPosition, Quaternion.identity).Completed += handle =>
            {
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    GameObject spawnedGoblin = handle.Result;

                    // ��� �ʱ�ȭ �� ����Ʈ �߰�
                    GoblinController controller = spawnedGoblin.GetComponent<GoblinController>();
                    if (controller != null)
                    {
                        controller.Initialize(goblinData, spawnPosition);
                    }

                    activeGoblins.Add(spawnedGoblin);
                }
                else
                {
                    Debug.LogError("��� ���� ����!");
                }
            };
        }
    }

    /// <summary>
    /// ��� ��� ����
    /// </summary>
    public void HandleGoblinDeath(GameObject goblin)
    {
        if (activeGoblins.Contains(goblin))
        {
            activeGoblins.Remove(goblin);
            remainingGoblinsInWave--;

            Debug.Log($"���� ���� ��� ��: {activeGoblins.Count}");
            Debug.Log($"���� ���̺���� ���� ��� ��: {remainingGoblinsInWave}");
        }
        Destroy(goblin);
    }

    /// <summary>
    /// ��� ������ �˻�
    /// </summary>
    private GoblinData GetGoblinData(GType.GoblinType goblinType)
    {
        foreach (var data in goblinDataList)
        {
            if (data.goblinType == goblinType)
                return data;
        }
        return null;
    }
}

