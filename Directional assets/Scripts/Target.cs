using UnityEngine;

/// <summary>
/// Attach this script to the targets in your scene.
/// </summary>
public class Target : MonoBehaviour
{
    [SerializeField] public Color targetColor;
    [SerializeField] public bool needArrowIndicator = true;
    [SerializeField] public bool isEthan = false;
    public bool placed = false;

    private void Awake()
    {
        if (isEthan)
            gameObject.tag = "Ethan";
        else
            gameObject.tag = "Target"; 
    }
}
