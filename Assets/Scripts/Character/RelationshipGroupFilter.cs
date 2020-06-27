using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationshipGroupFilter : MonoBehaviour
{
    public string GroupName;

    public List<Filter> groups;

    [Serializable]
    public struct Filter
    {
        public string GroupName;
        public Behavior Behavior;
    }

    public enum Behavior
    {
        Enemy, Neutral, Friendly
    }

    public Behavior GetBehavior(RelationshipGroupFilter filter)
    {
        if (filter.GroupName == GroupName)
            return Behavior.Friendly;

        foreach (var group in filter.groups)
        {
            if (group.GroupName == filter.GroupName)
                return group.Behavior;
        }
        return Behavior.Neutral;
    }

    public Behavior GetBehavior(GameObject go)
    {
        var filter = go.GetComponent<RelationshipGroupFilter>();
        if (filter)
            return GetBehavior(filter);

        return Behavior.Neutral;
    }
}