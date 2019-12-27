/// <summary>
/// CURRENT VERSION: 2 (May '16)
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

public class CameraFollower : MonoBehaviour {

	private GamePadInputs gPI;
	public float distanceAway = 8f;												//The default distance that the camera is away from the character on the x and z plane
	public float distanceUp = 1.8f;												//The default distance that the camera is away from the character on the y axis
	public float maxDistanceAway = 15f;											//The maximum distance away that the camera can move to
	[SerializeField] private LayerMask whatIsAnObstruction;						//The layermask which covers all the collision layers that can be obstructions
	[SerializeField] private LayerMask whatIsGround;							//The layermask which covers all the collision layers that count at level ground
	[HideInInspector] public float origDA;														//A float containing the original value set for 'distanceAway'
	[HideInInspector] public float origDU;														//A float containing the original value set for 'distanceUp'
	
	[HideInInspector] public bool behindMode;									//A bool set to true for when the camera is in its deault 'behind/orbiting mode'
	public bool isometricMode;													//A bool set to true for when the camera is in 'isometric mode'
	[HideInInspector] public bool targetMode;									//A bool set to true for when the camera is in 'targeting mode'
	[HideInInspector] public bool freeMode;										//A bool set to true for when the camera is in 'free mode'
	 public bool fpvMode;										//A bool set to true for when the camera is in 'first person view'
	[HideInInspector] public bool cutsceneMode;									//A bool set to true for when the camera is being positioned externally for cutscenes

	 public bool inFPV;										//A bool set to true for when the camera has entered 'first person view', used as a delay
	[HideInInspector] public bool exitFPV;										//A bool set to true for when the camera has exited 'first person view', used as a delay
	[HideInInspector] public bool canFreeMode;									//A bool that restricts whether the camera can go into 'free mode'

	public bool fpvInvertYAxis;													//A bool that can be set by the user and player to determine whether the y-axis movement should be inverted
	public bool freeInvertYAxis;												//should the y-axis of the free camera be inverted
	public bool freeInvertXAxis;												//should the x-axis of the free camera be inverted
	public float freeRotateSpeed;                             					//The speed (sensitivity) at which the camera rotates around the character in 'free mode'
	public float freecamXDamp;													//How quickly should the camera slow down when letting go of any input in the X direction in free mode (0 = instant, 0> = slower)
	public float freecamYDamp;													//How quickly should the camera slow down when letting go of any input in the Y direction in free mode (0 = instant, 0> = slower)
	public float cameraSpeedDamp;												//How quickly should the camera slow down when going to any kind of stop in behind mode (0 = instant, 0> = slower)
	public float isometricCameraAngle;											//When in isometric mode, at which angle will the camera be at (in the x rotation)

    [SerializeField] private float cliffDistCheck;								//How far ahead should the camera check for cliffs to look down?
	[SerializeField] private float cliffDepthCheck;								//How far below should the camera check for cliffs to look down?

    public bool whiskeringFunction;												//Should the camera use the feature to automatically move around corners in 'behind' mode?
	public float whiskeringSensitivity;											//How sensitive should the camera be to corners?
    [SerializeField] private float whiskeringIntensity;							//How quickly should the camera turn around corners?
    public bool autoBehindPlayer;												//Should the camera automatically return to being directly behind the character when there is no input for a few seconds?
    public bool hillAdjusting;													//Should the camera adjust to the slope of a hill by looking up/down it providing a clear view of what's ahead of the character?

    public bool altUnderWaterTargetMode;										//Use an alternate targeting mode for swimming (explained in the 'targetMode' section in Update())
	private ThirdPersonController chara;										//A reference to the characters 'ThirdPersonController' script
	[HideInInspector] public Transform fpvPOS;													//A reference to the transform whose position is used to position the camera in 'first person view'
	private float addedUp;														//A float that adds how far out the camera should move from the character when in FPV, but only when swimming
	[HideInInspector] public Transform targetTransform;							//A reference to the transform that the camera focuses on by default
	private Transform followMe;													//A reference to the transform that the camera focuses on in certain modes; is altered externally via the ThirdPersonController script
	[HideInInspector] public GameObject focusRelative;							//A reference to the transform that the camera focuses on when the character is focusing on a target
	[HideInInspector] public GameObject swimRelative;							//A reference to the transform that the camera focuses on when the character is swimming
	[HideInInspector] public float freeAway;									//A float that works as the distance away from the character when in 'free mode'
	private RaycastHit rayDump;													//A RaycastHit to store a hit value so that a Linecast could be made
	private bool dontCheck;														//A bool that is true if the camera shouldn't adjust its position when in contact with a wall under certain circumstances
	private RaycastHit rayHit;													//A RaycastHit used when handling the cameras collisions
	private Vector3 direct;														//A Vector3 used when handling the cameras position in collisions
	private float origFRSpeed;													//The original speed at which the 'freeRotateSpeed' float was set to
	private float usingYAxis;													//which y-axis to use for free camera
	[Range(0f, 5f)] [SerializeField] public float yFreeSpeed;					//the speed at which the free camera will move in the y-axis

	[HideInInspector] public bool transitionBackFromCutscene;
	[HideInInspector] public Vector3 prevLookAt;
	[HideInInspector] public int transitionTimer;
	private float upAngle;
	private bool inCoRoute;
	private float totalDist;
	private Vector3 toTheBackRight;
	private Vector3 toTheBackLeft;
	private float charaFacingDir;
	private float charaCamAngle;
	private bool hasBeenRepos1;
	private bool hasBeenRepos2;
	private bool turningAround;
	private float turnTimer;
	private bool canTurnAround;
	private bool turningRight;
	private bool hasTurnedRight;
	private bool hasTurnedLeft;
	private bool cancelTurn;
	private Rigidbody charaRB;
	private bool obsOnRight;
	private bool obsOnLeft;
	private RaycastHit g1;
	private RaycastHit g2;
	private float hillDif;
    private bool didntFree;
    
    private float xRotLerp;
    private float yRotLerp;

    [HideInInspector] public bool camUnderWater;
    [HideInInspector] public GameObject currentWB;

	private Vector3 lookAtPos;

	private bool firstPass;														//a bool that turns true after the first pass of the Update() function to check for error

	public void Start () {
		gPI = GameObject.FindWithTag ("Player").GetComponent<GamePadInputs> ();
		origDA = distanceAway;																															//The original values of 'distanceAway/Up' are assigned
		origDU = distanceUp;
		focusRelative = new GameObject ();							//The focusRelative object is created
		focusRelative.name = "FocusRelative";
		focusRelative.transform.parent = this.transform;			//The focusRelative's parented to this camera
		swimRelative = new GameObject ();
		swimRelative.name = "SwimRelative";
		behindMode = true;											//The initial mode for the camera is set to 'behind mode'
		freeAway = distanceAway;									//The initial value of 'freeAway' is set to 'distanceAway'
		canFreeMode = true;											//The camera can enter 'free mode' on start
		rayHit = new RaycastHit ();
		rayDump = new RaycastHit ();
		origFRSpeed = freeRotateSpeed;								//The original freeRotateSpeed is stored here
		usingYAxis = 0f;
		g1 = new RaycastHit ();
		g2 = new RaycastHit ();
		//targetTransform = GameObject.FindWithTag ("Player").transform.Find("FollowMe").transform;

        xRotLerp = 0f;
        yRotLerp = 0f;
		fpvMode = false;
	}
	
	void Update(){                                                  //Update() is only really used for errors and miscelaneous checks
        gPI = GameObject.FindWithTag("Player").GetComponent<GamePadInputs>();
        if (!firstPass) {
		//////DO NOT DELETE THIS AT ANY POINT; YOU CAN GET RID OF THE ERROR HANDLINGS BUT NOT THIS SEGMENT//
		/**/if(GameObject.FindWithTag("Player").GetComponent<ThirdPersonController>() != null)
		/**/		chara = GameObject.FindWithTag ("Player").GetComponent<ThirdPersonController> ();															//The camera finds the ThirdPersonController script in the path specified
		/**/	if (GameObject.FindWithTag ("Player").transform.Find ("FollowMe") != null) {
		/**/		targetTransform = GameObject.FindWithTag ("Player").transform.Find ("FollowMe").transform;												//The targetTransform is set to the transform specified
		/**/		followMe = GameObject.FindWithTag ("Player").transform.Find ("FollowMe").transform;													//The initial followMe is set to the transform specified
		/**/	}
		/**/	if(GameObject.FindWithTag("Player").transform.Find("FollowMe") != null){
		/**/		this.transform.position = new Vector3 ((-distanceAway * targetTransform.transform.forward.x) + targetTransform.transform.position.x, 		//The initial position of the camera is set
		/**/		                                       distanceUp + targetTransform.transform.position.y, 
		/**/		                                       (-distanceAway * targetTransform.transform.forward.z) + targetTransform.transform.position.z);
		/**/	}
		/**/	if(chara.fpvBonePosition != null)
		/**/		fpvPOS = chara.fpvBonePosition.transform;	//The cameras position for 'first person view' is set
		/**/	charaRB = chara.GetComponent<Rigidbody> ();
		/////////////////////////////////////////////////////////////////////////////////////////////////////
			gPI = GameObject.FindWithTag ("Player").GetComponent<GamePadInputs> ();
			bool hasError = false;

			if(GameObject.FindWithTag("Player") == null){
				Debug.LogError ("CameraFollower.cs: There doesn't seem to be a character in the scene, or is not tagged 'Player'. Either confirm that the character has the correct tag or use the 'Character' prefab from the 'prefabs' folder.");
				hasError = true;
			}
			if(GameObject.FindWithTag("Player").GetComponent<ThirdPersonController>() == null){
				Debug.LogError ("CameraFollower.cs: Your character doesn't seem to have the ThirdPersonController.cs script attached to them. Please attach the script, found in the 'scripts' folder, to the character object.");
				hasError = true;
			}
			if(fpvPOS == null){
				Debug.LogError ("CameraFollower.cs: The path to the bone in your characters armature to be used as the position for the cameras first-person view doesn't exist. Please change lines 88 and 89 of this script to a path to the object that you want the camera to be positioned at in first-person view.");
				hasError = true;
			}
			if(hasError){
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
				Debug.DebugBreak();
				Application.Quit();
				Debug.Break();
                #endif
            }else
				firstPass = true;
		}else {
			if(!behindMode && !targetMode && !freeMode && !fpvMode && !cutsceneMode){		//The camera will default to 'behindMode' if not in any particular mode
				behindMode = true;
			}

			if(isometricMode && (!behindMode || targetMode || freeMode || fpvMode)){			//Isometric mode works through behind mode - they function the same but the camera doesn't rotate
				behindMode = true;
				targetMode = false;
				freeMode = false;
				fpvMode = false;
			}
			if (isometricMode && this.transform.localEulerAngles.x != isometricCameraAngle)
				this.transform.localEulerAngles = new Vector3 (isometricCameraAngle, 0f, 0f);

			if(cutsceneMode){
				behindMode = false;
				targetMode = false;
				freeMode = false;
				fpvMode = false;
			}

		//If the characters head is still not rendered after being in 'fPVMode' then it will be set to render
			if(chara.fPVHiddenMeshes.Length > 0){
				if(!fpvMode && chara.fPVHiddenMeshes[0].hiddenMesh.GetComponent<SkinnedMeshRenderer>().shadowCastingMode != UnityEngine.Rendering.ShadowCastingMode.On){
					int f = 0;
					while(f < chara.fPVHiddenMeshes.Length){
						chara.fPVHiddenMeshes[f].hiddenMesh.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
						f++;
					}
				}
			}
		}
	}
	
	void LateUpdate () {											//LateUpdate() is where all the camera movement is done; this is so that the character will update before the camera does
		if (freeMode && !canFreeMode) {								//If the camera cannot be in 'freeMode' but currently is in 'freeMode' then it will switch to 'behindMode'
			behindMode = true;
		}

		if (transitionBackFromCutscene) {
			lookAtPos = Vector3.Lerp (prevLookAt, targetTransform.position, transitionTimer / 10f);
			transitionTimer++;
			if (transitionTimer >= 60) {
				transitionBackFromCutscene = false;
				transitionTimer = 0;
			}
		} else
			lookAtPos = targetTransform.position;

		if (gPI.RH == 0f && gPI.RV == 0f && !inCoRoute && !fpvMode && !targetMode && freeMode)
			StartCoroutine (CameraRepositionDelay ());

		if (behindMode || targetMode) {
			if(this.GetComponent<Camera> ().fieldOfView != 40f)
				this.GetComponent<Camera> ().fieldOfView = 40f;
		}

//////////When the camera is in BEHIND/ORBITING MODE - Essentially the camera movement for when the player is not controlling the camera
		if (behindMode) {
            if (!didntFree)
                didntFree = true;
			if (chara.fPVing) {
				inFPV = false;
				exitFPV = false;
				chara.fPVing = false;
			}
			if (inFPV)
				inFPV = false;
			float distAway = 0f;
			float yVelo = 0f;
			if (chara.leftY < -0.4f)																				//If the character is moving towards the camera, it will have a slight delay and move further back...
				distAway = Mathf.SmoothDamp (distanceAway, distanceAway + 3f, ref yVelo, Time.smoothDeltaTime);		//...so that the player has a better view
			else
				distAway = distanceAway;

			if (float.IsNaN (distanceAway) || float.IsNaN (distanceUp)) {											//The distance away/up can result as value that's not a number, so this counters that error
				dontCheck = true;																				//If it is the case that they're not numbers, the camera will not check for collisions
				if (float.IsNaN (distanceAway))
					distanceAway = origDA;
				if (float.IsNaN (distanceUp))
					distanceUp = origDU;
			} else
				dontCheck = false;

			Vector3 nextPos = targetTransform.transform.position + new Vector3 (chara.relative.transform.forward.x * -distAway,		//The next position of the camera is set as a temporary Vector3
			                                                          distanceUp, chara.relative.transform.forward.z * -distAway);

			this.transform.position = Vector3.Lerp (this.transform.position, nextPos,												//The camera lerps to its next position rather than switching straight to it
													Time.smoothDeltaTime * cameraSpeedDamp);

			if (!dontCheck)																						//If it can check for collisions, it will run the CameraChecks() method (which is at the end of each mode apart from FPVMode)
				CameraChecks ();

			if(!isometricMode)
				this.transform.LookAt (lookAtPos);													//The camera is finally set to look at the targetTransform
		}

//////////When the camera is in TARGETTING MODE	 - Keeping the camera directly behind the character and not moving from that orientation
		if (targetMode && !isometricMode) {
			if (float.IsNaN (distanceAway) || float.IsNaN (distanceUp)) {
				dontCheck = true;
				if (float.IsNaN (distanceAway))
					distanceAway = origDA;
				if (float.IsNaN (distanceUp))
					distanceUp = origDU;
			} else
				dontCheck = false;

			Vector3 nextPos = Vector3.zero;
			if (chara.isSwimming) {
				if (!altUnderWaterTargetMode) {
					/// IF YOU WANT THE CAMERA TO FOCUS BEHIND THE CHARACTER AND THEN KEEP THE SAME ANGLE THEN USE THIS:
					if (!chara.trigDelay) {
						swimRelative.transform.rotation = new Quaternion (0f, chara.transform.rotation.y, 0f, chara.transform.rotation.w);
						nextPos = targetTransform.transform.position - new Vector3 (swimRelative.transform.forward.x * distanceAway,
							                                                       	-distanceUp, swimRelative.transform.forward.z * distanceAway);
					} else {
						nextPos = targetTransform.transform.position - new Vector3 (chara.relative.transform.forward.x * distanceAway,
								                                                   	-distanceUp, chara.relative.transform.forward.z * distanceAway);
					}
				} else {
					/// IF YOU WANT THE CAMERA TO STAY BEHIND THE CHARACTER WHILE SWIMMING AND TARGETING AT ALL TIMES THEN USE THIS:
					swimRelative.transform.rotation = new Quaternion (0f, chara.transform.rotation.y, 0f, chara.transform.rotation.w);
					nextPos = targetTransform.transform.position - new Vector3 (swimRelative.transform.forward.x * distanceAway,
						                                                           -distanceUp, swimRelative.transform.forward.z * distanceAway);
				}
			} else {
                if(chara.isRolling && chara.onlyRollInTargetMode)
                    nextPos = targetTransform.transform.position + new Vector3(this.transform.forward.x * -distanceAway,
                                                                               distanceUp, this.transform.forward.z * -distanceAway);
                else
                    nextPos = targetTransform.transform.position + new Vector3 (chara.transform.forward.x * -distanceAway,
			                                                                   distanceUp, chara.transform.forward.z * -distanceAway);
			}
			if (!chara.trigDelay || chara.climbing) {																				//If the characters script is in 'trigDelay', the camera will lerp over to its new position for a nice transition effect
				this.transform.position = Vector3.Lerp (this.transform.position, nextPos,
				                                       Time.smoothDeltaTime * 20f);
			} else {																								//When the transition to the cameras new position is over...
				if (chara.isFocusing) {
					this.transform.position = Vector3.Lerp (this.transform.position, nextPos,					//If the character is focusing on a target, the camera will slowly move around the character for the 'epic battle effect'
				                                       Time.smoothDeltaTime / 2f);
				} else
					this.transform.position = nextPos;															//If not focusing on a target, the camera will stay directly behind the player
			}

			if (!dontCheck)
				CameraChecks ();
			this.transform.LookAt (lookAtPos);
		}

//////////When the camera is in FREE MODE	- The mode that is activated when the player decides to manually move the camera around the character
		if (freeMode && canFreeMode && !isometricMode) {
            if (didntFree)
                didntFree = false;
            float useXAxis = 0f;													//In free mode, the camera moves depending on certain X and Y inputs, which can either be from the mouse movement and scrollwheel or a second analog stick
			float useYAxis = 0f;
			float useZAxis = 0f;
			usingYAxis = 0f;
			if (Input.GetAxis ("ZVertical") != 0f)									//This gets the input from only one of the two methods if both or either are being used
				useZAxis = Input.GetAxis ("ZVertical");
			else {
				if (Input.GetAxis ("Mouse ScrollWheel") != 0f)
					useZAxis = Input.GetAxis ("Mouse ScrollWheel") * 50f;
			}
			useXAxis = gPI.RH;
			if (!freeInvertXAxis)
				useXAxis *= -1f;

			useYAxis = gPI.RV;

			if (useZAxis != 0f) {														//If there is no input from the player, the camera will keep its new distance away
				freeAway += -useZAxis * Time.deltaTime * 5f;
				if (freeAway < distanceAway)
					freeAway = distanceAway;
				if (freeAway > maxDistanceAway)
					freeAway = maxDistanceAway;
			}

			upAngle = Vector3.Angle (this.transform.up, chara.relative.transform.forward);
			if (freeInvertYAxis)
				useYAxis *= -1f;
			if (useYAxis > 0f && upAngle < 20f ||
				useYAxis < 0f && upAngle >= 160f) {
				usingYAxis = 0f;
			} else {
				if (useYAxis != 0f)
					usingYAxis = useYAxis;
			}
			float useAngle = upAngle;
			if (upAngle < 90f)
				useAngle = 90f;
			this.GetComponent<Camera> ().fieldOfView = Mathf.Lerp (40f, 60f, (useAngle - 90f) / 70f);
            
            RotateCameraY(useXAxis);
            RotateCameraX(usingYAxis);
			this.transform.position = Vector3.Lerp (this.transform.position, targetTransform.transform.position - 
				this.transform.forward * freeAway, Time.smoothDeltaTime *10f);
			CameraChecks ();
			this.transform.LookAt (lookAtPos);
		}


		if (!fpvMode) {
			focusRelative.transform.position = this.transform.position;						//Positions the transform that the character moves relative to when focusing on a target
			focusRelative.transform.LookAt (followMe);										//That transform is set to look at the character
		}

		chara.RelativeSet ();
	}

	void RotateCameraX(float amount){
		xRotLerp = Mathf.Lerp(xRotLerp, amount, Time.smoothDeltaTime * 5f);
        if (Mathf.Abs(xRotLerp) < 0.01f)
            xRotLerp = 0f;
        this.transform.RotateAround(targetTransform.transform.position, chara.relative.transform.right, xRotLerp * yFreeSpeed * Time.smoothDeltaTime * freecamXDamp);
    }

    void RotateCameraY(float amount){
        yRotLerp = Mathf.Lerp(yRotLerp, amount, Time.smoothDeltaTime * 5f);
        if (Mathf.Abs(yRotLerp) < 0.01f)
            yRotLerp = 0f;
        this.transform.RotateAround(targetTransform.transform.position, Vector3.up, yRotLerp * freeRotateSpeed * Time.smoothDeltaTime * freecamYDamp);
    }

    void FixedUpdate(){
//////////When the camera is in FIRST PERSON VIEW MODE
		if (fpvMode) {
			if (chara.isSwimming)
				addedUp = 2f;
			else
				addedUp = 1f;
			if (Mathf.RoundToInt (this.transform.position.y) != Mathf.RoundToInt (fpvPOS.position.y + 0.15f) && !inFPV) {				//While the camera isn't 'inFPV' it will transition over to the characters head
				if (!inFPV) {
					this.transform.position = Vector3.Lerp (this.transform.position,
					                                       fpvPOS.position + (Vector3.up * 0.15f * addedUp) + (chara.transform.forward * 0.2f),
				        	                               30f * Time.smoothDeltaTime);
					this.transform.forward = Vector3.Lerp (this.transform.forward, chara.transform.forward, 100f * Time.smoothDeltaTime);
				}
			} else {
				if(!inFPV){
					inFPV = true;
					if (chara.fPVHiddenMeshes.Length > 0) {
						int f = 0;
						while (f < chara.fPVHiddenMeshes.Length) {
							chara.fPVHiddenMeshes [f].hiddenMesh.GetComponent<SkinnedMeshRenderer> ().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;
							f++;
						}
					}
					this.transform.position = fpvPOS.position + (Vector3.up * 0.15f * addedUp) + (chara.transform.forward * 0.2f);
				}
			}
			if (inFPV && !chara.inACutscene) {
				this.transform.position = Vector3.Lerp (this.transform.position, fpvPOS.position + (this.transform.up * 0.15f * addedUp) + (chara.transform.forward * 0.2f), 2f);	//The cameras position is set first

				if(this.GetComponent<Camera> ().fieldOfView != 70f)
					this.GetComponent<Camera> ().fieldOfView = 40f;

				float zLeftY = 0f;
				float zLeftX = 0f;
				if (gPI.RV != 0f)																		//Like in 'freeMode' the script checks to see if the player is using the mouse or a second analog stick
					zLeftY = gPI.RV;
				if (gPI.RH != 0f)
					zLeftX = gPI.RH;

				if (zLeftX < 0.1f && zLeftX > -0.1f)																			//Dead zones
					zLeftX = 0f;
				if (zLeftY < 0.1f && zLeftY > -0.1f)
					zLeftY = 0f;
				float directionMinus = Mathf.Sign (this.transform.forward.y);																//Checks if the player is looking up or down; is 1 if up, is -1 if down
				if (!fpvInvertYAxis)																											//If the player wants an inverted y-axis it will be adjusted here
					zLeftY *= -1f;
				if (!chara.onLedge && !chara.grabbing && !chara.climbing && !chara.isSwimming && !chara.isDiving) {
					if (zLeftX != 0f)
						this.transform.RotateAround (this.transform.position, Vector3.up, zLeftX * 200f * Time.smoothDeltaTime);

					float lookAngle = Vector3.Angle (this.transform.forward, chara.transform.forward);
					if (lookAngle < 80f && zLeftY < 0f) {																						//These 4 if-statements control the camera looking up and down within the specified limits
						this.transform.RotateAround (this.transform.position, this.transform.right, zLeftY * 200f * Time.smoothDeltaTime);
					} else {
						if (zLeftY < 0f && directionMinus < 0f)
							this.transform.RotateAround (this.transform.position, this.transform.right, zLeftY * 200f * Time.smoothDeltaTime);
					}
					if (lookAngle < 80f && zLeftY > 0f) {
						this.transform.RotateAround (this.transform.position, this.transform.right, zLeftY * 200f * Time.smoothDeltaTime);
					} else {
						if (zLeftY > 0f && directionMinus > 0f)
							this.transform.RotateAround (this.transform.position, this.transform.right, zLeftY * 200f * Time.smoothDeltaTime);
					}
				} 
			}
		}
		if (exitFPV) {																																			//Camera transitioms back to 'behindMode'
			exitFPV = false;
			behindMode = true;
		}
    }

	/// <summary>
	/// This method is called each lateUpdate whenever the camera is in behind/target/free mode, it runs many checks to have it act as a dynamic camera
	/// </summary>
	void CameraChecks(){
	////////Check for obstructions between the character and the camera
		if (!fpvMode) {
			if (Physics.Linecast (followMe.position, this.transform.position, out rayHit, whatIsAnObstruction)) {			//If there is a collision between the character and the camera...
				this.transform.position = rayHit.point + (this.transform.forward * 0.1f);								//...the camera moves to in front of the closest collision to the character
				float potentialSpeed = Vector3.Distance (followMe.position, this.transform.position) / 3f;				//The speed of the camera rotating around the character in 'freeMode' is slowed down the closer it gets to the character
				if (potentialSpeed > 4f)
					freeRotateSpeed = Vector3.Distance (followMe.position, this.transform.position) / 3f;
				else
					freeRotateSpeed = 4f;
				direct = chara.transform.position;
			} else {																										//If there is no collision between the character and the camera...
				if (Physics.Raycast (this.transform.position, -this.transform.forward, 1f, whatIsAnObstruction)) {			//...and if there's a collision directly behind the camera...
					return;																								//...the camera will not try to get back to its default distance away from the character
				} else {																									//If there is no collision behind the camera, it will start moving back to its default distsance away from the character
					if (freeRotateSpeed != origFRSpeed)
						freeRotateSpeed = origFRSpeed;																	//The rotation speed of the camera in freeMode is returned to normal
					if (chara.direction != Vector3.zero && !Physics.Linecast (chara.transform.position, 																		//This if statement checks that if the camera goes back to its default distance away...
				                                                        chara.transform.position + ((rayHit.point + Vector3.up - this.transform.forward) - direct),	//...that there will not be a collision in between the camera and the character again;
				                                                    out rayDump, whatIsAnObstruction)																		//...if there is, it will cause the camera to continuously move backwards and forwards
						&& !Physics.Linecast (chara.transform.position, 
												                    chara.transform.position + ((rayHit.point - Vector3.up - this.transform.forward) - direct),
												                    out rayDump, whatIsAnObstruction)) {
						rayDump.point = Vector3.zero;
						if (distanceUp != origDU) {																		//So if there is no potential collision between the camera and the character...
							distanceUp = Mathf.Lerp (distanceUp, origDU, Time.smoothDeltaTime * 2f);							//The distances away/up are reset back to their original values 
							if (distanceUp > origDU - 0.01f)
								distanceUp = origDU;
						}
						if (distanceAway != origDA) {
							distanceAway = Mathf.Lerp (distanceAway, origDA, Time.smoothDeltaTime * 2f);
							if (distanceAway > origDA - 0.01f)
								distanceAway = origDA;
						}
					} else {																								//...otherwise it will not get back to its default distance away
					
					}
				}
			}
		}
////////Does 'whisker' checks for obstructions that may come to obstruct the players view; rotates the camera around the obstruction to avoid it
		if(behindMode && !fpvMode && !targetMode && !freeMode && !isometricMode){
            if (whiskeringFunction) { 
                totalDist = Vector3.Distance(chara.transform.position + (chara.transform.forward * chara.cRad * 7f), this.transform.position);
                toTheBackRight = Quaternion.Euler(0f, -25f, 0f) * this.transform.forward;
                toTheBackLeft = Quaternion.Euler(0f, 50f, 0f) * toTheBackRight;
                charaFacingDir = Vector3.Dot(new Vector3(chara.transform.forward.x, 0f, chara.transform.forward.z), new Vector3(this.transform.forward.x, 0f, this.transform.forward.z));
                charaCamAngle = Vector3.Angle(new Vector3(this.transform.forward.x, 0f, this.transform.forward.z), new Vector3(chara.transform.forward.x, 0f, chara.transform.forward.z));

                if (Physics.Linecast(chara.transform.position + (chara.transform.forward * chara.cRad * 7f), chara.transform.position - (toTheBackRight * totalDist),
                                     whatIsAnObstruction) && chara.onGround)
                    obsOnRight = true;
                else
                    obsOnRight = false;
                if (Physics.Linecast(chara.transform.position + (chara.transform.forward * chara.cRad * 7f), chara.transform.position - (toTheBackLeft * totalDist),
                                     whatIsAnObstruction) && chara.onGround)
                    obsOnLeft = true;
                else
                    obsOnLeft = false;


                if (obsOnLeft && obsOnRight){
                    obsOnLeft = false;
                    obsOnRight = false;
                }

                if (obsOnRight)
                    RotateCameraY(1f);
                if (obsOnLeft)
                    RotateCameraY(-1f);

                if (!obsOnLeft && !obsOnRight)
                    RotateCameraY(0f);
            }

            /////////If the character is facing towards the camera, the camera will rotate to be behind the character
            if (autoBehindPlayer) { 
                if (charaFacingDir < 0f && !turningAround && gPI.LH == 0f && gPI.LV == 0f && chara.onGround && !cancelTurn) {
				    if (!canTurnAround)
					    StartCoroutine (CameraTurnAroundDelay ());
				    else {
					    turningAround = true;
				    }
			    }
			    if (turningAround && gPI.RH == 0f && gPI.RV == 0f && chara.onGround) {
				    if (turningRight) {
					    this.transform.RotateAround (targetTransform.transform.position, Vector3.up, charaCamAngle * Time.smoothDeltaTime);
					    if (charaCamAngle < 10f) {
						    turningAround = false;
						    hasTurnedRight = true;
					    }
				    }
				    if (!turningRight) {
					    this.transform.RotateAround (targetTransform.transform.position, Vector3.up, -charaCamAngle * Time.smoothDeltaTime);
					    if (charaCamAngle < 10f) {
						    turningAround = false;
						    hasTurnedLeft = true;
					    }
				    }
			    }
			    if (hasTurnedRight) {
				    if (gPI.RH == 0f) {
					    this.transform.RotateAround (targetTransform.transform.position, Vector3.up, Time.smoothDeltaTime * charaCamAngle);
					    if (charaCamAngle < 5f) {
						    hasTurnedRight = false;
						    canTurnAround = false;
					    }
				    }
			    }
			    if (hasTurnedLeft) {
				    if (gPI.RH == 0f) {
					    this.transform.RotateAround (targetTransform.transform.position, Vector3.up, Time.smoothDeltaTime * -charaCamAngle);
					    if (charaCamAngle < 5f) {
						    hasTurnedLeft = false;
						    canTurnAround = false;
					    }
				    }
			    }
			    if(gPI.LH != 0f || gPI.LV != 0f){
				    cancelTurn = true;
				    turningAround = false;
				    hasTurnedLeft = false;
				    hasTurnedRight = false;
				    canTurnAround = false;
				    StartCoroutine(TurnAgainDelay());
			    }
            }

	////////Camera pans down when falling
			if(charaFacingDir > 0f && !chara.inWater && !chara.isPaused){
				if(!chara.onGround && charaRB.velocity.y < 5f && charaRB.velocity.y > -10f )
					this.transform.RotateAround (targetTransform.transform.position, chara.relative.transform.right, Mathf.Abs(charaRB.velocity.y)/20f);
			}

	////////Camera pans down when going down a hill and up when going up
            if(hillAdjusting){
			    if(Mathf.Abs(charaFacingDir) > 0.8f && chara.onGround &&
				     Physics.Raycast(chara.transform.position + (Vector3.up*chara.cHeight) + (chara.transform.forward*chara.cRad*3f), -Vector3.up, out g1, chara.cHeight*2f, whatIsGround) &&
				     Physics.Raycast(chara.transform.position + (Vector3.up*chara.cHeight) - (chara.transform.forward*chara.cRad*2f), -Vector3.up, out g2, chara.cHeight*2f, whatIsGround)){
                    if((g2.normal.y == 1f && g2.normal.y == 1f) || g1.normal.y <= chara.walkableSlopeTolerence || g2.normal.y <= chara.walkableSlopeTolerence){
                    }else{
				        hillDif = Mathf.Lerp(hillDif, g1.point.y - g2.point.y, Time.smoothDeltaTime);
				        if(hillDif != 0f)
					        this.transform.RotateAround (targetTransform.transform.position, chara.relative.transform.right, -hillDif * 100f * Time.smoothDeltaTime * Mathf.Sign(charaFacingDir));
                    }
			    }
            }

	////////Moving the free camera while the camera is automatically adjusting will cancel any automatic movements (players intent is priority)
	////Facing the same direction as the camera through moving the character will also cancel any automatic movements
			if (gPI.RH != 0f || gPI.LV != 0f && charaFacingDir > 0f) {
				canTurnAround = false;
				turningAround = false;
				turningRight = false;
				hasTurnedLeft = false;
				hasTurnedRight = false;
			}
		}
    }

	IEnumerator CameraRepositionDelay(){
		inCoRoute = true;
		yield return new WaitForSeconds (3f);
		if (gPI.RH == 0f && gPI.RV == 0f && freeMode && !cancelTurn) {
			freeMode = false;
			behindMode = true;
			distanceAway = origDA;
			distanceUp = origDU;
		}
		inCoRoute = false;
	}

	IEnumerator CameraTurnAroundDelay(){
		yield return new WaitForSeconds(1.5f);
		canTurnAround = true;
		float charaFacingDir2 = Vector3.Dot (new Vector3 (chara.transform.forward.x, 0f, chara.transform.forward.z), new Vector3 (this.transform.right.x, 0f, this.transform.right.z));
		if(charaFacingDir2 > 0f){
			turningRight = true;
		}else{
			turningRight = false;
		}
	}
    

	IEnumerator TurnAgainDelay(){
		yield return new WaitForSeconds (3.5f);
		inCoRoute = false;
		cancelTurn = false;
	}
}
