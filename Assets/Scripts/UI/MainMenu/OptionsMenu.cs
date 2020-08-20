using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenOptions
{
    public static int Resolution { get => PlayerPrefs.GetInt("screen.resolution", 1); set => PlayerPrefs.SetInt("screen.resolution", value); }
    public static bool FullScreen { get => PlayerPrefs.GetInt("screen.fullscreen", 1) == 1; set => PlayerPrefs.SetInt("screen.fullscreen", value ? 1 : 0); }

    public static Vector2Int DefaultResolution;

    public static void Apply()
    {
        var fullScreen = FullScreen;
        switch (Resolution)
        {
            case 0:
                Screen.SetResolution(DefaultResolution.x, DefaultResolution.y, fullScreen);
                break;

            case 1:
                Screen.SetResolution(2560, 1440, fullScreen);
                break;

            case 2:
                Screen.SetResolution(1920, 1080, fullScreen);
                break;

            case 3:
                Screen.SetResolution(1600, 1024, fullScreen);
                break;

            case 4:
                Screen.SetResolution(1600, 900, fullScreen);
                break;

            case 5:
                Screen.SetResolution(1400, 1050, fullScreen);
                break;

            case 6:
                Screen.SetResolution(1366, 768, fullScreen);
                break;

            case 7:
                Screen.SetResolution(1280, 1024, fullScreen);
                break;

            case 8:
                Screen.SetResolution(1280, 960, fullScreen);
                break;

            case 9:
                Screen.SetResolution(1152, 864, fullScreen);
                break;

            case 10:
                Screen.SetResolution(1024, 768, fullScreen);
                break;

            case 11:
                Screen.SetResolution(800, 600, fullScreen);
                break;
        }
    }
}

public class OptionsMenu : MonoBehaviour
{
    public void SetResolution(int res)
    {
        ScreenOptions.Resolution = res;
        ScreenOptions.Apply();
    }

    public void SetFullScreen(bool fullScreen)
    {
        ScreenOptions.FullScreen = fullScreen;
        ScreenOptions.Apply();
    }

    public void Awake()
    {
        if (ScreenOptions.DefaultResolution == new Vector2Int())
        {
#if UNITY_ANDROID
            DisplayMetricsAndroid.Init();
            ScreenOptions.DefaultResolution = new Vector2Int(DisplayMetricsAndroid.WidthPixels, DisplayMetricsAndroid.HeightPixels);
#else
            ScreenOptions.DefaultResolution = new Vector2Int(Screen.width, Screen.height);
#endif
        }
        ScreenOptions.Apply();
    }
}