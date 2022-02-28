using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeControl : ObjectControl
{
    public override void Move(Touch touch)
    {
        Ray my_ray = Camera.main.ScreenPointToRay(touch.position);
        Vector3 dest = my_ray.GetPoint(start_distance);
        this.pos = dest;
    }
}