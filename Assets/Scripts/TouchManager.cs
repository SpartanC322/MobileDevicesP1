using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchManager : MonoBehaviour
{
    enum Gestures { None, Determining, Tap, Drag, Pinch, Rotation, Scale, Zoom };
    Gestures current_gesture = Gestures.None;

    Vector3 start_pos;
    Vector3 end_pos;
    Vector3 initial_scale;

    Vector2 t1;
    Vector2 t2;

    IInteractable selected_item;
    IInteractable object_hit;

    Quaternion initial_rotation;

    Renderer rend;
    
    Touch touch;

    private float zoom_min_bound = 0.1f;
    private float zoom_max_bound = 179.9f;
    private float time_of_touch = 0f;
    private float tap_time_threshold = 0.5f;
    private float initial_angle;
    private float initial_distance;
    private float pinch_speed = 0.1f;
    private float current_delta;
    private float current_angle;
    private float threshold = 10;
    private float rotation_speed = 0.5f;

    private bool has_moved;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        current_gesture = Determine_Gesture();
        if (current_gesture != Gestures.None)
        {
            print("Gesture: " + current_gesture);
        }

        switch (current_gesture)
        {
            case Gestures.None:

                break;

            case Gestures.Determining:

                break;

            case Gestures.Tap:
                RaycastHit info;
                Ray ray;
                ray = Camera.main.ScreenPointToRay(Input.touches[0].position);

                Debug.DrawRay(ray.origin, 30 * ray.direction);

                if (Physics.Raycast(ray, out info))
                {
                    object_hit = info.transform.GetComponent<IInteractable>();

                    if (object_hit != null)
                    {
                        selected_item = object_hit;
                        selected_item.Toggle_Active();
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
                    //Drag_Camera();
                }

                break;

            case Gestures.Pinch:

                float dis = Determine_Factor();
                Pinch(dis, pinch_speed);

                break;

            case Gestures.Rotation:

                float r1 = touch.deltaPosition.y * rotation_speed;
                float r2 = -touch.deltaPosition.x * rotation_speed;
                Vector3 r = new Vector3(r1, r2, 0);
                selected_item.Rotate(r);

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

        if (Input.touchCount == 2)
        {
            Touch touch_first = Input.GetTouch(0);
            Touch touch_second = Input.GetTouch(1);

            if (touch_first.phase == TouchPhase.Began || touch_second.phase == TouchPhase.Began)
            {
                initial_distance = Vector2.Distance(touch_first.position, touch_second.position);
                Vector3 distance = touch_second.position - touch_first.position;
                initial_angle = Mathf.Atan2(distance.x, distance.y);

                if(selected_item != null)
                {
                    initial_rotation = selected_item.gameObject.transform.rotation;
                    initial_scale = selected_item.gameObject.transform.localScale;
                }
                else
                {
                    initial_rotation = Camera.main.transform.rotation;
                }

                return Gestures.Determining;
            }

            if (touch_first.phase == TouchPhase.Ended || touch_second.phase == TouchPhase.Ended)
            {
                current_delta = 0;
                current_angle = 0;
            }

            switch (current_gesture)
            {
                case Gestures.Rotation:
                    return Gestures.Rotation;

                case Gestures.Scale:
                    return Gestures.Scale;

                case Gestures.Zoom:
                    return Gestures.Zoom;
            }

            float change = Determine_Factor();
            float angle = Determine_Angle();

            if (change < 0)
            {
                current_delta = (change * -1);
            }

            else
            {
                current_delta = change;
            }

            if (angle < 0)
            {
                current_angle += (angle * -1);
            }

            else
            {
                current_angle += angle;
            }

            if (current_delta >= threshold && selected_item != null)
            {
                return Gestures.Scale;
            }

            if (current_delta >= threshold)
            {
                return Gestures.Zoom;
            }

            if (current_angle >= threshold / 100)
            {
                return Gestures.Rotation;
            }

            return Gestures.Determining;
        }

        return Gestures.None;
    }

    private float Determine_Factor()
    {
        Touch first_touch = Input.GetTouch(0);
        Touch second_touch = Input.GetTouch(1);

        Vector2 first_touch_prev = first_touch.position - first_touch.deltaPosition;
        Vector2 second_touch_prev = second_touch.position - second_touch.deltaPosition;

        float old_dist = Vector2.Distance(first_touch_prev, second_touch_prev);
        float current_dist = Vector2.Distance(first_touch.position, second_touch.position);

        float delta = old_dist - current_dist;

        return delta;
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

    public void Pinch(float deltaMagnitudeDiff, float speed)
    {
        Camera.main.fieldOfView += deltaMagnitudeDiff * speed;
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, zoom_min_bound, zoom_max_bound);
    }

    //public void Drag_Camera()
    //{
    //    Vector2 touchDeltaPosition = Input.touches[0].deltaPosition * Time.deltaTime;
    //    Camera.main.transform.Translate(-touchDeltaPosition.x, touchDeltaPosition.y, 0);
    //}

    private float Determine_Angle()
    {
        Vector3 v = Input.touches[1].position - Input.touches[0].position;
        float theta = Mathf.Atan2(v.y, v.x);
        theta = theta - initial_angle;

        return theta;
    }
}
