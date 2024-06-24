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
    public UnityAction<bool> OnGameFinished;

    private GameState _gameState = GameState.Show;
    private int _currentWave = 0;
    private Wave _wave = null;
    private int _enemiesCount = 0;
    private int _enemiesIndex = 0;

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
                    _timer = 0;
                    StartCoroutine(WaitForNextWave());
                }
                break;
            case GameState.Prepare:
                OnPrepare?.Invoke(_timer);
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
       
    }

    private void NextWave()
    {
        if(GameFinished())
        {
            _gameState = GameState.Victory;
            return;
        }

        _wave = _waves[_currentWave];
        _enemiesIndex = 0;
        _gameState = GameState.OnWave;
        _enemiesCount = _wave.TotalEnemies;
        OnNextWave?.Invoke(_currentWave);
        StartCoroutine(SpawnAllEnemiesForWave());
    }

    bool GameFinished() => _currentWave >= _waves.Count;

    private void FinishWave()
    {
        _currentWave += 1;

        if (GameFinished())
        {
            _gameState = GameState.Victory;
            OnGameFinished?.Invoke(true);
            return;
        }    
        _gameState = GameState.Show;
    }

    private IEnumerator<WaitForSeconds> SpawnAllEnemiesForWave()
    {
        for(int enemyIndex = 0; enemyIndex < _wave.NumberEnemies.Count; enemyIndex++)
        {
            for (int i = 0; i < _wave.NumberEnemies[enemyIndex]; i++)
            {
                Vector2 randomPos = Random.insideUnitCircle * _spawnRadius;
                Vector3 position = _spawnCenter.position + new Vector3(randomPos.x, 0, randomPos.y);
                GameObject enemy = Instantiate(_wave.Enemies[enemyIndex], position, Quaternion.identity);
                Enemy enemyComponent = enemy.GetComponent<Enemy>();
                Debug.Assert(enemyComponent, "No enemy component found in supposed enemy");
                enemyComponent.OnDeath += EnemyDied;
            }
            yield return new WaitForSeconds(_wave.SpawnInterval);
        }
    }

    private IEnumerator<WaitForSeconds> WaitForNextWave()
    {
        yield return new WaitForSeconds(_prepareDuration);
        NextWave();
    }

    private void EnemyDied() => _enemiesCount--;
}