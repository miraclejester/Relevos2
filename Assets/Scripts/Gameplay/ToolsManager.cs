using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ToolsManager : MonoBehaviour
{
    public static ToolsManager Instance { get; private set;}

    [SerializeField]
    private int _toolSlots = 3;
    [SerializeField]
    private List<Tool> _tools = new List<Tool>();

    public UnityAction<Tool[]> OnTools;

    private Tool[] _currentTools;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        } 
    }

    private void Start()
    {
        _currentTools = new Tool[_toolSlots];
    }

    public void SetupTools()
    {
        int index = 0;

        for(int i = 0; i < _toolSlots; i++)
        {
            index = RandomIndex(index);
            _currentTools[i] = _tools[index];
        }

        OnTools?.Invoke(_currentTools);
    }

    public void SelectTool(int index)
    {
        Instantiate(_currentTools[index].PlayerObject, Vector3.zero, Quaternion.identity);
    }

    private int RandomIndex(int lastIndex)
    {
        int index;
        do
        {
            index = Random.Range(0, _tools.Count);
        }
        while(index == lastIndex);

        return index;
    }
}
