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
        // �ε��� �Ҹ� ����
        if (SoundManager.Instance != null)
            SoundManager.Instance.StopAllSound();

        // �ε� �� ���� �ε�
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("Loading");

        UI_SceneLoading loadingScene = FindObjectOfType<UI_SceneLoading>();

        if (loadingScene != null)
        {
            loadingScene.ShowTitleLogo();
        }

        // ���� �� �ε�
        AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; // �ٷ� ��ȯ���� �ʵ��� ����

        // �ε��� ���� ������ ���
        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f) // 90% �ε� �Ϸ�
            {
                yield return new WaitForSeconds(1f);
             
                if (loadingScene != null)
                {
                    yield return loadingScene.HideTitleLogo();
                }

                asyncLoad.allowSceneActivation = true; // �� ��ȯ ����
            }
            yield return null;
        }

        // �ݹ� ����� ����
        if (pCallback != null)
            pCallback.Invoke();
    }
}
