using UnityEngine;
using UnityEngine.Audio;

public class VolumeInitializer : MonoBehaviour
{
    [SerializeField] private AudioMixer _audioMixer;

    private void Start()
    {
        InitializeRtpc("MasterVolume");
        InitializeRtpc("MusicVolume");
        InitializeRtpc("SfxVolume");
    }

    private void InitializeRtpc(string mixerParameterName)
    {
        var linearVolumeValue = PlayerPrefs.GetFloat(mixerParameterName, 0.5f);
        _audioMixer.SetFloat(mixerParameterName, Mathf.Log10(linearVolumeValue) * 20);
    }
}