using UnityEngine;
using UnityEngine.UI;

public class UI_StateBar : MonoBehaviour
{
    public float Percentage
    {
        get
        {
            return _percentage;
        }
        set
        {
            if(value != _percentage)
            {
                _percentage = value;
                UpdateBar();
            }
        }
    }
    [Header("Controls")]
    [SerializeField]
    [Range(0f, 1f)]
    private float _percentage = 1f;


    public string Title
    {
        get
        {
            return _title;
        }
        set
        {
            if(value != _title)
            {
                _title = value;
                UpdateTitle();
            }
        }
    }
    [SerializeField]
    private string _title = "XYZ";

    public Gradient Colours = new Gradient();

    [Header("References")]
    public Text TitleText;
    public Image Bar;

    public void Start()
    {
        UpdateBar();
        UpdateTitle();
    }

    public void UpdateBar()
    {
        if (Bar == null)
            return;

        Bar.fillAmount = Mathf.Clamp01(Percentage);
        Bar.color = Colours.Evaluate(Mathf.Clamp01(Percentage));
    }

    public void UpdateTitle()
    {
        if (TitleText == null)
            return;

        TitleText.text = Title;
    }
}