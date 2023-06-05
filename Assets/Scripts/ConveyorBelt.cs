using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public GameObject belt;
    public Transform endpoint;
    public float speed;
    public bool isLast = false;

    private void OnTriggerStay(Collider other)
    {
        other.transform.position = Vector3.MoveTowards(other.transform.position, endpoint.position, speed * Time.deltaTime);
    }

    private void OnTriggerExit(Collider other)
    {
        if (isLast)
        {
            other.GetComponent<Rigidbody>().freezeRotation = false;
        }
    }
}
