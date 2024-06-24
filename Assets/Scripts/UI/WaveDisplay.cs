using UnityEngine;
using TMPro;

public class WaveDisplay : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _indicator;

    private const string DEFAULT_APPEND = "WAVE: ";

    private void Start()
    {
        if(WavesManager.Instance)
            WavesManager.Instance.OnNextWave += NextWave;
    }

    private void OnDestroy()
    {
        if(WavesManager.Instance)
            WavesManager.Instance.OnNextWave -= NextWave;
    }

    private void NextWave(int wave)
    {
        _indicator.text = DEFAULT_APPEND + (wave + 1);
    }
}
