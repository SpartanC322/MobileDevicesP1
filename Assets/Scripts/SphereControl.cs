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

        Vector3 touchedPos = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, start_distance);
        Vector3 destination = Camera.main.ScreenToWorldPoint(touchedPos);
        this.pos = destination;

        //Vector3 touched_pos = new Vector3(Input.touches[0].position.x, Input.touches[0].position.y, start_distance);
        //Vector3 touch = Camera.main.ScreenToWorldPoint(touched_pos);
        //transform.position = Vector3.Lerp(transform.position, touch, Time.deltaTime);

        //Ray newPositionRay = Camera.main.ScreenPointToRay(t.position);
        //Vector3 destination = newPositionRay.GetPoint(start_distance);
        //pos = destination;

    }
}
