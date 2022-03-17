using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IInteractable
{
    GameObject gameObject { get; }

    void Move(Touch touch);

    void Scale(float scale);

    void Rotate(Quaternion rotate);

    void Toggle_Active();

    bool Get_Selected();
}