using TMPro;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverScreen;
    [SerializeField] private TextMeshProUGUI _survivalTimeText;
    [SerializeField] private TextMeshProUGUI _killCountText;

    private float _survivalTime = 0f;

    private void Awake()
    {
        _gameOverScreen.SetActive(false);
    }

    private void Update()
    {
        _survivalTime += Time.deltaTime;
    }
}
