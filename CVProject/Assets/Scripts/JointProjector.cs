using UnityEngine;
using UnityEngine.UI;

public class JointProjector : MonoBehaviour
{
    public Camera mainCamera;
    public RectTransform canvasRectTransform;
    public Image jointCirclePrefab; // Prefab of the circle image
    public Transform[] joints; // Array of joint transforms

    public Color jointsColor;
    
    private Image[] jointCircles;

    void Start()
    {
        // Initialize circles for each joint
        jointCircles = new Image[joints.Length];
        for (int i = 0; i < joints.Length; i++)
        {
            jointCircles[i] = Instantiate(jointCirclePrefab, canvasRectTransform);
        }
    }

    void Update()
    {
        for (int i = 0; i < joints.Length; i++)
        {
            // Convert the joint's world position to screen position
            Vector3 screenPoint = mainCamera.WorldToScreenPoint(joints[i].position);

            // Check if the joint is within the camera view
            if (screenPoint.z > 0)
            {
                jointCircles[i].gameObject.SetActive(true);
                
                // Convert screen position to canvas position
                Vector2 canvasPos;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    canvasRectTransform, screenPoint, mainCamera, out canvasPos);

                // Set the position of the circle image
                jointCircles[i].rectTransform.anchoredPosition = canvasPos;
            }
            else
            {
                jointCircles[i].gameObject.SetActive(false);
            }
        }
    }
}