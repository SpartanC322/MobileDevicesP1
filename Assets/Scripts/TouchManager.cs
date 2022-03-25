using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TouchManager : MonoBehaviour
{
    enum Gestures { None, Determining, Tap, Double_Tap, Drag, Rotation, Pinch, RotateCamera };
    Gestures current_gesture = Gestures.None;
    Gestures prev_gesture = Gestures.None;

    Vector3 start_pos;
    Vector3 start_scale;

    IInteractable selected_item;
    IInteractable object_hit;

    Quaternion start_rotation;
    
    Touch touch;

    private float time_of_touch = 0f;
    private float tap_time_threshold = 0.5f;
    private float start_angle;
    private float start_distance;
    private float scale_total;
    private float rotate_total;
    private float rotation_threshold = 0.5f;

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

                //Select Item
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

            //Reset
            case Gestures.Double_Tap:

                SceneManager.LoadScene(SceneManager.GetActiveScene().name);

                break;

            case Gestures.Drag:

                //Moves Item
                if (selected_item != null && selected_item.Get_Selected() == true)
                {
                    selected_item.Move(Input.touches[0]);
                }
                //Moves Camera if no Item
                else
                {
                    Move_Camera();
                }

                break;

            case Gestures.Rotation:

                //Rotate Item
                if (selected_item != null)
                {
                    float angle = Determine_Angle();
                    Quaternion rota = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, Camera.main.transform.forward);
                    selected_item.Rotate(rota);
                }
                //Rotate Camera if no Item
                else
                {
                    Rotate_Camera();
                }

                break;

            case Gestures.Pinch:
                //Should work with only 2 fingers but sometimes it decides it wants 3
                Touch first = Input.GetTouch(0);
                Touch second = Input.GetTouch(1);

                float f = Determine_Pinch(first,second);
                selected_item.Scale(f);

                break;
        }
    }

    private Gestures Determine_Gesture()
    {
        //No Touch
        if (Input.touchCount < 1)
        {
            return Gestures.None;
        }

        //One Touch
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
                    if (Determine_Tap())
                    {
                        if (Input.touches[0].tapCount == 2)
                        {
                            return Gestures.Double_Tap;
                        }
                        else
                        {
                            return Gestures.Tap;
                        }
                    }

                    has_moved = false;

                    return Gestures.None;

                default:
                    return Gestures.Determining;
            }
        }

        //Two Touch
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
                }
                else
                {
                    
                }

                return Gestures.Determining;
            }

            if (touch_first.phase == TouchPhase.Ended || touch_second.phase == TouchPhase.Ended)
            {
                rotate_total = 0;
            }

            switch (current_gesture)
            {
                case Gestures.Rotation:
                    return Gestures.Rotation;

                case Gestures.Pinch:
                    return Gestures.Pinch;

                case Gestures.RotateCamera:
                    return Gestures.RotateCamera;
            }

            if (Mathf.Approximately(start_distance, 0))
            {
                return Gestures.Determining;
            }

            float angle = Determine_Angle();

            if (angle < 0)
            {
                rotate_total += (angle * -1);
            }
            else
            {
                rotate_total += angle;
            }

            Vector2 first_touch_pre = touch_first.position - touch_first.deltaPosition;
            Vector2 second_touch_pre = touch_second.position - touch_second.deltaPosition;

            float prev_mag = (first_touch_pre - second_touch_pre).magnitude;
            float current_mag = (touch_first.position - touch_second.position).magnitude;

            float diff = current_mag - prev_mag;

            if (diff < 0 || diff > 0)
            {
                return Gestures.Pinch;
            }

            Debug.Log(diff);

            if (scale_total >= rotation_threshold)
            {
                return Gestures.RotateCamera;
            }

            if (rotate_total >= rotation_threshold)
            {
                return Gestures.Rotation;
            }

            return Gestures.Determining;
        }

        //Three Touch
        else if (Input.touchCount == 3)
        {
            Touch touch_first = Input.GetTouch(0);
            Touch touch_second = Input.GetTouch(1);
            Touch touch_third = Input.GetTouch(2);

            if (touch_first.phase == TouchPhase.Began || touch_second.phase == TouchPhase.Began || touch_third.phase == TouchPhase.Began)
            {
                
            }
        }

        return Gestures.None;
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

    public void Rotate_Camera()
    {
        Camera.main.transform.Rotate(new Vector3(Input.touches[0].deltaPosition.y, Input.touches[0].deltaPosition.x, 0) * 0.02f);
    }

    public void Move_Camera()
    {
        Vector2 touch_pos = Input.GetTouch(0).deltaPosition * Time.deltaTime;
        Camera.main.transform.Translate(-touch_pos.x, touch_pos.y, 0);
    }

    private bool Determine_Tap()
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
