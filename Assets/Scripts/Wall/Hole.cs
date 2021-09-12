using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : Wall
{
    public Vector2 purplePointCenter;

    protected override void setUpGameObject(Vector3 center, Vector3 scale)
    {
        this.gameObject.layer = Wall.LAYER;
        this.gameObject.transform.position = center;
        this.gameObject.transform.localScale = scale;
        
        this.gameObject.name = "HOLE_" + this.levelNo;
        this.gameObject.tag = "hole";
        this.activated = false;

        float randX = Random.Range(-0.45f, 0.45f);
        float randY = Random.Range(-0.45f, 0.45f);

        purplePointCenter = new Vector2(randX, randY);
    }
    public override void setColor(Color color)
    {
        mat = Instantiate(Resources.Load("Materials/HoleMaterial") as Material);
        mat.SetVector("Vector2_A2A9D6DA", purplePointCenter);
        this.GetComponent<MeshRenderer>().material = mat;

        purplePointCenter = transform.TransformPoint(purplePointCenter);
    }
    protected override void setDestroyDirection()
    {
        this.destroyDirection = Vector3.back * 0.1f;
    }

    public int calculateScore(Vector2 contactPtWS)
    {
        float hitDist = Vector2.Distance(contactPtWS, purplePointCenter);

        if (hitDist < 0.12f)
            return 3;
        else if (hitDist < 0.25f)
            return 2;
        else
            return 1;
    }
}
