/// <summary>
/// CURRENT VERSION: 2.0 (Nov '16)
/// This script was originally written by Yan Dawid of Zelig Games.
/// 
/// KEY (for the annotations in this script):
/// -The one referred to as 'User' is the person who uses/edits this asset
/// -The one referred to as 'Player' is the person who plays the project build
/// -The one referred to as 'Character' is the in-game character that the player controls
/// 
/// This script is to NOT be redistributed and can only be used by the person that has purchased this asset.
/// Editing or altering this script does NOT make redistributing this script valid.
/// This asset can be used in both personal and commercial projects.
/// The user is free to edit/alter this script to their hearts content.
/// You can contact the author for support via the Zelig Games official website: http://zeliggames.weebly.com/contact.html
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Suimono.Core;
using ORKFramework.Behaviours;
using UnityEngine.UI;

[RequireComponent (typeof(Rigidbody))]							// The GameObject requires a Rigidbody component
[RequireComponent (typeof(CapsuleCollider))]					// The GameObject requires a CapsuleCollider component - Collider used for standard movements (can be another collider that covers entire body)
[RequireComponent (typeof(SphereCollider))]						// The GameObject requires a SphereCollider component - Collider used for lower movements such as rolling and crouching - Not really needecd if the collider above is the same size as this

public class ThirdPersonController : MonoBehaviour
{
	private Vector3 zeroVelocity;
    private Vector3 rbtogo;
    public Vector3 offset;

	private GamePadInputs gPI;									//the GamePadInputs.cs script attached to the handler in the scene
	private UsingItems1 uItems;
	private Animations currentAnim;
	private bool targetting;									//if the character is in target mode
	private bool inFree;										//if the character is using the 'free movement' camera
	[HideInInspector] public bool fPVing;						//if the character is in first person view
	[HideInInspector] public bool jumping;						//if the character is in its jumping state
	[HideInInspector] public bool onGround;						//if the character is on the ground
	private bool startedJump;									//is set just after the character is 'jumping' and has made some adjustments
	[HideInInspector] public CapsuleCollider capCol;            //reference to the characters Capsule Collider
	[HideInInspector] public SphereCollider sphereCol;          //reference to the characters Sphere Collider
	[HideInInspector] public float cHeight;						//the original height of the Capsule Collider
	[HideInInspector] public float cRad;						//the original radius of the capsule collider
	private Transform altLookAt;								//an alternative transform for the camera to look at during certain points
	private Rigidbody rb;										//the character Rigidbody component
	private Transform modelTransform;							//the transform of the characters model parent game object
    private bool hasLandedThisFrame;

	[Header("Drag-In Objects")]
    public GameObject characterModelObject;                     //the characters model parent game object
	public bool useBlobShadow;									//should the character use an attached blob shadow
    [SerializeField] private GameObject blobShadow;				//what is the blob shadow that the player wants to use?
    private GameObject blobShadowFollow;						//obj that positions the blob shadow
    public GameObject rootBoneGameObject;						//the root bone of the characters model
	[System.Serializable] public class FPVHiddenMeshes{public GameObject hiddenMesh; }	//The class made to contain meshes that will be hidden in FPV mode
	public FPVHiddenMeshes[] fPVHiddenMeshes;					//Which meshes will be hidden in FPV mode
	public GameObject fpvBonePosition;							//the bone of the characters model that the camera will be at when in FPV
	public GameObject characterRightHand;						//The obj that works as the position of the characters right hand
	public GameObject characterLeftHand;						//The obj that works as the position of the characters left hand
	public GameObject itemUnequippedPos;						//The obj that works as the position to put unequipped objects (e.g. sword and shield)

    private float newDirMag;
    private float forwardAngle;
    private bool braking;
    private float turnSpeed;
    private int rotIterate;
    private float angleToRot;
    private bool onSteepGround;
    private Vector3 targDir;
    private float targAngle;
    private float targDot;

	[Header("Set Collision Layers")]
    [SerializeField] private int characterCollisionLayer;		//the collision layer int of the character game object
	[SerializeField] private int groundCollisionLayer;			//the collision layer int of the ground game objects
	[SerializeField] private int slopesCollisionLayer;			//the collision layer int of the slope game objects
	[SerializeField] private int liftablesCollisionLayer;		//the collision layer int of liftable game objects
	[SerializeField] private int projectilesCollisionLayer;		//the collision layer int of projectiles
	[SerializeField] private int waterCollisionLayer;		    //the collision layer int of water

	[Header("Movement Variables")]
    public float gravityIntesnity;								//the scale factor of the gravity that affects the character
	public float maxSpeed;                                      //the scale factor of the characters movement speed

	private float locoSpeed;									//the speed at which the character moves (to be used by the Animator)
	public float additionalSprintSpeed;							//how much faster should the character move when sprinting?
	private float sprinting;									//if the character is sprinting
	public float currentSpeed;									//the current speed variable that the character is moving at
	public float acceleration;									//how quickly does the character pick up speed?
	private float accel;										//the current acceleration variable that the character is accelerating at
    public float rollSpeed;										//the speed at which the character rolls

	public float horizontalClimbSpeed;							//the speed at which the character climbs horizontally
	public float verticalClimbSpeed;							//the speed at which the character climbs vertically

    public float hangingMoveSpeed;								//the speed at which the character moves across a ledge when hanging from it

	public float crawlSpeed;									//how quickly can the character crawl at?
	[HideInInspector] public float originalMoveSpeed;			//a reference to the original movement speed
	public float airMomentum;				   					 //how sensitive is the mid-air movement when 'midAirMovement' is off?
	public float airMoveSensitivity;          					//how sensitive is the mid-air movement when 'midAirMovement' is on?
	private float tempTurnS;
	[Range(0f,1f)]	public float walkableSlopeTolerence;		//set from 0(very steep) to 1(very horizontal); how steep can the ground be for the character to walk on
	public float throwForce;									//How powerful is the throw of a lift-able object?

	private Vector3 forwardBeforeRoll;

	[SerializeField] private float targetFocusRange;            //the maximum distance that a target can be away from the character to be able to be focused on

	[Header("Toggle Features/Actions")]
    public bool sprintFunction;									//is the SPRINT function on?
	public bool rollFunction;									//is the ROLL function on?
	public bool crouchFunction;                                 //is the CROUCH/CRAWL function on?
    public bool inACrouchBox;
    public bool jumpFunction;                                   //can the character move around in mid-air freely?
    public bool midAirMovement;				                    //can the character move around in mid-air freely?
	public bool autoLedgeJumping;								//is the AUTOMATIC LEDGE JUMPING function on?
	public bool onButtonJumping;								//is the ON BUTTON PRESS JUMPING function on?
	public bool tripleGroundJumps;								//is the TRIPLE GROUND JUMPS function on?
	public bool sidewaysJump;									//is the SIDEWAYS JUMP function on?
	public bool canJumpFromGrab;								//can the character jump while grabbing (hanging from) a ledge?
	public bool canJumpFromClimb;								//can the character jump from climbing?
	public bool jumpCancelRoll;									//can the character cancel a roll with a jump (ledge jump not included)?
	public bool ledgeGrabbing;									//is the LEDGE GRABBING function on?
	public bool ledgeClimbing;									//is the LEDGE CLIMBING function on?
	public bool onButtonLedgeClimb;								//should the character climb/mount ledges through button-press only?
	public bool interactWithWater;								//can the character INTERACT WITH WATER?
	public bool canDiveIfNoSwim;								//will the character be able to dive if they can't swim (if !canSwim)
	public bool ladderClimbing;									//is the LADDER CLIMBING function on?
	public bool fenceClimbing;                                  //is the FENCE CLIMBING function on?
	public bool boxPushing;										//is the BOX PUSHING function on? (NEW - the character pushes by simply moving against the box)
	public bool oldBoxPushing;									//is the OLD BOX PUSHING function on? (OLD  - the character enters a pushing state and moves the box step by step)
    public bool onlyRollInTargetMode;                           //when in target mode, the character will only roll instead of also performing backflips and side-hops
	public bool freeCamera;										//is the FREE CAMERA function on?
	public bool targetCamera;									//is the TARGET CAMERA function on?
	public bool focusOnTargets;									//can the camera FOCUS ON TARGETS?
	public bool fpvCamera;										//is the FIRST PERSON VIEW CAMERA function on?
	public bool targetHoldMode;	
    public bool levitateWithSelect;                              //hold Select/Back to levitate

	[Header("Jump Settings")]
	public int amountOfJumps;									//how many jumps can the character perform before landing again?

	[System.Serializable] public class JumpForce{ public string jumpName; public float forceAmount; }	//Class for storing different types of jumps
	public JumpForce[] jumpForces = new JumpForce[3];			//List of different jump types (their names and force amounts)

	[HideInInspector] public bool canFPV;
	private int jumpType;										//the current type of jump that the character is performing
	private bool canSidewaysJump;								//can the character perform a sideways jump at a specific moment?
	private bool canDoNextJump;									//can the character perform the next jump in a series of triple ground jumps?
	private bool dontTripleJump;								//if the character has just double-jumped, they cannot do the triple jump
	private float dirMag;										//the previous magnitude of direction
	private int jumpNo;											//which jump is the character currently performing (initial/double/triple)
	private bool hardLanded;									//has the character made a hard landing? (as opposed to a running-land)
	private float skinWidth;									//the skin width around the collider to check for ground via raycasts
	private float addedSpeed;									//an added speed for certain movements
	private float jumpImpulse;									//the force of the characters jump
	private bool stoppedMidAir;
    private bool lockDirection;
    private float rollY;
	private bool slowFall;

    [HideInInspector] public bool inACutscene;					//is the character currently in a cutscene? (interacting with an NPC)
	[HideInInspector] public Vector3 cutsceneLookAtPos;			//the position in world space that the character looks at when interacting with an NPC in first-person view
	[HideInInspector] public string currentNPCName;				//the 'name' of the NPC gameobject that the character is currently interacting with
	[HideInInspector] public float leftX;						//a reference to the Horizontal Input Axis
	[HideInInspector] public float leftY;						//a reference to the Vertical Input Axis
	[HideInInspector] public Vector3 direction;					//a 2D vector made of leftX (for x) and leftY (for z)
	private Vector3 newDir;
	private bool hasUnClicked;									//checks if the player has let go of the triggers on a gamepad
	private bool huggingWall;									//if the character is 'hugging' against a wall
	[HideInInspector] public CameraFollower cam;                //a reference to the Main Camera
	private Vector3 gravityDirection;							//the direction of the gravity acting on the character
	private float gravityFactor;								//the scale factor of the gravity
	private bool onIncline;										//if the character is on walkable, sloped ground
	[HideInInspector] public bool slidingDownSlope;				//if the character is sliding down a slope
	private bool startedSliding;								//is set just after the character is 'slidingDownSlope' and has made some adjustments
	private Vector3 slidingDir;                                 //the direction that the character is currently sliding in
    private bool slideDelay;                                    //is set true a short while after the character walks onto a slope
    private bool backOnSlope;									//is set to false if the character has left a slope collider for long enough
	private bool startedDelay;									//if the delay for backOnSlope has started
	[HideInInspector] public GameObject relative;				//a transform to emulate the camera's movements without its z rotation
	[HideInInspector] public bool delayed;						//is a delay set just after jumping so that the character doesn't land right after starting a jump
	private bool prolongJumpDelay;								//is the player holding down the jump button after jumping?
	private Vector3 midPointLedge;								//the point of contact made when the character can climb/grab on to a ledge
	[HideInInspector] public bool onLedge;						//if the character is climbing a ledge
	private bool onLedgeAdjusted;								//if the characters position has been adjusted so that they can climb the ledge
	private float distCalc;										//the distance from the top of the ray that checks for a climbable ledge down to the ledge
	private bool lowJump;										//if the character is climbing (jumping over) a small ledge
	private bool midLedgeGrab;									//if the character is climbing a ledge of 'medium' height
	private bool highLedgeGrab;									//if the character is climbing a ledge of 'high' height
	private bool inCoRout;										//if a co-routine is taking place while the character is climbing a ledge
	private bool jumpToGrab;									//if the character is jumping up to grab onto a ledge
	private bool adjustingCam;									//if the camera is inbetween the wall and the character when initially grabbing a ledge
	private bool grabbedSoLetGo;								//if the character has successfully grabbed onto a ledge and the player has let go of the control stick
	[HideInInspector] public bool grabbing;						//if the character is in the state of grabbing (holding onto) a ledge
	private bool grabAdjusted;									//if the characters position has been adjusted so that they can grab the ledge
	private bool grabCanMoveRight;								//if the character can move along the ledge to the right
	private bool grabCanMoveLeft;								//if the character can move along the ledge to the left
	private bool grabBeneath;                                   //if the character is in the state of grabbing the ledge beneath them
	private bool grabBeneathAdjusting;							//adjusting the character when grabbing the ledge beneath them
	private bool pushAway;										//if the character is being pushed away from a ledge while falling as to not get caught on a corner
	private bool pullUp;										//if the character is in the state of pulling themselves up a ledge
	private bool grabWhileFalling;								//if the character is attempting to grab onto a ledge while falling
	private bool fallOff;										//if the character is going to fall off from grabbing a ledge
	private bool fallDelay;										//is a delay just after 'fallOff' so that the character will not grab the same ledge again
	private bool againstWall;									//if the character is able to grab the ledge they are pressing against
	private float holdTimer;									//the timer to check how long the character has been pressing against a wall for without stopping
	[HideInInspector] public bool isRolling;					//if the character is in the state of rolling
	private bool rollKnockBack;									//if the character is in the state of being knocked back after rolling into a wall
	private bool isRollKnockedBack;								//is a delay just after being knocked back while rolling
	private bool isTargetJumping;								//if the character is in the state of jumping during targetting mode
	private bool targJumpDelay;									//is a delay just after jumping during targetting mode
	private bool rightJump;										//if the character is in the state of jumping to the right during targetting
	private bool leftJump;										//if the character is in the state of jumping to the left during targetting
	private bool backJump;										//if the character is in the state of jumping backwards during targetting
	public bool inWater;						//if the character is in a body of water in any way
	public bool onWaterSurface;				//if the character is floating on the surface of the water
	[HideInInspector] public bool underWater;					//if the character is fully submerged in the water

	[Header("Swimming Settings")]
	public bool canSwim;										//if the character has the ability to swim
	[HideInInspector] public bool isDiving;						//if the character is in the state of diving
	[HideInInspector] public bool isSwimming;					//if the character is in the state of swimming
	private bool swimImpulse;									//if the character is being pushed to start swimming
	[HideInInspector] public bool slowSwim;						//if the character is swimming slowly
	[HideInInspector] public bool swim;							//if the character is swimming quickly
	[HideInInspector] public bool swimDelay;					//a delay used for when the character starts swimming
	private bool enteredSurface;								//has the character entered the surface of a body of water
	private bool diveDelay;										//a delay used for when the character starts diving
	private bool rotateOnX;										//if the character is to be rotating on their x-axis
	private bool rotateOnY;										//if the character is to be rotating on their y-axis
	private Vector3 swimXRot;									//the vector on which the character rotates around while swimming
	private float xInvrt;										//for inverted horizontal axis
	private float yInvrt;										//for inverted vertical axis
	private List<GameObject> waterBoxes;						//list of water boxes currently affecting the character (usually just one)
    private GameObject currentWaterBox;							//one of the water boxes that the character is in that is prioritised above the rest

	[HideInInspector] public bool climbing;						//if the character is climbing anything
	private bool againstClimbable;								//if the character is against a climbable object
	[HideInInspector] public bool onLadder;						//if the character is on a ladder
	[HideInInspector] public bool onFence;						//if the character is on a fance
	private Vector3 climbPos;									//the position that the character moves to when begining to climbg
	private bool climbPosAdjusted;								//if the character is moving to climbPos
	private bool climbUp;										//if the character is climbing upwards
	private bool climbDown;										//if the character is climbing downwards
	private bool canClimbUp;									//if the character can climb up
	private bool climbLeft;										//if the character is climbing to the left
	private bool climbRight;									//if the character is climbing to the right
	private bool inLeftAnim;									//if the character is in either of their 'climbing left' animations
	private bool inRightAnim;									//if the character is in either of their 'climbing right' animations
	private bool inUpAnim;										//if the character is in either of their 'climbing up' animations
	private bool inDownAnim;									//if the character is in either of their 'climbing down' animations
	private float climbTimer;									//a timer that counts how long the character has been against a climbable object for
	private Collider currentClimbingObject;						//a referrence to the current object being climbed
	private bool canFall;										//if the character can begin to fall when not onGround
	public float divingForce;									//the 'speed' of the characters diving
	public float swimmingForce;									//the 'speed' of the characters fast swimming
	public float swimSpeed;										//the 'speed' of the characters slow swimming
	public bool invertYSwim;									//if the vertical controls should be inverted while swimming
	public bool invertXSwim;									//if the horizontal controls should be inverted while swimming
	public float swimmingRotateSpeed;							//how fast the character rotates in any direction whilst underwater


	[HideInInspector] public bool againstBox;					//if the character is against a box
	private BoxCollider currentBox;								//the collider of the box that the character is currently against
	private bool isInPush;										//if the character is currently in the state of grabbing a box (while holding down the action button)
	private bool isPushing;										//if the character is pushing or pulling a box
	private bool startedPush;									//a bool to set some values as the character begins pushing/pulling a box
	private bool pushDelay;										//a bool to delay the next push/pull of a box
	private Rigidbody boxRB;									//the rigidbody component of the box currently being pushed/pulled
	private Vector3 boxPos;										//the position that the character takes upon pushing/pulling a box
	private Vector3 origBoxPos;									//the start position of the box when it is about to be moved
	private float vecBoxDist;									//distance between the box and the point of contact with the character
	private float boxDist;										//distance between the box and its destination when being moved
	private bool boxPushAdjust;									//a bool to adjust some values once the character has started pushing the box
	private float boxTravelDist;								//distance that the box has travelled
	private float boxABDist;									//distance of the entire journey that the box will make when being moved
	private float pushYDir;										//float used to calculate which way the character is facing, relative to the camera
	private float pushXDir;										//float used to calculate which way the character is facing, relative to the camera
	private float pushSignDir;									//if the character is pushing (+1) or pulling (-1) the box
	private bool cannotBePushed;								//if the box cannot be pushed as it won't make the entire journey due to obstructions

	private bool againstLiftable;								
	[HideInInspector] public GameObject currentLiftable;		//The current object that the character is lifting
	[HideInInspector] public GameObject origLiftableParent;		//The currentLiftable 's original parent object;  to be returned to it when the character has let go of/thrown it
	private Vector3 origLiftLocPos;
	private bool liftDelay;
	private bool liftCorrectDelay;
	private bool liftThrowDelay;
	private bool puttingDown;
	[HideInInspector] public bool isLifting;
	private bool liftExtra;
	[HideInInspector] public bool liftOveride;
	[HideInInspector] public bool liftAction;

	[Header("Box Movement Settings")]
	[Range(0.1f,10f)][SerializeField] private float boxDistToBeMoved;	//how far does the user want the boxes to move
	[Range(0.1f,10f)][SerializeField] private float boxMoveSpeed;		//how fast does the user wan the boxes to move

	[HideInInspector] public bool masterBool;							//a bool that is false when certain other bools are all false; these are bools that are commonly checked for all being false

	private bool clickedTarget;											//a bool for when the player clicks on the target button
	private bool holdingTarget;											//a bool for when the player is holding on the target button

	private GameObject[] targets = new GameObject [128];														//an array for how many targetable objects exist in the scene (EXTEND THIS IF NEEDED)
	private SortedDictionary<float, GameObject> distDic = new SortedDictionary<float, GameObject> ();			//a dictionary of the distances between the character and the targets
	[HideInInspector] public GameObject currentTarget;															//a reference to the current target that is being focused on
	[HideInInspector] public int focusNumber;																	//an iterator which labels the targets that are withing focusing range
	[HideInInspector] public bool isFocusing;																	//if the character is in target mode and is focusing on a target
	[HideInInspector] public bool trigDelay;																	//a delay set so that transitions can be made when setting a new target

	[Header("Collision Layer Masks")]
	[SerializeField]
	private LayerMask whatIsGround;			//the layermask that holds the collision layers that count as walkable ground
	[SerializeField]
	private LayerMask whatIsSlope;			//the layermask that holds the collision layers that count as unwalkable slopes
	[SerializeField]
	private LayerMask whatIsACollision;		//the layermask that holds the collision layers that count as any type of ground (walkable or not)
	[SerializeField]
	private LayerMask whatIsLiftable;		//the layermask that holds the collision layers that count as objects that can be lifted up
    [SerializeField]
	private LayerMask whatIsWater;			//the layermask that holds the collision layers that count as water
    
	[Header("Miscelaneous")]																				//if the targetting mode should work by rotating through the targets or by focusing on one target as long as the target button is being held down
	[SerializeField] private GameObject targ;				//the yellow crosshair that appears on a target when it's being focused on
	GameObject targy;						//the instantiation of the prefab mentioned above
	[SerializeField]
	private GameObject mouseIconGrab;		//the prompt that appears above the character so that the player knows that the box can be grabbed
	GameObject mIconG;

	[HideInInspector] public Animator anim;						//a reference to the characters Animator (set in the child object that is the characters model)
		private Transform rayA;										//a ray that checks for ground below the character
	private Transform rayB;										//a ray that checks for ground below-behind the character (used for the jumping mechanic)
	private RaycastHit rayHitB;									//the hit that is output by rayB
	private Transform rayC;										//a ray that checks for ground in front of the character (used to judge how high the ledge the character is on is from the ground and for the 'pushBack' bool)
	private RaycastHit rayHitC2;								//anothher hit that can be output by RayC
	private Transform rayD;										//a ray that checks above-in front the character for ledges to climb or grab
	private RaycastHit rayHitD;									//the hit that is output by rayD
	private Transform rayE;										//a ray that checks to the sides of/above the character when grabbing a ledge
	private RaycastHit rayHitE1;								//the hit that is output by rayE when checking to one side
	private RaycastHit rayHitE2;								//the hit that is output by rayE when checking to the other side
	private RaycastHit rayHitForward;							//the hit that is output when adjusting the characters rotation when on a ledge or grabbing
	private RaycastHit rayDump;									//the hit that is used at certain times just to check a normal
	private RaycastHit rayDump2;								//the hit that is used at certain times just to check a normal
	private RaycastHit rayDump3;								//the hit that is used at certain times just to check a normal
	private RaycastHit rayCollide1;								//the hit that is when entering a collision
	private RaycastHit rayCollide2;								//the hit that is when staying in a collision
	private RaycastHit rayCollide3;								//the hit that is when exiting a collision
	private RaycastHit rayLift;
    private RaycastHit rayWater;
    private Vector3 rayNormal;									//the normal of the ground that the character is on
	private bool firstPass;										//a bool that turns true after the first pass of the Update() function to check for errors

	public bool isPaused;						//is the game currently paused

	[HideInInspector] public bool isCrouching;					//is the character crouching?
	[HideInInspector] public bool crouchDelay;					//is the character going into or out-of a crouch?
	[HideInInspector] public bool isUnderCollision;				//is the character under a collision while crouching
	private bool putIntoFPV;

	private GameObject slideRelative;							//the gameObject that is instantiated to keep track of the sliding direction of a slope
	private GameObject dRelative;								//the gameObject that is instantiated to keep track of the direction that the player is moving in
	private float yScale;										//the scale of the capsule colliders height relative to the default height (2f)

    [Header("Custom")]

    public fx_buoyancy buoyancyObject;
    public InteractionController actionText;
    public AudioClip actionSound;
    public bool actionStarted;

	public void Start ()
	{
		anim = gameObject.GetComponentInChildren<Animator> ();
		gPI = GameObject.FindWithTag ("Player").GetComponent<GamePadInputs> ();						//getting the GamePadInputs.cs script from the Handler object in the scene
		uItems = GameObject.FindWithTag ("Player").GetComponent<UsingItems1> ();
		currentAnim = GameObject.FindWithTag ("Player").GetComponent<Animations> ();
		capCol = this.GetComponent<CapsuleCollider> ();													//getting the capsule collider
		sphereCol = this.GetComponent<SphereCollider> ();												//getting the sphere collider (what the character uses when crouching)
		cHeight = capCol.bounds.size.y;																	//the height of the capsule collider
		cRad = capCol.bounds.size.x/2f;																	//the radius of the capsule collider
		sphereCol.center = new Vector3 (0f, -(cHeight / 2f) + cRad);									//setting the position of the sphere collider
		sphereCol.radius = cRad;																		//setting the radius of the sphere collider
		yScale = 2f/cHeight;	//setting the scale of the capsule colliders height (relative to 2f)
        buoyancyObject = GetComponentInChildren<fx_buoyancy>();
        actionText = GameObject.Find("Interaction Controller(Clone)").GetComponent<InteractionController>();

		//Creating required child objects
		GameObject followMe = new GameObject ();
		followMe.name = "FollowMe";
		followMe.transform.parent = this.transform;
		followMe.transform.localPosition = new Vector3 (0f, ((cHeight/2f)*(7f/8f)), 0f);
		GameObject altFollowMe = new GameObject ();
		altFollowMe.name = "AltFollowMe";
		altFollowMe.transform.parent = this.transform;
		GameObject fpvFollowMe = new GameObject ();
		fpvFollowMe.name = "LdgFollowMe";
		fpvFollowMe.transform.parent = this.transform;
		fpvFollowMe.transform.localPosition = new Vector3 (0f, ((cHeight/2f)*(7f/8f)), 0f);
		fpvFollowMe.transform.parent = rootBoneGameObject.transform;
		GameObject rayAgo = new GameObject ();
		rayAgo.name = "RayA";
		rayAgo.transform.parent = this.transform;
		rayAgo.transform.localPosition = Vector3.zero;
		GameObject rayBgo = new GameObject ();
		rayBgo.name = "RayB";
		rayBgo.transform.parent = this.transform;
		rayBgo.transform.localPosition = new Vector3 (0f, -(cHeight/4f), -(0.515625f*cRad));
		GameObject rayCgo = new GameObject ();
		rayCgo.name = "RayC";
		rayCgo.transform.parent = this.transform;
		rayCgo.transform.localPosition = new Vector3 (0f, -(cHeight/4f), (1.03125f*cRad));
		GameObject rayDgo = new GameObject ();
		rayDgo.name = "RayD";
		rayDgo.transform.parent = this.transform;
		rayDgo.transform.localPosition = new Vector3 (0f, (cHeight*1.255f), (1.375f*cRad));
		GameObject rayEgo = new GameObject ();
		rayEgo.name = "RayE";
		rayEgo.transform.parent = this.transform;
		rayEgo.transform.localPosition = new Vector3 (0f, (cHeight*1.255f), (0.6015625f*cRad));

		rb = this.GetComponent<Rigidbody> ();																			//referring to the characters RigidBody component
		if (Camera.main != null) {
			cam = Camera.main.GetComponent<CameraFollower> ();															//finding the Main Camera for 'cam'
		}
		altLookAt = altFollowMe.transform;																				//finding the alternative 'AltFollowMe' transform
		relative = new GameObject ();																					//creating the 'relative' transform
		relative.name = "Relative";
		if (cam != null) {
			relative.transform.position = cam.transform.position;														//setting 'relative's position to the cameras position
			relative.transform.rotation = new Quaternion (0f, cam.transform.rotation.y, 0f, cam.transform.rotation.w);	//setting 'relative's rotation to be the cameras, minus its z and x rotation
		}
		slideRelative = new GameObject ();
		slideRelative.name = "SlideRelative";
		dRelative = new GameObject ();
		dRelative.name = "dRelative";

		if (this.GetComponentInChildren<Animator> () != null)
			anim = this.GetComponentInChildren<Animator> ();															//getting the Animator of the child object that is the characters model

		rb.centerOfMass = new Vector3 (0f, (-cHeight / 2f) + cRad, 0f);													//setting the rigidbodys center of mass to be the center of the 'ball' shape at the bottom of the capsule collider
		originalMoveSpeed = maxSpeed;
		skinWidth = cRad;
		targy = Instantiate (targ, new Vector3(0f,1000f,0f), this.transform.rotation) as GameObject;					//instantiating the target crosshair prefab
		mIconG = Instantiate (mouseIconGrab, new Vector3 (0f,1000f,0f), this.transform.rotation) as GameObject;			//instantiating the box-grabbing prompt prefab
		modelTransform = characterModelObject.transform;

		rayA = rayAgo.transform;																						//finding the transforms of the rays that are children to the character
		rayB = rayBgo.transform;
		rayHitB = new RaycastHit ();
		rayC = rayCgo.transform;
		rayHitC2 = new RaycastHit ();
		rayD = rayDgo.transform;
		rayHitD = new RaycastHit ();
		rayE = rayEgo.transform;
		rayHitE1 = new RaycastHit ();
		rayHitE2 = new RaycastHit ();
		rayHitForward = new RaycastHit ();
		rayDump = new RaycastHit ();
		rayDump2 = new RaycastHit ();
		rayDump3 = new RaycastHit ();
		rayLift = new RaycastHit();
        rayWater = new RaycastHit();

        //Setting initial values of certain variables
        delayed = true;
		trigDelay = true;
		canFall = true;
		jumpNo = 0;
		canClimbUp = true;
		slidingDir = Vector3.zero;
        slideDelay = true;
        currentNPCName = "";
		canFPV = true;
		zeroVelocity = Vector3.zero;
        waterBoxes = new List<GameObject>();

		Physics.IgnoreLayerCollision (characterCollisionLayer, projectilesCollisionLayer, true);

        if (useBlobShadow && blobShadow != null) {
            blobShadowFollow = new GameObject();
            blobShadowFollow.gameObject.name = "blobShadowFollow";
            blobShadowFollow.transform.position = rootBoneGameObject.transform.position;
            blobShadow.transform.SetParent(blobShadowFollow.transform);
        }
        if (!useBlobShadow && blobShadow != null)
            blobShadow.gameObject.SetActive(false);
	}

	void Update (){
		useBlobShadow = false;
		gPI = GameObject.FindWithTag ("Player").GetComponent<GamePadInputs> ();						//getting the GamePadInputs.cs script from the Handler object in the scene
		uItems = GameObject.FindWithTag ("Player").GetComponent<UsingItems1> ();
		currentAnim = GameObject.FindWithTag ("Player").GetComponent<Animations> ();
		cam = Camera.main.GetComponent<CameraFollower>();
//////////Checking for errors
		if (!firstPass) {
			bool hasError = false;
			if (targ == null) {
				Debug.LogError ("ThirdPersonController.cs: You haven't assigned 'targ' to a game object. Please drag in 'Ridicule' from the prefabs folder into the 'Targ' field in the inspector for the character.");
				hasError = true;
			}
			if (mIconG == null) {
				Debug.LogError ("ThirdPersonController.cs: You haven't assigned 'mouseIconGrab' to a game object. Please drag in 'MouseIconGrab' from the prefabs folder into the 'Mouse Icon Grab' field in the inspector for the character.");
				hasError = true;
			}
			if (Camera.main == null) {
				Debug.LogError ("ThirdPersonController.cs: There doesn't seem to be a Main Camera in the scene. Please use the 'Main Camera' prefab from the 'prefabs' folder.");
				hasError = true;
			}
			if (cam == null) {
				Debug.LogError ("ThirdPersonController.cs: The CameraFollower.cs script cannot be found on the scenes Main Camera object. Please attach the CameraFollower.cs script from the 'Scripts' folder to the Main Camera object in the scene.");
				hasError = true;
			}
			if (this.GetComponentInChildren<Animator> () == null) {
				Debug.LogError ("ThirdPersonController.cs: You are missing an animator component for your character. Please locate the [Model name] child object and add an animator component. Set the Controller to be the 'PlayerAnimController' file in the 'animations' folder. Set culling mode to 'Always Animate'.");
				hasError = true;
			}
			if (this.GetComponentInChildren<Animator> ().runtimeAnimatorController == null) {
				Debug.LogError ("ThirdPersonController.cs: The animator component is missing an animator controller. Set the Controller to be the 'PlayerAnimController' file in the 'animations' folder. Set culling mode to 'Always Animate'.");
				hasError = true;
			}
			if (hasError) {
				#if UNITY_EDITOR
				UnityEditor.EditorApplication.isPlaying = false;
				Debug.DebugBreak ();
				Application.Quit ();
				Debug.Break ();
                #endif
            } else
				firstPass = true;
		} else {
            if (useBlobShadow)
                blobShadowFollow.transform.position = rootBoneGameObject.transform.position;
			if (!isPaused) {
				if (!grabbing && !onLedge && !isRollKnockedBack && !targJumpDelay && !startedSliding								//setting masterbool
					&& !climbing && !grabBeneathAdjusting && !isInPush && !currentAnim.inATargJumpAnim && !uItems.isUnequipping && !currentAnim.inReachItem
				    && !currentAnim.inRollKnockBack && !onLedge && !inACutscene && !crouchDelay && !liftDelay && !liftThrowDelay && !onSteepGround) {
					if (masterBool)
						masterBool = false;
				} else {
					if (!masterBool)
						masterBool = true;
				}

//////////Input Code
				if (gPI.LT > 0.1f && clickedTarget) {
					clickedTarget = false;
					hasUnClicked = true;
				}
				if (gPI.LT > 0.1f && !clickedTarget && !hasUnClicked)
					clickedTarget = true;
				if (gPI.LT < 0.1f && clickedTarget)
					clickedTarget = false;
				if (gPI.LT < 0.1f && !clickedTarget) {
					if (hasUnClicked)
						hasUnClicked = false;
				}
				if (gPI.LT > 0.1f)
					holdingTarget = true;
				else 
					holdingTarget = false;

//////////Direction Code
				relative.transform.position = Vector3.zero;;								//positions the 'relative' transform
				leftX = gPI.LH;												//sets the leftX and leftY floats to the Input Axes
				leftY = gPI.LV;
				if (leftX < 0.1f && leftX > -0.1f || swimDelay)													//sets dead-zones
					leftX = 0f;
				if (leftY < 0.1f && leftY > -0.1f || swimDelay)
					leftY = 0f;
                direction = new Vector3(leftX, 0f, leftY);
                if (Vector3.Magnitude(direction) > 0.8f)
					direction = Vector3.Normalize(new Vector3 (leftX, 0f, leftY));											//sets the direction vector
				
				leftX = direction.x;
				leftY = direction.z;
				if (grabBeneath)																	//sets direction to zero when grabbing beneath
					direction = Vector3.zero;

//////////Animation Control
				locoSpeed = Vector3.Magnitude (new Vector3(leftX, 0f, leftY));
				if (locoSpeed < 0.1f || masterBool || Vector3.Magnitude(new Vector3(rb.velocity.x, 0f, rb.velocity.z)) < 0.1f)																																						//sets a deadzone for 'locoSpeed'
					locoSpeed = 0f;
                if (againstWall)
                    locoSpeed *= 0.5f;
				anim.SetFloat ("Speed", locoSpeed);
                if(gPI.holdLStick && sprintFunction)
                    anim.SetFloat("Speed", locoSpeed * 2f);
                anim.SetFloat ("turnAmount", Mathf.Lerp(anim.GetFloat("turnAmount"),
				                                        1.5f * -Vector3.Cross ((relative.transform.forward * direction.z) + (relative.transform.right * direction.x), this.transform.forward).y,
				                                        Time.deltaTime * 15f));		//sets the float in the animator to determine how much the character is 'leaning' to the left/right when moving in any direction
				
				if (!targetting) {																																							//animation control for basic ground movement when the character is NOT targetting
					if (anim.GetBool ("inTrigLoco") == true)
						anim.SetBool ("inTrigLoco", false);
					if (anim.GetFloat ("Speed") > 0f  && !onLedge && !currentAnim.inRoll && !masterBool && !jumping){
						if(onGround || !onGround && slidingDownSlope && !startedSliding)
							anim.SetBool ("inLocomotion", true);
					}else {
						if (huggingWall && onGround && direction != Vector3.zero) {

						} else
							anim.SetBool ("inLocomotion", false);
					}
				} else {																																										//animation control for basic ground movement when the character IS targetting
					if (anim.GetBool ("inLocomotion") == true)
						anim.SetBool ("inLocomotion", false);
					if (!jumping && !grabbing && !onLedge && !isRollKnockedBack && !startedSliding) {
						anim.SetBool ("inTrigLoco", true);		
					} else {
						if (huggingWall && onGround && direction != Vector3.zero) {
					
						} else
							anim.SetBool ("inTrigLoco", false);
					}
				}
				if (anim.GetBool ("inTrigLoco")) {
					anim.SetFloat ("trigX", leftX);
					anim.SetFloat ("trigY", leftY);
				}

				if (boxPushing && !oldBoxPushing) {
                    if (againstBox && onGround) {
                        if (Vector3.Magnitude(direction) != 0f && !gPI.holdLStick) {
                            if (!isPushing && currentAnim.locomotionBlend) {
                                isPushing = true;
                                anim.SetBool("newboxPush", true);
                            }
                        }else {
                            if (isPushing)
                                ExitPush();
                        }
                    }else {
                        if (isPushing)
                            ExitPush();
					}
                }

				if (rb.velocity.y < 0f && !onGround && currentAnim.grabbingBlend && !climbing)					//sets the character to play the 'fall' animation if they're not on ground, falling down and aren't grabbing
					anim.SetTrigger ("falling");

                if (onGround && !hasLandedThisFrame && currentAnim.inFall && delayed)
                    anim.SetTrigger("land");

				if (onWaterSurface && !currentAnim.swimSurfaceBlend && !currentAnim.inSwimFlip && !isSwimming && !isDiving && !enteredSurface && rb.velocity.y < 0.1f) {
					enteredSurface = true;
					StartCoroutine(Delay(11));
					if (anim.GetBool ("diving"))
						anim.SetBool ("diving", false);
					jumpNo = 0;
                    anim.ResetTrigger("land");
					anim.Play("breathe", 0);
				}

				if (inWater && currentAnim.inFall && rb.velocity.y > 0f && !enteredSurface && !isSwimming)
					anim.SetBool ("diving", true);

				anim.SetBool ("swimming", isSwimming);

				if(anim.GetBool ("isClimbing"))
					anim.SetBool ("isClimbing", false);
				if(onGround && !onWaterSurface && !underWater && currentAnim.swimSurfaceBlend || onGround && delayed && currentAnim.inFall ||
				   onGround && currentAnim.inClimbStill1 && !climbing|| onGround && currentAnim.inClimbStill2 && !climbing){
					if(currentAnim.inClimbStill1 || currentAnim.inClimbStill2)
						anim.SetBool("isClimbing", true);
					if (!hasLandedThisFrame) {
						Land ();
					}
				}

                if (onGround && jumpNo != 0 && delayed && !canDoNextJump)
                    jumpNo = 0;

                if (isSwimming){
                    if (slowSwim)
                        anim.SetFloat("swimSpeed", Mathf.Lerp(anim.GetFloat("swimSpeed"), 1f, Time.smoothDeltaTime * 5f));
                    else
                        anim.SetFloat("swimSpeed", Mathf.Lerp(anim.GetFloat("swimSpeed"), 0f, Time.smoothDeltaTime * 5f));
                }

////////Jumping + Falling Control + Ledge Approaching
                if (!onWaterSurface && !underWater) {
					if (Physics.Linecast (rayA.position, rayA.position - (Vector3.up * (cHeight*0.8f)), whatIsGround)) {						
						if (pushAway)																												//if the character is above/on ground it cannot be pushed back
							pushAway = false;
						if (delayed && Physics.Linecast (rayB.position, rayB.position - (Vector3.up * (cHeight*(3f/8f))), out rayHitB, whatIsGround) && !onGround) {
							delayed = true;																											//if the character is above/on ground and has delayed after jumping, it cannot be jumping
							jumping = false;
							startedJump = false;
						}
					} else {
						if (onIncline)
							onIncline = false;
							if (Physics.Linecast (rayB.position, rayB.position - (Vector3.up * (cHeight*(3f/8f))), out rayHitB, whatIsGround) && rayHitB.normal.y >= walkableSlopeTolerence || //jumping off a ledge
							    Physics.Linecast (rayB.position, rayB.position - (Vector3.up * (cHeight*(3f/8f))) - (this.transform.forward * (cRad*0.9375f)), out rayHitB, whatIsGround) && rayHitB.normal.y >= walkableSlopeTolerence) {
							if(autoLedgeJumping || ledgeGrabbing || fenceClimbing || ladderClimbing){
								if (!Physics.Linecast (rayC.position + (Vector3.up * (cHeight * 0.5f)), rayC.position - (Vector3.up * (cHeight * 0.65f)), whatIsGround)) {
									rayHitB.point = Vector3.zero;
									if (!jumping && delayed && !grabBeneath && !targetting && !climbing
										&& Vector3.Angle (rb.velocity, this.transform.forward) < 80f) {
										if (Vector3.Magnitude (new Vector3(gPI.LH,0f,gPI.LV)) > 0.8f || isRolling) {																			//will jump if going fast enough
											if (isRolling)
												isRolling = false;
											if (Physics.Raycast (this.transform.position - (Vector3.up * (cHeight * 0.6f)) + (this.transform.forward * cRad), -this.transform.forward, out rayHitB, cRad * 4f,
										                   whatIsGround)) {
												if (rayHitB.collider.tag == "Ladder" && ladderClimbing || rayHitB.collider.tag == "Fence" && fenceClimbing) {								//If there is a ladder/fence at the ledge, the character will grab beneath to begin climbing
													if(!Physics.Raycast(rayC.position, -Vector3.up, cHeight*1.1f, whatIsACollision)){
														grabBeneath = true;
														grabBeneathAdjusting = true;
													}
												} else {
													if(jumpNo < amountOfJumps && !hardLanded && autoLedgeJumping){
														jumpNo = 0;
                                                        Jump("autoLedge");
													}
												}
											} else {
												if(jumpNo < amountOfJumps && !hardLanded && autoLedgeJumping){
													jumpNo = 0;
                                                    Jump("autoLedge");
												}
											}
										} else {
											if (Physics.Linecast (this.transform.position + (this.transform.forward * (cRad*2.34375f)),
											                      this.transform.position + (this.transform.forward * (cRad*2.34375f)) - (Vector3.up * (cHeight * 1.5f)),
									                    out rayHitForward, whatIsGround)) {
											} else {
												if (ledgeClimbing && Physics.Linecast (rayC.position - (Vector3.up * (cHeight*0.255f)), rayC.position - (this.transform.forward*3.125f*cRad) - (Vector3.up * (cHeight*0.255f)),
										                    out rayHitForward, whatIsACollision) && !currentAnim.inRoll && Vector3.Magnitude(direction) > 0f &&
												    !Physics.Raycast(rayC.position, -Vector3.up, cHeight*1.1f, whatIsACollision)) {
													grabBeneath = true;
													grabBeneathAdjusting = true;
												}
											}
										}
									}
								}
							}
						} else {
							if (rb.velocity.y < -0.05f && !currentAnim.inFall && !currentAnim.inALedgeClimbAnim && !targJumpDelay && !jumping && !grabWhileFalling && 
								!slidingDownSlope && !currentAnim.grabbingBlend && !currentAnim.inJumpToGrab && !currentAnim.trigLocoBlend && !currentAnim.inRunningLand && !currentAnim.inHardLand) {
								anim.SetTrigger ("falling");																												//general falling animation control
							} else {
								if (fallOff) {
									anim.SetTrigger ("falling");
								} else
									anim.ResetTrigger ("falling");
							}
							if (!targetting && !fPVing && !slidingDownSlope)
								direction = this.transform.forward;
						}
						if (Physics.Linecast (rayC.position, rayC.position - (Vector3.up * (cHeight/4f)), whatIsGround) && !onGround && !jumping && !isRolling && !isTargetJumping && !onLedge
							    && !Physics.Linecast (rayC.position, rayC.position - (Vector3.up * (cHeight/4f)), whatIsSlope) && rb.velocity.y < -3f) {
							pushAway = true;																															//this prevents the collider from getting 'caught' on corners when falling
						} else
							pushAway = false;
					}

					//Jump on button press
					if (gPI.pressLeftB && jumpFunction) {
						if (delayed && !grabBeneath && !isTargetJumping && !onLedge && !grabbing && !climbing && !isInPush && !inACutscene || canSidewaysJump) {
							if(jumpNo < amountOfJumps || canDoNextJump && onGround){
								if (isRolling)
									isRolling = false;
                                if (currentAnim.inBrake && Vector3.Magnitude(direction) >= 0.85f && sidewaysJump && !isLifting){
                                    braking = false;
                                    rotIterate = 0;
                                    Jump("sideways");
                                }else {
                                    if (!isCrouching && !crouchDelay && onButtonJumping && !currentAnim.inRollKnockBack && !currentAnim.inReachItem){
                                        if (isLifting && jumpNo == 0 || !isLifting)
                                        {
                                            if (currentAnim.inRoll && jumpCancelRoll || isRolling && jumpCancelRoll || !currentAnim.inRoll)
                                                Jump("default");
                                        }
                                    }
                                    if (isCrouching && !crouchDelay && !isUnderCollision && onButtonJumping){
                                        Jump("crouchJump");
                                        capCol.isTrigger = false;
                                        sphereCol.isTrigger = true;
                                        isCrouching = false;
                                        crouchDelay = false;
                                    }
                                }
							}
						}
						if(delayed){
							if(grabbing && canJumpFromGrab && !currentAnim.inALedgeClimbAnim && !onLedge && !rb.isKinematic && !pullUp){
								grabbing = false;
                                anim.ResetTrigger("grabbing");
								Jump("default");
							}
							if(climbing && canJumpFromClimb){
								climbing = false;
                                climbPosAdjusted = false;
                                anim.SetBool ("isClimbing", true);
								anim.SetInteger ("climbX", 0);
								anim.SetInteger ("climbY", 0);
								anim.ResetTrigger ("falling");
								Jump("default");
							}
						}
					}
					if(grabBeneath && grabBeneathAdjusting){
						Physics.Raycast(this.transform.position - (Vector3.up*(cHeight*0.51f)) + (this.transform.forward*(cRad*2f)),
						                -this.transform.forward, out rayDump3, cRad*4f, whatIsGround);
						if(rayDump3.collider != null){
							this.transform.forward = Vector3.Lerp(this.transform.forward, rayDump3.normal, Time.deltaTime * 5f);
							if(Vector3.Distance(this.transform.position + this.transform.forward, this.transform.position + rayDump3.normal) < 0.1f){
								this.transform.forward = rayDump3.normal;
								Physics.IgnoreLayerCollision (characterCollisionLayer, groundCollisionLayer, true);
								Physics.IgnoreLayerCollision (characterCollisionLayer, slopesCollisionLayer, true);
								grabBeneathAdjusting = false;
							}else{
								if(Physics.Linecast(this.transform.position, this.transform.position - (Vector3.up*(cHeight*0.51f)), whatIsGround))
									this.transform.position = Vector3.Lerp(this.transform.position,
									                                       this.transform.position + (this.transform.forward*(cRad*0.05f)), Time.deltaTime * 5f);
							}
						}else{
							Physics.IgnoreLayerCollision (characterCollisionLayer, groundCollisionLayer, true);
							Physics.IgnoreLayerCollision (characterCollisionLayer, slopesCollisionLayer, true);
							grabBeneathAdjusting = false;
						}
					}
				}
				if(!onGround && delayed && gPI.pressAction && !stoppedMidAir && !inWater){
					rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
					stoppedMidAir = true;
				}

//////////Swimming/Diving
                if (inWater) {
                    if(!underWater && !onWaterSurface && !isSwimming && !isDiving){
                        if((rb.velocity.y < -2f && Physics.Raycast(this.transform.position + (Vector3.up * cHeight * 5f), -Vector3.up, out rayWater, cHeight * 4.99f, whatIsWater)) ||
                            rb.velocity.y >= -2f && Physics.Raycast(this.transform.position + (Vector3.up * cHeight * 0.5f), -Vector3.up, out rayWater, cHeight * 0.499f, whatIsWater)) {
                            if (rayWater.collider.isTrigger && rayWater.collider.gameObject.layer == waterCollisionLayer && rayWater.collider.gameObject == currentWaterBox) {
                                underWater = true;
                            }
                        }
                    }
                    if(!onWaterSurface && underWater){
                        if(rb.velocity.y > 0f && Physics.Raycast(this.transform.position, -Vector3.up, out rayWater, cHeight) ) {
							if (rayWater.collider.isTrigger && rayWater.collider.gameObject.layer == waterCollisionLayer && rayWater.collider.gameObject == currentWaterBox) {
                                onWaterSurface = true;
                                underWater = false;
                                isSwimming = false;
                                isDiving = false;
                                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                            }
                        }
                    }
                    if (onWaterSurface && (this.transform.localRotation.x != 0f || this.transform.localRotation.z != 0f) && !isSwimming && !isDiving)
                        this.transform.localRotation = Quaternion.Euler(0f, this.transform.localRotation.y, 0f);
					if (gPI.pressLeftB && jumpFunction) {					//Jumping from the surface of the water
						if (onWaterSurface && delayed && !grabBeneath && !grabbing && !onLedge && !climbing && !isSwimming && !swimDelay && !isDiving && !diveDelay
						    && jumpNo < amountOfJumps && !hardLanded && onButtonJumping) {
							inWater = false;
							onWaterSurface = false;
							Jump("default");
						}
					}
					if (isRolling || grabbing || grabBeneath || jumping && delayed) {								//Canceling out certain actions when entering water
						isRolling = false;
						jumping = false;
						grabbing = false;
						grabBeneath = false;
					}
					if (canSwim) {																					//If the character can swim...
						if (onWaterSurface) {
							if (!swimDelay) {
								if (!currentAnim.inBreathe && !Physics.Raycast (this.transform.position, -Vector3.up, cHeight * 1.5f, whatIsACollision)
									&& !Physics.Raycast (this.transform.position, -Vector3.up + (this.transform.forward * cRad * 2f), cHeight * 1.4f, whatIsACollision)
									&& !Physics.Raycast (this.transform.position, -Vector3.up - (this.transform.forward * cRad * 2f), cHeight * 1.4f, whatIsACollision)) {	//First, a delay is set in place so that the character can flip around to begin swimming
									if (gPI.pressAction) {
										rotateOnX = false;
										rotateOnY = false;
										swimXRot = this.transform.right;
										isSwimming = true;
										swimDelay = true;
										anim.SetTrigger ("flip");
									}
								}
								if (!swimDelay) {
									if (swimImpulse || swim || slowSwim) {
										swim = false;
										slowSwim = false;
									}
								}
							}
							if (swimDelay) {
								float upAngle = Vector3.Angle (this.transform.up, Vector3.up);
								float turnAmount = Mathf.Abs (Time.deltaTime * 150f);
								if (upAngle + turnAmount <= 175f) {
									this.transform.RotateAround (this.transform.position, swimXRot, turnAmount);
								} else {
									swimDelay = false;
                                    onWaterSurface = false;
                                    underWater = true;
									swim = true;
									swimImpulse = true;
									StartCoroutine (Delay (7));
									anim.SetTrigger ("swimImpulse");
								}
							}
						}
						if (underWater && isSwimming) {																//Once the character has submerged and begun swimming
							if (!swimDelay) {
								if (leftX != 0f)
									rotateOnY = true;
								else
									rotateOnY = false;
								float upAngle = Vector3.Angle (this.transform.up, Vector3.up);
								float turnAmount = Time.deltaTime * 150f * yInvrt;
								if(!invertYSwim)
									yInvrt = Mathf.Lerp(yInvrt,-leftY, Time.deltaTime * 5f);
								else
									yInvrt = Mathf.Lerp(yInvrt,leftY, Time.deltaTime * 5f);
								xInvrt = Mathf.Lerp(xInvrt,leftX, Time.deltaTime * 5f);
								if (yInvrt > 0f && upAngle + turnAmount > 175f || //Controls rotating the character on their x-axis within specified limits
									yInvrt < 0f && upAngle + turnAmount < 5f) {
									rotateOnX = false;
								} else {
									if (yInvrt != 0f)
										rotateOnX = true;
									else
										rotateOnX = false;
								}
								if (invertXSwim)
									xInvrt *= -1f;
								if (gPI.holdAction|| gPI.holdLeftB) {												//Slow swimming controls
									slowSwim = true;
								}
								if (!currentAnim.inSwimImpulse && !swim) {										//Fast swimming controls
									if (gPI.pressAction) {
										swim = true;
										swimImpulse = true;
										StartCoroutine (Delay (7));
										anim.SetTrigger ("swimImpulse");
									}
								}
								if (!gPI.holdAction&& !gPI.holdLeftB || swim) {
									slowSwim = false;
								}
							} else {
								float upAngle = Vector3.Angle (this.transform.up, Vector3.up);
								if (swimDelay && upAngle > 170f)
									swimDelay = false;
							}
						}
					} else {																									//If the character cannot swim...
						if(canDiveIfNoSwim){
							if (onWaterSurface || isDiving) {
								if (gPI.pressAction) {									//Diving controls; like swimming, flips over first, then dives. When finished, will flip over again and swim upwards.
									if (!isDiving && !currentAnim.inBreathe && !Physics.Raycast (this.transform.position, -Vector3.up, cHeight * 1.5f, whatIsACollision)) {
										//Can include a timer here if you don't want diving to go on forever
										rotateOnX = false;
										rotateOnY = false;
										diveDelay = true;
										anim.SetTrigger ("flip");
										anim.SetBool ("diving", true);
                                        buoyancyObject.enabled = false;
                                    }
                                    onWaterSurface = false;
									isDiving = true;
								}
							}
							//If making a timer, use it to set a bool true when the timer is up and use it in the if-statement below
							if (!gPI.holdAction) {
								if (isDiving) {
									diveDelay = true;
									anim.SetTrigger ("flip");
									isDiving = false;
								}
							}
							float upAngle = Vector3.Angle (this.transform.up, Vector3.up);
							float turnAmount = Time.deltaTime * 150f;
							if (isDiving) {
								if (diveDelay) {
									if (upAngle + turnAmount <= 175f) {
										this.transform.RotateAround (this.transform.position, this.transform.right, turnAmount);
									} else {
										diveDelay = false;
									}
								}
							} else {
								if (diveDelay) {
									if (upAngle - turnAmount >= 5f) {
										this.transform.RotateAround (this.transform.position, this.transform.right, -turnAmount);
									}else {
										diveDelay = false;
									}
								}
							}
						}
					}
				}

//////////Climbing Action
				if(fenceClimbing || ladderClimbing){
					if (againstClimbable && !isRolling && !masterBool && huggingWall && !grabBeneath && onGround && !climbing && !onLedge && !grabWhileFalling && !isLifting) {										//times how long the player is moving the character against a ledge for
						if (Mathf.Sqrt ((leftX * leftX) + (leftY * leftY)) > 0.3f) {
							climbTimer += Time.deltaTime;
							if (maxSpeed != 2.5f)
								maxSpeed = 2.5f;
						} else {
							if (maxSpeed == 2.5f)
								maxSpeed = originalMoveSpeed;
							climbTimer = 0f;
						}	
					} else {
						if (maxSpeed != 5f)
							maxSpeed = originalMoveSpeed;
						climbTimer = 0f;
					}
					if (climbTimer > 0.15f && againstClimbable || currentAnim.inFall && !onGround && huggingWall && //Adjust attaching onto the climbable object
						againstClimbable && !climbing && Vector3.Magnitude (new Vector3 (leftX, 0f, leftY)) > 0.1f && !isLifting) {
						Transform climbingTrans = currentClimbingObject.transform;
						Physics.Raycast(this.transform.position, this.transform.forward, out rayDump2, cRad * 3f, whatIsGround);
						float forwardsAngle = Vector3.Angle (this.transform.forward, rayDump2.normal);
						float velosAngle = Vector3.Angle ((Mathf.Sign (direction.z) * direction.z * direction.z * relative.transform.forward) + 
							(Mathf.Sign (direction.x) * direction.x * direction.x * relative.transform.right),
						                                  rayDump2.normal);
						if (forwardsAngle >= 135f && velosAngle >= 135f || forwardsAngle >= 135f && !onGround && rb.velocity.y < 1f && !fallDelay) {
							climbPosAdjusted = false;
							if (currentClimbingObject.tag == "Ladder" && ladderClimbing) {
								onLadder = true;
                                this.transform.forward = -rayDump2.normal;
								Vector3 desiredPos = new Vector3 (climbingTrans.position.x, this.transform.position.y, climbingTrans.position.z) +
									(climbingTrans.forward * ((climbingTrans.localScale.z / 2f) + cRad));
								if (Physics.Raycast (desiredPos + (Vector3.up * cHeight * 0.45f), -Vector3.up, cHeight * 0.9f, whatIsACollision))
									desiredPos = new Vector3 (desiredPos.x, this.transform.position.y, desiredPos.z);
								climbPos = desiredPos + (Vector3.up * cHeight * 0.1f);
							}
							if (currentClimbingObject.tag == "Fence" && fenceClimbing) {
								onFence = true;
								climbPos = rayDump2.point + (rayDump2.normal*cRad) + (Vector3.up * cHeight * 0.05f);
							}
							if(onLadder || onFence){
								climbing = true;
                                anim.SetTrigger ("onClimb");
								onGround = false;
								rb.velocity = zeroVelocity;
								climbTimer = 0f;
							}
						}
					}
					if(climbing && !climbPosAdjusted){
                        rb.velocity = zeroVelocity;
						if(Vector3.Distance(this.transform.position, climbPos) > (0.99f*cRad)){
							this.transform.position = Vector3.Lerp(this.transform.position, climbPos, 2f * Time.deltaTime);
							anim.SetBool("isClimbing", false);
							anim.ResetTrigger ("land");
							anim.ResetTrigger ("falling");
						}else{
							this.transform.position = climbPos;
							Physics.Linecast (this.transform.position, new Vector3 (currentClimbingObject.transform.position.x, this.transform.position.y, currentClimbingObject.transform.position.z), out rayHitForward, whatIsACollision);
							this.transform.forward = Vector3.Lerp (this.transform.forward, -rayHitForward.normal, Time.smoothDeltaTime * 5f);
							if (Vector3.Distance (this.transform.forward, -rayHitForward.normal) < 0.1f && !onGround) {
								this.transform.forward = -rayHitForward.normal;
								climbPosAdjusted = true;
							}
						}
					}
					if(climbPosAdjusted){
						if (onLadder || onFence) {														//Some animation control; climbing is very reliant on animations
							if (currentAnim.inClimbUp1 || currentAnim.inClimbUp2)
								inUpAnim = true;
							else
								inUpAnim = false;
							if (currentAnim.inClimbDown1 || currentAnim.inClimbDown2)
								inDownAnim = true;
							else
								inDownAnim = false;
						}
						if (climbing && grabBeneath) {
							grabBeneath = false;
							anim.SetTrigger ("onClimb");
						}
					}
					if (onLadder && climbPosAdjusted) {																	//If on a ladder controls
						if (leftY < 0.1f) {
							climbUp = false;
							climbDown = true;
						} else {
							if (leftY > 0.1f) {
								climbUp = true;
								climbDown = false;
							}
						}
						if (leftY == 0f) {
							climbUp = false;
							climbDown = false;
						}

						bool checkTop = false;
						if (climbUp || inUpAnim)
							checkTop = true;
						if (checkTop && this.transform.position.y + (cHeight * 0.55f) > currentClimbingObject.transform.localPosition.y + //Checks if reached top of entire ladder
							(currentClimbingObject.transform.localScale.y * 0.5f)) {
							ReachedTop ("Ladder");
						}
					}
					if (onFence && climbPosAdjusted) {																	//If on a fence controls
						CheckClimbableObject ();
						if (currentAnim.inClimbLeft1 || currentAnim.inClimbLeft2)
							inLeftAnim = true;
						else
							inLeftAnim = false;
						if (currentAnim.inClimbRight1 ||currentAnim.inClimbRight2)
							inRightAnim = true;
						else
							inRightAnim = false;

                        climbUp = false;
                        climbDown = false;
                        climbRight = false;
                        climbLeft = false;
                        if(!inLeftAnim && !inRightAnim && !inUpAnim && !inDownAnim) {
                            if (leftY < -0.1f) { 
							    if (!inLeftAnim && !inRightAnim) {
								    climbUp = false;
								    climbDown = true;
								    climbRight = false;
								    climbLeft = false;
							    }
						    } else {
							    if (leftY > 0.1f) {
								    if (!inLeftAnim && !inRightAnim && canClimbUp) {
									    climbUp = true;
									    climbDown = false;
									    climbRight = false;
									    climbLeft = false;
								    }
							    } else {
								    if (leftX < -0.1f) {
									    if (!inUpAnim && !inDownAnim && !inRightAnim) {
										    climbUp = false;
										    climbDown = false;
										    climbRight = false;
										    climbLeft = true;
									    }
								    } else {
									    if (leftX > 0.1f) {
										    if (!inUpAnim && !inDownAnim && !inLeftAnim) {
											    climbUp = false;
											    climbDown = false;
											    climbRight = true;
											    climbLeft = false;
										    }
									    }
								    }
							    }
						    }
                        }
						if (Mathf.Abs(leftY) <= 0.1f && !inUpAnim && !inDownAnim) {
							climbUp = false;
							climbDown = false;
						}
						if (Mathf.Abs(leftX) <= 0.1f && !inLeftAnim && !inRightAnim) {
							climbRight = false;
							climbLeft = false;
							if (Physics.Raycast (this.transform.position, this.transform.forward, out rayDump2, cRad * 4f, whatIsGround)) {
								this.transform.forward = Vector3.Lerp (this.transform.forward, -rayDump2.normal, Time.deltaTime * 10f);
								currentClimbingObject = rayDump2.collider;
							}
						}

						bool checkTop = false;
						if (climbUp || inUpAnim)
							checkTop = true;
						if (checkTop && !Physics.Raycast(this.transform.position + (Vector3.up*cHeight*0.55f), this.transform.forward, cRad*3f, whatIsGround)) {
							ReachedTop ("Fence");
						}else{
                            if (!canClimbUp)
                                canClimbUp = true;
						}
					}

					if (climbing && climbPosAdjusted) {											//Some adjustments when climbing
						if (currentAnim.inIdle)
							anim.SetTrigger ("onClimb");
						anim.ResetTrigger ("falling");
						if (onFence) {
							if (climbRight)
								anim.SetInteger ("climbX", 1);
							if (climbLeft)
								anim.SetInteger ("climbX", -1);
							if (!climbLeft && !climbRight)
								anim.SetInteger ("climbX", 0);
						} else
							anim.SetInteger ("climbX", 0);
						if (climbUp)
							anim.SetInteger ("climbY", 1);
						if (climbDown)
							anim.SetInteger ("climbY", -1);
						if (!climbUp && !climbDown)
							anim.SetInteger ("climbY", 0);
                        
						if (Physics.Raycast (this.transform.position, this.transform.forward, out rayDump2, cRad * 2.1f, whatIsGround) ||
							Physics.Raycast (this.transform.position + (Vector3.up * cHeight * 0.05f), this.transform.forward, out rayDump2, cRad * 2.1f, whatIsGround)) {
							if (rayDump2.collider.tag != "Ladder" && rayDump2.collider.tag != "Fence" && againstClimbable)
								againstClimbable = false;
						} else {
							if(canFall){
								anim.ResetTrigger ("falling");
								anim.SetBool ("isClimbing", true);
								jumpNo = 0;
                                anim.SetTrigger ("land");
								climbing = false;
                                climbPosAdjusted = false;
                            }
						}
					}

					if (climbing && gPI.pressAction)				//Fall off the climbable object when clicking the action button
						FallOffClimbable ();

					if (climbing && 
						!Physics.Raycast (this.transform.position - (Vector3.up * cHeight * 0.51f) + (this.transform.right * cRad * 0.1f), this.transform.forward, out rayDump, cRad * 5f, whatIsACollision)) {
						if (!Physics.Raycast (this.transform.position - (Vector3.up * cHeight * 0.51f) - (this.transform.right * cRad * 0.1f), this.transform.forward, out rayDump, cRad * 5f, whatIsACollision)) {
							if (!Physics.Raycast (this.transform.position, -Vector3.up, cHeight * 0.7f, whatIsACollision))
								FallOffClimbable ();
						}
					} else {
						if (climbing && 
							Physics.Raycast (this.transform.position - (Vector3.up * cHeight * 0.51f), this.transform.forward, out rayDump, cRad * 5f, whatIsACollision)) {
							if (rayDump.collider.tag != "Ladder" && rayDump.collider.tag != "Fence")
								FallOffClimbable ();
						}
					}
				
					if (onGround && climbing && climbPosAdjusted && !hasLandedThisFrame) {
						anim.SetBool ("isClimbing", true);
						anim.SetInteger ("climbX", 0);
						anim.SetInteger ("climbY", 0);
						anim.ResetTrigger ("falling");
						Land();
						climbing = false;
                        climbPosAdjusted = false;
                    }

					if (!climbing && onLadder || !climbing && onFence) {
						onLadder = false;
						onFence = false;
					}
				}

//////////OLD Box Pushing
				if(oldBoxPushing){
					if (againstBox && !isInPush) {														//If the character is against a box
						mIconG.transform.position = this.transform.position + (Vector3.up * cHeight);		//Shows the prompt above the character
						if (!currentAnim.inRoll && gPI.holdAction && !isCrouching && !crouchDelay) {			//If the character holds the Action Button
							startedPush = false;
							isInPush = true;
							anim.SetTrigger ("push");
							if (!anim.GetBool ("isPushing"))
								anim.SetBool ("isPushing", true);
						}
					}
					if (!againstBox) {														//If not against the box, the prompt will be moved out of sight
						if (mIconG.transform.position != new Vector3 (0f, 1000f, 0f))			//This is best done via object pooling, but is done this way as a short-cut for creating this asset
							mIconG.transform.position = new Vector3 (0f, 1000f, 0f);
					}
					if (isInPush) {															//If the character is grabbing hold of the box, ready to push/pull it
						if (mIconG.transform.position != new Vector3 (0f, 1000f, 0f))
							mIconG.transform.position = new Vector3 (0f, 1000f, 0f);
						if (!gPI.holdAction&& !pushDelay) {										//If the player lets go of the Action Button, the character will let go of it
							isInPush = false;
							anim.SetBool ("isPushing", false);
						}
						if (!startedPush) {													//When first grabbing hold of the box, the vecBoxDist is taken
							Physics.Linecast (this.transform.position, currentBox.transform.position, out rayDump3, whatIsACollision);
							vecBoxDist = Vector3.Distance (rayDump3.point, currentBox.transform.position) + 0.1f;
							startedPush = true;
							boxPushAdjust = false;
						}
						if (startedPush) {													//Once the value is taken...
							if (!boxPushAdjust) {																	//The characters position and forward are adjusted
								boxPos = currentBox.transform.position + (rayDump3.normal * (vecBoxDist + cRad));
								if (this.transform.forward != -rayDump3.normal)
									this.transform.forward = Vector3.Lerp (this.transform.forward, -rayDump3.normal, Time.deltaTime * 20f);
								if (this.transform.position != boxPos)
									this.transform.position = Vector3.Lerp (this.transform.position, boxPos, Time.deltaTime * 5f);
								if (Vector3.Distance (this.transform.position, boxPos) < 0.01f) {
									this.transform.forward = -rayDump3.normal;
									this.transform.position = boxPos;
									boxPushAdjust = true;
								}
							}
							if (boxPushAdjust) {												//Once they've been adjusted, it now checks which direction the player wants to move the box (to push/pull it)
								pushYDir = Vector3.Angle (cam.transform.forward * leftY, this.transform.forward);
								pushXDir = Vector3.Angle (cam.transform.right * leftX, this.transform.forward);
								if (!pushDelay) {
									if (pushYDir <= 45f && Mathf.Abs (leftY) > 0.6f || pushXDir <= 45f && Mathf.Abs (leftX) > 0.6 ||
										pushYDir >= 135f && Mathf.Abs (leftY) > 0.6f || pushXDir >= 135f && Mathf.Abs (leftX) > 0.6) {
										if (cannotBePushed)
											cannotBePushed = false;
										pushSignDir = 1f;
										float addedCheckRoom = 1f;
										if (pushYDir >= 135f && Mathf.Abs (leftY) > 0.6f || pushXDir >= 135f && Mathf.Abs (leftX) > 0.6) {
											pushSignDir = -1f;
											addedCheckRoom = 1.01f + cRad;
										}													//Now checks to see if there will be any obstructions along the way; if so, the box will not move
										if (Physics.Raycast (currentBox.transform.position + (-rayDump3.normal * currentBox.transform.localScale.x * 0.49f * pushSignDir) - (Vector3.up * currentBox.transform.localScale.y * 0.49f),
									               	-rayDump3.normal * pushSignDir, 1f * addedCheckRoom * boxDistToBeMoved, whatIsACollision) ||
											Physics.Raycast (currentBox.transform.position + (-rayDump3.normal * currentBox.transform.localScale.x * 0.49f * pushSignDir) + (this.transform.right * currentBox.transform.localScale.x * 0.49f) 
											- (Vector3.up * currentBox.transform.localScale.y * 0.49f), -rayDump3.normal * pushSignDir, 1f * addedCheckRoom * boxDistToBeMoved, whatIsACollision) ||
											Physics.Raycast (currentBox.transform.position + (-rayDump3.normal * currentBox.transform.localScale.x * 0.49f * pushSignDir) - (this.transform.right * currentBox.transform.localScale.x * 0.49f) 
											- (Vector3.up * currentBox.transform.localScale.y * 0.49f), -rayDump3.normal * pushSignDir, 1f * addedCheckRoom * boxDistToBeMoved, whatIsACollision))
											cannotBePushed = true;
										if (!cannotBePushed) {								//If there are no obstructions, the box will begin moving and the character will follow
											isPushing = true;
											anim.SetFloat ("pushDir", pushSignDir);
											pushDelay = true;
											boxRB = currentBox.gameObject.GetComponent<Rigidbody> ();
											boxRB.isKinematic = false;
											boxRB.velocity = -rayDump3.normal * 4f * pushSignDir * boxMoveSpeed;
											rb.velocity = boxRB.velocity;
											origBoxPos = currentBox.transform.position;
											boxABDist = Vector3.Distance (origBoxPos, origBoxPos - (rayDump3.normal * pushSignDir * boxDistToBeMoved));
										}
									}
								}
							}
						}
					}
					if (isPushing) {																					//Once the box is moving...
						boxDist = Vector3.Distance (currentBox.transform.position, origBoxPos - (rayDump3.normal * pushSignDir * boxDistToBeMoved));	//The distance the box is away from its destination is taken
						boxTravelDist = Vector3.Distance (origBoxPos, currentBox.transform.position);											//The distance that the box has travelled
						if (boxDist < 0.01f || boxTravelDist > boxABDist) {											//If the box has just about reached its destination or overshot it...
							boxRB.velocity = Vector3.zero;
							rb.velocity = zeroVelocity;
							boxRB.isKinematic = true;
							currentBox.transform.position = origBoxPos - (rayDump3.normal * pushSignDir * boxDistToBeMoved);	//...The box will be stopped and placed in its destination
							isPushing = false;
							anim.SetFloat ("pushDir", 0f);
							boxPushAdjust = false;
							StartCoroutine (Delay (10));																//A delay is put in place to give a short break between each push/pull
						}
					}
				}

//////////Ledge Climbing / Jump to grab
				if (againstWall && !isRolling && !masterBool && !jumping && huggingWall && !grabBeneath && onGround && !hardLanded && !isLifting && !againstBox ||
                    onWaterSurface && againstWall && huggingWall) {																//times how long the player is moving the character against a ledge for
					if (Mathf.Sqrt ((leftX * leftX) + (leftY * leftY)) > 0.3f) {
						holdTimer += Time.deltaTime;
						if (maxSpeed != 2.5f)
							maxSpeed = 2.5f;
					} else {
						if (maxSpeed == 2.5f)
							maxSpeed = originalMoveSpeed;
                        holdTimer = 0f;
					}	
				} else {
					if (maxSpeed != 5f)
						maxSpeed = originalMoveSpeed;
                    holdTimer = 0f;
				}
				if(ledgeClimbing || ledgeGrabbing){
					if (Physics.Linecast (rayD.position, rayD.position - (Vector3.up * (1.675f*cHeight)), out rayHitD, whatIsGround) && //Finds a ledge to climb
						!Physics.Linecast (this.transform.position + (this.transform.forward * rayD.transform.localPosition.z) + (Vector3.up * cHeight * 0.25f),
			                (this.transform.position + (this.transform.forward * rayD.transform.localPosition.z)) + 
						(Vector3.up * Mathf.Abs ((rayHitD.point.y + (cHeight * 0.25f)) - this.transform.position.y)), whatIsGround) && //Checks for no ground/walls between the character and the ledge (and cannot grab from underneath)
						!Physics.Raycast (this.transform.position, Vector3.up, cHeight, whatIsACollision)) {
						if (onGround && !jumping && !onLedge && !fallDelay && !grabbing && !grabBeneath && sphereCol.isTrigger ||
							!onGround && onWaterSurface && !onLedge && !fallDelay && !grabbing && !grabBeneath) {
							Vector3 normalCheck = new Vector3 (0f, 0f, 0f);
							Vector3 normalChecker = new Vector3 (0f, 0f, 0f);
							Vector3 originPoint = this.transform.position - (Vector3.up * (0.475f*cHeight));	
							if (Physics.Linecast (originPoint + (this.transform.right * (0.9375f*cRad)),																								//Checks to see if character is climbing on a corner, meaning that one of the arms can't be grabbing onto nothing
									             	originPoint + (this.transform.right * (0.9375f*cRad)) + (this.transform.forward*3.125f*cRad), out rayHitForward, whatIsACollision))
								normalCheck = rayHitForward.normal;
								if (Physics.Linecast (originPoint - (this.transform.right * (0.9375f*cRad)),
									                      originPoint - (this.transform.right * (0.9375f*cRad)) + (this.transform.forward * (0.9375f*cRad)), out rayHitForward, whatIsACollision))
								normalChecker = rayHitForward.normal;
							if (Vector3.Angle (normalCheck, normalChecker) > 95f)																									//If it is on a corner, the character may not climb the ledge
								againstWall = false;
							else {
								if (rayHitD.normal.y > walkableSlopeTolerence) {																													//Checks if ledge isn't too steep; DECREASE to climb STEEPER ledges
									if (Mathf.Abs (rayHitD.normal.x) < 0.4f && Mathf.Abs (rayHitD.normal.z) < 0.4f) {
										distCalc = Vector3.Distance (rayD.position, rayHitD.point);																										//Checks distance from rayD to the ledge (point of contact); the larger the distance, the smaller the ledge
										midPointLedge = rayHitD.point;																																	//gets the point of contact
										if (Physics.Linecast (midPointLedge + (Vector3.up * 0.55f*cHeight) - (this.transform.forward * (0.9375f*cRad)), midPointLedge + (Vector3.up * 0.55f*cHeight) + (this.transform.forward * cRad * 1.5f), 	//Checks for obstructions on the ledge that may make it impossible to do a 'midLedgeGrab' or 'highLedgeGrab'; only low or to jump to grab
									                 out rayHitE1, whatIsGround) ||
										    Physics.Linecast (midPointLedge + (Vector3.up * 0.95f*cHeight) - (this.transform.forward * (0.9375f*cRad)), midPointLedge + (Vector3.up * 0.55f*cHeight) + (this.transform.forward * cRad * 1.5f), 	// "   "
									                 out rayHitE1, whatIsGround)) {
                                            if (distCalc > (cHeight*1.25f) || distCalc <= (cHeight*0.425f)) {																									//If the distance isn't too big or small, then it's a climbable ledge
												if (!againstWall && onGround) {
													againstWall = true;
												}																										//If all is good, then the timer to see how long the player is holding against the wall for starts
												if (onGround && (holdTimer >= 0.15f || onButtonLedgeClimb && gPI.pressAction)) {																										//If the timer has gone on for the time specified here, then the process of climbing the ledge begins and 'onLedge' is set to true
													onLedge = true;
													if (distCalc > (cHeight*1.25f) && ledgeClimbing) {																												//If the distance is over the one specified, the character will do a 'lowJump'; functionality is done in FixedUpdate()
														rb.velocity = zeroVelocity;
														lowJump = true;
													}
													if (distCalc <= (cHeight*0.425f) && ledgeGrabbing) {																												//If the distance is under/equal to the one specified, the character will do a 'ledgeJump' and grab onto the ledge; functionality is done in FixedUpdate()
														rb.velocity = zeroVelocity;
														jumpToGrab = true;
														holdTimer = 0f;
                                                    }
													if(!ledgeClimbing)
														onLedge = false;
												}
											} else
												againstWall = false;
										} else {																																			//If there ISN'T any obstacle on the ledge, blocking the way...
											if (Physics.Linecast (this.transform.position - (Vector3.up * (cHeight * 0.45f)), midPointLedge, out rayHitE2, whatIsACollision)) {
												if (rayHitE2.normal != rayHitD.normal) {
													if (!againstWall)
														againstWall = true;
												}
											}
											if (onGround && !onWaterSurface && (holdTimer >= 0.15f && !onButtonLedgeClimb || onButtonLedgeClimb && gPI.pressAction) || 
												onWaterSurface && !onGround && (holdTimer >= 0.15f && !onButtonLedgeClimb || onButtonLedgeClimb && gPI.pressAction) && distCalc <= (cHeight*1.25f) && distCalc > (cHeight*0.425f) 
											    && !swimDelay && !isSwimming && !diveDelay && !isDiving) {
												onLedge = true;
                                                if (onIncline) {																												//If on incline, raising the character up very slightly will prevent movement while the climbing
													gravityDirection = Vector3.zero;
													this.transform.position += (Vector3.up * 0.0005f * cHeight);
												}
												if (distCalc > (cHeight*1.25f) && distCalc < (rayD.transform.localPosition.y + cHeight/2f) - cRad && !onWaterSurface && !underWater && !onWaterSurface && ledgeClimbing) {
													rb.velocity = zeroVelocity;
													lowJump = true;
												}
												if (distCalc > (cHeight*1.25f) && inWater || distCalc > (cHeight*1.25f) && onWaterSurface && ledgeClimbing)
													onLedge = false;
												if (distCalc <= (cHeight*1.25f) && distCalc > (0.9f*cHeight) && ledgeClimbing) {																					//With 'midLedgeGrab' and 'highLedgeGrab', the layer collision is turned off between the character and 'whatIsACollision';...
													midLedgeGrab = true;																									//...this is because the collider may be inside the ground in order to adjust to the correct height for the animation to look realistic
													Physics.IgnoreLayerCollision (characterCollisionLayer, groundCollisionLayer, true);
													Physics.IgnoreLayerCollision (characterCollisionLayer, slopesCollisionLayer, true);
													this.transform.position += new Vector3 (0f, ((cHeight*1.25f) - distCalc) - (0.1875f*cHeight), 0f);
												}
												if (distCalc <= (0.9f*cHeight) && distCalc > (0.425f*cHeight) && ledgeClimbing) {
													highLedgeGrab = true;
													Physics.IgnoreLayerCollision (characterCollisionLayer, groundCollisionLayer, true);
													Physics.IgnoreLayerCollision (characterCollisionLayer, slopesCollisionLayer, true);
													this.transform.position += new Vector3 (0f, ((0.9f*cHeight) - distCalc) - (0.15f*cHeight), 0f);
												}
												if (distCalc <= (0.425f*cHeight) && !onWaterSurface && !underWater && !onWaterSurface && ledgeGrabbing) {
													rb.velocity = zeroVelocity;
													jumpToGrab = true;
													holdTimer = 0f;
                                                }
												if (distCalc <= (0.425f*cHeight) && inWater || distCalc <= (0.425f*cHeight) && onWaterSurface || !ledgeClimbing)
													onLedge = false;
												if(!lowJump && !midLedgeGrab && !highLedgeGrab && !jumpToGrab)
													onLedge = false;
											}
										}
									} else
										againstWall = false;
								} else
									againstWall = false;
							}
						} else
							againstWall = false;
					} else
						againstWall = false;
				}
				if (grabbing && onGround)																												//A precaution for grabbing to seize if the character is on ground to prevent any odd behaviour
					grabbing = false;
                
                if(onLedge && onWaterSurface){
                    ExitWater(null, true);
                }

				if (Physics.Linecast (rayD.position, rayD.position - (Vector3.up * (cHeight*1.25f)), out rayHitD, whatIsGround) && //Checks for a ledge to grab while falling
					!Physics.Linecast (rayA.position, rayA.position - (Vector3.up * (cHeight*0.6f)), whatIsACollision) && ledgeGrabbing
				    && !onIncline && !onGround && !masterBool && !jumping && !onWaterSurface && !underWater && rb.velocity.y < -0.05f && !fallDelay && !currentAnim.inRoll && !isLifting) {
					Vector3 normalCheck = new Vector3 (0f, 0f, 0f);																						//Checks if it's not on a corner, like with on ground
					Vector3 normalChecker = new Vector3 (0f, 0f, 0f);
					Vector3 originPoint = this.transform.position + (Vector3.up * (cHeight*(3f/8f)));
					if (Physics.Linecast (originPoint + (this.transform.right * (0.9375f*cRad)),
					                      originPoint + (this.transform.right * (0.9375f*cRad)) + (this.transform.forward*(3.125f*cRad)), out rayHitForward, whatIsACollision)) {
						normalCheck = rayHitForward.normal;
					}
					if (Physics.Linecast (originPoint - (this.transform.right * (0.9375f*cRad)),
					                      originPoint - (this.transform.right * (0.9375f*cRad)) + (this.transform.forward*(3.125f*cRad)), out rayHitForward, whatIsACollision)) {
						normalChecker = rayHitForward.normal;
					}
					if (Vector3.Angle (normalCheck, -this.transform.forward) > 45f || Vector3.Angle (normalChecker, -this.transform.forward) > 45f) {

					} else {
                        if(rayHitD.collider.tag != "Fence" && rayHitD.collider.tag != "Ladder"){
							midPointLedge = rayHitD.point;
							Vector3 rayE1Pos = rayE.position - (this.transform.right * (1.09375f*cRad));																								//Checks to see if the ledge isn't too steep
							Vector3 rayE2Pos = rayE.position + (this.transform.right * (1.09375f*cRad));
                            if (rayHitD.normal.y != 0f && 
							    !Physics.Raycast (rayE1Pos, this.transform.forward, 0.78125f*cRad, whatIsACollision) && //Checks for obstructions on ledge (can still grab onto narrow ledges to move along)
							    !Physics.Raycast (rayE2Pos, this.transform.forward, 0.78125f*cRad, whatIsACollision) &&
								!Physics.Raycast (midPointLedge + (Vector3.up * 0.005f*cHeight), Vector3.up, cHeight, whatIsGround)) {
								if (Mathf.Abs (rayHitD.normal.x) < 0.4f && Mathf.Abs (rayHitD.normal.z) < 0.4f) {
									distCalc = Vector3.Distance (rayD.position, rayHitD.point);
									if (distCalc > 0.7f*cHeight) {																																	//Checks distance so that the character grabs it at a realistic point
										if (!grabWhileFalling) {
											if (Physics.Linecast (this.transform.position + (Vector3.up * (0.325f*cHeight)), 
											                      this.transform.position + (Vector3.up * (0.325f*cHeight)) + (this.transform.forward * 4.6875f*cRad), out rayHitForward, whatIsACollision)) {
												this.transform.position = new Vector3 (this.transform.position.x, midPointLedge.y - (cHeight * 0.4f), this.transform.position.z);
												rb.velocity = zeroVelocity;
												gravityDirection = Vector3.zero;
												if(jumpToGrab)
													jumpToGrab = false;
												grabWhileFalling = true;
											}
										}
									}
								}
							}
						}
					}
				}
				if (grabbing && !jumpToGrab) {																																			//If the character is in its 'grabbing' state...
					if (grabBeneath)
						grabBeneath = false;
					if (!grabAdjusted && grabbing) {																																					//Adjusts the rotation of the character to face the ledge that they're grabbing
						grabbedSoLetGo = true;																																			//Is made true so that the character adjusts before being able to fall off/pull up/move around
						StartCoroutine (Delay (9));
						bool areSameZ = true;
						float deltAngle = Mathf.DeltaAngle (this.transform.localEulerAngles.y, relative.transform.localEulerAngles.y);				//Depending on where the cam is relative to the character, pressing up or down will pull the character up in the proper scenario
						if (deltAngle > 90f || deltAngle < -90f)
							areSameZ = false;
						if (!areSameZ && !adjustingCam && !cam.isometricMode) {
							adjustingCam = true;
							trigDelay = false;
							StartCoroutine (Delay (3));
							StartCoroutine (Delay (8));
							targetting = true;
							cam.behindMode = false;
							cam.targetMode = true;
						}
						if (Physics.Linecast (this.transform.position + (Vector3.up * 0.45f * cHeight), 
						                      this.transform.position + (Vector3.up * 0.45f * cHeight) + (this.transform.forward * 4.6875f*cRad), out rayHitForward, whatIsACollision)) {
							this.transform.forward = -new Vector3 (rayHitForward.normal.x, 0f, rayHitForward.normal.z);																	//Sets the characters forward to be the negative of the wall beneath the ledge
							this.transform.position = new Vector3 (rayHitForward.point.x, this.transform.position.y, rayHitForward.point.z) - (this.transform.forward * cRad);
							grabAdjusted = true;
						}
					}
					if (grabAdjusted && !grabbedSoLetGo && grabbing) {																																	//Once adjusted, this makes checks to see if the character can move left/right along the ledge
						Vector3 rayE1Pos = rayE.position - (this.transform.right * 1.875f*cRad) + (this.transform.forward * cRad * 0.6f);
						Vector3 rayE2Pos = rayE.position + (this.transform.right * 1.875f*cRad) + (this.transform.forward * cRad * 0.6f);
						Vector3 abovePos = this.transform.position + (Vector3.up*cHeight) + (this.transform.forward*cRad*1.2f);
						grabCanMoveLeft = false;																																			//The character can now move along the ledge
						if (Physics.Linecast (rayE1Pos, rayE1Pos - (Vector3.up * cHeight), out rayHitE1, whatIsACollision) &&
						    !Physics.Raycast(this.transform.position, -this.transform.right, cRad*1.2f, whatIsACollision) &&
						    !Physics.Raycast(abovePos, -this.transform.right, cRad*1.2f, whatIsACollision)) {																		//Firstly, it checks above to see if there is space to the left to move over
							if (rayHitE1.normal.y != 0f && Mathf.Abs (rayHitE1.normal.x) < 0.4f && Mathf.Abs (rayHitE1.normal.z) < 0.4f) {														//Secondly, it checks that the space is steep enough to move over
								if (Physics.Linecast ((rayHitE1.point - (Vector3.up * 0.025f*cHeight)) + (rayHitForward.normal*3.125f*cRad), rayHitE1.point - (Vector3.up * 0.025f*cHeight), out rayHitE1, whatIsACollision)) {	//Thirdly, it checks the ledge that we're about to move onto...
									if (new Vector3 (rayHitE1.normal.x, 0f, rayHitE1.normal.z) == new Vector3 (rayHitForward.normal.x, 0f, rayHitForward.normal.z))								//Finally, it checks that the ledge has the same normal that the character is already on
										grabCanMoveLeft = true;																																//'grabCanMoveLeft' is set to true; movement is handled in FixedUpdate()
								}
							}
						}
						grabCanMoveRight = false;																																			//Same code as above, made for moving to the right
						if (Physics.Linecast (rayE2Pos, rayE2Pos - (Vector3.up * cHeight), out rayHitE2, whatIsGround) &&
						    !Physics.Raycast(this.transform.position, this.transform.right, cRad*1.2f, whatIsACollision) &&
						    !Physics.Raycast(abovePos, this.transform.right, cRad*1.2f, whatIsACollision)) {
							if (rayHitE2.normal.y != 0f && Mathf.Abs (rayHitE2.normal.x) < 0.4f && Mathf.Abs (rayHitE2.normal.z) < 0.4f) {
								if (Physics.Linecast ((rayHitE2.point - (Vector3.up * 0.025f*cHeight)) + (rayHitForward.normal*3.125f*cRad), rayHitE2.point - (Vector3.up * 0.025f * cHeight), out rayHitE2, whatIsGround)) {
									if (new Vector3 (rayHitE2.normal.x, 0f, rayHitE2.normal.z) == new Vector3 (rayHitForward.normal.x, 0f, rayHitForward.normal.z))
										grabCanMoveRight = true;
								}
							}
						}
						if (Physics.Linecast (rayE.position - (this.transform.forward * cRad) - (Vector3.up * 0.25f*cHeight), rayE.position + (this.transform.forward * cRad * 1.2f) - (Vector3.up * 0.25f*cHeight), 			//For pulling up while grabbing; checks to see if there's room above to pull up into
				                    out rayHitE1, whatIsGround) ||
							Physics.Linecast (rayE.position - (this.transform.forward * cRad) - (Vector3.up * 0.7f*cHeight), rayE.position + (this.transform.forward * cRad * 1.2f) - (Vector3.up * 0.7f*cHeight), 
				                 out rayHitE1, whatIsGround) ||
							Physics.Linecast (rayE.position - (this.transform.forward * cRad) - (Vector3.up*0.5f*cHeight), rayE.position + (this.transform.forward * cRad * 1.2f) - (Vector3.up*0.5f*cHeight), 
				                 out rayHitE1, whatIsGround)) {

						} else {
							if (leftY > 0.4f && !pullUp) {																					//pullUp is set to true and is controller in FixedUpdate(); basically uses the same logic as 'highLedgeGrab'
								anim.SetBool ("pullUp", true);
								Physics.IgnoreLayerCollision (characterCollisionLayer, groundCollisionLayer, true);
								Physics.IgnoreLayerCollision (characterCollisionLayer, slopesCollisionLayer, true);
								rb.isKinematic = true;
								onLedge = true;
								onLedgeAdjusted = true;
								pullUp = true;
							}
						}

						if (gPI.pressAction && !fallOff && !pullUp) {																			//Pressing the action button will have the character fall off from grabbing and a delay will run as a co-routine for a split second...
							fallOff = true;																												//...to avoid grabbing the same ledge RIGHT after falling off from it
							jumpNo = 1;
							anim.SetTrigger ("falling");
							fallDelay = true;
							StartCoroutine (Delay (2));
						}
					}
				}
				if (onLedge && !onLedgeAdjusted) {																																				//If the character is in its 'onLedge' state...
					if (inWater || onWaterSurface) {
						if (Physics.Linecast (this.transform.position - (Vector3.up * cHeight * 0.49f), 
						                      this.transform.position - (Vector3.up * cHeight * 0.49f) + (this.transform.forward * 4.6875f*cRad),
					                      out rayHitForward, whatIsACollision)) {																													//Checks the wall beneath the ledge to adjust the characters rotation
							this.transform.forward = -new Vector3 (rayHitForward.normal.x, 0f, rayHitForward.normal.z);
							rb.isKinematic = true;
							rb.velocity = zeroVelocity;
							onLedgeAdjusted = true;
						} else {
							onLedge = false;
						}
					} else {
						if (!lowJump && !jumpToGrab)
							rb.isKinematic = true;
						rb.velocity = zeroVelocity;
						onLedgeAdjusted = true;
					}
				}
				if(onLedge && onLedgeAdjusted && !inCoRout && !currentAnim.inALedgeClimbAnim && !currentAnim.inJumpToGrab){
                    if (lowJump){
                        onLedge = false;
                        onLedgeAdjusted = false;
                        Jump("lowLedge");
                    }
					if (jumpToGrab)
						anim.Play("jumpToGrab", 0);
					//	Jump ("Jump To Grab");
					if(midLedgeGrab)
						anim.SetBool ("midLedge", true);
					if(highLedgeGrab)
						anim.SetBool ("pullUp", true);
				}

//////////Rolling/Crouching Action
				if (gPI.pressAction && Vector3.Magnitude (rb.velocity) > 2f && Vector3.Magnitude (direction) > 0.8f 						//If the action button is pressed while the character is moving fast enough under the right conditions...
				    && onGround && !currentAnim.inRoll && !targetting && !masterBool && !jumping && !onWaterSurface && !underWater && !isCrouching && rollFunction && !againstLiftable && !isLifting) {															//...the character will be in a 'rolling' state which is handled in FixedUpdate() and also uses a co-routine as a delay
					if(fPVing && gPI.LV > 0.8f || !fPVing){
						isRolling = true;
                        anim.Play("roll", 0);
						StartCoroutine (Delay (4));
					}
				}
				if(!uItems.isUnequipping && gPI.pressAction && !crouchDelay && !isCrouching && Vector3.Magnitude (direction) <= 0.8f 						//If the action button is pressed while the character is moving fast enough under the right conditions...
				   && onGround && !currentAnim.inRoll && !targetting && !masterBool && !jumping && !onWaterSurface && !underWater && !isRolling && !againstBox && crouchFunction && !againstLiftable && !isLifting && inACrouchBox){
					crouchDelay = true;
					isCrouching = true;
					anim.SetTrigger("crouch");
					anim.ResetTrigger("unCrouch");
                    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().fieldOfView = 15;
                    //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollower>().fpvMode = true;
                    //GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollower>().inFPV = true;
                    GetComponent<UsingItems1>().enabled = false;
                    fPVing = true;
                    StartCoroutine(Delay (15));
				}
				if(!crouchDelay && isCrouching){
				   if(!Physics.Raycast(this.transform.position + sphereCol.center + (this.transform.forward*cRad), Vector3.up, cHeight-sphereCol.radius, whatIsACollision)
				  	&& !Physics.Raycast(this.transform.position + sphereCol.center - (this.transform.forward*cRad), Vector3.up, cHeight-sphereCol.radius, whatIsACollision)
				  	&& !Physics.Raycast(this.transform.position + sphereCol.center + (this.transform.right*cRad), Vector3.up, cHeight-sphereCol.radius, whatIsACollision)
				   	&& !Physics.Raycast(this.transform.position + sphereCol.center - (this.transform.right*cRad), Vector3.up, cHeight-sphereCol.radius, whatIsACollision)){
						isUnderCollision = false;
						if(putIntoFPV && fPVing){
							cam.fpvMode = false;
							cam.exitFPV = true;
							putIntoFPV = false;
						}
						if(!gPI.holdAction){
							crouchDelay = true;
							isCrouching = false;
							anim.SetTrigger("unCrouch");
                            GetComponent<UsingItems1>().enabled = true;
							StartCoroutine(Delay (15));
						}
					}else
						isUnderCollision = true;
				}
				if (isRolling && !onGround && !underWater) {																													//If the character is off the ground just slightly and is not jumping, they will carry on rolling
					if (!jumping && !isTargetJumping && Physics.Linecast (this.transform.position, this.transform.position - (Vector3.up * cHeight * 0.55f), out rayDump, whatIsGround))	
						onGround = true;
					else
						isRolling = false;
				}

				if (isRolling && Physics.Raycast (this.transform.position + sphereCol.center, this.transform.forward, cRad * 1.5f, whatIsACollision) && !rollKnockBack	//If the character rolls into a wall at a speed...
					&& Mathf.Abs (Vector3.Magnitude (rb.velocity)) > 8f && !isUnderCollision) {
                    isRollKnockedBack = true;                                                                                                                   //...the character is put into a 'rollKnockBack' state and is pushed back in FixedUpdate()
                    rb.velocity = zeroVelocity;
                    rollKnockBack = true;
                    anim.SetTrigger("rollKnock");
                    isRolling = false;
                    StartCoroutine(Delay(6));
                }
				if(currentAnim.inRoll){
					if(!capCol.isTrigger){
						capCol.isTrigger = true;
						sphereCol.isTrigger = false;
					}
					if(Physics.Raycast(this.transform.position + sphereCol.center, Vector3.up, cHeight-sphereCol.radius, whatIsACollision))
						isUnderCollision = true;
					else
						isUnderCollision = false;
				}else {
					if(!crouchDelay && !isCrouching && capCol.isTrigger && !isUnderCollision){
						sphereCol.isTrigger = true;
						capCol.isTrigger = false;
					}
					if(!crouchDelay && !isCrouching && isUnderCollision && crouchFunction){
						isCrouching = true;
						crouchDelay = true;
						StartCoroutine(Delay (15));
					}
				}

//////////Lifting
				if(Physics.Linecast(this.transform.position + (this.transform.forward * cRad), this.transform.position + (this.transform.forward * cRad * 3f) - (Vector3.up*cHeight/2f),
				                    out rayLift, whatIsLiftable) && !isLifting && !actionStarted){
                    actionStarted = true;
                    StartCoroutine(ActionAvailable("Soulever"));
                    actionStarted = false;
					if(gPI.pressAction && !uItems.isUnequipping){
						currentLiftable = rayLift.collider.gameObject;
						origLiftableParent = rayLift.collider.gameObject.transform.parent.gameObject;
					}
					againstLiftable = true;
				}else{
					againstLiftable = false;
                    
				}
				if(againstLiftable){
					if(gPI.pressAction && !uItems.isUnequipping && onGround && !isLifting &&
					   Vector3.Angle(this.transform.forward, currentLiftable.transform.position - this.transform.position) < 50f){
						currentLiftable.GetComponent<Rigidbody>().isKinematic = true;
						liftDelay = true;
						liftCorrectDelay = true;
						StartCoroutine(Delay(19));
						isLifting = true;
						Physics.IgnoreLayerCollision(characterCollisionLayer, liftablesCollisionLayer, true);
						anim.SetTrigger("isLifting");
                        if(currentLiftable.tag =="Chicken")
                        {
                            gravityIntesnity = 0.1f;
                        }
					}
				}
				if(liftDelay){
					if(liftCorrectDelay)
						this.transform.LookAt(Vector3.Lerp(this.transform.position + this.transform.forward,
							new Vector3(currentLiftable.transform.position.x, this.transform.position.y, currentLiftable.transform.position.z),
						                                   Time.deltaTime * 0.6f));
				}
				if(!liftDelay && isLifting && currentLiftable.transform.position != this.transform.position + (Vector3.up*cHeight*0.6f) && !puttingDown)
					currentLiftable.transform.localPosition = origLiftLocPos;
				if(liftOveride && !liftDelay){
					origLiftableParent = GameObject.Find("Interactables").gameObject;
					currentLiftable.GetComponent<Rigidbody>().isKinematic = true;
					liftDelay = true;
					liftCorrectDelay = true;
					isLifting = true;
					Physics.IgnoreLayerCollision(characterCollisionLayer, liftablesCollisionLayer, true);
					anim.ResetTrigger("liftThrow");
					anim.ResetTrigger("liftPutDown");
					anim.SetTrigger("directLift");
                    actionText.InteractionAvailable(ORKFramework.InteractionType.Event, " ");
                    actionText.Interact();
                    StartCoroutine(Delay(22));
				}
				if(liftOveride && liftDelay)
					currentLiftable.transform.position = this.transform.position + (Vector3.up*cHeight*0.6f);
				if(isLifting && !liftDelay && !liftThrowDelay && !puttingDown || liftAction){
					if(gPI.pressAction || liftAction){
						if(Vector3.Magnitude(rb.velocity) > 1.5f){
                            actionText.InteractionAvailable(ORKFramework.InteractionType.Custom, " Jeter");
                            actionText.Interact();
                            anim.SetTrigger("liftThrow");
							liftThrowDelay = true;
							StartCoroutine(Delay(20));
						}else{
							puttingDown = true;
							anim.SetTrigger("liftPutDown");
							StartCoroutine(Delay(21));
						}
						liftAction = false;
					}
				}

                

//////////Side Jumping/Back Flip
				if (gPI.pressAction && targetting && onGround && !isRolling && !masterBool && !onWaterSurface && !underWater && !isLifting || //If the character is targetting and the player presses the action button...
				    gPI.pressAction && !masterBool && fPVing && !onWaterSurface && !underWater && !isLifting) {
					bool animCheck = false;

                    targDir = (Quaternion.Euler(0, cam.transform.eulerAngles.y, 0)) * direction;
                    targAngle = Vector3.Angle(targDir, this.transform.forward);
                    targDot = Vector3.Dot(targDir, this.transform.right);

                    if (currentAnim.inATargJumpAnim || currentAnim.inFall)
						animCheck = true;
					anim.ResetTrigger ("land");
					if ((targAngle > 45f && targAngle <= 135f && targDot > 0f && Vector3.Magnitude(direction) >= 0.8f) && !targJumpDelay && !animCheck && !onlyRollInTargetMode) {													//...while holding right will but the character into a 'targetJumping' state and will perform a right side-hop (controlled via FixedUpdate())
						rb.velocity = zeroVelocity;
						isTargetJumping = true;
						targJumpDelay = true;
						StartCoroutine (Delay (5));
						gravityDirection = Vector3.zero;
						rightJump = true;
						anim.SetTrigger ("rightSH");
					} else {
						if ((targAngle > 45f && targAngle <= 135f && targDot < 0f && Vector3.Magnitude(direction) >= 0.8f) && !targJumpDelay && !animCheck && !onlyRollInTargetMode) {												//...while holding left will but the character into a 'targetJumping' state and will perform a left side-hop (controlled via FixedUpdate())
							rb.velocity = zeroVelocity;
							isTargetJumping = true;
							targJumpDelay = true;
							StartCoroutine (Delay (5));
							gravityDirection = Vector3.zero;
							leftJump = true;
							anim.SetTrigger ("leftSH");
						} else {
							if (targAngle > 135f && Vector3.Magnitude(direction) >= 0.8f && !targJumpDelay && !animCheck && !onlyRollInTargetMode) {											//...while holding back will but the character into a 'backJumo' state and will perform a back-flip (controlled via FixedUpdate())
								rb.velocity = zeroVelocity;
								isTargetJumping = true;
								targJumpDelay = true;
								StartCoroutine (Delay (5));
								gravityDirection = Vector3.zero;
								anim.SetTrigger ("backFlip");
								backJump = true;
							} else {
								if ((targAngle <= 95f || onlyRollInTargetMode) && Vector3.Magnitude(direction) >= 0.8f && !currentAnim.inRoll && rollFunction) {                           //...while holding forward will have the character roll like normal
                                    forwardBeforeRoll = this.transform.forward;
                                    isRolling = true;
                                    anim.Play("roll", 0);
                                    StartCoroutine (Delay (4));
								}
							}
						}
					}
				}

//////////Camera Targetting (free or focus)
				if(targetCamera && !cam.isometricMode){
					if (clickedTarget && !targetting && trigDelay || clickedTarget && isFocusing && trigDelay) {		//When the player presses the target button
						trigDelay = false;																											//Sets a delay for camera to do its transition
						StartCoroutine (Delay (3));
						if (inFree) {														//Pressing the button cancels out 'free camera' and 'first person mode'
							inFree = false;
							cam.freeMode = false;
						}
						if (cam.fpvMode) {
							cam.fpvMode = false;
							cam.exitFPV = true;
						}

						if(focusOnTargets){
							CountTargets ();													//Runs a method to count all the targets in the scene within a certain distance
							if (distDic.Count == 0)											//If there is a target within range, the character will focus on it; otherwise, they will just enter target mode
								isFocusing = false;
							else
								isFocusing = true;

							if (isFocusing) {																									//If the character is focusing on a target...
								focusNumber += 1;
								int iterator = 0;
								if (distDic.Count == 1) {																						//If there's only one target within range...
									if (focusNumber <= 1) {																					//...it sets the current target to be the one in range
										foreach (var entry in distDic)
											currentTarget = entry.Value.gameObject;
									} else {																									//If the button is pressed again, it will un-focus on the target
										if (currentTarget != null)
											currentTarget = null;
										targy.transform.position = new Vector3 (1000f, 1000f, 1000f); //Because of the way that the prefab is programmed, the prefab can be removed from the scene by another means; it is currently this way for simplicity
										isFocusing = false;
										focusNumber = 0;
										cam.targetMode = false;
										cam.behindMode = true;
									}
								}
								if (distDic.Count >= 2) {																						//If there's more than one target within range...
									foreach (var entry in distDic) {																			//...it will cycle through the targets, from nearest to furthest on each button press...
										iterator += 1;																						//...and reaching the end will start at the start of the list again
										if (focusNumber > distDic.Count)
											focusNumber = 1;
										if (entry.Value.GetComponent<TargetNumber> () != null) {
											if (iterator == 1) {
												if (currentTarget == null) {
													currentTarget = entry.Value.gameObject;
													entry.Value.GetComponent<TargetNumber> ().number = iterator;									//EACH TARGET MUST HAVE THE 'TargetNumber' script to be correctly placed in the list
													focusNumber = iterator;
												} else {
													if (entry.Value.GetComponent<TargetNumber> ().number != 1 && entry.Value != currentTarget) {	//If the closest target is now different, the one that the character will focus on will now be the closest target
														currentTarget = entry.Value.gameObject;
														entry.Value.GetComponent<TargetNumber> ().number = iterator;
														focusNumber = iterator;
													}
												}
											}
											entry.Value.GetComponent<TargetNumber> ().number = iterator;
											if (focusNumber == entry.Value.GetComponent<TargetNumber> ().number)
												currentTarget = entry.Value.gameObject;
										} else {
											isFocusing = false;
                                            #if UNITY_EDITOR
                                            Debug.LogError ("ThirdPersonController.cs: There seems to at least one object in your scene with the tag 'Target' but hasn't got the TargetNumber.cs script attached to it. Please find this object/these objects and attach the script to it.");
											UnityEditor.EditorApplication.isPlaying = false;
											Debug.Break ();
											Debug.DebugBreak ();
											Application.Quit ();
                                            #endif
                                        }
									}
								}
							}
							distDic.Clear ();																								//The dictionary is cleared to recalculate the distances for each button press
						}
						if (!isFocusing) {																								//The camera is set back to orbiting mode is the character is no longer focusing
							maxSpeed = originalMoveSpeed;
							targetting = true;
							cam.behindMode = false;
							cam.targetMode = true;
						}
					}
					if (isFocusing) {																										//If the character is focusing...
						if (currentTarget != null) {																							//The characters movement speed is different depending on their distance away from their target...
							float slower = 3f / Vector3.Distance (this.transform.position, currentTarget.transform.position);				//...The closer the character is, the slower they'll move
							maxSpeed = originalMoveSpeed - slower;
						}
						if (Mathf.Abs (Vector3.Angle (this.transform.up, currentTarget.transform.position - this.transform.position)) < 15f) {	//If the target is directly above the character, the character will un-focus on the target
							maxSpeed = originalMoveSpeed;
							isFocusing = false;
							currentTarget = null;
							targy.transform.position = new Vector3 (1000f, 1000f, 1000f); //Because of the way that the prefab is programmed, the prefab can be removed from the scene by another means; it is currently this way for simplicity
							cam.targetMode = false;
							cam.behindMode = true;
						}
						if (currentTarget != null) {
							targy.transform.position = currentTarget.transform.position;													//The camera is set to look at the 'altLookAt' transform which is placed half way between the character and the target
							altLookAt.transform.position = (this.transform.position + currentTarget.transform.position) / 2f;
							cam.targetTransform = altLookAt;
							float distAway = Vector3.Distance (this.transform.position, currentTarget.transform.position) * 2f;			//The cameras distance away gets smaller the closer the character is to the target...
							if (distAway > 7f)																								//...The camera cannot go back further than the float stated in the if-statement
								cam.distanceAway = distAway;
							else
								cam.distanceAway = 7f;
							if (!targetting) {
								targetting = true;
								cam.behindMode = false;
								cam.targetMode = true;
							}
						}
						CountTargets ();																							//Counts the targets within range again...
						if (distDic.Count == 0) {																					//...If there aren't any within range, the character will un-focus and the camera will return to orbiting
							targy.transform.position = new Vector3 (1000f, 1000f, 1000f); //Because of the way that the prefab is programmed, the prefab can be removed from the scene by another means; it is currently this way for simplicity
							maxSpeed = originalMoveSpeed;
							if (currentTarget != null)
								currentTarget = null;
							isFocusing = false;
							focusNumber = 0;
							cam.targetMode = false;
							cam.behindMode = true;
							cam.targetTransform = this.transform.Find ("FollowMe").transform;
						}
						distDic.Clear ();
					}
					if (!holdingTarget && targetting && targetHoldMode && isFocusing) {								//If the target hold mode is ON, letting go of the target button will have the character un-focus the target
						if (currentTarget != null)
							currentTarget = null;
						targy.transform.position = new Vector3 (1000f, 1000f, 1000f); //Because of the way that the prefab is programmed, the prefab can be removed from the scene by another means; it is currently this way for simplicity
						isFocusing = false;
						focusNumber = 0;
					}
					if (!holdingTarget && targetting && trigDelay && !isFocusing && !adjustingCam) {									//If the player lets go of the target button while in target mode, without focusing, the camera will return to orbiting
						cam.targetTransform = this.transform.Find ("FollowMe").transform;
						targetting = false;
						cam.targetMode = false;
						cam.behindMode = true;
					}
				}else{
					if(focusOnTargets)
						targetCamera = false;
				}

//////////Camera Free Mode
				if(freeCamera && !cam.isometricMode){
					if (!targetting && gPI.RH > 0.5f || !targetting && gPI.RH < -0.5f || //If the other control stick is moved (on the Z Input Axes), the camera will enter 'free mode'
						!targetting && gPI.RV > 0.5f || !targetting && gPI.RV < -0.5f ||
						!targetting && Input.GetAxis ("Mouse ScrollWheel") > 0.01f || !targetting && Input.GetAxis ("Mouse ScrollWheel") < -0.01f) {
						if (cam.canFreeMode) {
							if (!inFree && !fPVing || inFree && !cam.freeMode) {
								cam.freeAway = cam.distanceAway;
								inFree = true;
								cam.behindMode = false;
								cam.freeMode = true;
							}
						}
					}
				}

//////////Camera First Person Mode
				if(fpvCamera && canFPV && !cam.isometricMode){
					if (!targetting && gPI.pressLBump && !fPVing && !cam.inFPV && !inACutscene || isUnderCollision && !fPVing) {	//Pressing the left bumper, while not in 'first person view' will put the camera into first person view
						fPVing = true;
						if(isUnderCollision)
							putIntoFPV = true;
						if (inFree) {
							inFree = false;
							cam.freeMode = false;
						}
						cam.behindMode = false;
						cam.fpvMode = true;
					}
					if (fPVing && gPI.pressLBump && cam.inFPV && !isUnderCollision) {										//Pressing it again will have the camera return to normal
						cam.fpvMode = false;
						cam.exitFPV = true;
					}
				}

//////////camera positioning help
				if (!cam.inFPV && !cam.freeMode && !cam.targetMode) {												//This lowers the position of the transform that the camera is looking at when the camera is closer to the character
					float minusAmount = cam.distanceAway;
					if (minusAmount < 1.75f*this.transform.localScale.y)
						minusAmount = 1.75f*this.transform.localScale.y;
					this.transform.Find ("FollowMe").transform.localPosition = new Vector3 (0f, (1f - (1f / minusAmount))/yScale, 0f);
				}

//////////misc adjustments
				if (grabbing || grabBeneath) {																	//Turns off the layer collisions when grabbing or grabbing beneath
					Physics.IgnoreLayerCollision (characterCollisionLayer, groundCollisionLayer, true);
					Physics.IgnoreLayerCollision (characterCollisionLayer, slopesCollisionLayer, true);
				} else {
					if (Physics.GetIgnoreLayerCollision (characterCollisionLayer, slopesCollisionLayer) && !onLedge) {
						Physics.IgnoreLayerCollision (characterCollisionLayer, groundCollisionLayer, false);
						Physics.IgnoreLayerCollision (characterCollisionLayer, slopesCollisionLayer, false);
					}
				}
				if (onGround && currentAnim.inAJumpAnim && delayed || onGround && jumping && delayed || onGround && currentAnim.inFall && delayed) {
					if(Physics.Raycast(this.transform.position, -Vector3.up, cHeight*0.51f, whatIsGround)){
						jumping = false;
						anim.ResetTrigger ("jumpNow");
						anim.ResetTrigger ("doubleJumpNow");
						if (!hasLandedThisFrame) {
							Land ();
						}
					}
				}
				if (grabbing && currentAnim.inFall)
					anim.SetTrigger ("grabbing");
				if (onLedge)
					anim.ResetTrigger ("falling");

				if(jumpToGrab && rb.velocity.y > 0f){
					anim.ResetTrigger ("land");
					anim.ResetTrigger ("falling");
				}

				if (currentAnim.trigLocoBlend && onGround) {
					anim.ResetTrigger ("land");
					anim.ResetTrigger ("falling");
				}

				if (currentAnim.inATargJumpAnim && rb.velocity.y < -0.1f) {
					onGround = false;
				}

				if (inACutscene) {
					rb.velocity = zeroVelocity;
					if(jumping)
						jumping = false;
					if(fPVing)
						this.transform.forward = Vector3.Lerp(this.transform.forward, relative.transform.forward, Time.deltaTime * 10f);
					else
						this.transform.forward = Vector3.Lerp(this.transform.forward, 
						                                      new Vector3(cutsceneLookAtPos.x - this.transform.position.x, 0f, cutsceneLookAtPos.z - this.transform.position.z),
						                                      Time.deltaTime * 10f);
					if(targetting){
						if (currentTarget != null)
							currentTarget = null;
						targy.transform.position = new Vector3 (1000f, 1000f, 1000f);
						focusNumber = 0;
						cam.targetMode = false;
						isFocusing = false;
						targetting = false;
					}
				}

				if(crouchDelay)
					rb.velocity = zeroVelocity;

				if(fPVing){
					if(crouchDelay || isCrouching){
						if(modelTransform.localPosition.z + (1.875f*cRad) > 0.01f)
							modelTransform.localPosition = new Vector3(0f, modelTransform.localPosition.y, Mathf.Lerp(modelTransform.localPosition.z, -(1.875f*cRad), Time.deltaTime*10f));
						else
							modelTransform.localPosition = new Vector3(0f, modelTransform.localPosition.y, -(1.875f*cRad));
					}else{
						if(modelTransform.localPosition.z < -0.01f)
			        		modelTransform.localPosition = new Vector3(0f, modelTransform.localPosition.y, Mathf.Lerp(modelTransform.localPosition.z, 0f, Time.deltaTime*10f));
						else
							modelTransform.localPosition = new Vector3(0f, modelTransform.localPosition.y, 0f);
					}
				}else{
                    if (modelTransform.localPosition.z != 0f && !onLedge && !grabBeneath)
                        modelTransform.localPosition = new Vector3(0f, modelTransform.localPosition.y, 0f);
                }

				anim.SetBool("underACol", isUnderCollision);

				if(startedSliding && slidingDownSlope){
					this.transform.forward = Vector3.Lerp(this.transform.forward, new Vector3(slidingDir.x, 0f, slidingDir.z), Time.deltaTime * 10f);
					if(!anim.GetBool("slidingDown"))
						anim.SetBool("slidingDown", true);
					else{
						if(!slideDelay)
							anim.ResetTrigger("land");
					}
				}
				if(jumping && slidingDownSlope || !onGround && !slidingDownSlope && currentAnim.inSliding ||
				   !onGround && startedSliding && !slidingDownSlope ||
				   !Physics.Raycast(this.transform.position + rb.centerOfMass, -slideRelative.transform.up, cHeight/2f, whatIsSlope) 
				   && jumpNo > 0 && !startedDelay && !backOnSlope){
					slidingDownSlope = false;
					startedSliding = false;
					slideDelay = true;
					anim.SetBool("slidingDown", false);
				}else{
					if(Physics.Raycast(this.transform.position + rb.centerOfMass, -Vector3.up, cHeight/2f, whatIsGround) && currentAnim.inSliding){
						slidingDownSlope = false;
						rb.velocity *= 0.9f;
						StartCoroutine(Delay(16));
					}
				}

                if (fPVing && rb.interpolation != RigidbodyInterpolation.None)
                    rb.interpolation = RigidbodyInterpolation.None;
                if (!fPVing && rb.interpolation == RigidbodyInterpolation.None)
                    rb.interpolation = RigidbodyInterpolation.Interpolate;

                if (anim.GetBool("useBow") && cam.freeMode){
                    cam.freeMode = false;
                    cam.fpvMode = true;
                }

                if (currentAnim.inFall && inWater && isSwimming && !onWaterSurface)
                    anim.SetTrigger("startSwimming");

                if (onGround && !inWater && currentAnim.inDiving)
                    anim.Play("Idle", 0);

                if (currentAnim.swimmingBlend && isSwimming && underWater && !inWater)
                    inWater = true;

				if (currentAnim.inReachItem && onGround)
                    rb.velocity = zeroVelocity;

                if (currentAnim.inFall && rb.velocity.y > 0f && !jumping && !delayed)
                    rb.velocity = new Vector3(rb.velocity.x, -1f, rb.velocity.z);

                if (currentAnim.inAClimbingAnim && !onGround && !climbing && !onFence && !onLadder && rb.velocity.y < 0f) {
                    anim.SetBool("isClimbing", true);
                    anim.SetTrigger("falling");
                }

                if ((!inWater && anim.GetBool("diving") == true) || (inWater && anim.GetBool("diving") == true && underWater && isSwimming))
                    anim.SetBool("diving", false);

				if ((!inWater && currentAnim.swimSurfaceBlend) || (rb.velocity.y < 0f && jumpToGrab && onGround)) {
					if (jumpToGrab)
						jumpToGrab = false;
					anim.Play ("falling", 0);
				}

				if(!onLedge && rb.velocity == Vector3.zero && (lowJump || jumpToGrab || pullUp || midLedgeGrab || highLedgeGrab)){
					lowJump = false;
					jumpToGrab = false;
					pullUp = false;
					midLedgeGrab = false;
					highLedgeGrab = false;
				}

				if (targetting && !isFocusing && !cam.targetMode)
					targetting = false;
            }
		}
	}

	public void RelativeSet(){
		//////////Selecting the relative transform (the transform that the character uses to rotate relative to the camera)
		if (cam != null) {
			if (!isFocusing || climbing || onLedge || grabbing){                                                                                                                            //When focusing, the transform that 'relative' refers to is changed
                relative.transform.rotation = new Quaternion (0f, cam.transform.rotation.y, 0f, cam.transform.rotation.w);
			}else {
				relative.transform.rotation = new Quaternion (0f, cam.focusRelative.transform.rotation.y, 0f, cam.focusRelative.transform.rotation.w);
				this.transform.LookAt (new Vector3 (currentTarget.transform.position.x, this.transform.position.y, currentTarget.transform.position.z));
			}
		}
		if(fPVing){
			if(onLedge || grabbing || climbing)
				cam.transform.forward = Vector3.Lerp(cam.transform.forward, cam.fpvPOS.forward, Time.deltaTime * 10f);
			else{
				if(inWater){
					if(isSwimming || isDiving)
						cam.transform.forward = Vector3.Lerp(cam.transform.forward, transform.up, Time.deltaTime * 5f);
					else{
						if(onWaterSurface)
							this.transform.forward = new Vector3(relative.transform.forward.x, 0f, relative.transform.forward.z);
						else
							cam.transform.forward = cam.fpvPOS.forward;
					}
				}else{
					this.transform.forward = relative.transform.forward;
				}
			}
		}
	}

	void FixedUpdate (){
        //////////Movement on the ground
		if ((!masterBool && !inCoRout && !lowJump && !underWater && !currentAnim.inJumpToGrab && !isDiving && !diveDelay)
			|| (inWater && onWaterSurface && !swimDelay && !isSwimming && !currentAnim.inBreathe && !isDiving && !isDiving && !diveDelay )){
            if (direction != Vector3.zero && !cam.exitFPV && !currentAnim.inHardLand && !hardLanded) {
				if (!targetting && !isFocusing && !fPVing) {																									//This controls how the character turns around while not targetting/in first person
				    forwardAngle = Vector3.Angle(this.transform.forward,
                            (leftY * relative.transform.forward) + (leftX * relative.transform.right));
					if (Vector3.Magnitude(direction) >= 0.1f && !braking && !climbing) {
                        if (forwardAngle < 150f || (midAirMovement && !onGround) || currentAnim.crawlingBlend) {
                            if (Vector3.Magnitude(rb.velocity) <= 0.05f) {
								if (leftX != 0f || leftY != 0f) {
									this.transform.forward = (leftY * relative.transform.forward) + (leftX * relative.transform.right);
								}
                            }else {
                                if (!braking){
                                    if (onGround)
                                        tempTurnS = 7f;
                                    else
                                        tempTurnS = airMoveSensitivity;
                                    this.transform.forward = Vector3.Lerp(this.transform.forward,
                                            (leftY * relative.transform.forward) + (leftX * relative.transform.right),
										Time.fixedDeltaTime * tempTurnS);
                                }
                            }
                        }
                        if (forwardAngle >= 150f){
                            if (midAirMovement && !onGround){
                                if (leftX != 0f || leftY != 0f)
                                    this.transform.forward = (leftY * relative.transform.forward) + (leftX * relative.transform.right);
                            } else {
                                Vector3 crossy = Vector3.Cross(this.transform.forward,
                                (leftY * relative.transform.forward) + (leftX * relative.transform.right));
								if (!braking && onGround && !isCrouching && !crouchDelay)
                                {
                                    braking = true;
                                    rbtogo = rb.velocity;
                                    angleToRot = forwardAngle * (Mathf.Sign(crossy.y));
                                    anim.Play("Brake", 0);
                                    if (onGround)
                                        rb.velocity = Vector3.zero;
                                }
                            }
                        }
                    }
                    if (!braking) { 
				        if( Vector3.Magnitude(rb.velocity) > 3.5f && Vector3.Magnitude(new Vector3(gPI.LH,0f,gPI.LV)) < 0.7f && 
				           dirMag > Vector3.Magnitude(new Vector3(gPI.LH,0f,gPI.LV)) && !currentAnim.inBrake
				           && delayed && onGround){
                            if ((dirMag - Vector3.Magnitude(new Vector3(gPI.LH,0f,gPI.LV))) > 0.05f || dirMag < 0.12f){
						        delayed = false;
						        StartCoroutine(Delay(1));
						        canSidewaysJump = true;
						        StartCoroutine(Delay(13));
                                rb.velocity *= 0.5f;
					        }
				        }

				        if(dirMag == Vector3.Magnitude(new Vector3(gPI.LH,0f,gPI.LV)))
					        anim.ResetTrigger ("brake");
                        dirMag = Vector3.Magnitude(new Vector3(gPI.LH, 0f, gPI.LV));
                    }

                    if (braking) {
                        if (rotIterate < (28) && rotIterate > 10)
                            this.transform.RotateAround(this.transform.position, Vector3.up, (angleToRot - 11) / 16);
                        rb.velocity = Vector3.Lerp(rbtogo, (leftY * relative.transform.forward) + (leftX * relative.transform.right), rotIterate/27);
                        if(rotIterate >= 28){
                            braking = false;
                            rotIterate = 0;
                        }
                        rotIterate += 1;
                    }
				}
				if (targetting && !isFocusing) {																		//This limits the character to only move in one direction while targetting/in first person
					if (Mathf.Abs (leftX) >= Mathf.Abs (leftY)) {
						if (Mathf.Sign (leftX) < 0f)
							leftX = -1f;
						else
							leftX = 1f;
						direction = new Vector3 (leftX, 0f, 0f);
					} else {
						if (Mathf.Sign (leftY) < 0f)
							leftY = -1f;
						else
							leftY = 1f;
						direction = new Vector3 (0f, 0f, leftY);
					}
				}

				if (Mathf.Abs (Vector3.Magnitude (direction)) < 0.6f && onIncline || onIncline && !isRolling) {			//If there is little input while the character is on an incline, this will prevent odd movements
					addedSpeed = 0f;
				} else {
					addedSpeed = 1f;
				}
                
				if (onGround || !onGround && midAirMovement || !onGround && inWater && onWaterSurface) {
                    newDir = Vector3.Normalize(direction);
					newDir *= Vector3.Magnitude(direction);
					if(Vector3.Magnitude(direction) > accel)
						accel = Mathf.Lerp(accel, Vector3.Magnitude(new Vector3(newDir.x,0f,newDir.z)), Time.fixedDeltaTime * acceleration);
					else
						accel = Vector3.Magnitude(new Vector3(newDir.x,0f,newDir.z));

					if(gPI.holdLStick && sprintFunction)
						currentSpeed = maxSpeed + additionalSprintSpeed;
					else
						currentSpeed = maxSpeed;

					if(isCrouching)
						currentSpeed = crawlSpeed;

                    if (againstWall)
                        currentSpeed = maxSpeed / 3f;
                    
                    newDirMag = (Vector3.Magnitude(newDir) - 0.2f) * 1.25f;
                    //**Remove LERP functionality if you want more precise (less realistic) movement**//

                    if (!onGround && midAirMovement && leftX == 0f && leftY == 0f)
                        currentSpeed = 0f;

                    if (!lockDirection){
                        if (!fPVing && !targetting){
                            rb.velocity = Vector3.Lerp(rb.velocity,
                                                new Vector3(0f, rb.velocity.y * addedSpeed, 0f) +
                                                (this.transform.forward * newDirMag * currentSpeed), accel);
                        }else{
                            if (!isFocusing){
                                rb.velocity = Vector3.Lerp(rb.velocity,
                                                    new Vector3(0f, rb.velocity.y * addedSpeed, 0f) +
                                                    (((this.transform.forward * direction.z) + (this.transform.right * direction.x)) * currentSpeed), accel);
                            }else {
                                rb.velocity = Vector3.Lerp(rb.velocity,
                                                    new Vector3(0f, rb.velocity.y * addedSpeed, 0f) +
													(((relative.transform.forward * direction.z) + (relative.transform.right * direction.x)) * currentSpeed), accel);
                            }
                        }
                    }
                }
			} else {
                if (!onIncline && !startedSliding && jumpType != 2) {			//This keeps the character still when there's no input from the player
					if(!fPVing || fPVing && onGround){
						rb.velocity = new Vector3 (0f, rb.velocity.y, 0f);
						accel = 0f;
					}
				} else {
					if (onIncline && !startedSliding){
						rb.velocity = zeroVelocity;
					}
				}
			}
		}
		if (currentAnim.inBreathe)
			rb.velocity *= 0.95f;

//////////Jumping movement
		if (jumping) {
            if (braking) {
                braking = false;
                rotIterate = 0;
                anim.ResetTrigger("brake");
            }
			if (!startedJump) {																					//When the jump is just started this will cancel out any rolling or anything that will oddly affect the speed
				if (isRolling || currentAnim.inRoll){
					isRolling = false;
					rb.velocity = new Vector3(rb.velocity.x/2f, rb.velocity.y, rb.velocity.z/2f);
				}
				if(jumpNo > 1)
					rb.velocity = (((leftY * relative.transform.forward) + (leftX * relative.transform.right)) * Vector3.Magnitude(new Vector3(rb.velocity.x, 0f, rb.velocity.z)) * 0.5f);
				rb.AddForce ((Vector3.up * jumpImpulse * rb.mass), ForceMode.Impulse);          //Vertical speed is added as an impulse to the characters ground speed 
                if (jumpType == 1)
					rb.AddForce (((leftY * relative.transform.forward) + (leftX * relative.transform.right)) * rb.mass * 5f, ForceMode.Impulse);
                if (jumpType == 2){
                    if(leftX != 0f || leftY != 0f)
                        this.transform.forward = (leftY * relative.transform.forward) + (leftX * relative.transform.right);
                    rb.AddForce(new Vector3(-this.transform.forward.x, 0f, -this.transform.forward.z) * rb.mass * 3f, ForceMode.Impulse);
                }
				startedJump = true;
			}
			if (rb.velocity.y < 0f && !currentAnim.inFall && !grabWhileFalling && delayed) {				//At the peak of the characters jump, the character will begin 'falling'
				anim.ResetTrigger ("jumpNow");
				anim.ResetTrigger ("doubleJumpNow");
				anim.SetTrigger ("falling");
				jumping = false;
			}
		}
		if (prolongJumpDelay) {
			if(gPI.holdLeftB)
				rb.AddForce(Vector3.up * rb.mass * (jumpImpulse/2f));
			else
				prolongJumpDelay = false;
		}


//////////Movement while not on the ground
        if (!midAirMovement && !lockDirection){
            if (!onGround && !grabBeneath && !masterBool && !jumping && !inCoRout && !lowJump && !onWaterSurface && !underWater && !slidingDownSlope || jumping && startedJump){
                rb.AddForce((leftY * relative.transform.forward * airMomentum * rb.mass), ForceMode.Force);     //Movements is applied by force while the character is not on the ground
                rb.AddForce((leftX * relative.transform.right * airMomentum * rb.mass), ForceMode.Force);
            }
        }

//////////Swimming movement
        if(underWater && !onWaterSurface && !isDiving && !isSwimming){
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.up * 4f, Time.fixedDeltaTime * 5f);
        }
        if (isDiving)																	//Diving movement
			rb.AddForce (-Vector3.up * rb.mass * divingForce, ForceMode.Force);
		if (isSwimming) {																		//Swimming rotations
			if(rotateOnX || rotateOnY){
				if(rotateOnX)
					this.transform.RotateAround(this.transform.position - (this.transform.up*cHeight*.5f), this.transform.right, Time.fixedDeltaTime * swimmingRotateSpeed * yInvrt);
				if(rotateOnY)
					this.transform.RotateAround(this.transform.position, Vector3.up, Time.fixedDeltaTime * swimmingRotateSpeed * xInvrt);
			}
			if(Mathf.Abs(Vector3.Magnitude(rb.velocity)) > 1f){
				rb.AddForce(-rb.velocity * rb.mass * 2f, ForceMode.Force);
			}else{
				if(!slowSwim){
					rb.velocity = zeroVelocity;
				}
			}
			if(swimImpulse){																		//fast swimming movement
				rb.AddForce(this.transform.up * rb.mass * swimmingForce, ForceMode.Impulse);
				swimImpulse = false;
			}
			if(slowSwim)																			//slow swimming movement
				rb.AddForce(this.transform.up * rb.mass * swimSpeed, ForceMode.Force);
		}

//////////Climbing movement
		if (onLadder && climbPosAdjusted) {
			if(climbUp || inUpAnim)
				rb.velocity = new Vector3(0f, verticalClimbSpeed, 0f);
			if(climbDown || inDownAnim)
				rb.velocity = new Vector3(0f, -verticalClimbSpeed, 0f);
			if(!climbUp && !climbDown && !inUpAnim && !inDownAnim)
				rb.velocity = zeroVelocity;
		}
		if (onFence && climbPosAdjusted) {
			if(climbUp || inUpAnim)
				rb.velocity = new Vector3(0f, verticalClimbSpeed, 0f);
			if(climbDown || inDownAnim)
				rb.velocity = new Vector3(0f, -verticalClimbSpeed, 0f);
			if(climbRight || inRightAnim)
				rb.velocity = this.transform.right * horizontalClimbSpeed;
			if(climbLeft || inLeftAnim)
				rb.velocity = - this.transform.right * horizontalClimbSpeed;
			if(!climbUp && !climbDown && !climbRight && !climbLeft && !inUpAnim && !inDownAnim && !inLeftAnim && !inRightAnim)
				rb.velocity = zeroVelocity;
		}
		
//////////Rolling movement
		if (isRolling) {
            rollY = rb.velocity.y;
            if (onlyRollInTargetMode && targetting && (leftX !=0f || leftY != 0f))
                this.transform.forward = (leftY * relative.transform.forward) + (leftX * relative.transform.right);
            if (Vector3.Magnitude(direction) != 0f) {
                rb.velocity = this.transform.forward * rb.mass * ((Vector3.Magnitude (direction) + 1f) * rollSpeed);
                rb.velocity = new Vector3(rb.velocity.x, rollY, rb.velocity.z);
            }else{
                rb.velocity = this.transform.forward * rb.mass * (rollSpeed * 1.875f);                 //If there is no input, the character will continue rolling in the direction that they're facing
                rb.velocity = new Vector3(rb.velocity.x, rollY, rb.velocity.z);
            }
        }

		
//////////Knock back when rolling
		if (rollKnockBack)
			rb.AddForce (-this.transform.forward * rb.mass * 0.4f, ForceMode.Impulse);	//This will add an impulse in the opposite direction that the character is moving in


//////////Sliding down a Slope
		if (slidingDownSlope) {
			Physics.Raycast(this.transform.position, -Vector3.up, out rayHitC2, cHeight, whatIsSlope);
			slideRelative.transform.forward = rayHitC2.normal;
			slideRelative.transform.RotateAround(slideRelative.transform.position, slideRelative.transform.right, 90f);
			slidingDir = slideRelative.transform.forward;

			if (!startedSliding) {
				StartCoroutine(Delay(17));
				if(rb.velocity.y < -1f)
					startedSliding = true;
				rb.AddForce(slidingDir * rb.mass * maxSpeed, ForceMode.Force);
			}
			if(startedSliding){
			//	rb.AddForce(-slidingDir * rb.mass * maxSpeed * 0.1f, ForceMode.Force);
				if(Vector3.Magnitude(direction) > 0.1f){
					int rotationSide = 0;
					float deltAngle = Mathf.DeltaAngle (cam.transform.localEulerAngles.y, slideRelative.transform.localEulerAngles.y);
					rotationSide = 1;
					if (deltAngle > 135f || deltAngle <= -135f)
						rotationSide = 3;
					if (deltAngle > 45f && deltAngle <= 135f)
						rotationSide = 4;
					if (deltAngle < -45f && deltAngle > -135f)
						rotationSide = 2;
					if(!fPVing){
						if (deltAngle > 45f && deltAngle <= 135f)
							rotationSide =2;
						if (deltAngle < -45f && deltAngle > -135f)
							rotationSide = 4;
					}

					if(leftY < -0.4f && rotationSide == 1 || leftY > 0.4f && rotationSide == 3 || 
					   leftX > 0.4f && rotationSide == 4 || leftX < -0.4f && rotationSide == 2)
						rb.AddForce(-slidingDir * rb.mass * maxSpeed * 0.5f, ForceMode.Force);
					if(leftY < -0.4f && rotationSide == 3 || leftY > 0.4f && rotationSide == 1 || 
					   leftX > 0.4f && rotationSide == 2 || leftX < -0.4f && rotationSide == 4)
						rb.AddForce(slidingDir * rb.mass * maxSpeed * 0.5f, ForceMode.Force);
					if(leftY < -0.4f && rotationSide == 2 || leftY > 0.4f && rotationSide == 4 || 
					   leftX > 0.4f && rotationSide == 1 || leftX < -0.4f && rotationSide == 3)
						rb.AddForce(this.transform.right * 3f * rb.mass, ForceMode.Force);
					if(leftY < -0.4f && rotationSide == 4 || leftY > 0.4f && rotationSide == 2 || 
					   leftX > 0.4f && rotationSide == 3 || leftX < -0.4f && rotationSide == 1)
						rb.AddForce(-this.transform.right * 3f * rb.mass, ForceMode.Force);
				}
			}
		}
		
//////////Movement while target-jumping (side hops and back flips)
		if (isTargetJumping) {
			if (rightJump || leftJump)
				rb.AddForce (((Mathf.Sign (leftX) * this.transform.right * 0.5f) + Vector3.up) * rb.mass * 0.35f, ForceMode.Impulse);		//The sidehops add a single impulse in either left or right and slightly upwards
			if (backJump)
				rb.AddForce ((-this.transform.forward + (Vector3.up * 1.5f)) * rb.mass * 0.3f, ForceMode.Impulse);							//The backflip adds a single impulse in the opposite direction that the character is facing and slightly upwards
		}
		
//////////Grabbing beneath movement
		if (grabBeneath && !currentAnim.grabbingBlend) {											//The characters collider is frozen in place while the animation plays
			rb.velocity = zeroVelocity;
			gravityDirection = Vector3.zero;
			if (!inCoRout && !currentAnim.inGrabBeneath && !grabBeneathAdjusting) {
				anim.SetBool ("gBeneath", true);
                StopCoroutine("LedgeHandler");
				StartCoroutine ("LedgeHandler", 4);													//When the co-routine finishes. the characters collider will move to the models position
			}
		}
		
//////////Movement while climbing ledges (while onLedge)
		if (onLedge && onLedgeAdjusted) {
			anim.ResetTrigger ("falling");
			int ledgeValue = 0;
			if (jumpToGrab) {
				if (jumpToGrab) {
					rb.velocity = Vector3.up * 3f;										//The jump to grab works by just moving up the character until it's at a position where it can grab the ledge
					if (this.transform.position.y > (midPointLedge.y - (cHeight*0.48f))) {
                        StopCoroutine("LedgeHandler");
						StartCoroutine("LedgeHandler", 0);
						onLedge = false;
						onLedgeAdjusted = false;
					}
				}
			}
			if (midLedgeGrab || highLedgeGrab) {														//Like with 'grabBeneath', the collider is frozen in place and the animation plays
				rb.velocity = zeroVelocity;
				gravityDirection = Vector3.zero;
				if (!inCoRout) {
					if (midLedgeGrab)
						ledgeValue = 1;
					if (highLedgeGrab)
						ledgeValue = 2;
                    StopCoroutine("LedgeHandler");
                    StartCoroutine ("LedgeHandler", ledgeValue);										//The collider is positioned at the models position at the end of the animation
				}
			}
		}
		
//////////Sets the camera to look at the armature during climbing/grab-beneath animations
		if (onLedge || pullUp || midLedgeGrab || grabBeneath) {
			cam.targetTransform = rootBoneGameObject.transform.Find ("LdgFollowMe").transform;
		} else {
			if (cam != null && this.transform.Find ("FollowMe") != null) {
				if (cam.targetTransform != this.transform.Find ("FollowMe").transform)
					cam.targetTransform = this.transform.Find ("FollowMe").transform;
			}
		}
		
//////////Grabbing while falling movement
		if (grabWhileFalling) {
			if (rb.isKinematic)
				rb.isKinematic = false;
			anim.SetTrigger ("grabbing");
			if (rb.velocity.x != 0f || rb.velocity.z != 0f)
				rb.velocity = new Vector3 (0f, rb.velocity.y, 0f);			//The character moves down until it's at the proper position for grabbing the ledge
			if (this.transform.position.y <= (midPointLedge.y - (cHeight * 0.45f))) {
				rb.velocity = zeroVelocity;												//The character is stopped in place and is put into its 'grabbing' state
				grabBeneath = false;
				grabbing = true;
				grabWhileFalling = false;
			} else {
				this.transform.position = Vector3.Lerp(this.transform.position,
				                                       new Vector3 (this.transform.position.x, midPointLedge.y - (cHeight * 0.45f), this.transform.position.z),
				                                       Time.fixedDeltaTime * 5f);
			}
		}
		
//////////Movement while grabbing
		if (grabbing && grabAdjusted) {
			if (grabBeneath) 
				grabBeneath = false;
			bool areSameX = true;
			float deltAngle = Mathf.DeltaAngle (this.transform.localEulerAngles.y, relative.transform.localEulerAngles.y);			//Checks which way the character is facing relative to the camera
			if (deltAngle > 90f || deltAngle < -90f)
				areSameX = false;
			if (grabCanMoveLeft && leftX < -0.6f && areSameX || 
				grabCanMoveLeft && leftX >= 0.6f && !areSameX) {
				rb.velocity = -this.transform.right * hangingMoveSpeed;																//Moves left if holding left (or right) and camera is in front of (or behind) the character
				anim.SetFloat ("grabbingDir", leftX);
			}
			if (!grabCanMoveLeft && leftX < -0.6f && areSameX || 
				!grabCanMoveLeft && leftX >= 0.6f && !areSameX) {
				anim.SetFloat ("grabbingDir", 0f);																					//Doesn't move to the left if cannot move left
				rb.velocity = zeroVelocity;
			}
			if (grabCanMoveRight && leftX > 0.6f && areSameX || 
				grabCanMoveLeft && leftX <= -0.6f && !areSameX) {
				rb.velocity = this.transform.right * hangingMoveSpeed;																//Moves right if holding right (or left) and camera is in front of (or behind) the character
				anim.SetFloat ("grabbingDir", leftX);
			}
			if (!grabCanMoveRight && leftX > 0.6f && areSameX || 
				!grabCanMoveLeft && leftX <= -0.6f && !areSameX) {
				rb.velocity = zeroVelocity;
				anim.SetFloat ("grabbingDir", 0f);																					//Doesn't move to the left if cannot move left
			}
			if (leftX > -0.6f && leftX < 0.6f) {																					//Acts as a deadzone
				rb.velocity = zeroVelocity;
				anim.SetFloat ("grabbingDir", 0f);
			}
			if (pullUp) {
				rb.velocity = zeroVelocity;											//Like with 'grabBeneath', the collider is frozen in place and the animation plays
				gravityDirection = Vector3.zero;
				if (!inCoRout) {
                    StopCoroutine("LedgeHandler");
					StartCoroutine ("LedgeHandler", 3);											//The collider is positioned at the models position at the end of the animation
				}
			}

		}
		if (fallOff) {																			//Makes adjustments if player decides to fall off from grabbing
			grabbing = false;
			grabAdjusted = false;
			grabBeneath = false;
			if (currentAnim.grabbingBlend) {
				anim.ResetTrigger ("grabbing");
				anim.SetTrigger ("falling");
			}
			fallOff = false;
		}
		if (!grabbing && grabAdjusted) {														//Sets 'grabAdjusted' to false just in case it is true and 'grabbing' isn't
			grabAdjusted = false;		
		}
		if (onGround && grabBeneath && !inCoRout && !grabBeneathAdjusting)												//Prevents character from repeating 'grabBeneath' just after finishing the animation
			grabBeneath = false;
		
//////////Gravity Control
		if (!onGround || !delayed || targJumpDelay)
			gravityDirection = new Vector3 (0f, Physics.gravity.y, 0f);													//Gravity always points downwards when not on the ground

		if (Physics.GetIgnoreLayerCollision (characterCollisionLayer, slopesCollisionLayer) || underWater || onWaterSurface || climbing || crouchDelay || diveDelay || isDiving)
			gravityDirection = Vector3.zero;																			//If that layer collision is being ignored during an animation then we don't want anything moving the collider

		if (startedSliding || onSteepGround)
			gravityDirection = new Vector3 (0f, Physics.gravity.y, 0f);
        
		if (!onLedge && !grabbing && rb.velocity.y > -20f) {
            if (pushAway)
				rb.AddForce (-this.transform.forward * 0.5f * rb.mass, ForceMode.Impulse);		//Adds an impulse, sending the character in the opposite direction to prevent getting stuck on corners
			rb.AddForce (gravityDirection * rb.mass * gravityIntesnity, ForceMode.Force);						//GRAVITY IS APPLIED HERE
		}
		
//////////Misc Adjustments
		if(crouchDelay && rb.velocity.y > 0f)
			rb.velocity = zeroVelocity;
		if (jumpToGrab)
			rb.velocity = new Vector3 (0f, rb.velocity.y, 0f);
		if (liftDelay || liftThrowDelay && onGround)
			rb.velocity = zeroVelocity;
        if ((Vector3.Magnitude(direction) < 1f || !onGround && !onWaterSurface) && holdTimer != 0f)
            holdTimer = 0f;

        if (rb.velocity.y <= -20f)
            rb.velocity = new Vector3(rb.velocity.x, -20f, rb.velocity.z);

		if(slowFall)
			rb.velocity = new Vector3(rb.velocity.x, -2f, rb.velocity.z);

		if (!inWater && this.transform.localEulerAngles.x != 0f && !isDiving && !diveDelay) {
			this.transform.localEulerAngles = Vector3.Lerp(this.transform.localEulerAngles, new Vector3(0f, this.transform.localEulerAngles.y, 0f), Time.fixedDeltaTime * 10f);
			if (this.transform.localEulerAngles.x < 0.1f)
				this.transform.localEulerAngles = new Vector3 (0f, this.transform.localEulerAngles.y, 0f);
		}

////////Levitate with Select
        if (levitateWithSelect && gPI.holdSelect){
            onGround = false;
            rb.velocity = new Vector3(rb.velocity.x, 4f, rb.velocity.z);
		}
    }

/// <summary>
/// /These following OnCollision functions deal with what sort of ground that the character is on and how it changes the gravity direction or behaviour of the characters movements.
	/// OnCollisionEnter is when the character lands on a collider and how it deals with it
	/// OnCollisionStay is when the character is moving on the ground or on a slope and how it deals with different situations (and if the character is against a climbable object)
	/// OnCollisionExit is when the character leaves the ground and checks if it was just a small bump on the ground
/// </summary>
	private void OnCollisionEnter (Collision collision)
	{
        if (delayed && !onGround && !underWater && !jumping && !grabBeneath && !grabbing && !onLedge && ((rb.velocity.y <= 0.01f && !onWaterSurface) || onWaterSurface) && !hasLandedThisFrame) {
            if (currentAnim.inFall || currentAnim.inAJumpAnim || currentAnim.inSliding || currentAnim.inATargJumpAnim || currentAnim.inLowJumpLedge || onWaterSurface) {
                ContactPoint cp = new ContactPoint();
                cp = collision.contacts[0];
                Physics.Linecast(cp.point + cp.normal, cp.point - cp.normal, out rayCollide1, whatIsGround);
				if (rayCollide1.normal.y >= walkableSlopeTolerence && (whatIsGround & 1 << cp.otherCollider.gameObject.layer) != 0) {                       //If the character touches Ground while not being onGround, they will 'land' and be on ground
					Land();
                }
            }
        }
	}

	private void OnCollisionStay (Collision collision){
		if (!currentAnim.inGrabBeneath && !grabbing && !onLedge) {
			ContactPoint pt = new ContactPoint ();
			int iterate = 0;
			bool foundOne = false;
			bool onASlope = false;
			bool nxtToBox = false;
			while (iterate < collision.contacts.Length && !foundOne) {
				pt = collision.contacts [iterate];
				Physics.Linecast (pt.point + pt.normal, pt.point - pt.normal, out rayCollide2, whatIsGround);
				if ((rayCollide2.normal.y > walkableSlopeTolerence) && ((whatIsGround & 1 << pt.otherCollider.gameObject.layer) != 0) &&
					(Vector3.Distance (this.transform.position + rb.centerOfMass, rayCollide2.point) < cRad + skinWidth) && !foundOne
					&& (Vector3.Angle(pt.normal, Vector3.up) < (90f*walkableSlopeTolerence))) {                     //If the character is moving on to Ground with a different gradient...
                    foundOne = true;
                    lockDirection = false;
                    onSteepGround = false;
                }
				if (pt.normal.y > 0.2f && (whatIsSlope & 1 << pt.otherCollider.gameObject.layer) != 0)												//If the character is about to move onto a Slope and the slope isn't a vertical wall (basically if the gradient is less than 0.2f...
					onASlope = true;
				if (onASlope && foundOne)																											//If the character is on both, they will only act like they're on Ground
					onASlope = false;

				if(pt.otherCollider.tag == "Box" && Physics.Linecast(this.transform.position, pt.otherCollider.transform.position, out rayCollide3, whatIsACollision)){
					if(rayCollide3.normal.y < 1f){
						nxtToBox = true;
						currentBox = pt.otherCollider.transform.GetComponent<BoxCollider>();
                        boxRB = pt.otherCollider.GetComponent<Rigidbody>();
						againstBox = true;
					}
				}
				if(pt.otherCollider.gameObject.tag == "Ladder" || pt.otherCollider.gameObject.tag == "Fence"){
					againstClimbable = true;
					currentClimbingObject = pt.otherCollider;
				}else
					againstClimbable = false;
				iterate += 1;
			}
            if (nxtToBox)
				againstBox = true;
			else
				againstBox = false;
			int newIterate = 0;
			ContactPoint hwpt = new ContactPoint ();
			while (!huggingWall && newIterate < collision.contacts.Length) {
				if (newIterate != iterate) {																											//Checks if character is hugging a wall and isn't the ground the character is on already
					hwpt = collision.contacts [newIterate];
					if ((whatIsSlope & 1 << hwpt.otherCollider.gameObject.layer) != 0 && hwpt.normal.y <= walkableSlopeTolerence ||
						(whatIsGround & 1 << hwpt.otherCollider.gameObject.layer) != 0 && hwpt.normal.y <= walkableSlopeTolerence) {
						huggingWall = true;
					} else
						huggingWall = false;
				} else
					huggingWall = false;
				if (!huggingWall)
					newIterate += 1;
			}
			if(!huggingWall && againstClimbable)																						//A fail-safe
				againstClimbable = false;
			if(!againstClimbable){
				if(!climbing)
					currentClimbingObject = null;
			}
			if (foundOne) {                                                                                                             //If they're on Ground, they will be made to 'land' just in case
				if (!onGround && delayed && !underWater && !hasLandedThisFrame){
					Land();
				}
				if(slideDelay && rb.velocity.y <= 0f){
					if (slidingDownSlope || anim.GetBool("slidingDown")) {
						slidingDownSlope = false;
						StartCoroutine(Delay(16));
					}
				}
				InclineGravity ((pt.point - (this.transform.position + rb.centerOfMass)).normalized);							//The direction from the point of contact to the colliders center of mass is passed through to be used as the gravitys direction
			} else {
				if (onASlope) {
					if(onGround)
						onGround = false;
					if(!backOnSlope)
						backOnSlope = true;
					if(!slidingDownSlope) {
						if (currentAnim.inFall && !hasLandedThisFrame)
							Land();
						slidingDownSlope = true;
                        if (delayed)
                            jumpNo = 0;
					}
				} else{
                    if (huggingWall && collision.contacts.Length == 1){
                        if (rb.velocity.y >= 2f && Vector3.Magnitude(direction) == 1f || rb.velocity.y >= 0.5f && Vector3.Magnitude(direction) < 1f){
                            onSteepGround = true;
                            if (Vector3.Angle(-this.transform.forward, new Vector3(collision.contacts[0].normal.x, 0f, collision.contacts[0].normal.z)) <= 90f)
                                lockDirection = true;
                        }
                    }
                    if (!huggingWall || !Physics.Raycast(this.transform.position, -Vector3.up, 0.6f * cHeight, whatIsGround))
                        gravityDirection = new Vector3(0f, Physics.gravity.y, 0f);
                    if (rayCollide2.normal.y < walkableSlopeTolerence && rayCollide2.normal.y > 0f){
						if(rb.velocity.y > 1f)
							rb.velocity = Vector3.Lerp(rb.velocity, zeroVelocity, Time.fixedDeltaTime *2f);
					}
                }
			}
		}
	}

	private void OnCollisionExit (Collision collision){
		if (Physics.Linecast (this.transform.position, this.transform.position - (Vector3.up * cHeight * 0.65f), out rayCollide3, whatIsACollision) && delayed && !onLedge && !inWater) {			//This checks if the character has gone off ground but hasn't jumped or anything like that...
			if (rayCollide3.normal.y > walkableSlopeTolerence) {
				if (rb.velocity.y > 0.3f && !jumping && !targJumpDelay && !onLedge && !grabbing) {									//If it was just a bump in the ground that pushed the character off for a split second, this will push...
					rb.velocity = new Vector3 (rb.velocity.x, -rb.velocity.y, rb.velocity.z);										//...the character back down to the ground to counter it
					gravityDirection = new Vector3 (0f, Physics.gravity.y, 0f);
				}
			}
		} else {
			onGround = false;
            onSteepGround = false;
            if (lockDirection){
                StopCoroutine("RegainControl");
                StartCoroutine("RegainControl");
            }
            if (slidingDownSlope && !startedDelay)
				StartCoroutine(Delay(18));
			if (huggingWall)
				huggingWall = false;
			if(jumpNo == 0)
				jumpNo = 1;
		}
	}

/// <summary>
/// /OnTrigger functions are used to work water boxes
/// </summary>
    void OnTriggerEnter(Collider other){
        if (interactWithWater && other.gameObject.layer == waterCollisionLayer && !onLedge && delayed)
            EnterWater(other);
    }

    void OnTriggerStay(Collider other){
        if (!inWater && interactWithWater && other.gameObject.layer == waterCollisionLayer && !onLedge && delayed)
            EnterWater(other);
    }

    void OnTriggerExit(Collider other) {
        if(interactWithWater && other.gameObject.layer == waterCollisionLayer && !swimDelay){
            ExitWater(other, false);
        }
    }

/// <summary>
/// The function used for when the character enters a new water box
/// </summary>
    void EnterWater (Collider other){
        GameObject prevWB = currentWaterBox;
        if (waterBoxes.Count != 0) {
            int watIt = 0;
            bool cantAdd = false;
            while (watIt < waterBoxes.Count && !cantAdd){
                if (waterBoxes[watIt] == other.gameObject)
                    cantAdd = true;
                watIt++;
            }
            if (!cantAdd)
                waterBoxes.Add(other.gameObject);
        }else{
            waterBoxes.Add(other.gameObject);
            inWater = true;
            isSwimming = false;
            isDiving = false;
            underWater = false;
            onWaterSurface = false;
        }
        jumpNo = 0;
        currentWaterBox = other.gameObject;
        RaycastHit watercheck = new RaycastHit();
        Physics.Linecast(this.transform.position, other.bounds.center, out watercheck, whatIsWater);
        if(watercheck.normal.y != 1f){
            if (canSwim) {
                underWater = true;
                isSwimming = true;
                onWaterSurface = false;
                rotateOnX = false;
                rotateOnY = false;
                swimXRot = this.transform.right;
                anim.SetTrigger("startSwimming");
            }else{
                onWaterSurface = false;
                underWater = true;
                anim.SetBool("diving", true);
            }
        }else{
            if(waterBoxes.Count > 1 && underWater && !onWaterSurface){
                currentWaterBox = prevWB;
            }
        }
	}

/// <summary>
/// The function used for when the character exits a water box
/// </summary>
    void ExitWater(Collider other, bool removeAll){
        if (waterBoxes.Count != 0) {
            int watIt = 0;
            bool found = false;
            while (watIt < waterBoxes.Count && !found){
                if(removeAll)
                    waterBoxes.Remove(waterBoxes[watIt]);
                else { 
                    if (waterBoxes[watIt] == other.gameObject) {
                        found = true;
                        waterBoxes.Remove(other.gameObject);
                    }
                }
                watIt++;
            }
            if(waterBoxes.Count == 0){
                inWater = false;
                isSwimming = false;
                isDiving = false;
                underWater = false;
                onWaterSurface = false;
                if(!jumping)
                    rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.y);
            }
            if (waterBoxes.Count != 0)
                currentWaterBox = waterBoxes[waterBoxes.Count - 1];
            else {
                if(!removeAll){
                    RaycastHit watercheck = new RaycastHit();
                    Physics.Linecast(this.transform.position, other.bounds.center, out watercheck, whatIsWater);
                    if(watercheck.normal.y != 1f){
                        onGround = false;
                        anim.SetTrigger("falling");
                        jumpNo = 1;
                        if (!jumping)
                            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.y);
						slowFall = true;
						StartCoroutine ("PreventFastFall");
                    }
                }
                currentWaterBox = null;
            }
        }
    }

/// <summary>
/// /This method uses the normal that is passed through it as the direction of gravity to act on the player
/// </summary>
	private void InclineGravity (Vector3 thisNormal)
	{
		gravityFactor = 1f + ((1f - thisNormal.y) / 1.5f);											//The steeper the ground is, the larger the gravity factor will be
		if (isRolling)
			gravityFactor += 2f;																//Since rolling is faster than walking, the gravity factor is increased to hold the roll on the ground
		if (thisNormal == Vector3.zero)															//If there's no normal, or it's equal to zero, then gravity points downwards
			thisNormal = Vector3.up;
		gravityDirection = -thisNormal * Physics.gravity.y * gravityFactor;                     //!!the gravity direction is set as the normal of the ground times the factor times the y value of the gravity vector in the project settings
    }

/// <summary>
/// This method is run each time the character reaches the top of a ladder/fence collider and checks if there's more above
/// </summary>
	private void ReachedTop (string climbableObjectTag){
		bool reachedTop = false;
		if (Physics.Raycast (this.transform.position + (Vector3.up * cHeight * 0.56f), this.transform.forward, out rayDump, cRad * 4f, whatIsGround)) {
			if (rayDump.collider.tag != climbableObjectTag) {
				reachedTop = true;
			} else {
				if (Vector3.Distance (this.transform.position + (Vector3.up * cHeight * 0.56f), rayDump.point) > cRad * 2f)
					reachedTop = true;
			}
		} else
			reachedTop = true;
		if (reachedTop) {
			if (Physics.Raycast (this.transform.position + (Vector3.up * cHeight * 0.6f) + (this.transform.right * cRad * 0.05f), this.transform.forward,
				   out rayDump3, cRad * 4f, whatIsGround) ||
			   Physics.Raycast (this.transform.position + (Vector3.up * cHeight * 0.6f) - (this.transform.right * cRad * 0.05f), this.transform.forward,
				   out rayDump3, cRad * 4f, whatIsGround)) {
				if (rayDump3.collider.tag == climbableObjectTag) {
					reachedTop = false;
				}
			} else
				climbUp = false;
			if(Physics.Raycast(this.transform.position, Vector3.up, cHeight*0.65f, whatIsACollision)){
				climbUp = false;
				canClimbUp = false;
				rb.velocity = zeroVelocity;
				reachedTop = false;
			}
		}
		if (reachedTop && !climbUp) {
            Vector3 checkPos = rayD.position;
			bool madeIt = false;
			if (Physics.Linecast (checkPos, checkPos - (Vector3.up * 3.35f), out rayDump3, whatIsGround)) {
				if (rayDump3.collider.tag != "Ladder" && rayDump3.collider.tag != "Fence") {
					madeIt = true;
					midPointLedge = rayDump3.point;
					onLedge = true;
					highLedgeGrab = true;
					Physics.IgnoreLayerCollision (characterCollisionLayer, groundCollisionLayer, true);
					Physics.IgnoreLayerCollision (characterCollisionLayer, slopesCollisionLayer, true);
					distCalc = Vector3.Distance (checkPos, rayDump3.point);																										//Checks distance from rayD to the ledge (point of contact); the larger the distance, the smaller the ledge
					this.transform.position += new Vector3 (0f, (cHeight*0.9f - distCalc) - (cHeight*0.15f), 0f);
					anim.SetBool ("pullUp", true);
					onLedgeAdjusted = true;
                    rb.isKinematic = true;
				}
			} else {
				if (Physics.Linecast (checkPos + (this.transform.forward * cRad * 0.1f), checkPos + (this.transform.forward * cRad * 0.1f) - (Vector3.up * 3.35f), 
				                     out rayDump3, whatIsGround)) {
					if (rayDump3.collider.tag != "Ladder" && rayDump3.collider.tag != "Fence") {
						madeIt = true;
						midPointLedge = rayDump3.point;
						onLedge = true;
						highLedgeGrab = true;
						Physics.IgnoreLayerCollision (characterCollisionLayer, groundCollisionLayer, true);
						Physics.IgnoreLayerCollision (characterCollisionLayer, slopesCollisionLayer, true);
						distCalc = Vector3.Distance (checkPos + (this.transform.forward * cRad * 0.1f), rayDump3.point);																										//Checks distance from rayD to the ledge (point of contact); the larger the distance, the smaller the ledge
						this.transform.position += new Vector3 (0f, (cHeight*0.9f - distCalc) - (cHeight*0.15f), 0f);
						anim.SetBool ("pullUp", true);
						onLedgeAdjusted = true;
                        rb.isKinematic = true;
                    }
				}
			}
			if (!madeIt) {
				canClimbUp = false;
				climbUp = false;
				rb.velocity = zeroVelocity;
				anim.SetTrigger ("onClimb");
			} else
				climbing = false;
		}
        else
			canClimbUp = true;
	}

/// <summary>
/// Checks the object the character is climbing for changes in gradients
/// </summary>
	private void CheckClimbableObject(){
		if(!inUpAnim && !inDownAnim){
			float checkX = leftX;
			if(inLeftAnim)
				checkX = -1f;
			if(inRightAnim)
				checkX = 1f;
			bool foundSide = false;
			bool sharpCorner = false;
			RaycastHit rayhit = new RaycastHit();
			RaycastHit rayhit2 = new RaycastHit();
			if(Physics.Linecast(this.transform.position - (this.transform.forward*cRad*2f),
			                    this.transform.position + (this.transform.right*cRad*Mathf.Sign(checkX)*2f), out rayhit, whatIsGround))
				foundSide = true;
			if(Physics.Linecast(this.transform.position + (this.transform.right*cRad*Mathf.Sign(checkX)*1.5f),
			                    this.transform.position + (this.transform.forward*cRad*2f), out rayhit2, whatIsGround) && !foundSide){
				foundSide = true;
				sharpCorner = true;
			}
			RaycastHit useRay = rayhit;
            if(sharpCorner)
				useRay = rayhit2;
			if(foundSide && Physics.Raycast(this.transform.position, this.transform.forward, out rayDump2, cRad*3f, whatIsGround)){
				if(useRay.collider.tag == currentClimbingObject.tag &&
				   rayDump2.collider.tag == currentClimbingObject.tag && useRay.normal.y == 0f){
					if(checkX < 0f){
                        if (rayhit2.normal != -this.transform.forward)
                            this.transform.position -= this.transform.right * cRad * 0.02f;
						climbRight = false;
						climbLeft = true;
						climbUp = false;
						climbDown = false;
					}else {
                        if (rayhit2.normal != -this.transform.forward)
                            this.transform.position += this.transform.right * cRad * 0.02f;
						climbRight = true;
						climbLeft = false;
						climbUp = false;
						climbDown = false;
					}
					if(Vector3.Distance(this.transform.position +
					                    (this.transform.right*cRad*Mathf.Sign(checkX)), useRay.point) <= cRad * 7f){
						if(useRay.normal != rayDump2.normal){
							canFall = false;
							this.transform.forward = Vector3.Lerp (this.transform.forward,-useRay.normal, Time.deltaTime*10f);
							this.transform.position = Vector3.Lerp (this.transform.position, useRay.point + (useRay.normal*cRad), Time.deltaTime*5f);
						}else{
							canFall = true;
							currentClimbingObject = useRay.collider;
							if(this.transform.forward != - currentClimbingObject.transform.forward)
								this.transform.forward = Vector3.Lerp (this.transform.forward, -useRay.normal, Time.deltaTime*10f);
							if(Physics.Raycast(this.transform.position, this.transform.forward, out rayDump2, cRad*2f, whatIsACollision)){
								if(Vector3.Distance(this.transform.position, rayDump2.point) > cRad*1.05f && rayDump2.normal == -this.transform.forward){
									this.transform.position = Vector3.Lerp(this.transform.position, this.transform.position + (this.transform.forward*cRad*0.05f), Time.deltaTime*10f);
								}
							}
						}
					}
				}
			}else{
				if(leftX < 0.1f)
					anim.SetTrigger ("onClimb");
				if(leftX > 0.1f)
					anim.SetTrigger ("onClimb");
				climbLeft = false;
				climbRight = false;
			}
		}
	}
    private void ExitPush() {
        isPushing = false;
        anim.SetBool("newboxPush", false);
    }

/// <summary>
/// /This method compiles a dictionary of all the targets in the scene that are within a certain distance
/// </summary>
	private void CountTargets (){
		targets = GameObject.FindGameObjectsWithTag ("Target");
		float disty = 0f;
		foreach (GameObject gOb in targets) {
			if (!Physics.Linecast (this.transform.position, gOb.transform.position, out rayHitE2, whatIsACollision) &&
				Mathf.Abs (Vector3.Angle (this.transform.forward, gOb.transform.position - this.transform.position)) < 90f &&
				Mathf.Abs (Vector3.Angle (this.transform.up, gOb.transform.position - this.transform.position)) > 15f) {
				disty = Vector3.Distance (gOb.transform.position, this.transform.position);
				if (disty < targetFocusRange)
					distDic.Add (disty, gOb);
			}
		}
	}

/// <summary>
/// Falls off the current climbable object
/// </summary>
	private void FallOffClimbable(){
		climbing = false;
		climbPosAdjusted = false;
		canFall = true;
		anim.SetInteger ("climbX", 0);
		anim.SetInteger ("climbY", 0);
		anim.SetBool ("isClimbing", true);
		anim.SetTrigger ("falling");
		fallDelay = true;
		StartCoroutine (Delay (2));
	}

/// <summary>
/// The function used for when the character jumps - takes the 'type' of jump that needs to match the name of a jump type in the 'Jump Forces' list in the inspector for this script
/// </summary>
	private void Jump (string type){
		if (!tripleGroundJumps && jumpNo > 0 || tripleGroundJumps && Vector3.Magnitude(direction) < 0.7f && type != "crouchJump") {
			canDoNextJump = false;
			type = "default";
            if (onGround)
                jumpNo = 0;
		}
        if (braking && type != "sideways") {
            type = "sideways";
            braking = false;
            rotIterate = 0;
        }
		int j = 0;
        while (j < jumpForces.Length) {
			if(jumpForces[j].jumpName == type){
				jumpImpulse = jumpForces[j].forceAmount;
				if (type == "default" || type == "autoLedge")
					jumpType = 0;
				if (type == "sideways")
					jumpType = 1;
				if (type == "crouchJump")
					jumpType = 2;
                if (type == "lowLedge")
                    jumpType = 3;
			}
			j++;
		}
		if (canDoNextJump)
			jumpImpulse *= (1f + (0.2f * jumpNo));
		startedJump = false;
		if (type == "autoLedge") {
			jumpNo = 0;
            canDoNextJump = false;
		}
		jumpNo++;
		dontTripleJump = false;
		if (!onGround)
			dontTripleJump = true;
		if (canDoNextJump && dontTripleJump && tripleGroundJumps) {
			jumpNo = 1;
			canDoNextJump = false;
		}
		jumping = true;
		prolongJumpDelay = true;
		delayed = false;
		anim.ResetTrigger ("land");
		anim.ResetTrigger ("falling");
		if (jumpNo < 2) {
			if (jumpType == 0)
				anim.SetTrigger ("jumpNow");
			if (jumpType == 1)
				anim.SetTrigger ("sideJumpNow");
			if(jumpType == 2)
				anim.SetTrigger ("backFlip");
            if (jumpType == 3) {
                anim.SetTrigger("lowJump");
                lowJump = false;
            }
		} else {
			if(canDoNextJump && onGround){
				if(jumpNo == 2)
					anim.SetTrigger("secJumpNow");
				if(jumpNo == 3){
					if(Vector3.Magnitude(direction) >= 0.9f) 
						anim.SetTrigger("doubleJumpNow");
					else{
						jumpNo = 0;
                        Jump (type);
					}
				}
				canDoNextJump = false;
			}else{
				anim.SetTrigger ("doubleJumpNow");
            }
		}
		if(jumpNo > 0)
			StartCoroutine (Delay (1));
	}

/// <summary>
/// The function used for when the character lands through the OnCollisionEnter function
/// </summary>
	private void Land(){
		if ((!onGround) || (currentAnim.swimSurfaceBlend && onGround)) {
			if(!isDiving && !diveDelay && !currentAnim.inSwimFlip && !currentAnim.inDiving){
				onGround = true;
				onSteepGround = false;
				anim.ResetTrigger("falling");
				anim.ResetTrigger ("land");
				if (!currentAnim.locomotionBlend && !currentAnim.trigLocoBlend) {
					if (currentAnim.inATargJumpAnim)
						anim.Play ("Idle", 0);
					else
						anim.SetTrigger ("land");
				}
	            if (stoppedMidAir)
				    stoppedMidAir = false;
			    if (prolongJumpDelay)
				    prolongJumpDelay = false;
			    if (jumpNo < 3 && !dontTripleJump) {
				    canDoNextJump = true;
			    }
			    if(!canDoNextJump || jumpNo >= 3){
				    jumpNo = 0;
	                canDoNextJump = false;
			    }
			    StartCoroutine (Delay (14));
			    if(slideDelay)
				    anim.SetTrigger ("land");
	            if (!currentAnim.swimSurfaceBlend) {
	                if ((Vector3.Magnitude(new Vector3(rb.velocity.x, 0f, rb.velocity.z)) < 4f || Vector3.Angle(rb.velocity, (leftY * relative.transform.forward) + (leftX * relative.transform.right)) > 45f) && !onWaterSurface && !underWater) {
	                    if (!currentAnim.inALedgeClimbAnim)
	                        hardLanded = true;
				        StartCoroutine(Delay(12));
	                }
	            }
	            if (!hasLandedThisFrame){
	                hasLandedThisFrame = true;
	                StartCoroutine("WaitForEnd");
				}
			}
        }
	}

    IEnumerator RegainControl(){
        yield return new WaitForSeconds(0.5f);
        lockDirection = false;
    }

    IEnumerator WaitForEnd(){
        yield return new WaitForEndOfFrame();
        hasLandedThisFrame = false;
    }

	IEnumerator PreventFastFall(){
		yield return new WaitForSeconds (0.2f);
		yield return new WaitForEndOfFrame ();
		slowFall = false;
	}

    /// <summary>
    /// /This IEnumerator delays certain bools from turning true/false depending on the situation
    /// </summary>
    IEnumerator Delay (int delayNo)
	{
		if (delayNo == 1) {
			int currentJumpNo = jumpNo;
			yield return new WaitForSeconds (0.2f);
			delayed = true;
			yield return new WaitForSeconds (1f);
			if(currentJumpNo == jumpNo)
				prolongJumpDelay = false;
		}
		if (delayNo == 2) {
			yield return new WaitForSeconds (0.5f);
			fallDelay = false;
		}
		if (delayNo == 3) {
			yield return new WaitForSeconds (0.2f);
			trigDelay = true;
		}
		if (delayNo == 4) {										//Some of the delays, such as rolling, left jump, etc. should have their WaitForSeconds parameters changed if the user...
			yield return new WaitForSeconds (0.4f);             //...includes their own animations which may be longer or shorter
            yield return new WaitForEndOfFrame();
            if (targetting && onlyRollInTargetMode)
                this.transform.forward = forwardBeforeRoll;
            isRolling = false;
		}
		if (delayNo == 5) {
			yield return new WaitForSeconds (0.2f);
			isTargetJumping = false;
			if (leftJump)
				leftJump = false;
			if (rightJump)
				rightJump = false;
			if (backJump)
				backJump = false;
			yield return new WaitForSeconds (0.3f);
			targJumpDelay = false;
		}
		if (delayNo == 6) {
			yield return new WaitForSeconds (0.1f);
			rollKnockBack = false;
			yield return new WaitForSeconds (0.2f);
			isRollKnockedBack = false;
		}
		if (delayNo == 7) {
			yield return new WaitForSeconds(1f);
			swim = false;
		}
		if (delayNo == 8) {
			yield return new WaitForSeconds(0.4f);
			adjustingCam = false;
		}
		if (delayNo == 9) {
			yield return new WaitForSeconds(0.4f);
			grabbedSoLetGo = false;
		}
		if (delayNo == 10) {
			yield return new WaitForSeconds(0.4f);
			pushDelay = false;
		}
		if (delayNo == 11) {
			yield return new WaitForSeconds(1f);
			enteredSurface = false;
		}
		if (delayNo == 12) {
			yield return new WaitForSeconds(0.5f);
			hardLanded = false;
		}
		if (delayNo == 13) {
			yield return new WaitForSeconds(1f);
			canSidewaysJump = false;
		}
		if (delayNo == 14) {
			yield return new WaitForSeconds(0.3f);
			if(onGround)
				canDoNextJump = false;
		}
		if (delayNo == 15) {
			yield return new WaitForSeconds(0.2f);
			if(isCrouching){
				capCol.isTrigger = true;
				sphereCol.isTrigger = false;
			}else{
				sphereCol.isTrigger = true;
				capCol.isTrigger = false;
			}
			crouchDelay = false;
		}
		if (delayNo == 16) {
			slideDelay = false;
			yield return new WaitForSeconds(1f);
			slideDelay = true;
			startedSliding = false;
			anim.SetBool ("slidingDown", false);
		}
		if (delayNo == 17) {
			yield return new WaitForSeconds (1f);
			if(!startedSliding && slidingDownSlope)
				startedSliding = true;
		}
		if (delayNo == 18) {
			startedDelay = true;
			yield return new WaitForSeconds (0.8f);
			if(!backOnSlope){
				slidingDownSlope = false;
				startedSliding = false;
				slideDelay = true;
				anim.SetBool("slidingDown", false);
				anim.SetTrigger("falling");
			}else
				startedDelay = false;
		}
		if (delayNo == 19) {
			yield return new WaitForSeconds (0.35f);
			liftCorrectDelay = false;
			currentLiftable.transform.SetParent(this.characterRightHand.transform);
            actionText.InteractionAvailable(ORKFramework.InteractionType.Custom, " Poser");
            actionText.Interact();
            yield return new WaitForSeconds (0.1f);
            Vector3 currentPos = currentLiftable.transform.position;
            int iterate = 1;
            while (iterate < 11){
                currentLiftable.transform.position = Vector3.Lerp(currentPos, this.transform.position + (Vector3.up * cHeight * 0.6f), iterate/10f);
                yield return new WaitForEndOfFrame();
                iterate++;
            }
            currentLiftable.transform.position = this.transform.position + (Vector3.up*cHeight*0.6f);
			currentLiftable.transform.localPosition = currentLiftable.transform.localPosition;
            Physics.IgnoreCollision(capCol, currentLiftable.gameObject.GetComponent<Collider>(), true);
            Physics.IgnoreLayerCollision(characterCollisionLayer, liftablesCollisionLayer, false);
            origLiftLocPos = currentLiftable.transform.localPosition;
			liftDelay = false;
		}
		if (delayNo == 20) {
			if(currentLiftable != null){
				Rigidbody cLRB = currentLiftable.GetComponent<Rigidbody>();
				yield return new WaitForSeconds (0.4f);
				currentLiftable.transform.SetParent(origLiftableParent.transform);
                Physics.IgnoreCollision(capCol, currentLiftable.gameObject.GetComponent<Collider>(), false);
				cLRB.isKinematic = false;
				cLRB.AddForce((this.transform.forward + (Vector3.up*0.3f)) * throwForce * cLRB.mass, ForceMode.Impulse);
				isLifting = false;
				liftThrowDelay = false;
				if(uItems.currentItem != null){
					if(uItems.currentItem.projectileModel.name == currentLiftable.name)
						uItems.currentItem = null;
				}
			}
		}
		if (delayNo == 21) {
			yield return new WaitForSeconds(0.5f);
			if(currentLiftable != null){
				puttingDown = false;
				Rigidbody cLRB = currentLiftable.GetComponent<Rigidbody>();
				cLRB.isKinematic = false;
                Physics.IgnoreCollision(capCol, currentLiftable.gameObject.GetComponent<Collider>(), false);
                Physics.IgnoreLayerCollision(characterCollisionLayer, liftablesCollisionLayer, false);
				currentLiftable.transform.SetParent(origLiftableParent.transform);
				cLRB.AddForce(this.transform.forward * 0.15f * cLRB.mass, ForceMode.Impulse);
				isLifting = false;
                if (uItems.currentItem != null){
                    if (uItems.currentItem.projectileModel.name == currentLiftable.name)
                        uItems.currentItem = null;
                }
			}
		}
		if (delayNo == 22) {
			yield return new WaitForSeconds(0.5f);
			if(currentLiftable != null){
				currentLiftable.transform.SetParent(this.characterRightHand.transform);
				currentLiftable.transform.position = this.transform.position + (Vector3.up*cHeight*0.6f);
				currentLiftable.transform.localPosition = currentLiftable.transform.localPosition;
				origLiftLocPos = currentLiftable.transform.localPosition;
				liftOveride = false;
				liftDelay = false;
			}
		}
		if (rb.isKinematic)
			rb.isKinematic = false;
	}

	/// <summary>
	/// /This IEnumerator waits for certain animations to play, then positions the collider at the models position at the end of the animation to simulate precise movements such as climbing ledges
	/// </summary>
	IEnumerator LedgeHandler (int lheight){
        Vector3 newPos = Vector3.zero;
		inCoRout = true;
        if (lheight == 1) {
			yield return new WaitForSeconds (1.5f);																											//Waits for the animation to reach its end
            yield return new WaitForEndOfFrame();
            newPos = rootBoneGameObject.transform.TransformPoint(Vector3.zero + offset); //positions the collider at the root bone of the armature
            Physics.IgnoreLayerCollision (characterCollisionLayer, groundCollisionLayer, false);																									//Ignored layer collisions become false again
			Physics.IgnoreLayerCollision (characterCollisionLayer, slopesCollisionLayer, false);
			anim.SetBool ("midLedge", false);
            midLedgeGrab = false;																														//The character is no longer climbing the ledge
		}
        if (lheight == 2) {
            yield return new WaitForSeconds (1.25f);
            yield return new WaitForEndOfFrame();
            newPos = rootBoneGameObject.transform.TransformPoint(Vector3.zero + offset);
            Physics.IgnoreLayerCollision (characterCollisionLayer, groundCollisionLayer, false);
			Physics.IgnoreLayerCollision (characterCollisionLayer, slopesCollisionLayer, false);
			anim.SetBool ("pullUp", false);
            highLedgeGrab = false;
		}
		if (lheight == 3) {
			yield return new WaitForSeconds (1.25f);
            yield return new WaitForEndOfFrame();
            newPos = rootBoneGameObject.transform.TransformPoint(Vector3.zero + offset);
            Physics.IgnoreLayerCollision (characterCollisionLayer, groundCollisionLayer, false);
			Physics.IgnoreLayerCollision (characterCollisionLayer, slopesCollisionLayer, false);
			anim.SetBool ("pullUp", false);
            pullUp = false;
		}
		if (grabbing)				//The character is made to not be grabbing just in case so that grabBeneath (which is when lheight == 4) functions properly
			grabbing = false;
		if (onLedge)					//The character is no longer onLedge at this point
			onLedge = false;
		onLedgeAdjusted = false;
		if (lheight == 4) {
			Vector3 prevyPos = this.transform.position - (this.transform.forward*cRad*2f);
			yield return new WaitForSeconds (0.9f);
            yield return new WaitForEndOfFrame();
            this.transform.position = rootBoneGameObject.transform.position
				+ (Vector3.up*0.5f);
			if (this.transform.position.y - prevyPos.y < cHeight*0.95f)
				this.transform.position = new Vector3 (this.transform.position.x, prevyPos.y - (cHeight*0.95f), this.transform.position.z);
			gravityDirection = Vector3.zero;
			onGround = false;
			anim.SetBool ("gBeneath", false);
			if(Physics.Raycast(this.transform.position, -this.transform.forward, out rayDump, cRad*3f, whatIsGround)){
				int climbableID = 0;
				if(rayDump.collider.tag == "Fence")
					climbableID = 1;
				if(rayDump.collider.tag == "Ladder")
					climbableID = 2;
				if(climbableID != 0){
					this.transform.forward = -rayDump.normal;
					currentClimbingObject = rayDump.collider;
				}else{
					if(!climbing)
						currentClimbingObject = null;
				}
				if(climbableID == 1 && fenceClimbing){
					climbing = true;
					climbPosAdjusted = true;
					onFence = true;
				}
				if(climbableID == 2 && ladderClimbing){
					climbing = true;
					climbPosAdjusted = true;
					onLadder = true;
				}
			}
			if(!climbing){
				grabbing = true;
				grabAdjusted = true;
			}
			Physics.Linecast (this.transform.position, prevyPos - (Vector3.up * cHeight),
			                  out rayHitForward, whatIsACollision);
			this.transform.forward = -new Vector3 (rayHitForward.normal.x, 0f, rayHitForward.normal.z);
		}
		if (rb.isKinematic)
			rb.isKinematic = false;
		inCoRout = false;
		if(lheight == 1 || lheight == 2 || lheight == 3) {
            anim.Play("Idle", 0);
            this.transform.position = newPos;
        }
        yield return new WaitForEndOfFrame();
    }
    IEnumerator ActionAvailable(string actionType)
    {

        actionText.InteractionAvailable(ORKFramework.InteractionType.Custom, actionType);
        actionText.Interact();
        //GetComponent<AudioSource>().PlayOneShot(actionSound);
        yield return new WaitForEndOfFrame();
        actionStarted = false;
    }
}