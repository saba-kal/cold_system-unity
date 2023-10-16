using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused { get; private set; } = false;

    [SerializeField] private GameObject _pauseScreen;
    [SerializeField] private InputAction _pauseGameInputAction;

    private void OnEnable()
    {
        _pauseGameInputAction.Enable();
    }

    private void OnDisable()
    {
        _pauseGameInputAction.Disable();
    }

    private void Start()
    {
        ResumeGame();
    }

    private void Update()
    {
        if (_pauseGameInputAction.WasPressedThisFrame())
        {
            if (GameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        GameIsPaused = false;
        Time.timeScale = 1f;
        _pauseScreen.SetActive(false);
    }

    public void PauseGame()
    {
        GameIsPaused = true;
        Time.timeScale = 0;
        _pauseScreen.SetActive(true);
    }
}
