using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudEmitter : MonoBehaviour
{
    public List<GameObject> cloudNormal = new List<GameObject>();
    public List<GameObject> cloudRainy = new List<GameObject>();

    public float speed = 1;
    public float emitTime = 1;
    public float emitTimeRange = 1;

    public Rect cloudArea;

    public int maxClouds = 10;

    public float timeToEmit = 0;

    private List<GameObject> clouds = new List<GameObject>();

    private Vector3 offset;

    private void Start()
    {
        offset = this.transform.position;

        float pos = cloudArea.xMin;
        int maxCloudsLeft = 100;
        while (pos < cloudArea.xMax && maxCloudsLeft > 0)
        {
            emitCloud();
            clouds[clouds.Count - 1].transform.position = new Vector3(pos + 0.01f, clouds[clouds.Count - 1].transform.position.y);
            pos += Mathf.Abs(speed) * emitTime;
            maxCloudsLeft--;
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y) + offset;
    }

    private void Update()
    {
        timeToEmit -= Time.deltaTime;
        if (timeToEmit < 0)
        {
            timeToEmit = emitTime + Random.Range(-emitTimeRange, emitTimeRange);
            emitCloud();
        }

        int i = 0;
        while (i < clouds.Count)
        {
            var c = clouds[i];
            c.name = this.name + "_cloud";
            c.transform.localPosition += new Vector3(speed * Time.deltaTime, 0, 0);
            if (!cloudArea.Contains(c.transform.localPosition))
            {
                clouds.RemoveAt(i);
                Destroy(c);
            }
            else
            {
                i++;
            }
        }
    }

    private void emitCloud()
    {
        int id = Random.Range(0, cloudNormal.Count);
        var cloud = Instantiate(cloudNormal[id]);
        cloud.transform.parent = transform;
        if (speed > 0)
            cloud.transform.localPosition = new Vector2(cloudArea.x + 0.01f, cloudArea.y + Random.Range(0, cloudArea.height));
        else
            cloud.transform.localPosition = new Vector2(cloudArea.x - 0.01f + cloudArea.width, cloudArea.y + Random.Range(0, cloudArea.height));
        clouds.Add(cloud);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(cloudArea.center, cloudArea.size);
        Gizmos.color = Color.white;
    }
}