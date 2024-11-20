using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    [SerializeField] private StageData currentStageData; // ���� �������� ������
    [SerializeField] private Transform[] spawnPoints; // ��ȯ ��ġ �迭
    [SerializeField] private float goblinSpawnInterval = 0.5f; // ��� ���� ����
    [SerializeField] private float waveInterval = 3.0f; // ���̺� �� ��� �ð�
    [SerializeField] private List<GoblinData> goblinDataList; // ��� ������ ����Ʈ

    private int currentWaveIndex = 0; // ���� ���̺� �ε���
    private List<GameObject> activeGoblins = new List<GameObject>(); // Ȱ��ȭ�� ��� ����Ʈ
    private List<GameObject> activeSlimes = new List<GameObject>(); // Ȱ��ȭ�� ������ ����Ʈ

    void Awake()
    {
        if (Instance == null) Instance = this;
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

    private IEnumerator RunWaves()
    {
        foreach (var wave in currentStageData.waves)
        {
            int spawnIndex = 0;

            foreach (var goblin in wave.goblinSpawnList)
            {
                for (int i = 0; i < goblin.count; i++)
                {
                    Transform spawnPoint = GetNextSpawnPoint(ref spawnIndex);
                    GameObject spawnedGoblin = SpawnGoblin(goblin.goblinType, spawnPoint.position);
                    if (spawnedGoblin != null)
                    {
                        activeGoblins.Add(spawnedGoblin);
                    }
                    yield return new WaitForSeconds(goblinSpawnInterval); // ��� ���� ����
                }
            }

            yield return new WaitUntil(() => activeGoblins.Count == 0); // ��� ��� ���� ���
            currentWaveIndex++;

            yield return new WaitForSeconds(waveInterval); // ���̺� �� ��� �ð�
        }
    }

    private Transform GetNextSpawnPoint(ref int spawnIndex)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return null;
        }

        Transform spawnPoint = spawnPoints[spawnIndex];
        spawnIndex = (spawnIndex + 1) % spawnPoints.Length;
        return spawnPoint;
    }

    private GameObject SpawnGoblin(GType.GoblinType goblinType, Vector3 spawnPosition)
    {
        GoblinData goblinData = GetGoblinData(goblinType);
        if (goblinData == null)
        {
            Debug.LogError($"GoblinData�� ã�� �� �����ϴ�: {goblinType}");
            return null;
        }

        AssetReferenceGameObject prefab = goblinData.GetPrefab();
        if (prefab != null)
        {
            GameObject spawnedGoblin = null;
            prefab.InstantiateAsync(spawnPosition, Quaternion.identity).Completed += handle =>
            {
                if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    spawnedGoblin = handle.Result;
                    GoblinController controller = spawnedGoblin.GetComponent<GoblinController>();
                    if (controller != null)
                    {
                        controller.Initialize(goblinData,spawnPosition);
                    }
                }
            };
            return spawnedGoblin;
        }
        return null;
    }

    private void HandleGoblinDeath(GameObject goblin)
    {
        if (activeGoblins.Contains(goblin))
        {
            activeGoblins.Remove(goblin);
        }
        Destroy(goblin);
    }

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

