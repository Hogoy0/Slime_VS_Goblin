using UnityEngine;
using UnityEngine.UI;

public class SlimeBtn : MonoBehaviour
{
    [SerializeField] private SType.SlimeType slimeType;  // 슬라임 타입 필드
    [SerializeField] private Image buttonImage;  // 버튼 이미지
    [SerializeField] private Button button;  // 버튼 컴포넌트

    [Header("딤드 효과 설정")]
    [SerializeField] private float unlockedAlpha = 1f;  // 언락 상태 알파
    [SerializeField] private float lockedAlpha = 0.5f;  // 잠김 상태 알파
    [SerializeField] private int slimeIndex;

    public void OnClick()
    {
        Debug.Log($"슬라임 버튼 클릭: 인덱스 {slimeIndex}");
        GManager.Instance.Spawn_Launching_Slime(slimeIndex);
    }

    // 슬라임 타입 속성 (올바르게 정의)
    public SType.SlimeType SlimeType => slimeType;

    /// <summary>
    /// 버튼 초기화 메서드
    /// </summary>
    public void Initialize(bool isUnlocked)
    {
        SetButtonState(isUnlocked);

        // 클릭 리스너 초기화
        button.onClick.RemoveAllListeners();
        if (isUnlocked)
        {
            button.onClick.AddListener(() =>
            {
                int index = (int)slimeType;
                GManager.Instance.Spawn_Launching_Slime(index);
            });
        }
    }

    /// <summary>
    /// 버튼 상태 설정 메서드
    /// </summary>
    private void SetButtonState(bool isUnlocked)
    {
        buttonImage.color = new Color(
            buttonImage.color.r,
            buttonImage.color.g,
            buttonImage.color.b,
            isUnlocked ? unlockedAlpha : lockedAlpha
        );

        button.interactable = isUnlocked;
    }
    public void SetInteractable(bool isActive)
    {
        if (button != null)
        {
            button.interactable = isActive;
        }
    }

}
