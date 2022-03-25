using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereControl : ObjectControl
{
    public override void Move(Touch touch)
    {
        Vector3 touched = new Vector3(touch.position.x, touch.position.y, start_distance);
        Vector3 destination = Camera.main.ScreenToWorldPoint(touched);
        this.pos = destination;
    }
}
