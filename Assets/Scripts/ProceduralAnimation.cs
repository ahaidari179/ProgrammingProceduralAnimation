using UnityEngine;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class ProceduralAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    [Header("IK Targets")]
    //Transforms of each limb/Rig 
    public Transform lFootTarget;//Leg Left
    public Transform rFootTarget;//Right Leg
    public Transform leftTipTarget;//Left Toe
    public Transform rightTipTarget;    //Right Toe
    public Transform headTarget;

    [Header("Step Settings")]
    public float FstepLength = 0.3f; //Toe length
    public float FstepHeight = 0.15f;//Toe Height
    public float legStepLength; //Leg length
    public float legStepHeight; //Leg Height
    public float walkSpeed = 5f;//walking speed
    public Transform camera;
    //raycast to detect elevated or different levels of terrain
    [SerializeField] private float rayCastLength = 2;
    [SerializeField] private LayerMask groundCheck;


    float horizontalInput;
    float verticalInput;
    private void Start()
    {
      
    }
    void Update()
    {
        //Repeating motion that is multiplied by the walkspeed
        float phase = Mathf.Repeat(Time.time * walkSpeed, 1f);

        //Feet and Toes are begin animating 
        AnimateFoot(rFootTarget, phase, new Vector3(0.10f, 0, 0), legStepLength, legStepHeight);
        AnimateFoot(lFootTarget, -phase, new Vector3 (-0.10f,0,0), legStepLength, legStepHeight);

        AnimateFoot(rightTipTarget, phase, new Vector3(0.10f, 0, 0), FstepLength, FstepHeight);
        AnimateFoot(leftTipTarget, -phase, new Vector3(-0.10f, 0, 0), FstepLength, FstepHeight);

    }

    //takes in the limb and the repeating phase along with a predetermined foot placement to avoid the feet being too close
    //Length and height as well

    void AnimateFoot(Transform limb, float phase, Vector3 footPlacement, float length, float heightStep)

    {
        //Using Mathf.sin to multiply phase with mathf.pi since pi is 180 degrees in RADIANS!
        //multiplying it by two would give us a proper back and forward motion depending on the length
        float forward = Mathf.Sin(phase * Mathf.PI *2) * length;
        float height = Mathf.Max(0, Mathf.Sin(phase * Mathf.PI * 2)) * heightStep;
        // applying it to the basePosition and added to each of the limbs along with the characters transform forward etc.
        //basically gives the feet a place to track or move forward to 
        Vector3 basePos = transform.position;
        limb.position = basePos + transform.forward * forward + Vector3.up * height + footPlacement ;
  
        Vector3 terrainPosition = CollideWithTerrain(limb.position);
        limb.position = terrainPosition;

    }

    Vector3 CollideWithTerrain( Vector3 legPosition)
    {
        Ray ray = new Ray(legPosition + rayCastLength * Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 2f * rayCastLength, groundCheck))
        {
            return hit.point; // returns the collision point
        }

        // if there is no collision with the ground go back to the original leg position
        return legPosition;
    }
  
        
}
    

