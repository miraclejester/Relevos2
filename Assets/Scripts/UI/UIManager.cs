using UnityEngine;

public class UIManager : MonoBehaviour
{

    [SerializeField] private GameObject _victoryUI;
    [SerializeField] private GameObject _defeatUI;

    private void Start() {
        WavesManager.Instance.OnGameFinished += SetupGameFinished;
    }

    private void OnDestroy() {
        WavesManager.Instance.OnGameFinished -= SetupGameFinished;
    }

    private void SetupGameFinished(bool victory) {
        _victoryUI.SetActive(victory);
        _defeatUI.SetActive(!victory);
    }
}
