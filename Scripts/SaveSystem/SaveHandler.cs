using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using Newtonsoft.Json;

public class SaveHandler : MonoBehaviour
{
    public static SaveHandler Instance { get; private set; }

    [field: SerializeField] public SaveDatabaseSO SaveDatabaseSO { get; private set; }
    [SerializeField] string saveName = "save.sav";

    SaveData saveData;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SaveDatabaseSO.Init();
    }

    void Start()
    {
        if (!SceneLoader.Instance.IsMainMenu)
        {
            Load();
        }
    }

    public void StartNewSave()
    {
        saveData = new SaveData();
        DataHandler.Instance.InitSave(saveData);
        SaveToFile();
    }

    public void Save()
    {
        saveData = new SaveData();

        var saveables = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
        foreach (var saveable in saveables)
        {
            saveable.Save(saveData);
        }

        SaveToFile();
    }

    public void Load()
    {
        saveData = new SaveData();

        string path = Path.Combine(Application.persistentDataPath, saveName);
        saveData = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(path));

        var saveables = FindObjectsOfType<MonoBehaviour>(true).OfType<ISaveable>();
        foreach (var saveable in saveables)
        {
            saveable.Load(saveData);
        }
    }

    public bool HasExistingSave()
    {
        string path = Path.Combine(Application.persistentDataPath, saveName);
        return File.Exists(path);
    }

    void SaveToFile()
    {
        string savePath = Path.Combine(Application.persistentDataPath, saveName);
        File.WriteAllText(savePath, JsonConvert.SerializeObject(saveData));
        using var file = File.CreateText(savePath);
        JsonSerializer serializer = new JsonSerializer
        {
            Formatting = Formatting.Indented,
        };
        serializer.Serialize(file, saveData);
    }
}
