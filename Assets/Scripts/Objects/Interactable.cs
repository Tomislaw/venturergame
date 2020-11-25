using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public interface IInteractable
{
    void Use(InteractionData data);
}

[Serializable]
public struct InteractionData
{
    public GameObject caller;
}

[Serializable]
public class OnUseEvent : UnityEvent<InteractionData> { }

public class Interactable : MonoBehaviour, IInteractable
{
    public string displayName;
    public OnUseEvent OnUse = new OnUseEvent();

    public InteractionHud interactionHud;
    private InteractionHud _interactionHud;
    public Vector3 interactionHudOffset;

    public int Layer = 0;

    public void SetDisplayName(string name)
    {
        displayName = name;
        if (_interactionHud)
            _interactionHud.Text = name;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnPlayerExit(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnPlayerEnter(collision.gameObject);
    }

    private void OnPlayerEnter(GameObject gameObject)
    {
        var player = gameObject.GetComponent<DebugPlayer>();
        if (player == null)
            return;

        if (interactionHud == null)
            return;

        _interactionHud = Instantiate(interactionHud);
        _interactionHud.name = name + "_interactionHud";
        _interactionHud.transform.position = transform.position + interactionHudOffset;
        _interactionHud.interactable = this;
        _interactionHud.Text = displayName;
        _interactionHud.player = player.gameObject;
        _interactionHud.layer = Layer;
        _interactionHud.gameObject.SetActive(enabled);
    }

    private void OnPlayerExit(GameObject gameObject)
    {
        var player = gameObject.GetComponent<DebugPlayer>();
        if (player == null)
            return;

        if (_interactionHud == null)
            return;

        Destroy(_interactionHud.gameObject);
        _interactionHud = null;
    }

    public void Use(InteractionData data)
    {
        OnUse.Invoke(data);
    }

    private void OnEnable()
    {
        if (_interactionHud)
            _interactionHud.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (_interactionHud)
            _interactionHud.gameObject.SetActive(false);
    }
}