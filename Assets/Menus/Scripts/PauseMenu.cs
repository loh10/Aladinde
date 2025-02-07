using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _settingsPrefab;
    private GameObject _settingsInstance;

    public void Resume()
    {
        gameObject.SetActive(false);
    }

    public void OpenParameters()
    {
        if (_settingsInstance == null)
        {
            _settingsInstance = Instantiate(_settingsPrefab, transform.parent);
        }
        else
        {
            _settingsInstance.SetActive(true); 
        }
    }
}
