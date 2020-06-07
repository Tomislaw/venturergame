using UnityEngine;

[System.Serializable]
public class WorldEnvironment : MonoBehaviour
{
    public long time;
    public float realTimeToGameTimeFactor = 1;

    public float normalizedDayTime { get => (time % 86400000f) / 86400000f; }

    public bool useRealTime = false;

    public GlobalTime _timeData;

    public GlobalTime timeData
    {
        get => _timeData;
        set
        {
            _timeData = value;
            time = _timeData.ToLong();
        }
    }

#if (UNITY_EDITOR)

    public void OnValidate()
    {
        time = _timeData.ToLong();
    }

#endif

    //#if (!UNITY_EDITOR)
    public void Start()
    {
        if (useRealTime)
        {
            var date = System.DateTime.Now;
            _timeData.hour = date.Hour;
            _timeData.minute = date.Minute;
            _timeData.second = date.Second;
            time = _timeData.ToLong();
        }
    }

    //#endif

    public void Update()
    {
        time += (long)(Time.deltaTime * realTimeToGameTimeFactor * 1000);
        _timeData = new GlobalTime(time);
    }
}