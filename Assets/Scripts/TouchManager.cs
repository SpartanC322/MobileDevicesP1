using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchManager : MonoBehaviour
{
    enum Gestures { None, Determining, Tap, Drag, Rotation, Zoom, Scale };
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
    
    Quaternion initial_rotation;

    private const float delta_change_threshold = 10f;
    private const float rotation_threshold = 0.1f;
    private float starting_distance_to_selected_object;
    private float start_distance;
    private float current_angle;
    private float current_delta_change;
    private float zoom_min_bound = 0.1f;
    private float zoom_max_bound = 179.9f;
    private float rotation_rate = 3.0f;
    private float time_of_touch = 0f;
    private float tap_time_threshold = 0.5f;
    private float initial_angle;

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
                        if (selected_item.Get_Selected() == false)
                        {
                            starting_distance_to_selected_object = Vector3.Distance(Camera.main.transform.position, info.transform.position);
                            selected_item.Selected();
                        }
                        if (selected_item.Get_Selected() == true)
                        {
                            //selected_item.Deselected();
                            //starting_distance_to_selected_object = 0;
                        }
                    }

                    else
                    {
                        if (selected_item != null)
                        {
                            selected_item.Deselected();
                        }

                        selected_item = null;
                    }
                }

                break;

            case Gestures.Drag:

                if (selected_item != null)
                {
                    selected_item.Move(Input.touches[0].position);
                }

                else
                {
                    Drag_Camera();
                }

                break;

            case Gestures.Rotation:
                float val1 = touch.deltaPosition.y * rotation_rate;
                float val2 = -touch.deltaPosition.x * rotation_rate;
                Vector3 v = new Vector3(val1, val2, 0);
                selected_item.Rotate(v);

                break;

            case Gestures.Scale:


                t1 = Input.GetTouch(0).position;
                t2 = Input.GetTouch(1).position;

                float newDistance = (t1 - t2).sqrMagnitude;

                float changeInDistance = newDistance - start_distance;

                /* if (Mathf.Approximately(initialDistance, 0))
                  {
                      //if bad 
                      break;
                  }*/

                float percentageChange = changeInDistance / start_distance;

                selected_item.Scale(percentageChange);


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
            Touch touch = Input.GetTouch(0);

            Touch touch2 = Input.GetTouch(1);

            if (touch.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                start_distance = Vector2.Distance(touch.position, touch2.position);
                Vector3 v2 = touch2.position - touch.position;
                initial_angle = Mathf.Atan2(v2.y, v2.x);

                if (selected_item != null)
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

            if (touch.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Ended)
            {
                current_delta_change = 0;
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

            float angle = Determine_Angle();
            float deltaChange = Determine_Factor();

            if (deltaChange < 0)
            {
                current_delta_change = (deltaChange * -1);
            }

            else
            {
                current_delta_change = deltaChange;
            }

            if (angle < 0)
            {
                current_angle += (angle * -1);
            }

            else
            {
                current_angle += angle;
            }

            if (current_delta_change >= delta_change_threshold && selected_item != null)
            {
                return Gestures.Scale;
            }

            if (current_delta_change >= delta_change_threshold)
            {
                return Gestures.Zoom;
            }

            if (current_angle >= rotation_threshold)
            {
                return Gestures.Rotation;
            }

            return Gestures.Determining;
        }

        return Gestures.None;
    }

    private float Determine_Factor()
    {
        Touch tZero = Input.GetTouch(0);
        Touch tOne = Input.GetTouch(1);

        Vector2 tZeroPrevPos = tZero.position - tZero.deltaPosition;
        Vector2 tOnePrevPos = tOne.position - tOne.deltaPosition;

        float oldTouchDistance = Vector2.Distance(tZeroPrevPos, tOnePrevPos);
        float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

        float deltaDistance = oldTouchDistance - currentTouchDistance;

        return deltaDistance;
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

    public void Zoom(float deltaMagnitudeDiff, float speed)
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
