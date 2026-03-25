using System;
using UnityEngine;

public class TitleScreenFloat : MonoBehaviour
{
    public float amplitude;
    public float period;
    public float phase;

    private float midpoint;
    private float time;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        midpoint = transform.position.y + amplitude;
        time = phase * period;
    }

    // Update is called once per frame
    void Update()
    {
        time = (time + Time.deltaTime) % period;

        float y = midpoint + amplitude * (float) Math.Cos(time / period * 2 * Math.PI);
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
}
