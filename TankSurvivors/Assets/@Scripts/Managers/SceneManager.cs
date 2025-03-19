using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{
    public void LoadScene(string sceneName, Action pCallback = null)
    {
        StartCoroutine(LoadSceneRoutine(sceneName, pCallback));
    }

    private IEnumerator LoadSceneRoutine(string sceneName, Action pCallback)
    {
        // 로딩시 소리 끄기
        if (SoundManager.Instance != null)
            SoundManager.Instance.StopAllSound();

        // 로딩 씬 먼저 로드
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Loading");

        UI_SceneLoading loadingScene = FindObjectOfType<UI_SceneLoading>();

        if (loadingScene != null)
        {
            loadingScene.ShowTitleLogo();
        }

        // 다음 씬 로드
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // 바로 전환되지 않도록 설정

        // 로딩이 끝날 때까지 대기
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f) // 90% 로딩 완료
            {
                yield return new WaitForSeconds(1f);
             
                if (loadingScene != null)
                {
                    yield return loadingScene.HideTitleLogo();
                }

                asyncLoad.allowSceneActivation = true; // 씬 전환 실행
            }
            yield return null;
        }

        // 콜백 존재시 실행
        if (pCallback != null)
            pCallback.Invoke();
    }
}
