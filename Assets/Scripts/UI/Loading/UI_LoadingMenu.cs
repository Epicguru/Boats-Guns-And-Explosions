using UnityEngine;
using UnityEngine.UI;

public class UI_LoadingMenu : MonoBehaviour
{
    [Header("Controls")]
    [Range(0f, 1f)]
    public float Percentage;

    [Header("References")]
    public Text StepText;
    public Image StepBar;

    public void Update()
    {
        StepText.text = (Percentage * 100f).ToString("n1") + "%";
        StepBar.fillAmount = Percentage;
    }
}