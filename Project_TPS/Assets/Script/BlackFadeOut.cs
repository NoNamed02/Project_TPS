using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BlackFadeInOut : MonoBehaviour
{
    [SerializeField]
    private float fadeSpeed = 1f; // 페이드 속도를 조절하는 Public 값

    private Image image; // UI Image 컴포넌트
    private bool isFading = false; // 페이드 상태 체크

    void Awake()
    {
        // Image 컴포넌트 초기화
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError("Image 컴포넌트를 찾을 수 없습니다. 프리팹을 확인하세요.");
        }
    }

    public void StartFadeOut()
    {
        if (image == null)
        {
            Debug.LogError("Image 컴포넌트가 초기화되지 않았습니다. 프리팹을 확인하세요.");
            return;
        }

        if (!isFading)
        {
            StartCoroutine(FadeOut());
        }
    }

    public void StartFadeIn()
    {
        if (image == null)
        {
            Debug.LogError("Image 컴포넌트가 초기화되지 않았습니다. 프리팹을 확인하세요.");
            return;
        }

        if (!isFading)
        {
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeOut()
    {
        isFading = true;

        while (image.color.a < 1)
        {
            Color color = image.color;
            color.a += Time.deltaTime * fadeSpeed; // 페이드 속도 조절
            image.color = color;

            yield return null; // 다음 프레임까지 대기
        }

        // 알파값이 1로 고정
        Color finalColor = image.color;
        finalColor.a = 1;
        image.color = finalColor;

        isFading = false; // 페이드 아웃 종료
    }

    private IEnumerator FadeIn()
    {
        isFading = true;

        while (image.color.a > 0)
        {
            Color color = image.color;
            color.a -= Time.deltaTime * fadeSpeed; // 페이드 속도 조절
            image.color = color;

            yield return null; // 다음 프레임까지 대기
        }

        // 알파값이 0으로 고정
        Color finalColor = image.color;
        finalColor.a = 0;
        image.color = finalColor;

        isFading = false; // 페이드 인 종료
    }
}
