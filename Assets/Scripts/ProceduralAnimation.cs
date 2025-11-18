using UnityEngine;
using UnityEngine.Animations.Rigging;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class ProceduralAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public Animator playerAnimator;

    [Header("IK Targets")]
    //Transforms of each limb/Rig 
    public Transform lFootTarget;//Leg Left
    public Transform rFootTarget;//Right Leg
    public Transform leftTipTarget;//Left Toe
    public Transform rightTipTarget;    //Right Toe
    public Transform headTarget;


    public bool isClimbing=false;
    public TwoBoneIKConstraint leftArmConstraint;
    public TwoBoneIKConstraint rightArmConstraint;

    public Transform leftArmTarget;
    public Transform rightArmTarget;

    public Vector3 posPropHands = new Vector3(3f, 10f, 2f);
    public Vector3 rotationPropHands = new Vector3(90, 50f, 50);
    [Header("Step Settings")]
    public float FstepLength = 0.3f; //Toe length
    public float FstepHeight = 0.15f;//Toe Height
    public float legStepLength; //Leg length
    public float legStepHeight; //Leg Height
    public float walkSpeed = 5f;//walking speed
    //raycast to detect elevated or different levels of terrain
    [SerializeField] private float rayCastLength = 2;
    [SerializeField] private LayerMask groundCheck;


    private void Start()
    {
      leftArmConstraint.weight = 0f;
        rightArmConstraint.weight = 0f;
    }
    void Update()
    {
        //Repeating motion that is multiplied by the walkspeed
        float phase = Mathf.Repeat(Time.time * walkSpeed, 1f);

        //Feet and Toes are begin animating 
        if (playerAnimator.GetBool("IsMoving") == true)
        {
            AnimateLimb(rFootTarget, phase, new Vector3(0.10f, 0, 0), legStepLength, legStepHeight, transform.forward);
            AnimateLimb(lFootTarget, -phase, new Vector3(-0.10f, 0, 0), legStepLength, legStepHeight, transform.forward);

            AnimateLimb(rightTipTarget, phase, new Vector3(0.10f, 0, 0), FstepLength, FstepHeight, transform.forward);
            AnimateLimb(leftTipTarget, -phase, new Vector3(-0.10f, 0, 0), FstepLength, FstepHeight, transform.forward);
        }



        PropHandsWhenGrabbing(leftArmTarget, posPropHands, rotationPropHands);
        PropHandsWhenGrabbing(rightArmTarget, posPropHands, rotationPropHands);
        if(isClimbing ==true)
        {
            AnimateLimb(leftArmTarget, phase, new Vector3(-0.15f, 0, 0), 2, 2, transform.right);
                AnimateLimb(rightArmTarget, phase, new Vector3(0.15f,0,0), 2, 2, transform.right);

        }
    }

    //takes in the limb and the repeating phase along with a predetermined foot placement to avoid the feet being too close
    //Length and height as well

    void AnimateLimb(Transform limb, float phase, Vector3 limbPlacement, float length, float heightStep, Vector3 direction)

    {
        //Using Mathf.sin to multiply phase with mathf.pi since pi is 180 degrees in RADIANS!
        //multiplying it by two would give us a proper back and forward motion depending on the length
        float forward = Mathf.Sin(phase * Mathf.PI *2) * length;
        float height = Mathf.Max(0, Mathf.Sin(phase * Mathf.PI * 2)) * heightStep;
        // applying it to the basePosition and added to each of the limbs along with the characters transform forward etc.
        //basically gives the feet a place to track or move forward to 
        Vector3 basePos = transform.position;
        limb.position = basePos + direction * forward + Vector3.up * height + limbPlacement ;
  
        Vector3 terrainPosition = CollideWithTerrain(limb.position, groundCheck);
        limb.position = terrainPosition;

       
    }

    void PropHandsWhenGrabbing(Transform arm, Vector3 positionProp, Vector3 rotationProp)
    {
        if (isClimbing == true)
        {
            
            Vector3 startingRotation = rotationProp;
            Vector3 startingPosition = positionProp;

            arm.localPosition = Vector3.Lerp(
                arm.localPosition,
                startingPosition,
                Time.deltaTime * 5f
            );

            arm.localRotation = Quaternion.Lerp(
                arm.localRotation,
                Quaternion.Euler(startingRotation),
                Time.deltaTime * 5f
            );


        }
    }
    

        Vector3 CollideWithTerrain(Vector3 legPosition, LayerMask layer)
        {
            Ray ray = new Ray(legPosition + rayCastLength * Vector3.up, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 2f * rayCastLength, layer))
            {
                return hit.point; // returns the collision point
            }

            // if there is no collision with the ground go back to the original leg position
            return legPosition;
        }

    }

    

