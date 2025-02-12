using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; 

public class LoadingScreen : MonoBehaviour
{
    public string sceneToLoad = "NomDeLaScene";
    public Slider progressBar; 
    public TMP_Text progressText;  

    private bool loadingComplete = false;

    void Start()
    { 
        StartCoroutine(LoadSceneAsync());
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneToLoad);
        operation.allowSceneActivation = false; 

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); 
            progressBar.value = progress;
            progressText.text = $"{progress * 100:F0}%";

            yield return null;
        }
    }

    void Update()
    {
        if (loadingComplete)
        {
            SceneManager.LoadScene(sceneToLoad);
            gameObject.SetActive(false);
        }
    }
}
