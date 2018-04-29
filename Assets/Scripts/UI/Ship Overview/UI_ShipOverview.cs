using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShipOverview : MonoBehaviour
{
    public static UI_ShipOverview Instance;

    public Ship Ship;

    public UI_StateBar HealthBar;
    public UI_StateBar HullWaterBar;
    public UI_StateBar SunkBar;

    public Text Title;

    public CanvasGroup Group;
    public AnimationCurve Curve;
    public float OpenTime;

    [Header("Extension")]
    [ReadOnly]
    public bool IsExtended;
    public RectTransform ExtendedIcon;
    public float ExtensionTime = 1f;
    public Vector2 YSize;
    public RectTransform ExtensionRect;
    public RectTransform DetailsTextRect;
    public Text DetailsText;

    public bool IsEnemy;

    private float fadeTimer;
    private float exTimer;

    public void OnExtendedButton()
    {
        IsExtended = !IsExtended;
    }

    public void Awake()
    {
        Instance = this;
    }

    public void OnDestroy()
    {
        Instance = null;
    }

    public void Start()
    {
        fadeTimer = 0f;
    }

    public void Update()
    {
        // Update opening and closing.
        UpdateOpenClose();

        // Update icon rotation
        UpdateExtendedIcon();

        // Update Y extension size.
        UpdateExtension();

        if (Ship != null)
        {
            UpdateVisuals();
            UpdateDetailedView();
        }
    }

    private StringBuilder str = new StringBuilder();
    public void UpdateDetailedView()
    {
        if (Ship == null)
            return;

        if (IsEnemy)
        {
            // Do not display any details if this is an enemy ship.
            DetailsText.text = "Cannot see stats or details on this enemy ship...";
            return;
        }

        str.Clear();
        const string BOLD_START = "<b>";
        const string BOLD_END = "</b>";
        const string COLON = " : ";
        const string HYPHEN = " - ";
        const string RED = "<b><color=#ff6666>";
        const string ORANGE = "<b><color=#ffa500ff>";
        const string GREEN = "<b><color=#70ff77>";
        const string END_COLOUR = "</color></b>";
        string DESTROYED = RichText.InBold(RichText.InItalics("<b><color=#ff6666> [DESTROYED]</color></b>"));
        string DESTROYED_ESS = RichText.InBold(RichText.InItalics("<b><color=#ff6666> [DESTROYED] [!]</color></b>"));

        if(Ship.Damage.WaterOntake > 0f)
        {
            str.Append(RED);
            string water = ((Ship.Damage.WaterOntake / Ship.Damage.MaxWater) * 100f).ToString("n1");
            str.Append("--> Taking On Water! ");
            str.Append(water);
            str.Append("% /s");
            str.Append(END_COLOUR);
            str.Append('\n');
        }

        if(Ship.Locomotion.GetAgilityMultiplier() != 1f)
        {
            str.Append("Max throttle at ");
            if(Ship.Locomotion.GetAgilityMultiplier() > 1f)
            {
                str.Append("<b><color=#70ff77>");
            }
            else
            {
                str.Append("<b><color=#ff6666>");
            }
            str.Append((Ship.Locomotion.GetAgilityMultiplier() * 100f).ToString("n1"));
            str.Append('%');
            str.Append("</color></b>");
            str.Append('\n');
        }

        foreach (var part in Ship.DamageModel.Parts)
        {
            str.Append(BOLD_START);
            str.Append(part.Name.Trim());
            str.Append(BOLD_END);
            str.Append(COLON);
            str.Append(Mathf.Ceil(part.CurrentHealth));
            str.Append('/');
            str.Append(Mathf.Ceil(part.MaxHealth));
            str.Append(HYPHEN);
            float p = part.HealthPercentage;
            string c = null;
            if (p >= 0.7f)
            {
                c = GREEN;
            }
            else if(p >= 0.4f)
            {
                c = ORANGE;
            }
            else
            {
                c = RED;
            }
            str.Append(c);
            str.Append(Mathf.RoundToInt(p * 100f));
            str.Append('%');
            str.Append(END_COLOUR);
            if (part.IsDestroyed)
            {
                if (part.IsEssential)
                {
                    str.Append(DESTROYED_ESS);
                }
                else
                {
                    str.Append(DESTROYED);
                }
            }
            str.Append('\n');
        }

        DetailsText.text = str.ToString();
    }

    private void UpdateExtendedIcon()
    {
        float rot = ExtendedIcon.localEulerAngles.z;
        float speed = 1000f * Time.unscaledDeltaTime;
        if (IsExtended)
        {
            rot += speed;
        }
        else
        {
            rot -= speed;
        }
        rot = Mathf.Clamp(rot, 0f, 180f);
        ExtendedIcon.localEulerAngles = new Vector3(0f, 0f, rot);
    }

    private void UpdateExtension()
    {
        YSize.y = YSize.x + 15f + DetailsTextRect.sizeDelta.y;

        if (IsExtended)
        {
            exTimer += Time.unscaledDeltaTime;
        }
        else
        {
            exTimer -= Time.unscaledDeltaTime;
        }
        exTimer = Mathf.Clamp(exTimer, 0f, ExtensionTime);
        float p = Mathf.Clamp01(exTimer / ExtensionTime);

        float size = Mathf.Lerp(YSize.x, YSize.y, p);
        var ex = ExtensionRect.sizeDelta;
        ex.y = size;
        ExtensionRect.sizeDelta = ex;
    }

    private void UpdateOpenClose()
    {
        if (IsOpen())
        {
            fadeTimer += Time.unscaledDeltaTime;
            if (fadeTimer > OpenTime)
                fadeTimer = OpenTime;
        }
        else
        {
            fadeTimer -= Time.unscaledDeltaTime;
            if (fadeTimer < 0f)
                fadeTimer = 0f;
        }

        float p = Mathf.Clamp01(fadeTimer / OpenTime);
        Group.alpha = Curve.Evaluate(p);
    }

    public bool IsOpen()
    {
        return Ship != null;
    }

    public void UpdateVisuals()
    {
        if (Ship == null)
            return;

        HealthBar.IsUnknown = IsEnemy;
        HullWaterBar.IsUnknown = IsEnemy;
        SunkBar.IsUnknown = IsEnemy;

        if (!IsEnemy)
        {
            // Is locally owned ship, show all real details.
            HealthBar.Percentage = Ship.DamageModel.GetAverageHealthPercentage();
            HullWaterBar.Percentage = Ship.Damage.GetWaterPercentage();
            SunkBar.Percentage = Ship.Damage.GetSinkState();
            Title.text = Ship.Unit.Name;
        }
        else
        {
            // Is enemy ship, cannot show real details.
            Title.text = "Enemy " + Ship.Unit.Name;
        }
    }
}