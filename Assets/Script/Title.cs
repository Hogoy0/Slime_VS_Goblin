using System.Collections;
using UnityEngine;
using TMPro;

public class TitleScene : MonoBehaviour
{
    enum TitleState { WaitingForInput, ShowingButtons }
    TitleState currentState = TitleState.WaitingForInput;

    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private GameObject Btns;

    private bool isTextFadingOut = false; // ���̵� ���� ����
    private float fadeDuration = 1f;      // ���̵� ��ȯ �ð�
    private float fadeTimer = 1f;         // �ʱ� ���´� ���� 1 (���̴� ����)
    private float buttonDelay = 0.2f;     // ��ư Ȱ��ȭ ������

    void Start()
    {
        // �ʱ�ȭ
        SetAlpha(1f); // �ؽ�Ʈ�� ������ ���̴� ���·� ����
        Btns.SetActive(false); // ��ư ��Ȱ��ȭ
    }

    void Update()
    {
        switch (currentState)
        {
            case TitleState.WaitingForInput:
                HandleFading();

                if (Input.anyKeyDown)
                {
                    currentState = TitleState.ShowingButtons;
                    StartCoroutine(ActivateButtonsWithDelay());
                }
                break;

            case TitleState.ShowingButtons:
                // ��ư Ȱ��ȭ ���� ����
                break;
        }
    }

    void HandleFading()
    {
        // Ÿ�̸� ������Ʈ
        fadeTimer += Time.deltaTime / fadeDuration * (isTextFadingOut ? -1 : 1);
        fadeTimer = Mathf.Clamp01(fadeTimer); // 0~1�� ����

        // ���� �� ����
        SetAlpha(fadeTimer);

        // ���� ���� 0 �Ǵ� 1�� �����ϸ� ���� ��ȯ
        if (fadeTimer <= 0f)
        {
            fadeTimer = 0f;
            isTextFadingOut = false;
        }
        else if (fadeTimer >= 1f)
        {
            fadeTimer = 1f;
            isTextFadingOut = true;
        }
    }

    void SetAlpha(float alpha)
    {
        Color color = m_text.color;
        color.a = alpha;
        m_text.color = color;
    }

    IEnumerator ActivateButtonsWithDelay()
    {
        m_text.gameObject.SetActive(false); // �ؽ�Ʈ ��Ȱ��ȭ
        yield return new WaitForSeconds(buttonDelay); // 0.5�� ���
        Btns.SetActive(true); // ��ư Ȱ��ȭ
    }
}

