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
    public override void Move(Touch t)
    {
        Vector3 touch = Camera.main.ScreenToWorldPoint(t.position);
        transform.position = Vector3.Lerp(transform.position, touch, Time.deltaTime);
    }
}
