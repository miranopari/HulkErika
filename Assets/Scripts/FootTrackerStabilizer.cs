using UnityEngine;

/// <summary>
/// Deletes the noise movements of the foot trackers cause by the Crocs shoes
/// vibrating when stomping.
/// </summary>
public class FootTrackerStabilizer : MonoBehaviour
{
    [SerializeField] float accelerometerUpdateInterval = 1.0f / 60.0f;
    [SerializeField] float lowPassKernelWidthInSeconds = 1.0f;

    private float lowPassFilterFactor;
    private Vector3 lowPassValue = Vector3.zero;

    private void Start()
    {
        lowPassFilterFactor = accelerometerUpdateInterval / lowPassKernelWidthInSeconds;
        lowPassValue = transform.position;
    }

    Vector3 LowPassFilterAccelerometer(Vector3 prevValue)
    {
        Vector3 newValue = Vector3.Lerp(prevValue, transform.position, lowPassFilterFactor);
        return newValue;
    }

    private void LateUpdate()
    {
        lowPassValue = LowPassFilterAccelerometer(lowPassValue);
        transform.position = lowPassValue;
    }
}
