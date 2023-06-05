using System;
using UnityEngine;

public class ForceBodyRotation : MonoBehaviour
{
    public GameObject HeadTarget;

    private void Awake()
    {
        if (HeadTarget == null)
            throw new ArgumentNullException(nameof(HeadTarget) + " is required");

        if (!HeadTarget.name.ToLower().Contains("head"))
            Debug.LogWarning("Check " + nameof(HeadTarget) + " variable");
    }

    private void Update()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, HeadTarget.transform.eulerAngles.y, transform.eulerAngles.z);
    }
}
