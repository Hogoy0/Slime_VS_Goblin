using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "StageData", menuName = "Game/StageData", order = 1)]
public class StageData : ScriptableObject
{
    public string stageName; // 스테이지 이름
    public List<WaveData> waves; // 웨이브 리스트
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
