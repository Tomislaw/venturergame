using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LocalEntrace : MonoBehaviour, IInteractable
{
    public Zone outdoors;
    public Zone indoors;

    public UnityEvent OnPlayerEnter = new UnityEvent();
    public UnityEvent OnPlayerExit = new UnityEvent();

    public void Enter(GameObject enter)
    {
        if (indoors != null)
            enter.transform.parent = indoors.transform;

        var zoneObject = enter.GetComponent<IZoneObject>();
        if (zoneObject != null)
            zoneObject.Zone = indoors;

        if (enter.GetComponent<DebugPlayer>())
            OnPlayerEnter.Invoke();
    }

    public void Exit(GameObject exit)
    {
        if (indoors != null)
            exit.transform.parent = outdoors.transform;

        var zoneObject = exit.GetComponent<IZoneObject>();
        if (zoneObject != null)
            zoneObject.Zone = outdoors;

        if (exit.GetComponent<DebugPlayer>())
            OnPlayerExit.Invoke();
    }

    public void Use(InteractionData data)
    {
        if (data.caller.transform.parent == outdoors.transform)
            Enter(data.caller);
        else
            Exit(data.caller);
    }
}