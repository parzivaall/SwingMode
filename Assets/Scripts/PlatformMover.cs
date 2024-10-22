using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    public float verticalSpeed = 2f;
    public float amplitude = 2f;
    
    private float startingY;
    private float time;
    
    void Start()
    {
        startingY = transform.localPosition.y;
    }
    
    void Update()
    {
        time += Time.deltaTime;
        
        // Simple sine wave movement
        float newY = startingY + Mathf.Sin(time * verticalSpeed) * amplitude;
        Vector3 pos = transform.localPosition;
        pos.y = newY;
        transform.localPosition = pos;
    }
}