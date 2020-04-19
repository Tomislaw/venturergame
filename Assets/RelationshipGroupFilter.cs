using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelationshipGroupFilter : MonoBehaviour
{
    public List<Type> groups;

    public enum Type
    {
        TownAnimal, ForestAnimal, Aggresive, Human
    }
}