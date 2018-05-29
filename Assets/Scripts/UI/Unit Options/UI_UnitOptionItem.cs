using UnityEngine;
using UnityEngine.UI;

public class UI_UnitOptionItem : MonoBehaviour
{
    [Header("Data")]
    public UnitOption Option;
    public Sprite Icon;
    public string Title;
    public KeyCode Key;
    public int Count;

    [Header("References")]
    public Image IconImage;
    public Text TitleText;
    public Text CountText;
    public UI_UnitOptions Options;

    public void UpdateVisuals()
    {
        IconImage.sprite = Icon;
        TitleText.text = Title.Trim() + " [{0}]".Form(Key.ToString());
        CountText.text = "x" + Count.ToString();
    }

    public void Clicked()
    {
        Options.Selected(this);
    }
}