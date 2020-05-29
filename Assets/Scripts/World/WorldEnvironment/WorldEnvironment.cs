using UnityEngine;

[System.Serializable]
public class WorldEnvironment : MonoBehaviour
{
    public long time;
    public float realTimeToGameTimeFactor = 1;

    public float normalizedDayTime { get => (time % 86400000f) / 86400000f; }

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

    public void OnValidate()
    {
        time = _timeData.ToLong();
    }

    public void Update()
    {
        time += (long)(Time.deltaTime * realTimeToGameTimeFactor * 1000);
        _timeData = new GlobalTime(time);
    }
}