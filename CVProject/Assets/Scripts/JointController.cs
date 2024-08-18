using System;
using EditorAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

public class JointController : MonoBehaviour
{
    [Serializable]
    public struct Joint
    {
        public GameObject joint;
        public Color color;
    }

    [SerializeField, DataTable] public Joint[] joints;
    [SerializeField] private GameObject leftToeBase;
    [SerializeField] private GameObject leftFoot;
    [SerializeField] private GameObject leftLeg;
    [SerializeField] private GameObject leftUpLeg;
    [SerializeField] private GameObject rightToeBase;
    [SerializeField] private GameObject rightFoot;
    [SerializeField] private GameObject rightLeg;
    [SerializeField] private GameObject rightUpLeg;
    [SerializeField] private GameObject leftHand;
    [SerializeField] private GameObject leftForeArm;
    [SerializeField] private GameObject leftArm;
    [SerializeField] private GameObject leftShoulder;
    [SerializeField] private GameObject rightHand;
    [SerializeField] private GameObject rightForeArm;
    [SerializeField] private GameObject rightArm;
    [SerializeField] private GameObject rightShoulder;
    [SerializeField] private GameObject spine;
    [SerializeField] private GameObject upperChest;

    public int[][] bones =
    {
        new int[] {0,1},
        new int[] {1,2},
        new int[] {2,3},
        new int[] {4,5},
        new int[] {5,6},
        new int[] {6,7},
        new int[] {3,16},
        new int[] {7,16},
        new int[] {16,17},
        new int[] {8,9},
        new int[] {9,10},
        new int[] {10,11},
        new int[] {12,13},
        new int[] {13,14},
        new int[] {14,15},
        new int[] {11,17},
        new int[] {15,17},
    };
    
    public bool useRandomColor = true;

    public Vector3[] screenPointsC1;
    public Vector3[] screenPointsC2;
    public Camera camera1;
    public Camera camera2;

    void Awake()
    {
        joints = new Joint[18];
        joints[0].joint = leftToeBase;
        joints[1].joint = leftFoot;
        joints[2].joint = leftLeg;
        joints[3].joint = leftUpLeg;
        joints[4].joint = rightToeBase;
        joints[5].joint = rightFoot;
        joints[6].joint = rightLeg;
        joints[7].joint = rightUpLeg;
        joints[8].joint = leftHand;
        joints[9].joint = leftForeArm;
        joints[10].joint = leftArm;
        joints[11].joint = leftShoulder;
        joints[12].joint = rightHand;
        joints[13].joint = rightForeArm;
        joints[14].joint = rightArm;
        joints[15].joint = rightShoulder;
        joints[16].joint = spine;
        joints[17].joint = upperChest;
        
        if (useRandomColor)
        {
            AssignColors();
        }

        screenPointsC1 = new Vector3[joints.Length];
        screenPointsC2 = new Vector3[joints.Length];
    }
    
    void Update() {}

    Color GetRandomColor()
    {
        float r = Random.Range(0f, 1f);
        float g = Random.Range(0f, 1f);
        float b = Random.Range(0f, 1f);

        return new Color(r, g, b);
    }
    
    void AssignColors()
    {
        int size = joints.Length;
        for (int i = 0; i < size; i++)
        {
            joints[i].color = GetRandomColor();
        }
    }
}
