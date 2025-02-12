using UnityEngine;

public class GrassTrap : MonoBehaviour, ITrapEffect
{
    public void ApplyEffect(GameObject player)
    {
        SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();
        if (sprite != null)
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0);
    }

    public void RemoveEffect(GameObject player)
    {
        SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();
        if (sprite != null)
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1);
    }

    public void UpdateTransparency(GameObject player)
    {
    }
}