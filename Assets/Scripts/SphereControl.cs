using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereControl : ObjectControl
{
    //OLD MOVE
    //public void Move(Vector3 pos)
    //{
    //    Ray newPositionRay = Camera.main.ScreenPointToRay(pos);
    //    Vector3 destination = newPositionRay.GetPoint(start_distance);
    //    this.pos = destination;
    //}

    //NEW MOVE, NOW WITH EXTRA LERP!
    public override void Move(Touch touch)
    {
        Vector3 touched = new Vector3(touch.position.x, touch.position.y, start_distance);
        Vector3 destination = Camera.main.ScreenToWorldPoint(touched);
        this.pos = destination;
    }
}
