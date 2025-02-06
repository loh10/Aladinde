using UnityEngine;

public class GrassTrap : MonoBehaviour, ITrapEffect
{
    public void ApplyEffect(GameObject player)
    {
    }

    public void RemoveEffect(GameObject player)
    {
        SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, 1f);
        }
    }

    public void UpdateTransparency(GameObject player)
    {
        SpriteRenderer sprite = player.GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            float overlapPercentage = CalculateOverlap(player);
            float targetAlpha = Mathf.Lerp(1f, 0f, overlapPercentage);
            sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, targetAlpha);
        }
    }

    private float CalculateOverlap(GameObject player)
    {
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        if (playerCollider == null) return 0f;

        Bounds playerBounds = playerCollider.bounds;
        Bounds grassBounds = GetComponent<Collider2D>().bounds;

        float minX = Mathf.Max(playerBounds.min.x, grassBounds.min.x);
        float maxX = Mathf.Min(playerBounds.max.x, grassBounds.max.x);
        float minY = Mathf.Max(playerBounds.min.y, grassBounds.min.y);
        float maxY = Mathf.Min(playerBounds.max.y, grassBounds.max.y);

        if (minX >= maxX || minY >= maxY)
            return 0f;

        float intersectionArea = (maxX - minX) * (maxY - minY);
        float playerArea = playerBounds.size.x * playerBounds.size.y;

        return Mathf.Clamp01(intersectionArea / playerArea);
    }
}
