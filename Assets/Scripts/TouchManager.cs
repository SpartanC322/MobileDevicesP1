using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchManager : MonoBehaviour
{
    enum Gestures { None, Determining, Tap, Drag, Pinch };
    Gestures current_gesture = Gestures.None;

    Vector3 start_pos;
    Vector3 end_pos;
    Vector3 initial_scale;

    Vector2 t1;
    Vector2 t2;

    IInteractable selected_item;
    IInteractable object_hit;

    Renderer rend;
    
    Touch touch;

    private float zoom_min_bound = 0.1f;
    private float zoom_max_bound = 179.9f;
    private float time_of_touch = 0f;
    private float tap_time_threshold = 0.5f;
    private float initial_angle;
    private float pinch_speed = 0.1f;

    private bool has_moved;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        current_gesture = Determine_Gesture();
        print("Gesture: " + current_gesture);

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
                    Drag_Camera();
                }

                break;

            case Gestures.Pinch:

                float dis = Determine_Factor();
                Pinch(dis, pinch_speed);

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
            
        }

        return Gestures.None;
    }

    private float Determine_Factor()
    {
        Touch t_zero = Input.GetTouch(0);
        Touch t_one = Input.GetTouch(1);

        Vector2 t_zero_pre = t_zero.position - t_zero.deltaPosition;
        Vector2 t_one_prev = t_one.position - t_one.deltaPosition;

        float pre_touch_dist = Vector2.Distance(t_zero_pre, t_one_prev);
        float cur_touch_dist = Vector2.Distance(t_zero.position, t_one.position);

        float d_dist = pre_touch_dist - cur_touch_dist;

        return d_dist;
    }

    private bool Is_Tap()
    {
        float touchTime = Time.time - time_of_touch;

        if (touchTime <= tap_time_threshold && !has_moved)
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

    public void Drag_Camera()
    {
        Vector2 touchDeltaPosition = Input.touches[0].deltaPosition * Time.deltaTime;
        Camera.main.transform.Translate(-touchDeltaPosition.x, touchDeltaPosition.y, 0);
    }

    private float Determine_Angle()
    {
        Vector3 v = Input.touches[1].position - Input.touches[0].position;
        float theta = Mathf.Atan2(v.y, v.x);
        theta = theta - initial_angle;

        return theta;
    }
}
