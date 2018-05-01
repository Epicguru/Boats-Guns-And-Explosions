using UnityEngine;
using UnityEngine.UI;

public class UI_LoadingMenu : MonoBehaviour
{
    [Header("Controls")]
    public int TotalSteps;
    public int CurrentStep;

    public string SubLabel = "Loading xyz...";
    [Range(0f, 1f)]
    public float SubProgress;

    [Header("References")]
    public Text StepText;
    public Text SubText;

    public Image StepBar;
    public Image SubBar;

    public void Update()
    {
        StepText.text = CurrentStep + "/" + TotalSteps;
        float p = (float)CurrentStep / TotalSteps;
        StepBar.fillAmount = p;

        SubText.text = SubLabel;
        SubBar.fillAmount = SubProgress;
    }
}