using UnityEngine;

public class UI_UnitSelect : MonoBehaviour
{
    public Vector3 WorldPos;

    public void Update()
    {
        Vector2 pos = Camera.main.WorldToScreenPoint(WorldPos);
        (transform as RectTransform).anchoredPosition = pos;
    }
}