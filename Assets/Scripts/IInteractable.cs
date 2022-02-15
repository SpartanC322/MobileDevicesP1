using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable
{
    GameObject gameObject { get; }

    void Move(Touch t);

    void Scale(float percentageChange);

    void Rotate(Vector3 v);

    void Toggle_Active();

    bool Get_Selected();
}