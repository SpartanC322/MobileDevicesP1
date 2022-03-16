using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleControl : ObjectControl
{
    public override void Move(Touch touch)
    {
        //Ray my_ray = Camera.main.ScreenPointToRay(touch.position);
        //RaycastHit[] hits = Physics.RaycastAll(my_ray);
        //int ground_mask = LayerMask.NameToLayer("Ground");

        //float closest = Mathf.Infinity;

        //foreach (RaycastHit hit in hits)
        //{
        //    if (hit.transform.gameObject.layer == ground_mask)
        //    {
        //        if (hit.distance < closest)
        //        {
        //            closest = hit.distance;
        //            this.pos = hit.point;
        //            this.pos += new Vector3(0, 1, 0);
        //        }
        //    }
        //}

        float lift = transform.localScale.y + 0.2f;

        Camera my_cam = Camera.main;
        Ray ray = my_cam.ScreenPointToRay(touch.position);
        RaycastHit hit;

        GameObject ground = GameObject.Find("Ground");
        Collider col = ground.GetComponent<Collider>();

        if (col.Raycast(ray, out hit, 100f))
        {
            Vector3 ray_point = ray.GetPoint(Vector3.Distance(my_cam.transform.position, hit.point));
            Vector3 newPos = new Vector3(ray_point.x, ray_point.y + lift, ray_point.z);

            this.pos = newPos;
        }
    }
}