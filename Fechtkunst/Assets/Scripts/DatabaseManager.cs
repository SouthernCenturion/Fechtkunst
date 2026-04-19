using UnityEngine;
using SQLite;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance { get; private set; }

    private SQLiteConnection db;

    public string LastWinnerName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        string dbPath = Path.Combine(Application.persistentDataPath, "fechtkunst.db");
        db = new SQLiteConnection(dbPath);
        db.CreateTable<PlayerStats>();
    }

    public void AddWin(string playerName)
    {
        var stats = db.Table<PlayerStats>().Where(p => p.PlayerName == playerName).FirstOrDefault();
        if (stats == null)
        {
            db.Insert(new PlayerStats { PlayerName = playerName, Wins = 1 });
        }
        else
        {
            stats.Wins++;
            db.Update(stats);
        }
    }

    public int GetWins(string playerName)
    {
        var stats = db.Table<PlayerStats>().Where(p => p.PlayerName == playerName).FirstOrDefault();
        return stats != null ? stats.Wins : 0;
    }

    private void OnDestroy()
    {
        db?.Close();
    }
}

public class PlayerStats
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Unique]
    public string PlayerName { get; set; }
    public int Wins { get; set; }
}