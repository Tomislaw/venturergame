using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class TabsGui : MonoBehaviour
{
    public List<TabGui> Tabs;

    public int SelectetTabId = -1;
    private int _selectetTabId = -1;

    public TabGui SelectedTab { get => Tabs.Find(it => it.IsSelected); }

    public OnTabSelectedEvent OnTabSelected = new OnTabSelectedEvent();

    [Serializable]
    public class OnTabSelectedEvent : UnityEvent<string, TabGui>
    {
    }

    // Start is called before the first frame update
    private void Start()
    {
        Tabs = GetComponentsInChildren<TabGui>().ToList();
        foreach (var tab in Tabs)
            tab.tabsGui = this;

        SelectTab(SelectetTabId);
    }

    // Update is called once per frame
    private void Update()
    {
        if (SelectetTabId != _selectetTabId)
            SelectTab(SelectetTabId);
    }

    public void SelectTab(TabGui tab)
    {
        var _tab = Tabs.Find(it => tab == it);

        if (_tab == null)
        {
            foreach (var mTab in Tabs)
                mTab.Select(false);
            _selectetTabId = -1;
            SelectetTabId = -1;
        }
        else
        {
            foreach (var mTab in Tabs)
            {
                if (_tab != mTab && mTab.IsSelected)
                    mTab.Select(false);
            }
            _tab.Select(true);

            _selectetTabId = Tabs.IndexOf(_tab);
            SelectetTabId = _selectetTabId;
        }
    }

    public void SelectTab(string tab)
    {
        var _tab = Tabs.Find(it => tab == it.TabName);
        SelectTab(_tab);
    }

    public void SelectTab(int tab)
    {
        if (tab >= 0 && tab < Tabs.Count)
            SelectTab(Tabs[tab]);
        else
            SelectTab((TabGui)null);
    }
}