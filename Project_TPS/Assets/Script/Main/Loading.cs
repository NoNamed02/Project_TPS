using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image loadingBar; // 로딩 진행 표시를 위한 UI Image (Fill 타입이어야 함)
    [SerializeField] private TextMeshProUGUI loadingText; // 로딩 진행률을 표시할 Text (선택 사항)

    public string sceneToLoad; // 로드할 씬 이름

    private void OnEnable()
    {
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError("Scene name is not set!");
            yield break;
        }

        // 비동기 로딩 시작
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            // 로딩 진행률 업데이트
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            if (loadingBar != null)
            {
                loadingBar.fillAmount = progress;
            }
            if (loadingText != null)
            {
                loadingText.text = $"Loading... {Mathf.RoundToInt(progress * 100)}%";
            }

            // 로딩이 완료되면 씬 활성화
            if (asyncLoad.progress >= 0.9f)
            {
                yield return new WaitForSeconds(0.5f); // 완료 표시를 잠시 보여줌
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
