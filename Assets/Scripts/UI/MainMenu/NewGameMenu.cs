using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameMenu : MonoBehaviour
{
    public WorldEnvironment environment;
    public Camera worldCamera;

    public float changeEnvironmentTime = 1;
    public float cameraMoveTime = 1;

    public Vector3 cameraMovePos;

    private Vector3 startingCameraPos;

    private void Start()
    {
        startingCameraPos = worldCamera.transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void Activate()
    {
        var date = System.DateTime.Now;
        GlobalTime _timeData = new GlobalTime();
        _timeData.day = 1;
        _timeData.hour = 12;
        var endTime = _timeData.ToLong();
        var startTime = environment.time;

        LeanTween.value(gameObject, startTime, endTime, changeEnvironmentTime).setEase(LeanTweenType.easeOutCubic).setOnUpdate((float val) =>
        {
            environment.time = (long)val;
        });

        LeanTween.value(gameObject, startingCameraPos, cameraMovePos, cameraMoveTime).setEase(LeanTweenType.easeOutCubic).setOnUpdate((Vector3 val) =>
        {
            worldCamera.transform.position = val;
        });
    }

    public void Deactivate()
    {
        var date = System.DateTime.Now;
        GlobalTime _timeData = new GlobalTime();
        _timeData.hour = date.Hour;
        _timeData.minute = date.Minute;
        _timeData.second = date.Second;

        var endTime = _timeData.ToLong();
        var startTime = environment.time;

        LeanTween.value(gameObject, startTime, endTime, changeEnvironmentTime).setEase(LeanTweenType.easeOutCubic).setOnUpdate((float val) =>
        {
            environment.time = (long)val;
        });

        LeanTween.value(gameObject, cameraMovePos, startingCameraPos, cameraMoveTime).setEase(LeanTweenType.easeOutCubic).setOnUpdate((Vector3 val) =>
        {
            worldCamera.transform.position = val;
        });
    }
}