using Unity.Netcode;
using UnityEngine;

public class GrassTrap : MonoBehaviour, ITrapEffect
{
    public void ApplyEffect(GameObject player)
    {
        SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();
        player.GetComponent<PlayerInfos>().canvas.SetActive(false);
        if (sprite != null)
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0f);
    }

    public void RemoveEffect(GameObject player)
    {
        SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();
        player.GetComponent<PlayerInfos>().canvas.SetActive(true);
        if (sprite != null)
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
    }

    public void UpdateTransparency(GameObject player)
    {
    }
}