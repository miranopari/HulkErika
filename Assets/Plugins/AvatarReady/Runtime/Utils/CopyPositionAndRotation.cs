using UnityEngine;

public class CopyPositionAndRotation : MonoBehaviour
{
    public enum Relative
    {
        Local,
        World,
    }

    public Transform transformToCopy;

    public Relative relative = Relative.World;

    void Update()
    {
        if (relative == Relative.Local)
        {
            transform.localPosition = transformToCopy.localPosition;
            transform.localRotation = transformToCopy.localRotation;
        }
        else if (relative == Relative.World)
        {
            transform.position = transformToCopy.position;
            transform.rotation = transformToCopy.rotation;
        }
    }
}
