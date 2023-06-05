using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ForceBodyPosition : MonoBehaviour
{
    public GameObject HeadTarget;

    //private Animator animator;

    //private float headToHipsYOffset;

    private void Awake()
    {
        if (HeadTarget == null)
            throw new ArgumentNullException(nameof(HeadTarget) + " is required");

        if (!HeadTarget.name.ToLower().Contains("head"))
            Debug.LogWarning("Check " + nameof(HeadTarget) + " variable");
    }

    private void Start()
    {
        //animator = GetComponent<Animator>();

        //Transform head = animator.GetBoneTransform(HumanBodyBones.Head);
        //Transform hips = animator.GetBoneTransform(HumanBodyBones.Hips);
        //headToHipsYOffset = head.transform.position.y - hips.position.y;
    }

    private void Update()
    {
        // TODO it's perhaps not a good idea to based this position with the ground, an offset with the head might be better to support jumping for exemple
        RaycastHit hit;
        if (Physics.Raycast(HeadTarget.transform.position, Vector3.down, out hit, 10f, LayerMask.GetMask("Ground")))
        {
            transform.position = hit.point;
        }
    }
}
