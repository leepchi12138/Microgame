using UnityEngine;

[RequireComponent(typeof(WaterBase))]
[ExecuteInEditMode]
public class SpecularLighting : MonoBehaviour
{
    public Transform specularLight;
    private WaterBase waterBase = null;

    public void Start()
    {
        waterBase = gameObject.GetComponent<WaterBase>();
    }

    public void Update()
    {
        if (!waterBase)
            waterBase = gameObject.GetComponent<WaterBase>();

        if (specularLight && waterBase.sharedMaterial)
            waterBase.sharedMaterial.SetVector("_WorldLightDir", specularLight.forward);
    }
}
