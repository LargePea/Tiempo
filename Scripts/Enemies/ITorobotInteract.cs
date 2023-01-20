using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITorobotInteract
{
    public void Collide(Vector3 direction, float force);
}
