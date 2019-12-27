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
using ORKFramework;

public class UsingItems1 : MonoBehaviour
{

    public int rightHandAnimatorLayerID;            //The animation layer of the right hand of the character that is animated (as a mask) when handling an item
    public int leftHandAnimatorLayerID;             //The animation layer of the left hand of the character that is animated (as a mask) when handling an item
    private bool hasSetLayerWeight;
    private float lerpTo;

    [HideInInspector] public Item currentItem;      //The item the character is currently using
    [HideInInspector] public Item rightHandItem;    //The item the character is holding in their right hand
    [HideInInspector] public Item leftHandItem;     //The item the character is holding in their left hand
    [HideInInspector] public HUDWrapper hudScript;
    //[HideInInspector] public Inventory inventory;
    [HideInInspector] public GamePadInputs gPI;
    [HideInInspector] public Animations currentAnims;
    [HideInInspector] public bool buttonBeingHeld;
    private string itemBeingHeld;
    [HideInInspector] public ThirdPersonController character;
    [HideInInspector] public Camera cam;
    private CameraFollower camF;
    [HideInInspector] public int hotkeysAmount;
    [HideInInspector] public Dictionary<int, GameObject> modelObjPool = new Dictionary<int, GameObject>();
    [HideInInspector] public GameObject poolObj;
    [HideInInspector] public GameObject poolProjectile;

    [HideInInspector] public bool canUseItems;
    [HideInInspector] public bool isUnequipping;

    private Dictionary<int, GameObject> arrowsPool = new Dictionary<int, GameObject>();
    private Dictionary<int, GameObject> bombPool = new Dictionary<int, GameObject>();
    [HideInInspector] public Dictionary<int, GameObject> explosionPool = new Dictionary<int, GameObject>();
    [Header("Test")]
    private float bowAngle;
    private Combatant m_Combatant;
    [Header("Bow Settings")]
    private bool holdingArrow;
    private bool bendingBow;
    public AudioClip bowDown;
    public AudioClip bowUp;
    public AudioClip noArrow;
    public GameObject ArrowPrefab;
    private GameObject ArrowSpawnPos;
    private GameObject SpawnedArrow;
    public Vector3 Direction;
    private bool arrowAttached;
    [Header("Hookshot")]
    public bool pressedKey;
    public GameObject hook;
    public GameObject hookHolder;

    public float hookSpeed;
    public float travelSpeed;
    public bool hooked;
    public bool isHookTraveling;
    public float hookAngle;
    public float maxDistance;
    private float currentDistance;
    public AudioClip hookCharged;
    public AudioClip hookReleased;
    public AudioClip hookCollision;
    public AudioClip hookCollisionError;


    void Start()
    {
        character = GameObject.FindWithTag("Player").GetComponent<ThirdPersonController>();
        m_Combatant = ComponentHelper.GetCombatant(gameObject);
        currentItem = null;
        rightHandItem = null;
        leftHandItem = null;
        buttonBeingHeld = false;
        itemBeingHeld = "";
        cam = Camera.main;
        camF = cam.GetComponent<CameraFollower>();

        gPI = this.GetComponent<GamePadInputs>();
        currentAnims = this.GetComponent<Animations>();
        canUseItems = true;
        lerpTo = 1f;
    }

    void Update()
    {
        ////Set miscelaneous settings here for while certain items are equipped
        hudScript = GameObject.Find("HUDWrapper").GetComponent<HUDWrapper>();
        ArrowSpawnPos = GameObject.Find("ArrowSpawn");
        HasArrows();
        hookHolder = GameObject.Find("HookHolder");
        hook = GameObject.Find("Hook");
        Debug.Log(HasArrows());

        /** BOW **/
        if (character.anim.GetBool("useBow"))
        {
            
            if (gPI.RT > 0.5f && holdingArrow == false && HasArrows())
            {
                holdingArrow = true;

                StartCoroutine(HoldArrow());
                if (holdingArrow)
                {
                    character.anim.SetBool("bendBow", true);
                    SpawnedArrow.transform.parent = ArrowSpawnPos.transform;
                    SpawnedArrow.transform.localPosition = Vector3.zero;
                    
                    

                }


            }
            else if (gPI.RT > 0.5f && !bendingBow && !HasArrows())
            {
                character.anim.SetBool("bendBow", true);
                bendingBow = true;
                StartCoroutine(BendBow());
            }
            if (gPI.RT < 0.5f && holdingArrow)
            {
                holdingArrow = false;

                if (!holdingArrow)
                {
                    character.anim.SetBool("bendBow", false);
                    character.anim.SetBool("ShootArrow", true);
                    SpawnedArrow.transform.parent = null;
                    SpawnedArrow.AddComponent<Rigidbody>();
                    SpawnedArrow.GetComponent<Rigidbody>().useGravity = true;
                    SpawnedArrow.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                    SpawnedArrow.GetComponent<Rigidbody>().AddRelativeForce(Direction, ForceMode.Force);

                    StartCoroutine(ReleaseArrow());
                }



            }
            else if (gPI.RT < 0.5f && bendingBow && !HasArrows())
            {
                character.anim.SetBool("bendBow", false);
                character.anim.SetBool("ShootArrow", true);
                bendingBow = false;
                StartCoroutine(BowEmpty());
            }
            if (character.canFPV)
                character.canFPV = false;
            if (camF.fpvMode)
            {
                bowAngle = cam.transform.localEulerAngles.x + 90f;
                if (bowAngle > 180f)
                    bowAngle -= 360f;
                if (character.fPVing == false)
                    character.fPVing = true;
            }
            if (camF.targetMode)
            {
                if (character.fPVing)
                    character.fPVing = false;
                if (character.currentTarget != null)
                {
                    bowAngle = Vector3.Angle(character.currentTarget.transform.position - character.transform.position, Vector3.up);
                    if (bowAngle > 180f)
                        bowAngle -= 360f;
                }
                else
                    bowAngle = 90f;
                if (bowAngle > 165f)
                    bowAngle = 165f;
                if (bowAngle < 15f)
                    bowAngle = 15f;
            }
            character.anim.SetFloat("bowAngle", bowAngle);
            if (camF.behindMode || camF.freeMode)
            {
                camF.behindMode = false;
                camF.freeMode = false;
            }
            if (!camF.targetMode && !camF.fpvMode || character.inACrouchBox)
            {
                character.fPVing = true;
                camF.fpvMode = true;
            }
            if (camF.fpvMode && !hudScript.aimGUIElement.gameObject.activeSelf)
            { ToggleAimGUIElement(true); }

            if (!camF.fpvMode && hudScript.aimGUIElement.gameObject.activeSelf)
            { ToggleAimGUIElement(false); }
        }
        if (!character.anim.GetBool("useBow") && hudScript.aimGUIElement.gameObject.activeSelf || !character.anim.GetBool("useHookshot") && hudScript.aimGUIElement.gameObject.activeSelf)
            ToggleAimGUIElement(false);
        
        /** BOW **/

        /** HOOKSHOT **/
        /*if (character.anim.GetBool("useHookshot"))
        {

            if (gPI.RT > 0.5f && pressedKey == false)
            {
                pressedKey = true;

                StartCoroutine(KeyPressed());
            }

            if (gPI.RT < 0.5f && pressedKey && !hooked)
            {
                MoveHookShot();

                //StartCoroutine(KeyRealeased());
                //pressedKey = false;

            }
            if (character.canFPV)
                character.canFPV = false;
            if (camF.fpvMode)
            {
                bowAngle = cam.transform.localEulerAngles.x + 90f;
                if (bowAngle > 180f)
                    bowAngle -= 360f;
                if (character.fPVing == false)
                    character.fPVing = true;
            }
            if (camF.targetMode)
            {
                if (character.fPVing)
                    character.fPVing = false;
                if (character.currentTarget != null)
                {
                    bowAngle = Vector3.Angle(character.currentTarget.transform.position - character.transform.position, Vector3.up);
                    if (bowAngle > 180f)
                        bowAngle -= 360f;
                }
                else
                    bowAngle = 90f;
                if (bowAngle > 165f)
                    bowAngle = 165f;
                if (bowAngle < 15f)
                    bowAngle = 15f;
            }
            character.anim.SetFloat("bowAngle", bowAngle);
            if (camF.behindMode || camF.freeMode)
            {
                camF.behindMode = false;
                camF.freeMode = false;
            }
            if (!camF.targetMode && !camF.fpvMode)
            {
                character.fPVing = true;
                camF.fpvMode = true;
            }
            if (camF.fpvMode && !hudScript.aimGUIElement.gameObject.activeSelf)
            { ToggleAimGUIElement(true); }

            if (!camF.fpvMode && hudScript.aimGUIElement.gameObject.activeSelf)
                ToggleAimGUIElement(false);
        
        if (!character.anim.GetBool("useHookshot") && hudScript.aimGUIElement.gameObject.activeSelf)
            ToggleAimGUIElement(false);


    }

        /** HOOKSHOT **/

        ////Unequipping
        if (gPI.LH == 0f && gPI.LV == 0f && gPI.pressAction && ORK.Game.Variables.GetBool("holdingItem") == true && m_Combatant.Equipment[1].Equipped)
        {
            m_Combatant.Equipment.Unequip(1, m_Combatant.Inventory, true, true);
            camF.exitFPV = true;
            camF.fpvMode = false;
            camF.inFPV = false;
            //character.fPVing = false;
            character.anim.SetBool("useBow", false);
            character.anim.SetBool("useHookshot", false);
            character.anim.SetTrigger("reach");
            character.anim.SetBool("equippedRight", false);
            character.anim.SetBool("bendBow", false);
            character.anim.SetBool("ShootArrow", false);
            ORK.Game.Variables.Set("holdingItem", false);
            holdingArrow = false;
            Destroy(SpawnedArrow);
            
            //this.enabled = false;
                

            
        }

        ////Animation Override
        if (currentAnims.inALedgeClimbAnim || currentAnims.inAClimbingAnim || currentAnims.inASwimmingAnimation || currentAnims.grabbingBlend ||
            currentAnims.inABoxPushingAnim || currentAnims.inATargJumpAnim ||
            character.onLedge || character.grabbing)
        {
            if (!hasSetLayerWeight)
            {
                hasSetLayerWeight = true;
                lerpTo = 0f;
            }
        }
        else
        {
            if (hasSetLayerWeight)
            {
                hasSetLayerWeight = false;
                lerpTo = 1f;
            }
        }

        if (Mathf.Abs(currentAnims.anim.GetLayerWeight(rightHandAnimatorLayerID) - lerpTo) > 0.1f)
        {
            currentAnims.anim.SetLayerWeight(rightHandAnimatorLayerID, Mathf.Lerp(currentAnims.anim.GetLayerWeight(rightHandAnimatorLayerID), lerpTo, Time.deltaTime * 5f));
            currentAnims.anim.SetLayerWeight(leftHandAnimatorLayerID, Mathf.Lerp(currentAnims.anim.GetLayerWeight(leftHandAnimatorLayerID), lerpTo, Time.deltaTime * 5f));
        }
        else
        {
            if (currentAnims.anim.GetLayerWeight(rightHandAnimatorLayerID) != lerpTo)
            {
                currentAnims.anim.SetLayerWeight(rightHandAnimatorLayerID, lerpTo);
                currentAnims.anim.SetLayerWeight(leftHandAnimatorLayerID, lerpTo);
            }
        }

        ////Restrictions
        if (character.onLedge || character.grabbing || character.climbing || character.inWater || character.isCrouching || character.inACutscene)
        {
            if (canUseItems)
            {
                canUseItems = false;
                currentItem = null;

            }
        }
        else
        {
            if (!canUseItems)
                canUseItems = true;
        }

    }




    /// <summary>
    /// The function that turns on the aiming element on the GUI Canvas
    /// </summary>
    public void ToggleAimGUIElement(bool turnOn)
    {
        hudScript.aimGUIElement.gameObject.SetActive(turnOn);
    }

    public bool HasArrows ()
    {
        return m_Combatant.Inventory.Items.GetCount(12) > 0;
    } 
    IEnumerator HoldArrow()
    {
        GetComponent<AudioSource>().PlayOneShot(bowDown);
        SpawnedArrow = Instantiate(ArrowPrefab, ArrowSpawnPos.transform.position, ArrowSpawnPos.transform.rotation);
        arrowAttached = true;
        SpawnedArrow.gameObject.name = "currentArrow";
        yield return new WaitForEndOfFrame();
    }

    IEnumerator KeyPressed()
    {
        GetComponent<AudioSource>().PlayOneShot(hookCharged);
        yield return new WaitForEndOfFrame();
    }

    IEnumerator KeyRealeased()
    {
        GetComponent<AudioSource>().PlayOneShot(hookReleased);
        yield return new WaitForEndOfFrame();
        
    }
    IEnumerator ReleaseArrow()
    {
        GetComponent<AudioSource>().PlayOneShot(bowUp);
        SpawnedArrow.GetComponent<DestroyAfterTime>().enabled = true;
        m_Combatant.Inventory.Items.Remove(12, 1, false, false);
        yield return new WaitForEndOfFrame();
        character.anim.SetBool("ShootArrow", false);
        //Destroy(SpawnedArrow,5f);
    }

    IEnumerator BendBow()
    {
        GetComponent<AudioSource>().PlayOneShot(bowDown);
        yield return new WaitForEndOfFrame();
    }

    IEnumerator BowEmpty()
    {
        GetComponent<AudioSource>().PlayOneShot(noArrow);
        yield return new WaitForEndOfFrame();
        character.anim.SetBool("ShootArrow", false);
    }

    void ReturnHook()
    {
        hook.transform.position = hookHolder.transform.position;
        pressedKey = false;
    }

    void MoveHookShot()
    {
        camF.enabled = false;
        hook.transform.Translate(Vector3.forward * Time.deltaTime * hookSpeed);
        currentDistance = Vector3.Distance(transform.position, hook.transform.position);

        if (currentDistance >= maxDistance)
        {
            ReturnHook();
            camF.enabled = true;
        }

        if(hooked)
        {
            transform.position = Vector3.MoveTowards(transform.position, hook.transform.position, travelSpeed);
            camF.enabled = true;
            float distanceToHook = Vector3.Distance(transform.position, hook.transform.position);

            if(distanceToHook < 1)
            {
                hooked = false;
                ReturnHook();
                camF.enabled = true;
            }
        }
    }

}


