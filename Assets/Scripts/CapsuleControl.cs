using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleControl : ObjectControl
{
    //public override void Move(Vector3 pos)
    //{
    //    Ray newPositionRay = Camera.main.ScreenPointToRay(pos);
    //    Vector3 destination = newPositionRay.GetPoint(start_distance);
    //    this.pos = destination;
    //}

    public override void Move(Touch t)
    {
        Ray newPositionRay = Camera.main.ScreenPointToRay(t.position);
        RaycastHit[] hits = Physics.RaycastAll(newPositionRay);
        int groundMask = LayerMask.NameToLayer("Ground");

        float closest = Mathf.Infinity;

        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject.layer == groundMask)
            {
                if (hit.distance < closest)
                {
                    closest = hit.distance;
                    pos = hit.point;
                    pos += new Vector3(0, 1, 0);
                }
            }
        }
    }
}
