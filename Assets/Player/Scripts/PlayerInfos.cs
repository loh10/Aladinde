using UnityEngine;

public class PlayerInfos : MonoBehaviour
{
    public CharacterClass characterClass;
    [SerializeField] private bool _isBot;
    [SerializeField] private CharacterClass[] _classes;
    public GameObject canvas;
    public int score;
    public string name;
    public int grillTrophy;
    public int spicesTrophy;
    public int herbsTrophy;
    private void Awake()
    {
        if (_isBot)
        {
            characterClass = _classes[Random.Range(0, _classes.Length)];
        }
    }
}
