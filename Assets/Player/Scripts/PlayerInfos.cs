using UnityEngine;

public class PlayerInfos : MonoBehaviour
{
    public CharacterClass characterClass;
    [SerializeField] private bool _isBot;
    [SerializeField] private CharacterClass[] _classes;
    public int score;
    private void Awake()
    {
        if (_isBot)
        {
            characterClass = _classes[Random.Range(0, _classes.Length)];
        }
    }
}
