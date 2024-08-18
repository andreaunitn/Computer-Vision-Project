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
    public GameObject sphere;
    
    private Image[] _jointCircles;
    private Image[] _boneImages;
    
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

            if (i == 14 && isCamera1)
            {
                sphere.transform.position = Triangulate(jointController.screenPointsC1[i], jointController.screenPointsC2[i]);
            }
        }
        
        // Render bones
        for (int i = 0; i < jointController.bones.Length; i++)
        {
            Vector3 screenPos0 = mainCamera.WorldToScreenPoint(jointController.joints[jointController.bones[i][0]].joint.transform.position);
            Vector2 canvasPos0;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPos0, mainCamera, out canvasPos0);
            Vector3 screenPos1 = mainCamera.WorldToScreenPoint(jointController.joints[jointController.bones[i][1]].joint.transform.position);
            Vector2 canvasPos1;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPos1, mainCamera, out canvasPos1);
            CreateLine(canvasPos0, canvasPos1, _boneImages[i], _boneImages[i].rectTransform, Color.white);
        }
    }
    
    void CreateLine(Vector2 positionOne, Vector2 positionTwo, Image image, RectTransform transform, Color color)
    {
        // Set the color of the image
        image.color = color;
    
        // Calculate the midpoint between the two positions
        Vector2 midpoint = (positionOne + positionTwo) / 2f;

        // Set the position of the RectTransform to the midpoint
        transform.anchoredPosition = midpoint;
        // Calculate the direction from positionOne to positionTwo
        Vector2 dir = positionTwo - positionOne;

        // Calculate the angle in degrees from the direction vector
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Lock rotation to the Z-axis by setting the X and Y rotation components to zero
        transform.localRotation = Quaternion.Euler(0f, 0f, angle);

        // Set the scale of the RectTransform. 
        // The width (x scale) is set to the distance between the points
        // Ensure Y and Z scales are consistent to prevent unwanted rotations
        transform.localScale = new Vector3(dir.magnitude, transform.localScale.y, 1f);
    }

    Vector3 Triangulate(Vector3 screenPointC1, Vector3 screenPointC2)
    {
        // Given pixel coordinates from the left and right cameras
        Vector2 leftPixel = new Vector2(screenPointC1.x, screenPointC1.y);
        Vector2 rightPixel = new Vector2(screenPointC2.x, screenPointC2.y);
        
        // Convert focal length from millimeters to pixels
        // Access the camera component
        Camera camera = GetComponent<Camera>();

        // Focal length in mm (from the Physical Camera settings)
        float focalLengthMM = camera.focalLength;

        // Get the sensor size (in mm) from the camera
        float sensorSizeX = camera.sensorSize.x; // Width of the sensor in mm

        // Get the resolution of the rendered image (in pixels)
        int imageWidthPixels = Screen.width;

        // Calculate focal length in pixels
        float focalLengthPixelsX = (focalLengthMM * imageWidthPixels) / sensorSizeX;
        
        // Focal length (in pixels) and baseline (in meters)
        float focalLength = focalLengthPixelsX;
        float baseline = 0.08f;

        // Calculate disparity
        float disparity = rightPixel.x - leftPixel.x;

        // Calculate depth
        float Z = (focalLength * baseline) / disparity;

        // Principal point (usually center of the image)
        Vector2 principalPoint = new Vector2(Screen.width / 2, Screen.height / 2);

        // Calculate 3D point in the camera's local space
        float X = (leftPixel.x - principalPoint.x) * Z / focalLength;
        float Y = (leftPixel.y - principalPoint.y) * Z / focalLength;
        
        // Position of the point in the left camera's local coordinate system
        Vector3 pointInLeftCameraSpace = new Vector3(X, Y, Z);
        
        // Get the left camera's position and rotation in world space
        Vector3 leftCameraPosition = stereoCamera.transform.position;
        Quaternion leftCameraRotation = stereoCamera.transform.rotation;

        // If no rotation is applied to the camera (for simplicity):
        Vector3 pointInWorldSpace = leftCameraPosition + (leftCameraRotation * pointInLeftCameraSpace);

        return pointInWorldSpace;
    }
}