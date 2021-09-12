using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BorderGlass : MonoBehaviour
{
    protected Material mat;
    protected Transform ballTransform;

    protected virtual void Start()
    {
        mat = this.GetComponent<MeshRenderer>().material;
        ballTransform = GameObject.FindGameObjectWithTag("Player")
            .transform;
    }

    protected void Update()
    {
        // feed the shader
        mat.SetVector("ballPos", ballTransform.position);
    }

    protected void OnDestroy()
    {
        Destroy(this.mat);
    }
}
