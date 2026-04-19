using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private bool isPaused = false;

    private void Start()
    {
    if (pauseMenuUI != null)
        pauseMenuUI.SetActive(false);

    // Apply saved settings first
    if (SaveLoadManager.Instance != null)
        SaveLoadManager.Instance.ApplySettings();

    if (AudioManager.Instance != null)
    {
        masterSlider.value = AudioManager.Instance.GetMasterVolume();
        musicSlider.value = AudioManager.Instance.GetMusicVolume();
        sfxSlider.value = AudioManager.Instance.GetSFXVolume();
    }

    masterSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
    musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
    sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            TogglePause();
    }

    private void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(isPaused);
    }

    public void Resume()
    {
    isPaused = false;
    Time.timeScale = 1f;
    if (pauseMenuUI != null)
        pauseMenuUI.SetActive(false);
    if (SaveLoadManager.Instance != null)
        SaveLoadManager.Instance.SaveSettings();
    }

    public void QuitToMenu()
    {
    Time.timeScale = 1f;
    if (SaveLoadManager.Instance != null)
        SaveLoadManager.Instance.SaveSettings();
    if (AudioManager.Instance != null)
        AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonClickSound);
    NetworkManager.Singleton.Shutdown();
    SceneManager.LoadScene("MainMenuScene");
    }

    private void OnMasterVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMasterVolume(value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }
}