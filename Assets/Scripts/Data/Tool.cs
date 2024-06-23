using UnityEngine;

[CreateAssetMenu(fileName = "Tool_", menuName = "Scriptable Objects/Gameplay/Tool")]
public class Tool : ScriptableObject
{
    public string Title;
    [TextArea]
    public string Description;
    public int Tier;
    public Sprite Icon;
    public GameObject PlayerObject;
}
