using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/StageData", order = 1)]
public class StageData : ScriptableObject
{
    public string StageName;             // 스테이지 이름
    public bool IsUnlocked;              // 스테이지 해금 여부
    public bool IsCleared;               // 스테이지 클리어 여부
    public Sprite DefaultSprite;         // 버튼 이미지
    public StageData NextStage;          // 다음 스테이지 참조
    public List<WaveData> waves; // 웨이브 리스트
    public UnitUnlockData[] unlockedSlime;//해금된 슬라임
    public List<Vector3> spawnPoints;
}

[System.Serializable]
public class WaveData
{
    public string waveName; // 웨이브 이름
    public List<GoblinSpawnInfo> goblinSpawnList; // 웨이브에 포함된 고블린 정보
}

[System.Serializable]
public class GoblinSpawnInfo
{
    public GType.GoblinType goblinType; // 고블린 타입
    public int count; // 고블린 수
}
[System.Serializable]
public class UnitUnlockData
{
    public SType.SlimeType slimeType;  // 슬라임 타입
    public bool isUnlocked;  // 해금 여부
}