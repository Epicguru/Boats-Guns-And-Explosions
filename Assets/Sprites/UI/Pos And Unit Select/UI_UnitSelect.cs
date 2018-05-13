using UnityEngine;
using UnityEngine.UI;

public class UI_UnitSelect : MonoBehaviour
{
    public Vector3 WorldPos;
    public Image Image;
    public Color Colour;

    public void Update()
    {
        Image.color = Colour;
        Vector2 pos = Camera.main.WorldToScreenPoint(WorldPos);
        (transform as RectTransform).anchoredPosition = pos;
    }
}