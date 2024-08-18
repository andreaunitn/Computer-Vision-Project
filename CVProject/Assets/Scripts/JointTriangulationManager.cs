using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class JointTriangulationManager : MonoBehaviour
{
    public JointController jointController;
    public GameObject spherePrefab; // // Prefab of the sphere to be put in the 3d world

    private List<GameObject> instantiatedSpheres;
    
    // Start is called before the first frame update
    void Start()
    {
        instantiatedSpheres = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < jointController.joints.Length; i++)
        {
            // For debug print only the position of one joint. To be removed later
            Vector3 joint3DPos = Triangulate(
                jointController.screenPointsC1[i],
                jointController.screenPointsC2[i],
                jointController.camera1,
                jointController.camera2
            );
        
            // For debug
            //Debug.Log($"Joint {i} 3D Position: {joint3DPos}");
            
            // If there's already a sphere at this index, move it to the new position
            if (i < instantiatedSpheres.Count)
            {
                instantiatedSpheres[i].transform.position = joint3DPos;
            }
            else
            {
                // Instantiate a new sphere at the calculated position
                GameObject newSphere = Instantiate(spherePrefab, joint3DPos, Quaternion.identity);
                instantiatedSpheres.Add(newSphere);
            }
        }
    }

    Vector3 Triangulate(Vector3 screenPointC1, Vector3 screenPointC2, Camera camera1, Camera camera2)
    {
        Ray rayC1 = camera1.ScreenPointToRay(screenPointC1);
        Ray rayC2 = camera2.ScreenPointToRay(screenPointC2);

        Vector3 point3D = FindClosestPointOnRays(rayC1, rayC2, camera1.transform.position, camera2.transform.position);
        return point3D;
    }

    Vector3 FindClosestPointOnRays(Ray rayC1, Ray rayC2, Vector3 originC1, Vector3 originC2)
    {
        Vector3 dirC1 = rayC1.direction;
        Vector3 dirC2 = rayC2.direction;

        Vector3 closestPointC1;
        Vector3 closestPointC2;
        ClosestPointsOnTwoLines(out closestPointC1, out closestPointC2, originC1, dirC1, originC2, dirC2);

        return (closestPointC1 + closestPointC2) / 2;
    }
    
    void ClosestPointsOnTwoLines(out Vector3 closestPoint1, out Vector3 closestPoint2, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
    {
        float a = Vector3.Dot(lineVec1, lineVec1);
        float b = Vector3.Dot(lineVec1, lineVec2);
        float e = Vector3.Dot(lineVec2, lineVec2);

        float d = a * e - b * b;

        if (Mathf.Abs(d) < Mathf.Epsilon)
        {
            closestPoint1 = linePoint1;
            closestPoint2 = linePoint2;
            return;
        }

        Vector3 r = linePoint1 - linePoint2;
        float c = Vector3.Dot(lineVec1, r);
        float f = Vector3.Dot(lineVec2, r);

        float s = (b * f - c * e) / d;
        float t = (a * f - c * b) / d;

        closestPoint1 = linePoint1 + lineVec1 * s;
        closestPoint2 = linePoint2 + lineVec2 * t;
    }
}
