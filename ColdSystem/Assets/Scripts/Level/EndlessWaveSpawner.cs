using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EndlessWaveSpawner : MonoBehaviour
{
    public static int WaveNumber { get; private set; }
    public static float TimeUntilNextWave { get; private set; }

    [SerializeField] private List<WaveInfo> _waves;

    private Queue<WaveInfo> _waveQueue;
    private List<UnitSpawner> _unitSpawners;
    private WaveInfo _lastWave = null;

    private void Start()
    {
        if (_waves.Count == 0)
        {
            Debug.LogError("No waves were set for the endless wave spawner. Enemies will not spawn as a result.");
        }
        _waveQueue = new Queue<WaveInfo>(_waves);
        _unitSpawners = GetComponentsInChildren<UnitSpawner>().ToList();
        if (_unitSpawners.Count == 0)
        {
            Debug.LogError("Endless wave spawner has no unit spawners as child objects. Enemies will not spawn as a result.");
        }
    }

    private void Update()
    {
        if (TimeUntilNextWave <= 0f)
        {
            StartWave();
        }
        else
        {
            TimeUntilNextWave -= Time.deltaTime;
        }
    }

    private void StartWave()
    {
        WaveInfo wave;
        if (_waveQueue.Count == 0)
        {
            wave = _lastWave;
        }
        else
        {
            wave = _waveQueue.Dequeue();
        }
        if (wave == null || _unitSpawners.Count == 0)
        {
            return;
        }

        WaveNumber++;
        var enemyCountPerSpawner = wave.EnemyCount / _unitSpawners.Count;
        var remainderEnemyCount = wave.EnemyCount % _unitSpawners.Count;
        for (var i = 0; i < _unitSpawners.Count; i++)
        {
            var spawnCount = enemyCountPerSpawner;
            if (remainderEnemyCount > 0)
            {
                spawnCount += 1;
            }
            _unitSpawners[i].SpawnUnits(spawnCount);
            remainderEnemyCount--;
        }

        TimeUntilNextWave = Mathf.Max(wave.Duration, 1) + 1;
        _lastWave = wave;
    }
}

[Serializable]
public class WaveInfo
{
    public float Duration;
    public int EnemyCount;
}
