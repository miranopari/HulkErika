using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Object : MonoBehaviour
{
    // Minimum punch speed to break a stone
    [SerializeField] private float minSpeedToBreak = 12f;

    // Gameobject of broken version of the stone
    [SerializeField] private GameObject explodedObjectPrefab;

    // Audio clips to play
    [SerializeField] private string[] breakSounds;
    [SerializeField] private string[] punchSounds;
    
    // Reference to ObjectManager to record punch speed at each punch
    private ObjectManager objectManager;

    private void Start()
    {
        objectManager = FindObjectOfType<ObjectManager>();
        if (objectManager == null)
        {
            Debug.Log("Could not find the Object Manager!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out HulkHands hand))
        {
            AudioManager audio = FindObjectOfType<AudioManager>();          

            // If punch speed is fast enough, explode the rock into pieces.
            if (hand.Speed >= minSpeedToBreak)
            {
                // Instantiate rock debris
                var destroyedcube = Instantiate(explodedObjectPrefab, transform.position, transform.rotation);
                objectManager.activeObjectList.Add(destroyedcube);

                // Play random breaking sound from list
                audio.Play(breakSounds[Random.Range(0, breakSounds.Length)]);

                // Haptic feedback
                HapticPulse(hand.handedness, 0.4f, 1f);

                // Delete the full rock go.
                objectManager.activeObjectList.Remove(this.gameObject);
                Destroy(this.gameObject);
            }
            // Otherwise, just hit the stone
            else
            {
                // By default, the stone remains straight up. Once hit, we want to allow
                // it to rotate in any direction.
                this.GetComponent<Rigidbody>().freezeRotation = false;

                // Play random punching sound from list
                audio.Play(punchSounds[Random.Range(0, punchSounds.Length)]);

                // Haptic feedback
                HapticPulse(hand.handedness, 0.2f, 0.5f);
            }

            // Record the punch speed for logging
            objectManager.hitList.Add(hand.Speed);
        }
    }
    
    // Apply vibration to the controller of the hand that hit the object    
    static void HapticPulse(SteamVR_Input_Sources handedness, float duration, float strength)
    {
        SteamVR_Actions.default_Haptic[handedness].Execute(0f, duration, 150f, strength);
    }

}
