using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public WorldEnvironment environment;
    public WorldMap map;

    private void OnEnable()
    {
        environment = GetComponent<WorldEnvironment>();
        map = GetComponent<WorldMap>();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}