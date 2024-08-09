using UnityEngine;
using UnityEngine.UI;

public class JointProjector : MonoBehaviour
{
    public Camera mainCamera;
    public RectTransform canvasRectTransform;
    public Image jointCirclePrefab; // Prefab of the circle image
    public JointController jointController;
    public Image imagePrefab; // Prefab of the rectangle to be drawn as bone
    
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

            // Check if the joint is within the camera view
            if (screenPoint.z > 0)
            {
                _jointCircles[i].gameObject.SetActive(true);
                
                // Convert screen position to canvas position
                Vector2 canvasPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRectTransform, screenPoint, mainCamera, out canvasPos);

                // Set the position of the circle image
                _jointCircles[i].rectTransform.anchoredPosition = canvasPos;
            }
            else
            {
                _jointCircles[i].gameObject.SetActive(false);
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

}