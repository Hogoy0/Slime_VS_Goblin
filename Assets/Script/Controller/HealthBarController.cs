using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;  // IEnumerator ���ǵ� ���ӽ����̽�

public class HealthBarController : MonoBehaviour
{
    [SerializeField] private GameObject healthBarUI;    // ü�¹� UI ������Ʈ
    [SerializeField] private Slider healthSlider;       // ü�� �����̴�
    [SerializeField] private TMP_Text healthText;       // ü�� ��ġ �ؽ�Ʈ (�߰�)
    [SerializeField] private float hideDelay = 2.0f;    // �ڵ� ����� ���� �ð�
    [SerializeField] private bool alwaysVisible = false; // �׻� ǥ�� ����

    private Coroutine hideCoroutine;

    /// <summary>
    /// ü�¹� ������Ʈ
    /// </summary>
    public void UpdateHealthBar(int currentHp, int maxHp)
    {
        // �����̴� �� ������Ʈ
        healthSlider.value = (float)currentHp / maxHp;

        // ü�� ��ġ �ؽ�Ʈ ������Ʈ
        if (healthText != null)
        {
            healthText.text = $"{currentHp} / {maxHp}";
        }

        // �׻� ǥ�� ���� Ȯ��
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
    /// �Ͻ������� ü�¹� ǥ��
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
    /// ���� �ð� �� ü�¹� �����
    /// </summary>
    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);
        healthBarUI.SetActive(false);
    }
}
