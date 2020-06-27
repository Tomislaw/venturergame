using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public float openingTime = 1f;
    public bool isOpen = false;

    private float timeToOpen = 0;

    private void Start()
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
    }

    public void Close()
    {
        isOpen = false;
    }
}