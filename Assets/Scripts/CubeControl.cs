using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeControl : ObjectControl
{
    public override void Move(Touch t)
    {
        Ray newPositionRay = Camera.main.ScreenPointToRay(t.position);
        Vector3 destination = newPositionRay.GetPoint(start_distance);
        this.pos = destination;
    }
}