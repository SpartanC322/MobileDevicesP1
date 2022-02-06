using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControl : MonoBehaviour, IController
{
    private Vector3 pos;
    private Renderer rend;
    private Color32 colour;
    private float start_distance;
    private bool selected = false;

    public bool Get_Selected()
    {
        return selected;
    }

    public void Move(Vector3 pos)
    {
        Ray newPositionRay = Camera.main.ScreenPointToRay(pos);
        Vector3 destination = newPositionRay.GetPoint(start_distance);
        this.pos = destination;
    }

    public void Deselected()
    {
        rend.material.SetColor("_Color", colour);
        selected = false;
    }

    public void Selected()
    {
        start_distance = Vector3.Distance(Camera.main.transform.position, transform.position);
        rend.material.SetColor("_Color", Color.red);
        selected = true;
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
    void Start()
    {
        pos = transform.position;
        rend = GetComponent<Renderer>();
        colour = GetComponent<Renderer>().material.color;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, pos, 0.05f);
    }
}