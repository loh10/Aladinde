using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.Serialization;

public class LeaderboardManager : MonoBehaviour
{
    public GameObject playerUIPrefab;
    public Transform leaderboardUiParent;
    
    [Space(10)]
    public List<Transform> leaderboardUI = new List<Transform>();
    public List<PlayerScore> listConnectedPlayer = new List<PlayerScore>();

    private void Start()
    {
        SpawnPlayer("est", 10);
        SpawnPlayer("marre", 5);
        SpawnPlayer("Jean", 20);
    }

    

    public void SpawnPlayer(string playerName, int score)
    {
        GameObject newPlayerUI = Instantiate(playerUIPrefab, leaderboardUiParent);
        PlayerScore newPlayer = new PlayerScore
        {
            playerName = playerName,
            score = score,
            uiElement = newPlayerUI,
            nameText = newPlayerUI.transform.Find("NameText").GetComponent<TextMeshProUGUI>(),
            scoreText = newPlayerUI.transform.Find("ScoreText").GetComponent<TextMeshProUGUI>()
        };

        
        
        listConnectedPlayer.Add(newPlayer);
        listConnectedPlayer = listConnectedPlayer.OrderByDescending(p => p.score).ToList();
        
        newPlayerUI.name = $"username : {newPlayer.playerName}";
        leaderboardUI.Add(newPlayerUI.transform);
        leaderboardUI = listConnectedPlayer.Select(p => p.uiElement.transform).ToList();

        ReorderLeaderboardUiParent();
        UpdateLeaderboard();
    }
    
    private void ReorderLeaderboardUiParent()
    {
        for (int i = 0; i < leaderboardUI.Count; i++)
        {
            leaderboardUI[i].SetSiblingIndex(i);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var playerToFind = listConnectedPlayer.FirstOrDefault(p => p.playerName == "est");
            if (playerToFind != null)
            {
                playerToFind.score += 10;
                UpdateLeaderboard();
            }
        }
    }

    private void UpdateLeaderboard()
    {
        listConnectedPlayer = listConnectedPlayer.OrderByDescending(p => p.score).ToList();
        leaderboardUI = listConnectedPlayer.Select(p => p.uiElement.transform).ToList();

        for (int i = 0; i < listConnectedPlayer.Count; i++)
        {
            switch (i)
            {
                case < 5:
                    UpdatePlayerUI(listConnectedPlayer[i], leaderboardUI[i], i + 1);
                    break;
                case 5:
                    listConnectedPlayer[i].uiElement.SetActive(true);
                    UpdatePlayerUI(listConnectedPlayer[i], leaderboardUI[5], 6);
                    break;
                default:
                    listConnectedPlayer[i].uiElement.SetActive(false);
                    break;
            }
        }

        //UpdateOutsideTop6();
        ReorderLeaderboardUiParent();
    }

    private void UpdatePlayerUI(PlayerScore player, Transform targetSlot, int rank)
    {
        player.nameText.text = $"{rank}. {player.playerName}";
        player.scoreText.text = player.score.ToString();
        
        player.uiElement.transform.DOMove(targetSlot.position, 0.5f).SetEase(Ease.OutQuad);
    }

    /*private void UpdateOutsideTop6()
    {
        var activePlayer = listConnectedPlayer.FirstOrDefault(p => p.playerName == "qqqqqqqqqq");
        if (activePlayer != null && !listConnectedPlayer.Take(6).Contains(activePlayer))
        {
            activePlayer.uiElement.SetActive(true);
            //activePlayer.uiElement.transform.position = slot6.position;
            activePlayer.nameText.text = $"{listConnectedPlayer.IndexOf(activePlayer) + 1}e. {activePlayer.playerName}";
            activePlayer.scoreText.text = activePlayer.score.ToString();
            activePlayer.nameText.color = Color.cyan; // Highlight
        }
    }*/
}
