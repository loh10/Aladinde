using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Unity.Netcode;

public class LeaderboardManager : NetworkBehaviour
{
    public GameObject playerUIPrefab;
    public Transform leaderboardUiParent;

    [Space(10)]
    private List<Transform> _leaderboardUI = new List<Transform>();
    private List<PlayerScore> _listConnectedPlayer = new List<PlayerScore>();

    [Space(25)]
    [Header("Feedback")]
    public float scaleFactor = 1.25f;
    public float duration = 0.5f;

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

        _listConnectedPlayer.Add(newPlayer);
        _listConnectedPlayer = _listConnectedPlayer.OrderByDescending(p => p.score).ToList();

        newPlayerUI.name = $"username : {newPlayer.playerName}";
        _leaderboardUI.Add(newPlayerUI.transform);
        _leaderboardUI = _listConnectedPlayer.Select(p => p.uiElement.transform).ToList();

        ReorderLeaderboardUiParent();
        UpdateLeaderboard();
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddPlayerServerRpc(string playerName, int score, ServerRpcParams rpcParams = default)
    {
        SpawnPlayer(playerName, score);
        UpdateLeaderboardClientRpc();
    }

    [ClientRpc]
    private void UpdateLeaderboardClientRpc()
    {
        UpdateLeaderboard();
    }

    private void ReorderLeaderboardUiParent()
    {
        for (int i = 0; i < _leaderboardUI.Count; i++)
        {
            _leaderboardUI[i].SetSiblingIndex(i);
        }
    }

    private void UpdateLeaderboard()
    {
        _listConnectedPlayer = _listConnectedPlayer.OrderByDescending(p => p.score).ToList();
        _leaderboardUI = _listConnectedPlayer.Select(p => p.uiElement.transform).ToList();

        for (int i = 0; i < _listConnectedPlayer.Count; i++)
        {
            switch (i)
            {
                case < 5:
                    UpdatePlayerUI(_listConnectedPlayer[i], _leaderboardUI[i], i + 1);
                    break;
                case 5:
                    _listConnectedPlayer[i].uiElement.SetActive(true);
                    UpdatePlayerUI(_listConnectedPlayer[i], _leaderboardUI[5], 6);
                    break;
                default:
                    _listConnectedPlayer[i].uiElement.SetActive(false);
                    break;
            }
        }
        ReorderLeaderboardUiParent();
    }

    private void UpdatePlayerUI(PlayerScore player, Transform targetSlot, int rank)
    {
        player.nameText.text = $"{rank}. {player.playerName}";
        player.scoreText.text = player.score.ToString();
    }

    public void PlayFeedback(TextMeshProUGUI scoreText)
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence.Append(scoreText.transform.DOScale(scaleFactor, duration / 2).SetEase(Ease.OutQuad));
        sequence.Append(scoreText.transform.DOScale(1f, duration / 2).SetEase(Ease.InQuad));
        sequence.Play();
    }
}
