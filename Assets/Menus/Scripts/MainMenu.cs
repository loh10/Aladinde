using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform _settingsWheel;
    
    [SerializeField] private float _settingsWheelSpeed;
    private void Start()
    {
        StartCoroutine(SettingsWheelRotation(_settingsWheelSpeed));
    }

    public void OpenPanel(GameObject panel)
    {
        panel.SetActive(true);
    }

    public void LoadSceneByName(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
    private IEnumerator SettingsWheelRotation(float time)
    {
        while (true)
        {
            _settingsWheel.rotation = Quaternion.Euler(0, 0, _settingsWheel.rotation.eulerAngles.z+0.1f);
            yield return new WaitForSeconds(time);
        }
    }
}