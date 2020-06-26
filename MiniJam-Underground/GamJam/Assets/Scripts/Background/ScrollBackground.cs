using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollBackground : MonoBehaviour
{

    public enum ScrollAxis
    {
        Vertical,
        Horizontal
    }

    public float speed = 0.1f;
    public ScrollAxis scrollAxis = ScrollAxis.Vertical;

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = new Vector2(0, 0);
        switch (scrollAxis)
        {
            case ScrollAxis.Horizontal:
                offset.x = Time.time * speed;
                break;
            case ScrollAxis.Vertical:
                offset.y = Time.time * speed;
                break;
        }

        GetComponent<Renderer>().material.mainTextureOffset = offset;
    }
}
