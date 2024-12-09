using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using TMPro;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    private TMP_Text WaveText;                          // 웨이브 텍스트 참조
    [SerializeField] private StageData currentStageData;  // 현재 스테이지 데이터
    [SerializeField] private Transform[] spawnPoints;     // 소환 위치 배열
    [SerializeField] private float goblinInterval = 0.5f; // 고블린 생성 간격
    [SerializeField] private List<GoblinData> goblinDataList; // 고블린 데이터 리스트

    private int currentWaveIndex = 0;                  // 현재 웨이브 인덱스
    private int remainingGoblinsInWave = 0;            // 다음 웨이브까지 남은 고블린 수
    private List<GameObject> activeGoblins = new();    // 활성화된 고블린 리스트

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        // 하이어라키에 있는 WaveText 자동 참조
        WaveText = GameObject.Find("WaveText").GetComponent<TMP_Text>();

        LoadStageData();
    }

    private void LoadStageData()
    {
        currentStageData = StageBtnManager.GetSelectedStageData();

        if (currentStageData == null)
        {
            Debug.LogError("선택된 스테이지 데이터가 없습니다!");
            return;
        }

        Debug.Log($"스테이지 데이터 로드 완료: {currentStageData.stageName}");
    }

    void Start()
    {
        StartStage();
    }

    public void StartStage()
    {
        if (currentStageData == null || currentStageData.waves.Count == 0)
        {
            Debug.LogError("스테이지 데이터가 유효하지 않습니다.");
            return;
        }
        StartCoroutine(RunWaves());
    }

    /// <summary>
    /// 웨이브 실행
    /// </summary>
    private IEnumerator RunWaves()
    {
        foreach (var wave in currentStageData.waves)
        {
            currentWaveIndex++;
            remainingGoblinsInWave = 0;

            // 웨이브 시작 텍스트 표시
            ShowWaveText($"Wave {currentWaveIndex}");

            yield return new WaitForSeconds(1f); // 1초 대기

            Debug.Log($"=== 웨이브 {currentWaveIndex} 시작 ===");

            int spawnIndex = 0;

            foreach (var goblin in wave.goblinSpawnList)
            {
                remainingGoblinsInWave += goblin.count;
                Debug.Log($"다음 웨이브까지 남은 고블린 수: {remainingGoblinsInWave}");

                for (int i = 0; i < goblin.count; i++)
                {
                    Transform spawnPoint = GetNextSpawnPoint(ref spawnIndex);
                    SpawnGoblin(goblin.goblinType, spawnPoint.position);
                    yield return new WaitForSeconds(goblinInterval);
                }
            }

            // 고블린이 모두 사망할 때까지 대기
            yield return new WaitUntil(() => remainingGoblinsInWave == 0);

            Debug.Log($"=== 웨이브 {currentWaveIndex} 완료 ===");
        }

        Debug.Log("모든 웨이브가 완료되었습니다!");
    }

    /// <summary>
    /// 웨이브 텍스트 표시
    /// </summary>
    private void ShowWaveText(string text)
    {
        if (WaveText == null) return;

        WaveText.text = text;
        WaveText.gameObject.SetActive(true);

        // 1초 후 텍스트 비활성화
        Invoke(nameof(HideWaveText), 1f);
    }

    /// <summary>
    /// 웨이브 텍스트 숨기기
    /// </summary>
    private void HideWaveText()
    {
        if (WaveText != null)
        {
            WaveText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 소환 위치 관리
    /// </summary>
    private Transform GetNextSpawnPoint(ref int spawnIndex)
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("소환 위치가 설정되지 않았습니다.");
            return null;
        }

        Transform spawnPoint = spawnPoints[spawnIndex];
        spawnIndex = (spawnIndex + 1) % spawnPoints.Length;
        return spawnPoint;
    }

    /// <summary>
    /// 고블린 소환
    /// </summary>
    private void SpawnGoblin(GType.GoblinType goblinType, Vector3 spawnPosition)
    {
        GoblinData goblinData = GetGoblinData(goblinType);
        if (goblinData == null)
        {
            Debug.LogError($"GoblinData를 찾을 수 없습니다: {goblinType}");
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

                    // 고블린 초기화 및 리스트 추가
                    GoblinController controller = spawnedGoblin.GetComponent<GoblinController>();
                    if (controller != null)
                    {
                        controller.Initialize(goblinData, spawnPosition);
                    }

                    activeGoblins.Add(spawnedGoblin);
                }
                else
                {
                    Debug.LogError("고블린 생성 실패!");
                }
            };
        }
    }

    /// <summary>
    /// 고블린 사망 관리
    /// </summary>
    public void HandleGoblinDeath(GameObject goblin)
    {
        if (activeGoblins.Contains(goblin))
        {
            activeGoblins.Remove(goblin);
            remainingGoblinsInWave--;

            Debug.Log($"현재 남은 고블린 수: {activeGoblins.Count}");
            Debug.Log($"다음 웨이브까지 남은 고블린 수: {remainingGoblinsInWave}");
        }
        Destroy(goblin);
    }

    /// <summary>
    /// 고블린 데이터 검색
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

