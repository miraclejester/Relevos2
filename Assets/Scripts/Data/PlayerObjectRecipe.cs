using UnityEngine;

[CreateAssetMenu(fileName = "PlayerObjectRecipe", menuName = "Scriptable Objects/PlayerObjectRecipe")]
public class PlayerObjectRecipe : ScriptableObject
{
    public PlayerObjectData Ingredient;
    public PlayerObjectData Result;
}
