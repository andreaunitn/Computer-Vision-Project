using UnityEngine;
using UnityEngine.UI;

public class JointProjector : MonoBehaviour
{
    public Camera mainCamera;
    public RectTransform canvasRectTransform;
    public Image jointCirclePrefab; // Prefab of the circle image
    public JointController jointController;
    
    private Image[] _jointCircles;

    void Start()
    {
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
    }
}