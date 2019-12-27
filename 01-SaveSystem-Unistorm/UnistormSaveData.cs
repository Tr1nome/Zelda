using UnityEngine;
using ORKFramework;

public class UnistormSaveData : MonoBehaviour, ISaveData
{
    private UniStormSystem Unistorm;
    private int StartingMinute = 0;
    private int StartingHour = 0;
    private int Minute = 1;
    private int Hour = 0;
    private int Day = 0;
    private int Month = 0;
    private int Year = 0;

    
    void Start()
    {
        ORK.SaveGame.RegisterCustomData("test", this, false);
        Unistorm = GameObject.FindWithTag("Unistorm").GetComponent<UniStormSystem>();
        GameObject.DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        this.Minute = Unistorm.Minute;
        this.Hour = Unistorm.Hour;
        this.Day = Unistorm.Day;
        this.Month = Unistorm.Month;
        this.Year = Unistorm.Year;
    }

    
    public DataObject SaveGame()
    {
        DataObject data = new DataObject();

        data.Set("minute", this.Minute);
        data.Set("hour", this.Hour);
        data.Set("day", this.Day);

        return data;
    }

    
    public void LoadGame(DataObject data)
    {
        if (data != null)
        {
            data.Get("minute", ref this.Minute);
            data.Get("hour", ref this.Hour);
            data.Get("day", ref this.Day);
            Unistorm.Minute = Minute;
            Unistorm.Hour = Hour;
            Unistorm.Day = Day;
        }
    }
}