using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomLeftWall : Wall
{
    protected override void setDestroyDirection()
    {
        this.destroyDirection = Vector3.Lerp(Vector3.back, Vector3.down, 0.5f) * 0.1f;
    }
}
