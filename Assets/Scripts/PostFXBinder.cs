using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostFXBinder : MonoBehaviour
{
    private GameObject postFXObj;
    private Material postFXMaterial;
    private Vector3 baseScale;

    void Start()
    {
        postFXObj = GameObject.Find("RenderedQuad");
        postFXMaterial = postFXObj.GetComponent<Renderer>().material;
        baseScale = new Vector3().fromValue(0.3f);
    }
    
    public void bindBlurPosAndSpd(Vector3 pos, float spd)
    {
        postFXObj.transform.setPositionXY(pos);
        postFXObj.transform.localScale = baseScale.xy() * spd;
        //postFXObj.GetComponent<Renderer>().enabled = true;
        postFXMaterial.SetFloat("_CollisionSpeed", spd);
    }

    public void disableFX()
    {
        postFXObj.transform.setPositionXY(new Vector2(10, 10));
        //postFXObj.GetComponent<Renderer>().enabled = false;
    }

    private void OnDestroy()
    {
        Destroy(this.postFXMaterial);
    }
}
