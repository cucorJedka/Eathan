using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(ArrowObjectPool))]
public class DirectionalIndicator : MonoBehaviour
{
    public GameObject cursor;
    private Camera mainCamera;

    private float cursorDistanceFromCamera = 5f;
    private float targetSafeFactor = -0.116f;
    private float arrowDistanceFromCursor = 0.3f;    
    public bool active = false;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        cursor.transform.SetParent(mainCamera.transform, true); 
        cursor.transform.localPosition = new Vector3(0, 0, cursorDistanceFromCamera); 
    }

    void Update()
    {
        if (active)
            DeactivateAllIndicators();
    }

    void LateUpdate()
    {
        if (active)
            DrawIndicators();
    }

    private void DrawIndicators()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Target");
        GameObject ethan = GameObject.FindGameObjectWithTag("Ethan");
        List<Target> targets = new List<Target>();
        objects.ToList().ForEach(obj =>
        {
            Target target = obj.GetComponent<Target>();
            if (target && target.placed) { 
                targets.Add(target); 
            }
        });

        targets.Add(ethan.GetComponent<Target>());

        foreach (Target target in targets)
        {
            Vector3 camToObjectDirection = target.transform.position - mainCamera.transform.position;
            camToObjectDirection.Normalize();

            if (target.needArrowIndicator && !IsTargetVisible(target))
            {
                Indicator arrow = ArrowObjectPool.current.GetPooledObject();

                if (target.isEthan)
                    arrow.transform.localScale = new Vector3(0.15f, 0.15f, 0.7f);

                Quaternion defaultRotation = arrow.DefaultRotation;
                arrow.SetColor(target.targetColor);
                Vector3 position;
                Quaternion rotation;
                GetArrowIndicatorPositionAndRotation(camToObjectDirection, out position, out rotation);
                arrow.transform.position = position;
                arrow.transform.rotation = rotation * defaultRotation;
                
                arrow.Activate(true);
            }
        }
    }

    private void DeactivateAllIndicators()
    {
        ArrowObjectPool.current.DeactivateAllPooledObjects();
    }

    private bool IsTargetVisible(Target target)
    {
        Vector3 targetViewportPosition = mainCamera.WorldToViewportPoint(target.transform.position);
        return (targetViewportPosition.z > 0 &&
            targetViewportPosition.x > targetSafeFactor &&
            targetViewportPosition.x < 1 - targetSafeFactor &&
            targetViewportPosition.y > targetSafeFactor &&
            targetViewportPosition.y < 1 - targetSafeFactor
            );
    }

    private void GetArrowIndicatorPositionAndRotation(Vector3 camToObjectDirection, out Vector3 position, out Quaternion rotation)
    {
        Vector3 origin = cursor.transform.position;
        Vector3 cursorIndicatorDirection = Vector3.ProjectOnPlane(camToObjectDirection, -1 * mainCamera.transform.forward);
        cursorIndicatorDirection.Normalize();

        if (cursorIndicatorDirection == Vector3.zero)
        {
            cursorIndicatorDirection = mainCamera.transform.right;
        }

        position = origin + cursorIndicatorDirection * arrowDistanceFromCursor;

        rotation = Quaternion.LookRotation(mainCamera.transform.forward, cursorIndicatorDirection);
    }
}
