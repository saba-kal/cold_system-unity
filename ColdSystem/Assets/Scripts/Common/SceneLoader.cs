using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Loads a main scene with additive scenes.
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [SerializeField] private bool _releodCurrentScene;
    [SerializeField] private string _sceneName;
    [SerializeField] private List<string> _additiveScenes;

    /// <summary>
    /// Loads a single scene based on scene name.
    /// </summary>
    public void LoadScene()
    {
        if (_releodCurrentScene)
        {
            _sceneName = SceneManager.GetActiveScene().name;
        }

        if (_additiveScenes == null || _additiveScenes.Count == 0)
        {
            StartCoroutine(LoadSceneAsync());
        }
        else
        {
            StartCoroutine(LoadMultipleScenesAsync());
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        var asyncLoad = SceneManager.LoadSceneAsync(_sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private IEnumerator LoadMultipleScenesAsync()
    {
        var asyncLoads = new List<AsyncOperation>
        {
            SceneManager.LoadSceneAsync(_sceneName),
        };

        foreach (var additiveScene in _additiveScenes)
        {
            asyncLoads.Add(SceneManager.LoadSceneAsync(additiveScene, LoadSceneMode.Additive));
        }

        // Wait until the asynchronous scene fully loads
        while (asyncLoads.Any(a => !a.isDone))
        {
            yield return null;
        }
    }
}
