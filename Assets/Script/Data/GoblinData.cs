using UnityEngine;
using UnityEngine.AddressableAssets;
using static GType;

[CreateAssetMenu(fileName = "GoblinData", menuName = "Game/GoblinData", order = 2)]
public class GoblinData : CData
{
    public float Speed = 0.0f; // 이동속도
    public GType.GoblinType goblinType; // 고블린 타입
    public AssetReferenceGameObject ShovelsPrefab;
    public AssetReferenceGameObject PickaxPrefab;
    public AssetReferenceGameObject DrillPrefab;
    public AssetReferenceGameObject ChiefPrefab;
    public AssetReferenceGameObject BombPrefab;

    public AssetReferenceGameObject GetPrefab()
    {
        switch (goblinType)
        {
            case GType.GoblinType.Shovels: return ShovelsPrefab;
            case GType.GoblinType.Pickax: return PickaxPrefab;
            case GType.GoblinType.Drill: return DrillPrefab;
            case GType.GoblinType.Chief: return ChiefPrefab;
            case GType.GoblinType.Bomb: return BombPrefab;
            default: return null;
        }
    }
}
