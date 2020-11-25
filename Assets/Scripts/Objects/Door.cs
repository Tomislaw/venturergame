using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Door : MonoBehaviour, IInteractable
{
    public float openingTime = 1f;
    public bool isOpen = false;

    private bool previousIsOpen;
    private float timeToOpen = 0;

    public UnityEvent OnOpen = new UnityEvent();
    public UnityEvent OnClose = new UnityEvent();

    private void Awake()
    {
        if (isOpen)
            timeToOpen = 0;
        else
            timeToOpen = openingTime;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isOpen)
        {
            if (timeToOpen > 0)
            {
                timeToOpen -= Time.deltaTime;
                if (timeToOpen < 0)
                    timeToOpen = 0;
                float factor = timeToOpen / openingTime;
                transform.localScale = new Vector3(factor + (1f - factor) * 0.1f, 1, 1);
            }
        }
        else
        {
            if (timeToOpen <= openingTime)
            {
                timeToOpen += Time.deltaTime;
                if (timeToOpen > openingTime)
                    timeToOpen = openingTime;

                float factor = timeToOpen / openingTime;
                transform.localScale = new Vector3(factor + (1f - factor) * 0.1f, 1, 1);
            }
        }
    }

    public void Open()
    {
        isOpen = true;
        OnOpen.Invoke();
    }

    public void Close()
    {
        isOpen = false;
        OnClose.Invoke();
    }

    public void Toggle()
    {
        isOpen = !isOpen;
        if (isOpen)
            OnOpen.Invoke();
        else
            OnClose.Invoke();
    }

    public void Use(InteractionData gameObject)
    {
        Toggle();
    }
}