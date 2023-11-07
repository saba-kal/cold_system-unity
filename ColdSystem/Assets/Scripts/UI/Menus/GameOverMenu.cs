using System;
using TMPro;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private TextMeshProUGUI _survivalTimeText;
    [SerializeField] private TextMeshProUGUI _killCountText;

    private float _survivalTime = 0f;
    private int _killCount = 0;

    private void Awake()
    {
        _gameOverScreen.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerUnitManager.OnAllPlayerUnitsDestroyed += ShowGameOver;
        Unit.OnUnitDestroyed += OnUnitDestroyed;
    }

    private void OnDisable()
    {
        PlayerUnitManager.OnAllPlayerUnitsDestroyed -= ShowGameOver;
        Unit.OnUnitDestroyed -= OnUnitDestroyed;
    }

    private void Update()
    {
        _survivalTime += Time.deltaTime;
    }

    private void OnUnitDestroyed(Unit unit)
    {
        if (unit.Type == UnitType.Enemy)
        {
            _killCount++;
        }
    }

    private void ShowGameOver()
    {
        _gameOverScreen.SetActive(true);
        _survivalTimeText.text = TimeSpan.FromSeconds(_survivalTime).ToString("mm\\:ss");
        _killCountText.text = _killCount.ToString();
    }
}
