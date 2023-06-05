using UnityEngine;

public class CalibrateAvatar : MonoBehaviour
{
    public float userHeight = 1.65f;
    public float scale;
    [SerializeField]
    private Camera VRCamera; 

    private void Resize()
    {
        float headHeight = VRCamera.transform.localPosition.y;
        scale = userHeight / headHeight;
        transform.localScale = Vector3.one * scale;
    }
    
    void OnEnable()
    {
        Resize();
    }
}
