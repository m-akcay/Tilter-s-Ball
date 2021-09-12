using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureRenderer : MonoBehaviour
{
    public static Texture2D pointTexture;
    public Texture2D pt;
    public RenderTexture rt;
    private Material quadMat;
    void Start()
    {
        Util.createPointTexture();
        //pt = pointTexture;
        Vector2 screenSize = new Vector2(Display.main.renderingWidth, Display.main.renderingHeight) / 2;
        quadMat = GameObject.Find("RenderedQuad").GetComponent<Renderer>().material;
        quadMat.SetVector("_ScreenWidthHeight", screenSize);

        if (rt.IsCreated())
            rt.Release();

        rt.width = (int)screenSize.x;
        rt.height= (int)screenSize.y;
        rt.format = RenderTextureFormat.DefaultHDR;
        rt.filterMode = FilterMode.Trilinear;
        rt.Create();

        quadMat.SetTexture("_MainTex", rt);
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        Camera dummyCam = GameObject.Find("Dummy Camera").GetComponent<Camera>();
        dummyCam.clearFlags = CameraClearFlags.Skybox;
        Shader.SetGlobalMatrix("_projMatrix", dummyCam.worldToCameraMatrix);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnDestroy()
    {
        Destroy(quadMat);
    }

}
