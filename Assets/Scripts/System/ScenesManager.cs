using System.Collections;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    private static ScenesManager instance;
    public static ScenesManager Instance 
    {
        get
        {
            if(instance == null)
            {
                instance = FindObjectOfType<ScenesManager>();
                if(instance == null)
                {
                    instance = new GameObject("_ScenesManager").AddComponent<ScenesManager>();

                    DontDestroyOnLoad(instance);
                }
            }
            return instance;
        }
    }

    private UnityAction<float> onProgress;
    private UnityAction onComleted;

    private float additionalWaitTime = 1f;

    private AsyncOperation asyncOperation;
    private int currentSceneIndex;

    private float currentProgress;
    private float targetProgress;
    private float progressAnimationMultiplier = 0.5f;

    private int indexMenuScene = 0;
    private int indexGameScene = 1;

    public ScenesManager SetupLoad(UnityAction<float> progress = null, UnityAction completedLoad = null, float additionalWaitTime = 0f)
    {
        onProgress = progress;
        onComleted = completedLoad;

        this.additionalWaitTime = additionalWaitTime;

        return this;
    }
    public void LoadGameScene()
    {
        LoadScene(indexGameScene);
    }
    public void LoadMenuScene()
    {
        LoadScene(indexMenuScene);
    }
    public void LoadScene(int index)
    {
        currentSceneIndex = index;

        StartCoroutine(LoadSceneAsync());
    }

    public void Allow()
    {
        if (Mathf.Approximately(currentProgress, 1))
        {
            asyncOperation.allowSceneActivation = true;
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        currentProgress = targetProgress = 0;

        asyncOperation = SceneManager.LoadSceneAsync(currentSceneIndex);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            targetProgress = asyncOperation.progress / 0.9f;

            currentProgress = Mathf.MoveTowards(currentProgress, targetProgress, progressAnimationMultiplier * Time.deltaTime);

            //Debug.LogError(currentProgress);

            onProgress?.Invoke(currentProgress);

            if (currentProgress == 1)
            {
                if(additionalWaitTime > 0)
                    yield return new WaitForSeconds(additionalWaitTime);
                onComleted?.Invoke();
            }

            yield return null;
        }

        if(currentSceneIndex == indexGameScene)
        {
            AdMobManager.Instance.ShowBanner();
            AdMobManager.Instance.StartDurationADS();
        }
        else if (currentSceneIndex == indexMenuScene)
        {
            AdMobManager.Instance.ShowBanner();
            AdMobManager.Instance.StopDurationADS();
        }
    }
}