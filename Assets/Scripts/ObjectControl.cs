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

    public void Rotate(Vector3 v)
    {
        transform.Rotate(v, Space.World);
    }

    public void Scale(float percentageChange)
    {
        Vector3 newScale = transform.localScale;
        newScale += percentageChange * transform.localScale;

        transform.localScale = newScale;
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
            rend.material.color = colour;
        }
    }
}
