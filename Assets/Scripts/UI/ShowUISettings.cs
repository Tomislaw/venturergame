using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct UIItem
{
    public GameObject gameObject;
    public KeyCode keyToShow;
}

[System.Serializable]
[CreateAssetMenu(fileName = "ShowEvents", menuName = "ScriptableObjects/UI/ShowEvent", order = 1)]
public class ShowUISettings : MonoBehaviour
{
    [SerializeField]
    public UIItem[] items;

    private void Update()
    {
        foreach (var item in items)
        {
            if (Input.GetKeyDown(item.keyToShow))
            {
                if (item.gameObject == null)
                    continue;
                item.gameObject.SetActive(!item.gameObject.activeSelf);
            }
        }
    }
}