using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSaveController : MonoBehaviour
{
    [SerializeField]
    private Slider musicSlider = null;

    [SerializeField]
    private Slider soundFXSlider = null;

    [SerializeField]
    private Text musicVolumeText = null;

    [SerializeField]
    private Text soundFXVolumeText = null;

    [SerializeField]
    private AudioSource musicAudioSource = null;

    [SerializeField]
    private AudioSource soundFXAudioSource = null;

    [SerializeField]
    private GameObject settingsScreen = null;

    private void Start()
    {
        LoadVolumeValues();
    }

    public void MusicSlider(float volume)
    {
        if (musicVolumeText != null)
        {
            musicVolumeText.text = volume.ToString("0.0");
        }

        if (musicAudioSource != null)
        {
            musicAudioSource.volume = volume;
        }
    }

    public void SoundFXSlider(float volume)
    {
        if (soundFXVolumeText != null)
        {
            soundFXVolumeText.text = volume.ToString("0.0");
        }

        if (soundFXAudioSource != null)
        {
            soundFXAudioSource.volume = volume;
        }
    }

    public void SaveVolumeButton()
    {
        if (musicSlider != null)
        {
            float musicVolumeValue = musicSlider.value;
            PlayerPrefs.SetFloat("MusicVolumeValue", musicVolumeValue);
        }

        if (soundFXSlider != null)
        {
            float soundFXVolumeValue = soundFXSlider.value;
            PlayerPrefs.SetFloat("SoundFXVolumeValue", soundFXVolumeValue);
        }

        if (settingsScreen != null)
        {
            settingsScreen.SetActive(false);
        }
        LoadVolumeValues();
    }

    public void LoadVolumeValues()
    {
        float musicVolumeValue;
        float soundFXVolumeValue;

        if (PlayerPrefs.HasKey("MusicVolumeValue"))
        {
           musicVolumeValue = PlayerPrefs.GetFloat("MusicVolumeValue");
        } 
        else
        {
            musicVolumeValue = 1.0f;
        }

        if (PlayerPrefs.HasKey("SoundFXVolumeValue"))
        {
            soundFXVolumeValue = PlayerPrefs.GetFloat("SoundFXVolumeValue");
        }
        else
        {
            soundFXVolumeValue = 1.0f;
        }

        if (musicSlider != null)
        {
            musicSlider.value = musicVolumeValue;
        }

        if (soundFXSlider != null)
        {
            soundFXSlider.value = soundFXVolumeValue;
        }

        if (musicAudioSource != null)
        {
            musicAudioSource.volume = musicVolumeValue;
        }

        if (soundFXAudioSource != null)
        {
            soundFXAudioSource.volume = soundFXVolumeValue;
        }
    }
}
