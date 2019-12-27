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

public class Animations : MonoBehaviour {

	//This script is generally to simplify finding out which animation is currently playing for the character, essentially just reference this script and check a boolean value.

	[HideInInspector] public Animator anim;
	private int iterator;
	private AnimatorStateInfo animSInfo;

////Climbing Animations
	[HideInInspector] public bool inClimbDown1;
	[HideInInspector] public bool inClimbDown2;
	[HideInInspector] public bool inClimbUp1;
	[HideInInspector] public bool inClimbUp2;
	[HideInInspector] public bool inClimbRight1;
	[HideInInspector] public bool inClimbRight2;
	[HideInInspector] public bool inClimbLeft1;
	[HideInInspector] public bool inClimbLeft2;
	[HideInInspector] public bool inClimbStill1;
	[HideInInspector] public bool inClimbStill2;

	[HideInInspector] public bool inAClimbingAnim;

////Crouching Animations
	[HideInInspector] public bool inCrouchDown;

	[HideInInspector] public bool crawlingBlend;

////Grabbing Animations
	[HideInInspector] public bool grabbingBlend;

////Box-Pushing Animations
	[HideInInspector] public bool inGrabPull;
	[HideInInspector] public bool inGrabPush;
	[HideInInspector] public bool inGrabStill;

	[HideInInspector] public bool inABoxPushingAnim;

////Airborne Animations
	[HideInInspector] public bool inFall;
	[HideInInspector] public bool inRunningLand;
	[HideInInspector] public bool inHardLand;

////Rolling Animations
	[HideInInspector] public bool inRoll;
	[HideInInspector] public bool inRollKnockBack;

////Ledge-Climbing Animations
	[HideInInspector] public bool inGrabBeneath;
	[HideInInspector] public bool inJumpToGrab;
	[HideInInspector] public bool inLowJumpLedge;
	[HideInInspector] public bool inMidLedge;
	[HideInInspector] public bool inPullUpLedge;

	[HideInInspector] public bool inALedgeClimbAnim;

////Jumping Animations
	[HideInInspector] public bool inJump;
	[HideInInspector] public bool inDoubleJump;
	[HideInInspector] public bool inSecondJump;
	[HideInInspector] public bool inSidewaysJump;

	[HideInInspector] public bool inAJumpAnim;

////Target Jumping Animations
	[HideInInspector] public bool inBackFlip;
	[HideInInspector] public bool inSideHopLeft;
	[HideInInspector] public bool inSideHopRight;

	[HideInInspector] public bool inATargJumpAnim;

////Swimming Animations
	[HideInInspector] public bool inDiving;
	[HideInInspector] public bool inBreathe;
	[HideInInspector] public bool inSwimFlip;
	[HideInInspector] public bool inSwimImpulse;

	[HideInInspector] public bool swimSurfaceBlend;
	[HideInInspector] public bool swimmingBlend;
	[HideInInspector] public bool inASwimmingAnimation;

////Locomotion Animations
	[HideInInspector] public bool inIdle;
	[HideInInspector] public bool inRun;
	[HideInInspector] public bool inRunLeft;
	[HideInInspector] public bool inRunRight;
	[HideInInspector] public bool inSideStepLeft;
	[HideInInspector] public bool inSideStepRight;
	[HideInInspector] public bool inSprint;
	[HideInInspector] public bool inSprintLeft;
	[HideInInspector] public bool inSprintRight;
	[HideInInspector] public bool inWalk;
	[HideInInspector] public bool inBackwalk;
	[HideInInspector] public bool inBrake;
	[HideInInspector] public bool inSliding;

	[HideInInspector] public bool locomotionBlend;
	[HideInInspector] public bool trigLocoBlend;

////Lifting Animations
	[HideInInspector] public bool inInLift;
	[HideInInspector] public bool inLiftDown;
	[HideInInspector] public bool inLifting;
	[HideInInspector] public bool inThrow;

	[HideInInspector] public bool inALiftingAnimation;

	////Item-Usage Animations
	[HideInInspector] public bool inShieldHold;
	[HideInInspector] public bool inSwordHold;
	[HideInInspector] public bool inSwordSwing;
	[HideInInspector] public bool inReachItem;
	[HideInInspector] public bool inSwordCharge;
	[HideInInspector] public bool inSwordSpin;
	[HideInInspector] public bool inShieldDefend;
	[HideInInspector] public bool inBowUp;
	[HideInInspector] public bool inBowDown;

	[HideInInspector] public bool inAnItemUsageAnim;

////Empty Animations
	[HideInInspector] public bool inEmptyRH;
	[HideInInspector] public bool inEmptyLH;


	void Start () {
		anim = GameObject.FindWithTag ("Player").gameObject.GetComponentInChildren<Animator> ();
	}

	void Update () {

		animSInfo = anim.GetCurrentAnimatorStateInfo(0);					//Finding animations on layer 0

		inClimbDown1 = animSInfo.IsName("climbDown") ? true : false;
		inClimbDown2 = animSInfo.IsName("climbDown2") ? true : false;
		inClimbStill1 = animSInfo.IsName("climbStill") ? true : false;
		inClimbStill2 = animSInfo.IsName("climbStill2") ? true : false;
		inClimbUp1 = animSInfo.IsName("climbUp") ? true : false;
		inClimbUp2 = animSInfo.IsName("climbUp2") ? true : false;
		inClimbLeft1 = animSInfo.IsName("climbLeft") ? true : false;
		inClimbLeft2 = animSInfo.IsName("climbLeft2") ? true : false;
		inClimbRight1 = animSInfo.IsName("climbRight") ? true : false;
		inClimbRight2 = animSInfo.IsName("climbRight2") ? true : false;

		crawlingBlend = animSInfo.IsName("crawling") ? true : false;
		inCrouchDown = animSInfo.IsName("crouchDown") ? true : false;

		inGrabPull = animSInfo.IsName("boxPull") ? true : false;
		inGrabPush = animSInfo.IsName("boxPush") ? true : false;
		inGrabStill = animSInfo.IsName("BlockPushing") ? true : false;

		inFall = animSInfo.IsName("falling") ? true : false;
		inRunningLand = animSInfo.IsName("runningLand") ? true : false;
		inHardLand = animSInfo.IsName("hardLand") ? true : false;

		inRoll = animSInfo.IsName("roll") ? true : false;
		inRollKnockBack = animSInfo.IsName("rollKnockBk") ? true : false;

		inJump = animSInfo.IsName("jump") ? true : false;
		inDoubleJump = animSInfo.IsName("doubleJump") ? true : false;
		inSecondJump = animSInfo.IsName("secondJump") ? true : false;
		inSidewaysJump = animSInfo.IsName("sidewaysJump") ? true : false;

		inBackFlip = animSInfo.IsName("backFlip") ? true : false;
		inSideHopLeft = animSInfo.IsName("leftSH") ? true : false;
		inSideHopRight = animSInfo.IsName("rightSH") ? true : false;

		inDiving = animSInfo.IsName("diving") ? true : false;
		swimSurfaceBlend = animSInfo.IsName("swimSurface") ? true : false;
		swimmingBlend = animSInfo.IsName("swimming") ? true : false;
		inBreathe = animSInfo.IsName("breathe") ? true : false;
		inSwimFlip = animSInfo.IsName("flip") ? true : false;
		inSwimImpulse = animSInfo.IsName("swimImpulse") ? true : false;
		
		inIdle = animSInfo.IsName("Idle") ? true : false;
		locomotionBlend = animSInfo.IsName("Locomotion") ? true : false;
		trigLocoBlend = animSInfo.IsName("TrigLocomotion") ? true : false;
		inBrake = animSInfo.IsName("Brake") ? true : false;
		inSliding = animSInfo.IsName("slopeSlide") ? true : false;
		
		inGrabBeneath = animSInfo.IsName("grabBeneath") ? true : false;
		inJumpToGrab = animSInfo.IsName("jumpToGrab") ? true : false;
		inLowJumpLedge = animSInfo.IsName("lowJump") ? true : false;
		inMidLedge = animSInfo.IsName("midLedge") ? true : false;
		inPullUpLedge = animSInfo.IsName("pullUp") ? true : false;
		
		grabbingBlend = animSInfo.IsName("Grabbing") ? true : false;


		animSInfo = anim.GetCurrentAnimatorStateInfo(1);					//Finding animations on layer 1

		inInLift = animSInfo.IsName("lifting") ? true : false;
		inLiftDown = animSInfo.IsName("liftPutDown") ? true : false;
		inLifting = animSInfo.IsName("lift") ? true : false;
		inThrow = animSInfo.IsName("liftThrow") ? true : false;


		animSInfo = anim.GetCurrentAnimatorStateInfo(2);					//Finding animations on layer 2

		inSwordHold = animSInfo.IsName("holdWeapon") ? true : false;
		inSwordSwing = animSInfo.IsName("swordSwing") ? true : false;
		inReachItem = animSInfo.IsName("reachItems") ? true : false;
		inSwordCharge = animSInfo.IsName("swordCharge") ? true : false;
		inSwordSpin = animSInfo.IsName("spinAttack") ? true : false;
		inBowUp = animSInfo.IsName("ZZ8BowUp") ? true : false;
		inBowDown = animSInfo.IsName("ZZ9BowDown") ? true : false;
		inEmptyRH = animSInfo.IsName("EmptyRH") ? true : false;


		animSInfo = anim.GetCurrentAnimatorStateInfo(3);					//Finding animations on layer 3
		
		inShieldHold = animSInfo.IsName("holdShield") ? true : false;
		inShieldDefend = animSInfo.IsName("useShield") ? true : false;
		inEmptyLH = animSInfo.IsName("EmptyLH") ? true : false;

		//Here we combine similar animations to determine a certain boolean; for example, any fence climbing animation instead of checking each individual one when referencing the script

		if((inClimbDown1 || inClimbDown2 || inClimbLeft1 || inClimbLeft2 || inClimbRight1 || inClimbRight2 || inClimbStill1 || 
		   inClimbStill2 || inClimbUp1 || inClimbUp2) ? inAClimbingAnim = true : inAClimbingAnim = false);

		if((inGrabStill || inGrabPull || inGrabPush) ? inABoxPushingAnim = true : inABoxPushingAnim = false);

		if((inLowJumpLedge || inMidLedge || inPullUpLedge || inGrabBeneath) ? inALedgeClimbAnim = true : inALedgeClimbAnim = false);

		if((inJump || inSecondJump || inDoubleJump || inSidewaysJump) ? inAJumpAnim = true : inAJumpAnim = false);

		if((inSideHopLeft || inSideHopRight || inBackFlip) ? inATargJumpAnim = true : inATargJumpAnim = false);

		if((inDiving || inBreathe || inSwimFlip || inSwimImpulse) ? inASwimmingAnimation = true : inASwimmingAnimation = false);

		if((inLifting || inInLift || inLiftDown || inThrow) ? inALiftingAnimation = true : inALiftingAnimation = false);

		if((inSwordSwing || inSwordCharge || inSwordSpin || inReachItem ||
		   inShieldDefend || inBowUp || inBowDown) ? inAnItemUsageAnim = true : inAnItemUsageAnim = false);
	}
}
