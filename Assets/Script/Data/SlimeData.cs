using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "SlimeData", menuName = "Game/SlimeData", order = 1)]
public class SlimeData : CData
{
    public float Cost = 0.0f; // 소환 코스트
    public SType.SlimeType slimeType; // 슬라임 타입
    public AssetReferenceGameObject DiamondPrefab;
    public AssetReferenceGameObject SapphirePrefab;
    public AssetReferenceGameObject TopazPrefab;
    public AssetReferenceGameObject RubyPrefab;
    public AssetReferenceGameObject EmeraldPrefab;
    public AssetReferenceGameObject MergedDiamondPrefab;
    public AssetReferenceGameObject MergedSapphirePrefab;
    public AssetReferenceGameObject MergedTopazPrefab;
    public AssetReferenceGameObject MergedRubyPrefab;
    public AssetReferenceGameObject MergedEmeraldPrefab;

    public AssetReferenceGameObject GetPrefab()
    {
        switch (slimeType)
        {
            case SType.SlimeType.Diamond: return DiamondPrefab;
            case SType.SlimeType.Sapphire: return SapphirePrefab;
            case SType.SlimeType.Topaz: return TopazPrefab;
            case SType.SlimeType.Ruby: return RubyPrefab;
            case SType.SlimeType.Emerald: return EmeraldPrefab;
            case SType.SlimeType.MergedDiamond: return MergedDiamondPrefab;
            case SType.SlimeType.MergedSapphire: return MergedSapphirePrefab;
            case SType.SlimeType.MergedTopaz: return MergedTopazPrefab;
            case SType.SlimeType.MergedRuby: return MergedRubyPrefab;
            case SType.SlimeType.MergedEmerald: return MergedEmeraldPrefab;
            default: return null;
        }
    }
}
