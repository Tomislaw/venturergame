using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct TimeData
{
    private const long _seconds = 1000;
    private const long _minutes = 60000;
    private const long _hours = 3600000;
    private const long _days = 86400000;
    private const long _months = 2592000000;
    private const long _years = 31104000000;

    public TimeData(long time)
    {
        second = (int)(time / _seconds) % 60;
        minute = (int)(time / _minutes) % 60;
        hour = (int)(time / _hours) % 24;
        day = (int)(time / _days) % 30;
        month = (int)(time / _months) % 12;
        year = (int)(time / _years);
    }

    public int second;
    public int minute;
    public int hour;
    public int day;
    public int month;
    public int year;
}

[System.Serializable]
public class WorldEnvironment
{
    public long time;
    public float realTimeToGameTimeFactor = 1;

    public float normalizedDayTime { get => (time % 86400000f) / 86400000f; }

    public TimeData _timeData;
    public TimeData timeData { get => _timeData; }

    public void Update()
    {
        time += (long)(Time.deltaTime * realTimeToGameTimeFactor * 1000);
        _timeData = new TimeData(time);
    }
}

public class World : MonoBehaviour
{
    public WorldEnvironment environment = new WorldEnvironment();

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        environment.Update();
    }
}