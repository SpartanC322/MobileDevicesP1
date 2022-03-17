using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchManager : MonoBehaviour
{
    enum Gestures { None, Determining, Tap, Drag, Rotation, Pinch, Zoom };
    Gestures current_gesture = Gestures.None;
    Gestures prev_gesture = Gestures.None;

    Vector3 start_pos;
    Vector3 end_pos;
    Vector3 start_scale;

    Vector2 t1;
    Vector2 t2;

    IInteractable selected_item;
    IInteractable object_hit;

    Quaternion start_rotation;

    Renderer rend;
    
    Touch touch;

    private float time_of_touch = 0f;
    private float tap_time_threshold = 0.5f;
    private float start_angle;
    private float start_distance;
    private float scale_total;
    private float rotate_total;
    private float rotation_threshold = 0.1f;
    private float scale_threshold = 1;
    private float dist;
    private float dist2;

    private bool has_moved;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        current_gesture = Determine_Gesture();
        if (current_gesture != Gestures.None && current_gesture != Gestures.Determining && current_gesture != prev_gesture)
        {
            print("Gesture: " + current_gesture);
        }

        prev_gesture = current_gesture;

        switch (current_gesture)
        {
            case Gestures.None:

                break;

            case Gestures.Determining:

                break;

            case Gestures.Tap:
                RaycastHit ray_hit;
                Ray ray;
                ray = Camera.main.ScreenPointToRay(Input.touches[0].position);

                if (Physics.Raycast(ray, out ray_hit))
                {
                    object_hit = ray_hit.transform.GetComponent<IInteractable>();

                    if (object_hit != null)
                    {
                        if (selected_item != null)
                        {
                            selected_item.Toggle_Active();
                        }
                        selected_item = object_hit;
                        selected_item.Toggle_Active();
                    }
                    else
                    {
                        selected_item = null;
                    }

                }

                break;

            case Gestures.Drag:

                if (selected_item != null && selected_item.Get_Selected() == true)
                {
                    selected_item.Move(Input.touches[0]);
                }

                else
                {
                    Drag_Camera();
                }

                break;

            case Gestures.Rotation:

                float angle = Determine_Angle();
                Quaternion rota = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Camera.main.transform.forward);
                selected_item.Rotate(rota);

                break;

            case Gestures.Pinch:

                Touch first = Input.GetTouch(0);
                Touch second = Input.GetTouch(1);

                float f = Determine_Pinch(first,second);
                selected_item.Scale(f);

                break;

        }
    }

    private Gestures Determine_Gesture()
    {
        if (Input.touchCount < 1)
        {
            return Gestures.None;
        }

        if (Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    start_pos = touch.position;
                    time_of_touch = Time.time;

                    return Gestures.Determining;

                case TouchPhase.Moved:
                    has_moved = true;
                    return Gestures.Drag;

                case TouchPhase.Ended:
                    if (Is_Tap())
                    {
                        return Gestures.Tap;
                    }

                    has_moved = false;

                    return Gestures.None;

                default:
                    return Gestures.Determining;
            }
        }

        else if (Input.touchCount == 2)
        {
            Touch touch_first = Input.GetTouch(0);
            Touch touch_second = Input.GetTouch(1);

            if (touch_first.phase == TouchPhase.Began || touch_second.phase == TouchPhase.Began)
            {
                start_distance = Vector2.Distance(touch_first.position, touch_second.position);
                Vector3 distance = touch_second.position - touch_first.position;
                start_angle = Mathf.Atan2(distance.x, distance.y);

                if(selected_item != null)
                {
                    start_rotation = selected_item.gameObject.transform.rotation;
                    start_scale = selected_item.gameObject.transform.localScale;
                }
                else
                {
                    start_rotation = Camera.main.transform.rotation;
                }

                return Gestures.Determining;
            }

            if (touch_first.phase == TouchPhase.Ended || touch_second.phase == TouchPhase.Ended)
            {
                scale_total = 0;
                rotate_total = 0;
            }

            switch (current_gesture)
            {
                case Gestures.Rotation:
                    return Gestures.Rotation;

                case Gestures.Pinch:
                    return Gestures.Pinch;

                case Gestures.Zoom:
                    return Gestures.Zoom;
            }

            if (Mathf.Approximately(start_distance, 0))
            {
                return Gestures.Determining;
            }

            float change = Determine_Change(touch_first, touch_second);
            float angle = Determine_Angle();

            if (change < 0)
            {
                scale_total = (change * -1);
            }

            else
            {
                scale_total = change;
            }

            if (angle < 0)
            {
                rotate_total += (angle * -1);
            }

            else
            {
                rotate_total += angle;
            }

            if (scale_total >= scale_threshold && selected_item != null)
            {
                return Gestures.Pinch;
            }

            if (scale_total >= scale_threshold)
            {
                return Gestures.Zoom;
            }

            if (rotate_total >= rotation_threshold)
            {
                return Gestures.Rotation;
            }

            return Gestures.Determining;
        }

        return Gestures.None;
    }

    private float Determine_Change(Touch first_touch, Touch second_touch)
    {
        float start_dist = Vector2.Distance(first_touch.position - first_touch.deltaPosition, second_touch.position - second_touch.deltaPosition);
        float new_dist = Vector2.Distance(first_touch.position,second_touch.position);

        float change =(new_dist - start_dist) * -1;
        change *= Time.deltaTime;

        return change;
    }

    private float Determine_Pinch(Touch first_touch, Touch second_touch)
    {

        float current_dist = Vector2.Distance(first_touch.position, second_touch.position);
        float r = current_dist / start_distance;

        return r;
    }

    private float Determine_Angle()
    {
        Vector3 vector_angle = Input.touches[1].position - Input.touches[0].position;
        float angle = Mathf.Atan2(vector_angle.y, vector_angle.x);
        angle = angle - start_angle;

        return angle;
    }

    public void Drag_Camera()
    {
        Vector2 touch_pos = Input.touches[0].deltaPosition * Time.deltaTime;
        Camera.main.transform.Translate(-touch_pos.x, touch_pos.y, 0);
    }

    private bool Is_Tap()
    {
        float time = Time.time - time_of_touch;

        if (time <= tap_time_threshold && !has_moved)
        {
            return true;
        }

        else
        {
            return false;
        }
    }
}
