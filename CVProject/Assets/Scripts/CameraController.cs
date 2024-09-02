using EditorAttributes;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject camera1;
    public GameObject camera2;
    
    public float camDistance = 0.03f;

    void Start()
    {
       SetCameraPosition();
    }
    
    void Update()
    {}

    [Button]
    void SetCameraPosition()
    {
        camera1.transform.localPosition = transform.localPosition + new Vector3(camDistance / 2, 0f, 0f);
        camera2.transform.localPosition = transform.localPosition + new Vector3(-camDistance / 2, 0f, 0f);
    }
}
