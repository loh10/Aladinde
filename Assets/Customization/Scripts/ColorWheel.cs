using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ColorWheel : MonoBehaviour
{
    public Image playerSpriteColor;
    public Image colorWheel;
    public RectTransform selector;
    private RectTransform wheelRect;

    public GraphicRaycaster raycaster;
    public EventSystem eventSystem;


    private void Start()
    {
        wheelRect = colorWheel.rectTransform;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (IsPointerOverUI(colorWheel.gameObject))
            {
                ProcessClick(Input.mousePosition);
            }
        }
    }

    /// <summary>
    /// Verification of where the pointer is, if it's on the Image the raycast send the result, if not nothing is returned
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    public bool IsPointerOverUI(GameObject target)
    {
        PointerEventData eventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == target)
                return true;
        }

        return false;
    }

    /// <summary>
    /// Calculs to know where is the click and to apply a color in function of where the click happens
    /// </summary>
    /// <param name="screenPosition"></param>
    private void ProcessClick(Vector2 screenPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(wheelRect, screenPosition, Camera.main, out Vector2 localPosition);

        Vector2 wheelCenter = wheelRect.rect.center;
        Vector2 direction = localPosition - wheelCenter;
        float distance = direction.magnitude;
        float radius = wheelRect.rect.width / 2;

        if (distance > radius)
            return;

        selector.localPosition = wheelCenter + direction;

        float angle = Mathf.Atan2(-direction.x, -direction.y) * Mathf.Rad2Deg;
        float hue = (angle + 180) / 360;
        float saturation = Mathf.Clamp01(distance / radius);

        Color selectedColor = Color.HSVToRGB(hue, saturation, 1f);
        playerSpriteColor.color = selectedColor;
    }
}


