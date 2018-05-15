using UnityEngine;
using UnityEngine.UI;

public class UI_PosOrUnitSelect : MonoBehaviour
{
    public static UI_PosOrUnitSelect Instance;
    public delegate void SelectionOutput(bool isUnit, Unit unit, Vector2 position);

    [Header("Controls")]
    [ReadOnly]
    public bool Active;
    public bool AllowPositions;
    public bool AllowUnits;
    public Color PosSelectorColour;
    public Color UnitSelectorColour;

    [Header("References")]
    public GameObject All;
    public UI_PosSelect PosSelector;
    public UI_UnitSelect UnitSelector;
    public Text Title;

    private bool overUnit = false;
    private SelectionOutput output;

    public void GetSelection(SelectionOutput t, bool allowPositions, bool allowUnits)
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
        if(!allowPositions && !allowUnits)
        {
            Debug.LogError("You must allow either positions or units, or both, to run this selection query. Ignored.");
            return;
        }
        this.output = t;
        this.AllowPositions = allowPositions;
        this.AllowUnits = allowUnits;
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
            Title.text = "Select a " + (AllowUnits ? "unit" : "") + (AllowPositions && AllowUnits ? " or a " : "") + (AllowPositions ? "position" : "");
            All.SetActive(true);

            // Get unit under mouse, if any...
            overUnit = false;
            Unit u = null;
            if (AllowUnits)
            {
                u = Unit.GetUnder(InputManager.MousePos);
                overUnit = u != null;
            }

            UnitSelector.gameObject.SetActive(overUnit && AllowUnits);
            PosSelector.gameObject.SetActive(!overUnit && AllowPositions);
            PosSelector.Colour = PosSelectorColour;
            UnitSelector.Colour = UnitSelectorColour;

            if (overUnit)
            {
                UnitSelector.WorldPos = u.transform.position;
            }

            if (InputManager.IsDown("Select"))
            {
                // Something has been selected!
                bool isUnit = overUnit && AllowUnits;
                Unit unit = null;
                if (isUnit)
                {
                    unit = u;
                }
                Vector2 position = Vector2.zero;
                if (!isUnit)
                {
                    position = InputManager.MousePos;
                }

                Finish(isUnit, unit, position);
            }
        }        
    }

    public void OnDestroy()
    {
        Instance = null;
    }
}