using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureMotion : MonoBehaviour
{
    public float distanceTravelled = 0f;
    public Transform objectTransform;
    private Vector3 lastPosition;
    public bool isMeasuring = false;

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMeasuring)
        {
            var positionDif = (objectTransform.position - lastPosition).magnitude;
            if (positionDif > 0.001)
            {
                distanceTravelled += (objectTransform.position - lastPosition).magnitude;
            }
            lastPosition = objectTransform.position;
        }
    }
}
