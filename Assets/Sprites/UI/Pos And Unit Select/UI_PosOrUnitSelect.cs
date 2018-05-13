using UnityEngine;
using UnityEngine.UI;

public class UI_PosOrUnitSelect : MonoBehaviour
{
    public static UI_PosOrUnitSelect Instance;
    public delegate void SelectionOutput(bool isUnit, Unit unit, Vector2 position);

    [Header("Controls")]
    [ReadOnly]
    public bool Active;
    public Color PosSelectorColour;
    public Color UnitSelectorColour;

    [Header("References")]
    public GameObject All;
    public UI_PosSelect PosSelector;
    public UI_UnitSelect UnitSelector;

    private bool overUnit = false;
    private SelectionOutput output;

    public void GetSelection(SelectionOutput t)
    {
        if (Active)
        {
            Debug.LogError("Unit or pos selection is already active, cannot use right now!");
            return;
        }
        if(t == null)
        {
            Debug.LogError("The selection output parameter was null.");
            return;
        }
        this.output = t;
        Active = true;
    }

    private void Finish(bool isUnit, Unit unit, Vector2 pos)
    {
        if (!Active)
        {
            Debug.LogError("Finish was called when not active!");
            return;
        }
        Active = false;

        if(this.output != null)
        {
            this.output.Invoke(isUnit, unit, pos);
            this.output = null;
        }
        else
        {
            Debug.LogError("Output is null when finish was called.");
        }
    }

    public void Awake()
    {
        Instance = this;
    }

    public void Update()
    {
        if (!Active)
        {
            All.SetActive(false);
        }
        else
        {
            All.SetActive(true);

            // Get unit under mouse, if any...
            Unit u = Unit.GetUnder(InputManager.MousePos);
            overUnit = u != null;

            UnitSelector.gameObject.SetActive(overUnit);
            PosSelector.gameObject.SetActive(!overUnit);
            PosSelector.Colour = PosSelectorColour;
            UnitSelector.Colour = UnitSelectorColour;

            if (overUnit)
            {
                UnitSelector.WorldPos = u.transform.position;
            }
        }        
    }

    public void OnDestroy()
    {
        Instance = null;
    }
}