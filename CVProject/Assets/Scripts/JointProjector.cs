using UnityEngine;
using UnityEngine.UI;

public class JointProjector : MonoBehaviour
{
    public Camera mainCamera;
    public RectTransform canvasRectTransform;
    public Image jointCirclePrefab; // Prefab of the circle image
    public JointController jointController;
    public Image imagePrefab; // Prefab of the rectangle to be drawn as bone

    public bool isCamera1;
    public Transform stereoCamera;
    public GameObject spherePrefab; // Prefab of the sphere to be rendered in the scene

    public CameraController cameraController;
    
    private Image[] _jointCircles;
    private Image[] _boneImages;
    private GameObject[] _spheres;

    private float _focalLengthMM; // Focal length in mm (from the Physical Camera settings)
    private float _sensorSizeX; // Width of the sensor in mm
    private int _imageWidthPixels; // Get the resolution of the rendered image (in pixels)
    private float _baseline; // Baseline (in meters)
    private Vector3 _leftCameraPosition; // Left camera position
    private Quaternion _leftCameraRotation; // Left camera rotation in world space
    
    void Start()
    {
        // Initialize bones as images
        _boneImages = new Image[jointController.bones.Length];
        for (int i = 0; i < jointController.bones.Length; i++)
        {
            _boneImages[i] = Instantiate(imagePrefab, canvasRectTransform);
        }
        
        // Initialize circles for each joint
        _jointCircles = new Image[jointController.joints.Length];
        for (int i = 0; i < jointController.joints.Length; i++)
        {
            _jointCircles[i] = Instantiate(jointCirclePrefab, canvasRectTransform);
            _jointCircles[i].color = jointController.joints[i].color;
        }

        if (isCamera1)
        {
            // Initialize spheres for each joint
            _spheres = new GameObject[jointController.joints.Length];
            for (int i = 0; i < jointController.joints.Length; i++)
            {
                _spheres[i] = Instantiate(spherePrefab);
                _spheres[i].GetComponent<MeshRenderer>().material.color = jointController.joints[i].color;
            }   
        }
    }

    void Update()
    {
        // Render circles
        for (int i = 0; i < jointController.joints.Length; i++)
        {
            // Convert the joint's world position to screen position
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(jointController.joints[i].joint.transform.position);
            
            // Save joints position
            if (isCamera1)
            {
                jointController.screenPointsC1[i] = screenPoint;
            }
            else
            {
                jointController.screenPointsC2[i] = screenPoint;
            }
            
            // Check if the joint is within the camera view
            if (screenPoint.z > 0)
            {
                _jointCircles[i].gameObject.SetActive(true);
                
                // Convert screen position to canvas position
                Vector2 canvasPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPoint, mainCamera, out canvasPos);

                // Set the position of the circle image
                _jointCircles[i].rectTransform.anchoredPosition = canvasPos;
            }
            else
            {
                _jointCircles[i].gameObject.SetActive(false);
            }

            if (isCamera1)
            {
                // Update parameters for triangulation
                _focalLengthMM = mainCamera.focalLength;
                _sensorSizeX = mainCamera.sensorSize.x;
                _imageWidthPixels = Screen.width;
                _baseline = cameraController.camDistance;
                _leftCameraPosition = stereoCamera.transform.position;
                _leftCameraRotation = stereoCamera.transform.rotation;
                
                _spheres[i].transform.position = Triangulate(jointController.screenPointsC1[i], jointController.screenPointsC2[i]);
            }
        }
        
        // Render bones
        for (int i = 0; i < jointController.bones.Length; i++)
        {
            Vector3 screenPos0 = WorldToScreenPointProjected(mainCamera, jointController.joints[jointController.bones[i][0]].joint.transform.position);
            Vector2 canvasPos0;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPos0, mainCamera, out canvasPos0);
            Vector3 screenPos1 = WorldToScreenPointProjected(mainCamera, jointController.joints[jointController.bones[i][1]].joint.transform.position);
            Vector2 canvasPos1;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPos1, mainCamera, out canvasPos1);
            
            if (screenPos0.z < 0 && screenPos1.z < 0)
            {
                _boneImages[i].gameObject.SetActive(false);
            }
            else
            {
                _boneImages[i].gameObject.SetActive(true);
            }
            
            // Draw line
            CreateLine(canvasPos0, canvasPos1, _boneImages[i], _boneImages[i].rectTransform, Color.white);   
        }
    }
    
    // We use this function because WorldToScreenPoint of Unity does not work when the joint is behind the camera
    public static Vector3 WorldToScreenPointProjected(Camera camera, Vector3 worldPos)
    {
        Vector3 camNormal = camera.transform.forward;
        Vector3 vectorFromCam = worldPos - camera.transform.position;
        float camNormDot = Vector3.Dot(camNormal, vectorFromCam);
        
        if (camNormDot <= 0)
        {
            // We are behind the camera forward facing plane, project the position in front of the plane
            Vector3 proj = (camNormal * (camNormDot * 1.01f));
            worldPos = camera.transform.position + (vectorFromCam - proj);
        }

        return RectTransformUtility.WorldToScreenPoint(camera, worldPos);
    }
    
    void CreateLine(Vector2 positionOne, Vector2 positionTwo, Image image, RectTransform transform, Color color)
    {
        image.color = color;
        
        Vector2 midpoint = (positionOne + positionTwo) / 2f;
        transform.anchoredPosition = midpoint;
        
        Vector2 dir = positionTwo - positionOne;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        transform.localScale = new Vector3(dir.magnitude, transform.localScale.y, 1f);
    }

    Vector3 Triangulate(Vector3 screenPointC1, Vector3 screenPointC2)
    {
        // Given pixel coordinates from the left and right cameras
        Vector2 leftPixel = new Vector2(screenPointC1.x, screenPointC1.y);
        Vector2 rightPixel = new Vector2(screenPointC2.x, screenPointC2.y);
        
        // Calculate focal length in pixels
        float focalLengthPixelsX = (_focalLengthMM * _imageWidthPixels) / _sensorSizeX;
        
        float focalLength = focalLengthPixelsX;
        float disparity = rightPixel.x - leftPixel.x;

        // Calculate depth
        float Z = (focalLength * _baseline) / disparity;
        
        Vector2 principalPoint = new Vector2(Screen.width / 2, Screen.height / 2);

        // Calculate 3D point in the camera's local space
        float X = (leftPixel.x - principalPoint.x) * Z / focalLength;
        float Y = (leftPixel.y - principalPoint.y) * Z / focalLength;
        
        Vector3 pointInLeftCameraSpace = new Vector3(X, Y, Z);
        Vector3 pointInWorldSpace = _leftCameraPosition + (_leftCameraRotation * pointInLeftCameraSpace);
        
        return pointInWorldSpace;
    }
}