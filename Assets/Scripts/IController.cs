using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IController
{
    GameObject gameObject { get; }

    void Move(Vector3 destination);

    void Scale(float percentageChange);

    void Rotate(Vector3 v);

    void Selected();

    void Deselected();

    bool Get_Selected();
}