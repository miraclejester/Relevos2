using System.Collections.Generic;
using UnityEngine;

public struct PlayerObjectMergeData {
    public bool mergeable;
    public PlayerObjectData result;
}

[CreateAssetMenu(fileName = "PlayerObjectData", menuName = "Scriptable Objects/PlayerObjectData")]
public class PlayerObjectData : ScriptableObject
{
    public string ObjectName;
    public int ObjectID;
    public GameObject ObjectPrefab;
    public List<PlayerObjectRecipe> recipes;

    public PlayerObjectMergeData GetMergeData(PlayerObjectData other) {
        PlayerObjectMergeData res = new PlayerObjectMergeData();
        int idx = recipes.FindIndex(po => po.Ingredient == other);
        res.mergeable = idx >= 0;
        res.result = res.mergeable ? recipes[idx].Result : null;
        return res;
    }
}
