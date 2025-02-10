/*using DG.Tweening;
using UnityEngine;

public class InteractionFeedback : MonoBehaviour
{
    public Transform targetTransform;
    public float scaleFactor = 1.2f;
    public float duration = 0.5f;

    public void PlayFeedback()
    {
        DG.Tweening.Sequence sequence = DOTween.Sequence();
        sequence.Append(targetTransform.DOScale(scaleFactor, duration / 2).SetEase(Ease.OutQuad));
        sequence.Append(targetTransform.DOScale(1f, duration / 2).SetEase(Ease.InQuad));
        sequence.Play();
    }
}*/