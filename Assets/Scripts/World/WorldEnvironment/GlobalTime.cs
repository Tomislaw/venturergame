[System.Serializable]
public struct GlobalTime
{
    private const long _seconds = 1000;
    private const long _minutes = 60000;
    private const long _hours = 3600000;
    private const long _days = 86400000;
    private const long _months = 2592000000;
    private const long _years = 31104000000;

    public GlobalTime(long time)
    {
        second = (int)(time / _seconds) % 60;
        minute = (int)(time / _minutes) % 60;
        hour = (int)(time / _hours) % 24;
        day = (int)(time / _days) % 30;
        month = (int)(time / _months) % 12;
        year = (int)(time / _years);
    }

    public long ToLong()
    {
        return _seconds * second + _minutes * minute + _hours * hour + _days * day + _months * month + _years * year;
    }

    public int second;
    public int minute;
    public int hour;
    public int day;
    public int month;
    public int year;
}