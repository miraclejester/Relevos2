using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolsDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject _sliderParent;
    [SerializeField]
    private GameObject _slotsParent;
    [SerializeField]
    private Image _durationSlider;
    [SerializeField]
    private Slot[] _slots; 

    private float _duration = 0;

    private void Start()
    {
        if(WavesManager.Instance)
        {
            WavesManager.Instance.OnPrepare += UpdateDuration;
            _duration = WavesManager.Instance.PrepareDuration;
        }

        if(ToolsManager.Instance)
            ToolsManager.Instance.OnTools += ShowTools;

        ShowDisplay(false);
    }

    private void OnDestroy()
    {
        if(WavesManager.Instance)
            WavesManager.Instance.OnPrepare -= UpdateDuration;

        if(ToolsManager.Instance)
            ToolsManager.Instance.OnTools -= ShowTools;
    }

    private void ShowTools(Tool[] currentTool)
    {
        _durationSlider.fillAmount = _duration;

        for(int i = 0; i < currentTool.Length; i++)
        {
            _slots[i].UpdateData(currentTool[i]);
        }

        ShowDisplay(true);
    }

    public void SelectTool(int index)
    {
        if(ToolsManager.Instance)
            ToolsManager.Instance.SelectTool(index);
        
        _slotsParent.SetActive(false);
    }

    private void UpdateDuration(float timer)
    {
        float current = _duration - timer;
        _durationSlider.fillAmount = current / _duration;

        if(current <= 0)
            ShowDisplay(false);
    }

    private void ShowDisplay(bool state)
    {
        _sliderParent.SetActive(state);
        _slotsParent.SetActive(state);
    }
}
