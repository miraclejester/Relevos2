using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    [SerializeField]
    private Image _base;
    [SerializeField]
    private TMP_Text _title;
    [SerializeField]
    private TMP_Text _description;
    [SerializeField]
    private Image _icon;

    public void UpdateData(Tool tool)
    {
        _title.text = tool.Title;
        _description.text = tool.Description;
        _base.color = PickColorByTier(tool.Tier);
        _icon.sprite = tool.Icon;
    }

    private Color PickColorByTier(int tier)
    {
        Color current = Color.white;

        switch (tier)
        {
            case 1:
                current = Color.cyan;
                break;
            case 2:
                current = new Color(128, 0, 128);
                break;
            case 3:
                current = Color.yellow;
                break;
            default:
                break;
        }

        return current;
    }
}
