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
    
    public struct Bone
    {
        public Joint End1;
        public Joint End2;
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
    public bool useRandomColor = true;

    void Start()
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
