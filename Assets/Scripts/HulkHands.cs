using UnityEngine;
using Valve.VR;

public class HulkHands : MonoBehaviour
{
    public SteamVR_Input_Sources handedness;
    public float minSpeedThreshold = 0.001f;
    public float forceScale = 1.0f;

    [SerializeField] private float m_speed;
    private Vector3 m_lastPosition;
    private Vector3 m_velocity;

    public float Speed => m_speed;

    public Vector3 Velocity => m_velocity;


    private void Reset()
    {
        minSpeedThreshold = 0.001f;
    }

    // Start is called before the first frame update
    void Start()
    {
        m_lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        m_velocity = (transform.position - m_lastPosition) / Time.deltaTime;
        m_speed = m_velocity.magnitude;
        if (m_speed < minSpeedThreshold)
            m_speed = 0.0f;

        // Apply scale
        m_speed *= forceScale;

        m_lastPosition = transform.position;
    }
}
