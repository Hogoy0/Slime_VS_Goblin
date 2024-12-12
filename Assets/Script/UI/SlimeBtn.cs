using UnityEngine;
using UnityEngine.UI;

public class SlimeBtn : MonoBehaviour
{
    [SerializeField] private SType.SlimeType slimeType;  // ������ Ÿ�� �ʵ�
    [SerializeField] private Image buttonImage;  // ��ư �̹���
    [SerializeField] private Button button;  // ��ư ������Ʈ

    [Header("���� ȿ�� ����")]
    [SerializeField] private float unlockedAlpha = 1f;  // ��� ���� ����
    [SerializeField] private float lockedAlpha = 0.5f;  // ��� ���� ����
    [SerializeField] private int slimeIndex;

    public void OnClick()
    {
        Debug.Log($"������ ��ư Ŭ��: �ε��� {slimeIndex}");
        GManager.Instance.Spawn_Launching_Slime(slimeIndex);
    }

    // ������ Ÿ�� �Ӽ� (�ùٸ��� ����)
    public SType.SlimeType SlimeType => slimeType;

    /// <summary>
    /// ��ư �ʱ�ȭ �޼���
    /// </summary>
    public void Initialize(bool isUnlocked)
    {
        SetButtonState(isUnlocked);

        // Ŭ�� ������ �ʱ�ȭ
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
    /// ��ư ���� ���� �޼���
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
