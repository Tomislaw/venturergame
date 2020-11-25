using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class InteractionHud : MonoBehaviour
{
    [HideInInspector]
    public Interactable interactable;

    [HideInInspector]
    public GameObject player;

    public InteractionContentHud content;

    private static Dictionary<int, InteractionHud> activeHuds = new Dictionary<int, InteractionHud>();

    [SerializeField]
    private int _layer = 0;

    public int layer
    {
        get => _layer;
        set
        {
            InteractionHud currentActiveHud;
            if (activeHuds.TryGetValue(layer, out currentActiveHud))
                if (this == currentActiveHud)
                    activeHuds.Remove(layer);
            _layer = value;

            if (Controls.Controls is KeyboardPlayerControls)
            {
                var key = layer == 0
                    ? (Controls.Controls as KeyboardPlayerControls).Use1.ToString()
                    : (Controls.Controls as KeyboardPlayerControls).Use2.ToString();
                content.CharText = key;
            }
        }
    }

    [SerializeField]
    private string _Text;

    public string Text
    {
        get => _Text;
        set
        {
            _Text = value;
            if (content)
                content.LabelText = Text;
        }
    }

    public MainPlayerControls Controls;

    private void Awake()
    {
        if (content)
        {
            content.LabelText = Text;
            layer = _layer;
        }
    }

    private void OnValidate()
    {
        if (content)
        {
            content.LabelText = Text;
            layer = _layer;
        }
    }

    private void Update()
    {
        // enable or based on distance to previous active hud element
        InteractionHud currentActiveHud;
        if (activeHuds.TryGetValue(layer, out currentActiveHud))
        {
            if (this == currentActiveHud)
            {
                content.gameObject.SetActive(true);
            }
            else
            {
                if (XDistance(gameObject, player) < XDistance(currentActiveHud.gameObject, player))
                {
                    currentActiveHud.content.gameObject.SetActive(false);
                    activeHuds[layer] = this;
                    content.gameObject.SetActive(true);
                }
                else
                {
                    content.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            activeHuds[layer] = this;
            content.gameObject.SetActive(true);
        }

        // use interactive element on input
        if (content.gameObject.activeSelf && interactable != null)
        {
            InteractionData data;
            data.caller = player;
            switch (layer)
            {
                case 0:
                    if (Controls.IsUsing1)
                        interactable.Use(data);
                    break;

                case 1:
                    if (Controls.IsUsing2)
                        interactable.Use(data);
                    break;
            }
        }
    }

    private float XDistance(GameObject a, GameObject b)
    {
        return Mathf.Abs(a.transform.position.x - b.transform.position.x);
    }

    private void OnDestroy()
    {
        InteractionHud currentActiveHud;
        if (activeHuds.TryGetValue(layer, out currentActiveHud))
            if (this == currentActiveHud)
                activeHuds.Remove(layer);
    }

    private void OnDisable()
    {
        InteractionHud currentActiveHud;
        if (activeHuds.TryGetValue(layer, out currentActiveHud))
            if (this == currentActiveHud)
                activeHuds.Remove(layer);
    }
}