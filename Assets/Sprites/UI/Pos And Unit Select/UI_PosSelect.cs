using UnityEngine;
using UnityEngine.UI;

public class UI_PosSelect : MonoBehaviour
{
    public Color Color = Color.white;

    public Image[] Images;

    public void Update()
    {
        foreach (var image in Images)
        {
            image.color = Color;
        }

        Vector2 worldPos = InputManager.MousePos;
        Vector2 pos = Camera.main.WorldToScreenPoint(worldPos);

        (transform as RectTransform).anchoredPosition = pos;
    }
}