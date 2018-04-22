using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    public bool CanSelect = true;
    public Color SelectionColour = Color.white;

    private Vector2 start;
    private SpriteRenderer sel;

    public void Update()
    {
        if (InputManager.IsDown("Select"))
        {
            start = InputManager.MousePos;
        }

        if (InputManager.IsPressed("Select"))
        {
            if (sel == null)
            {
                sel = SelectionBoundsPool.Instance.GetFromPool().Renderer;
            }

            if (sel != null)
            {
                sel.transform.position = start;
                sel.size = InputManager.MousePos - start;
            }
        }
        else
        {
            if(sel != null)
            {
                SelectionBoundsPool.Instance.ReturnToPool(sel.GetComponent<SelectionBounds>());
                sel = null;
            }
        }
    }
}