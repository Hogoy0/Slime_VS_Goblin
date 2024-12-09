using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;  // IEnumerator 정의된 네임스페이스

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private GameObject healthBarUI;    // 체력바 UI 오브젝트
    [SerializeField] private Slider healthSlider;       // 체력 슬라이더
    [SerializeField] private TMP_Text healthText;       // 체력 수치 텍스트 (추가)
    [SerializeField] private float hideDelay = 2.0f;    // 자동 숨기기 지연 시간
    [SerializeField] private bool alwaysVisible = false; // 항상 표시 여부

    private Coroutine hideCoroutine;

    /// <summary>
    /// 체력바 업데이트
    /// </summary>
    public void UpdateHealthBar(int currentHp, int maxHp)
    {
        // 슬라이더 값 업데이트
        healthSlider.value = (float)currentHp / maxHp;

        // 체력 수치 텍스트 업데이트
        if (healthText != null)
        {
            healthText.text = $"{currentHp} / {maxHp}";
        }

        // 항상 표시 여부 확인
        if (alwaysVisible)
        {
            healthBarUI.SetActive(true);
        }
        else
        {
            ShowHealthBarTemporary();
        }
    }

    /// <summary>
    /// 일시적으로 체력바 표시
    /// </summary>
    private void ShowHealthBarTemporary()
    {
        healthBarUI.SetActive(true);

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        hideCoroutine = StartCoroutine(HideAfterDelay());
    }

    /// <summary>
    /// 일정 시간 후 체력바 숨기기
    /// </summary>
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        healthBarUI.SetActive(false);
    }
}
