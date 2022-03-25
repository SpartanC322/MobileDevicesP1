using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleControl : ObjectControl
{
    public override void Move(Touch touch)
    {
        Ray my_ray = Camera.main.ScreenPointToRay(touch.position);
        RaycastHit[] hits = Physics.RaycastAll(my_ray);

        float closest = Mathf.Infinity;

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.layer == 1)
            {
                if (hit.distance < closest)
                {
                    closest = hit.distance;
                    this.pos = hit.point;
                    this.pos += new Vector3(0, 1, 0);
                }
            }
        }
    }
}