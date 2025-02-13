using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Transform _settingsWheel;
    
    [SerializeField] private float _settingsWheelSpeed;

    [SerializeField] private CharacterClass _selectedCharacterClass;

    [SerializeField] private TMP_Text className;
    [SerializeField] private TMP_Text classDescription;
    [SerializeField] private TMP_Text firstAbilityDescription;
    [SerializeField] private TMP_Text secondAbilityDescription;
    [SerializeField] private TMP_Text life;
    [SerializeField] private TMP_Text speed;
    [SerializeField] private TMP_Text damages;

    public GameObject playButton;

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

    public void ActiveObject()
    {
        if(_selectedCharacterClass != null)
            playButton.SetActive(true);
    }

    public void SelectClass(CharacterClass characterClass)
    {
        _selectedCharacterClass = characterClass;
        SetClassInfosUI();
    }

    void SetClassInfosUI()
    {
        className.text = "Class name:  " + _selectedCharacterClass.className;
        classDescription.text = "Desciption:  " + _selectedCharacterClass.classDescription;
        firstAbilityDescription.text = "First Ability:  " + _selectedCharacterClass.firstAbilityDescription;
        secondAbilityDescription.text = "Second Ability:  " + _selectedCharacterClass.secondAbilityDescription;
        life.text = "Life:  " + _selectedCharacterClass.maxHealth.ToString();
        speed.text = "Speed:  " + _selectedCharacterClass.speed.ToString();
        damages.text = "Damage:  " + _selectedCharacterClass.damageBonus.ToString();
    }
}