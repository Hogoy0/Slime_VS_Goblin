using System.Collections;
using UnityEngine;
using TMPro;

public class TitleScene : MonoBehaviour
{
    enum TitleState { WaitingForInput, ShowingButtons }
    TitleState currentState = TitleState.WaitingForInput;

    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private GameObject Btns;

    private bool isTextFadingOut = false; // 페이드 방향 제어
    private float fadeDuration = 1f;      // 페이드 전환 시간
    private float fadeTimer = 1f;         // 초기 상태는 알파 1 (보이는 상태)
    private float buttonDelay = 0.2f;     // 버튼 활성화 딜레이

    void Start()
    {
        // 초기화
        SetAlpha(1f); // 텍스트를 완전히 보이는 상태로 설정
        Btns.SetActive(false); // 버튼 비활성화
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
                // 버튼 활성화 상태 유지
                break;
        }
    }

    void HandleFading()
    {
        // 타이머 업데이트
        fadeTimer += Time.deltaTime / fadeDuration * (isTextFadingOut ? -1 : 1);
        fadeTimer = Mathf.Clamp01(fadeTimer); // 0~1로 제한

        // 알파 값 적용
        SetAlpha(fadeTimer);

        // 알파 값이 0 또는 1에 도달하면 방향 전환
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
        m_text.gameObject.SetActive(false); // 텍스트 비활성화
        yield return new WaitForSeconds(buttonDelay); // 0.5초 대기
        Btns.SetActive(true); // 버튼 활성화
    }
}

