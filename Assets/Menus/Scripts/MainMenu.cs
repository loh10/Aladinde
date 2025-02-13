using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform _settingsWheel;
    
    [SerializeField] private float _settingsWheelSpeed;

    [SerializeField] private CharacterClass _selectedCharacterClass;
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

    public void CloseMenu(GameObject panel)
    {
        panel.SetActive(false);
    }

    public void SelectClass(int classIndex)
    {
        PlayerPrefs.SetInt("SelectedClass", classIndex);
    }

}