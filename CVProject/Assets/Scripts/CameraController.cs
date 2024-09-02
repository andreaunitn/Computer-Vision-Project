using EditorAttributes;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject camera1;
    public GameObject camera2;
    
    public float camDistance = 0.08f;

    void Start()
    {
       SetCameraPosition();
       
       camera1.SetActive(false);
       camera1.SetActive(true);
    }
    
    void Update()
    {}

    [Button]
    void SetCameraPosition()
    {
        camera1.transform.localPosition = new Vector3(camDistance / 2, 0f, 0f);
        camera2.transform.localPosition = new Vector3(-camDistance / 2, 0f, 0f);
    }
}
