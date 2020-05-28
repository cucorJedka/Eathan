using UnityEngine;

public class Indicator : MonoBehaviour
{
    private MeshRenderer indicatorRenderer;
    private Material indicatorMaterial;
    private Quaternion defaultRotation;

    public Quaternion DefaultRotation
    {
        get
        {
            return defaultRotation;
        }
        set
        {
            if (defaultRotation != value)
            {
                defaultRotation = value;
            }
        }
    }

    public bool Active
    {
        get
        {
            return transform.gameObject.activeInHierarchy;
        }
    }

    void Awake()
    {
        indicatorRenderer = transform.GetComponent<MeshRenderer>();
        if (indicatorRenderer == null)
        {
            indicatorRenderer = transform.gameObject.AddComponent<MeshRenderer>();
        }
        indicatorMaterial = indicatorRenderer.material;
        foreach (Collider collider in transform.gameObject.GetComponents<Collider>())
        {
            Destroy(collider);
        }

        foreach (Rigidbody rigidBody in transform.gameObject.GetComponents<Rigidbody>())
        {
            Destroy(rigidBody);
        }
    }

    public void SetColor(Color color)
    {
        indicatorMaterial.color = color;
        indicatorMaterial.SetColor("_TintColor", color);
    }

    public void Activate(bool value)
    {
        transform.gameObject.SetActive(value);
    }
}
