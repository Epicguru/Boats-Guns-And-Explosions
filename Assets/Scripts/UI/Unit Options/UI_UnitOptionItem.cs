using UnityEngine;
using UnityEngine.UI;

public class UI_UnitOptionItem : MonoBehaviour, IPoolable
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

    public void UpdateVisuals()
    {
        IconImage.sprite = Icon;
        TitleText.text = Title.Trim() + " [{0}]".Form(Key.ToString());
        CountText.text = Count.ToString();
    }

    public void Clicked()
    {
        Debug.Log("Clicked on {0}".Form(Option.ToString()));
    }

    public void Begin(Transform pool)
    {
        gameObject.SetActive(true);
    }

    public void End(Transform pool)
    {
        transform.SetParent(pool);
        gameObject.SetActive(false);
    }
}