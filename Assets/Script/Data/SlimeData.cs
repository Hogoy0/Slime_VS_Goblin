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

    public AssetReferenceGameObject GetPrefab()
    {
        switch (slimeType)
        {
            case SType.SlimeType.Diamond: return DiamondPrefab;
            case SType.SlimeType.Sapphire: return SapphirePrefab;
            case SType.SlimeType.Topaz: return TopazPrefab;
            case SType.SlimeType.Ruby: return RubyPrefab;
            case SType.SlimeType.Emerald: return EmeraldPrefab;
            default: return null;
        }
    }
}
