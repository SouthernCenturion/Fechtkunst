using UnityEngine;
using System.IO;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private SettingsData settingsData;
    private string settingsPath;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        settingsPath = Path.Combine(Application.persistentDataPath, "settings.json");
        LoadSettings();
    }

    // Called after AudioManager is ready
    private void Start()
    {
        ApplySettings();
    }

    public void SaveSettings()
    {
        settingsData.masterVolume = AudioManager.Instance.GetMasterVolume();
        settingsData.musicVolume = AudioManager.Instance.GetMusicVolume();
        settingsData.sfxVolume = AudioManager.Instance.GetSFXVolume();

        string json = JsonUtility.ToJson(settingsData, true);
        File.WriteAllText(settingsPath, json);
        Debug.Log("Settings saved!");
    }

    public void LoadSettings()
    {
        if (File.Exists(settingsPath))
        {
            try
            {
                string json = File.ReadAllText(settingsPath);
                settingsData = JsonUtility.FromJson<SettingsData>(json);
                Debug.Log("Settings loaded!");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load settings: " + e.Message);
                settingsData = new SettingsData();
            }
        }
        else
        {
            settingsData = new SettingsData();
            Debug.Log("No settings file found, using defaults.");
        }
    }

    public void ApplySettings()
    {
        if (AudioManager.Instance == null) return;
        AudioManager.Instance.SetMasterVolume(settingsData.masterVolume);
        AudioManager.Instance.SetMusicVolume(settingsData.musicVolume);
        AudioManager.Instance.SetSFXVolume(settingsData.sfxVolume);
    }

    public SettingsData GetSettingsData()
    {
        return settingsData;
    }

    private void OnApplicationQuit()
    {
        if (AudioManager.Instance != null)
            SaveSettings();
    }
}