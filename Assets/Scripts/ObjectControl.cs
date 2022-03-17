using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectControl : MonoBehaviour, IInteractable
{
    protected Vector3 pos;
    protected Renderer rend;
    protected Color32 colour;
    protected float start_distance;
    protected bool selected = false;

    public bool Get_Selected()
    {
        return selected;
    }

    public abstract void Move(Touch t);

    public void Toggle_Active()
    {
        if (selected == false)
        {
            start_distance = Vector3.Distance(Camera.main.transform.position, transform.position);
            rend.material.SetColor("_Color", Color.red);
            selected = true;
        }

        else if (selected == true)
        {
            rend.material.SetColor("_Color", colour);
            selected = false;
        }
    }

    public void Rotate(Quaternion rotation)
    {
        //Quaternion start_rotation = transform.rotation;
        //transform.rotation = rotation * start_rotation;

        transform.rotation = rotation * Quaternion.identity;
    }

    public void Scale(float change)
    {
        Vector3 current_scale = transform.localScale;

        transform.localScale = current_scale * change;
    }

    // Start is called before the first frame update
    public void Start()
    {
        pos = transform.position;
        rend = GetComponent<Renderer>();
        colour = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    public void Update()
    {
        transform.position = Vector3.Lerp(transform.position, pos, 0.5f);
        if (selected == false && rend.material.color == Color.red)
        {
            rend.material.SetColor("_Color", Color.white);
        }
    }
}
