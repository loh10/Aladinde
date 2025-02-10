using TMPro;
using UnityEngine;

[System.Serializable]
public class PlayerScore
{
    public string playerName;
    public int score;
    public GameObject uiElement;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;
}