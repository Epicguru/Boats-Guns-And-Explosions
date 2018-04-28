using UnityEngine;
using UnityEngine.UI;

public class UI_ShipOverview : MonoBehaviour
{
    public Ship Ship;

    public UI_StateBar HealthBar;
    public UI_StateBar HullWaterBar;
    public UI_StateBar SunkBar;

    public Text Title;

    public CanvasGroup Group;
    public AnimationCurve Curve;
    public float OpenTime;

    [ReadOnly]
    public bool IsExtended;
    public RectTransform ExtendedIcon;

    private float timer;

    public void OnExtendedButton()
    {
        IsExtended = !IsExtended;
    }

    public void Start()
    {
        timer = 0f;
    }

    public void Update()
    {
        // Update opening and closing.
        UpdateOpenClose();
    }

    private void UpdateOpenClose()
    {
        if (IsOpen())
        {
            timer += Time.unscaledDeltaTime;
            if (timer > OpenTime)
                timer = OpenTime;
        }
        else
        {
            timer -= Time.unscaledDeltaTime;
            if (timer < 0f)
                timer = 0f;
        }

        float p = Mathf.Clamp01(timer / OpenTime);
        Group.alpha = Curve.Evaluate(p);

        if (Ship != null)
            Title.text = Ship.Unit.Name;

        if (Ship != null)
        {
            UpdateVisuals();
        }
    }

    public bool IsOpen()
    {
        return Ship != null;
    }

    public void UpdateVisuals()
    {
        if (Ship == null)
            return;

        HealthBar.Percentage = Ship.DamageModel.GetAverageHealthPercentage();
        HullWaterBar.Percentage = Ship.Damage.GetWaterPercentage();
        SunkBar.Percentage = Ship.Damage.GetSinkState();
    }
}