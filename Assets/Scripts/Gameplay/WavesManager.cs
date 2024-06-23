using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public enum GameState
{
    None,
    Show,
    Prepare,
    OnWave,
    Victory,
    Defeat
}

public class WavesManager : MonoBehaviour
{
    public static WavesManager Instance { get; private set;}

    [SerializeField]
    private List<Wave> _waves = new List<Wave>();
    [SerializeField]
    private float _prepareDuration;
    [SerializeField]
    private Transform _spawnCenter;
    [SerializeField]
    private float _spawnRadius;

    public UnityAction<int> OnNextWave;
    public UnityAction<float> OnPrepare;

    private GameState _gameState = GameState.Show;
    private int _currentWave = -1;
    private Wave _wave = null;
    private int _enemiesCount = 0;
    private int _enemiesIndex = -1;

    private float _timer = 0;

    public float PrepareDuration => _prepareDuration;

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

    private void Update()
    {
        _timer += Time.deltaTime;

        switch (_gameState)
        {
            case GameState.Show:
                if(ToolsManager.Instance)
                {
                    ToolsManager.Instance.SetupTools();
                    _gameState = GameState.Prepare;
                }
                break;
            case GameState.Prepare:
                OnPrepare?.Invoke(_timer);
                if(_timer > _prepareDuration)
                {
                    _timer = 0;
                    NextWave();
                }

                break;
            case GameState.OnWave:
                if(_enemiesCount == 0)
                {
                    FinishWave();
                    break;
                }

                UpdateWave();
                break;
            case GameState.Victory:
                break;
            case GameState.Defeat:
                break;
            default:
                break;
        }

    }

    private void UpdateWave()
    {
        if(_timer > _wave.SpawnInterval)
        {
            SpawnEnemies();
            _timer = 0;
        }
    }

    private void NextWave()
    {
        _currentWave += 1;

        if(_currentWave > _waves.Count)
        {
            _gameState = GameState.Victory;
            return;
        }

        _wave = _waves[_currentWave];
        _gameState = GameState.OnWave;
        _enemiesCount = _wave.TotalEnemies;
        OnNextWave?.Invoke(_currentWave + 1);
    }

    private void FinishWave()
    {
        _enemiesIndex = 0;
        _gameState = GameState.Show;
    }

    private void SpawnEnemies()
    {
        _enemiesIndex = Mathf.Min(_enemiesIndex + 1, _wave.NumberEnemies.Count);

        if(_enemiesIndex >= _wave.NumberEnemies.Count)
            return;

        for(int i = 0; i < _wave.NumberEnemies[_enemiesIndex]; i++)
        {
            Vector2 randomPos = Random.insideUnitCircle * _spawnRadius;
            Vector3 position = _spawnCenter.position + new Vector3(randomPos.x, 0, randomPos.y);
            Instantiate(_wave.Enemies[_enemiesIndex], position, Quaternion.identity);
        }
    }
}