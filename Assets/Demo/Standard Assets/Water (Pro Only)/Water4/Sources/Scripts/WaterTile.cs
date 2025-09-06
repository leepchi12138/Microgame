using UnityEngine;

[ExecuteInEditMode]
public class WaterTile : MonoBehaviour
{
    public PlanarReflection reflection;
    public WaterBase waterBase;

    public void Start()
    {
        AcquireComponents();
    }

    private void AcquireComponents()
    {
        if (!reflection)
        {
            if (transform.parent)
                reflection = transform.parent.GetComponent<PlanarReflection>();
            else
                reflection = GetComponent<PlanarReflection>();
        }

        if (!waterBase)
        {
            if (transform.parent)
                waterBase = transform.parent.GetComponent<WaterBase>();
            else
                waterBase = GetComponent<WaterBase>();
        }
    }

#if UNITY_EDITOR
    public void Update()
    {
        AcquireComponents();
    }
#endif

    public void OnWillRenderObject()
    {
        if (reflection)
            reflection.WaterTileBeingRendered(transform, UnityEngine.Camera.current);
        if (waterBase)
            waterBase.WaterTileBeingRendered(transform, UnityEngine.Camera.current);
    }
}
