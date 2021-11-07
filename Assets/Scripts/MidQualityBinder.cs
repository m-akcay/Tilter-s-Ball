using UnityEngine;
using System.Linq;

public class MidQualityBinder : MonoBehaviour
{
    [SerializeField] private Light directionalLight = null;

    private void OnEnable()
    {
        updateShader();
    }

    private void updateShader()
    {
        //Shader.SetGlobalVector("_LightDirection", directionalLight.transform.forward);
        directionalLight.gameObject.SetActive(true);
        Shader.SetGlobalVector("_LightDirection", -directionalLight.transform.forward);
        directionalLight.gameObject.SetActive(false);

        Shader.SetGlobalVector("_ViewPos", Camera.main.transform.position);

        GameObject.FindGameObjectsWithTag("wall").ToList().ForEach(
            wallObj =>
            {
                var wall = wallObj.GetComponent<Wall>();
                wall.setColor(wall.color);
            }
        );

    }

}
