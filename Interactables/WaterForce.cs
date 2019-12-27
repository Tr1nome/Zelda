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

public class WaterForce : MonoBehaviour {

	private GameObject mainCam;                             //The main camera in the scene
    private CameraFollower camF;
	[HideInInspector] public int waterBoxID;									//The unique identifier for each body of water using their own WaterForce script
	[SerializeField] private Color underwaterFogColour;		//The color of the fog inside of the waterbox
	[SerializeField] private float underwaterFogDensity;	//The density of the fog inside of the waterbox

	public bool setOutsideFogOnStart;					//Should the below values be set automatically at Start() by checking the render settings?
	[SerializeField] private Color outsideFogColor;		//The color of the fog outside of the waterbox
	[SerializeField] private float outsideFogDensity;   //The density of the fog outside of the waterbox

	void Start () {
		mainCam = GameObject.FindWithTag ("MainCamera");
        camF = mainCam.GetComponent<CameraFollower>();
		if (setOutsideFogOnStart) {
			outsideFogColor = RenderSettings.fogColor;
			outsideFogDensity = RenderSettings.fogDensity;
		}
	}

	void Update() {
		if (mainCam == null) {
			mainCam = GameObject.FindWithTag ("MainCamera");
			bool hasError = false;
			if(mainCam == null){
				Debug.LogError ("WaterForce.cs: Your scene is missing a main camera object. Please add a Main Camera to your scene.");
				hasError = true;
			}
			if(hasError){
                #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
				Debug.DebugBreak();
				Application.Quit();
                #endif
            }
		}
	}

	/// <summary>
	/// Sets the fog color and density when the camera enters the trigger zone
	/// </summary>
    void OnTriggerStay(Collider other){
        if (camF.currentWB != this.gameObject && other.gameObject == mainCam){
            camF.currentWB = this.gameObject;
            RenderSettings.fogColor = underwaterFogColour;
            RenderSettings.fogDensity = underwaterFogDensity;
            camF.camUnderWater = true;
        }
    }

	/// <summary>
	/// Reverts the fog color and density when the camera enters the trigger zone
	/// </summary>
    void OnTriggerExit(Collider other){
        if (other.gameObject.tag == "Player")
        {
            gameObject.layer = 4;
        }
        if (camF.camUnderWater && camF.currentWB == this.gameObject && other.gameObject == mainCam){
			RenderSettings.fogColor = outsideFogColor;
			RenderSettings.fogDensity = outsideFogDensity;
            camF.camUnderWater = false;
            if (camF.currentWB == this.gameObject)
                camF.currentWB = null;
        }
    }
}
