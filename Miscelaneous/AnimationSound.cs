using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AnimationSound : MonoBehaviour {

	[Header("Link Voices")]
	public AudioSource VoiceAudioSource;
	public AudioClip Ahh;
	public AudioClip[] Attacks;

	public AudioClip Jump1;
	public AudioClip Jump2;
	public AudioClip LedgeClimb1;
	public AudioClip LedgeFall1;
    public AudioClip CriticalVoice;


	[Header("Link SFX")]
	public AudioSource SFXAudioSource;
	public AudioClip Dive;
	public AudioClip Roll;
	public AudioClip RollWall;
	public AudioClip Surface;
	public AudioClip Swim1;
	public AudioClip Swim2;
	public AudioClip EquipJiggle;
    public AudioClip VinesSound;
    public AudioClip DamageSound;
    public AudioClip Slash;

    [Header("Camera Shake Effect")]
    public CameraFilterPack_FX_EarthQuake shakeEffect;
	void Start(){

        shakeEffect = GameObject.Find("Main Camera").GetComponent<CameraFilterPack_FX_EarthQuake>();
	
	}

	/// <summary>
	/// Rolls the sound.
	/// </summary>
	/// <param name="value">Value.</param>
	void RollSound(float value = 1f)
	{
		int Index = Random.Range (0, Attacks.Length);
		Debug.Log (value);
		VoiceAudioSource.PlayOneShot(Attacks[Index],1f);
		SFXAudioSource.PlayOneShot (Roll, 1f);
	}

    void AttackSound(float value = 1f)
    {
        int Index = Random.Range(0, Attacks.Length);
        Debug.Log(value);
        VoiceAudioSource.PlayOneShot(Attacks[Index], 1f);
        //SFXAudioSource.PlayOneShot(Roll, 1f);
    }

    void SlashSound(float value= 1f)
    {
        SFXAudioSource.PlayOneShot(Slash);
    }
    /// <summary>
    /// Ahhs the sound.
    /// </summary>
    /// <param name="value">Value.</param>
    void AhhSound(float value = 1f)
	{
		SFXAudioSource.PlayOneShot (Surface, 1f);
		VoiceAudioSource.PlayOneShot (Ahh, 1f);
	
	}
	/// <summary>
	/// Jumps the sound.
	/// </summary>
	/// <param name="value">Value.</param>
	void JumpSound(float value = 1f)
	{

		VoiceAudioSource.PlayOneShot (Jump1, 1f);

	}
	/// <summary>
	/// Ledges the clim sound.
	/// </summary>
	/// <param name="value">Value.</param>
	void LedgeClimSound(float value = 1f)
	{

		VoiceAudioSource.PlayOneShot (LedgeClimb1, 1f);

	}
	/// <summary>
	/// Ledges the fall sound.
	/// </summary>
	/// <param name="value">Value.</param>
	void LedgeFallSound(float value = 1f)
	{

		VoiceAudioSource.PlayOneShot (LedgeFall1, 1f);

	}

	/// <summary>
	/// Rolls the wall sound.
	/// </summary>
	/// <param name="value">Value.</param>
	void RollWallSound(float value = 1f)
	{

		VoiceAudioSource.PlayOneShot (RollWall, 1f);
		SFXAudioSource.PlayOneShot (LedgeClimb1, 1f);
        shakeEffect.enabled = true;

	}

	void SwimSound(float value = 1f)
	{


		SFXAudioSource.PlayOneShot (Swim1, 1f);

	}

	void DiveSound(float value = 1f)
	{


		SFXAudioSource.PlayOneShot (Dive, 1f);

	}

    void PlayVinesSound(float value = 1f)
    {
        SFXAudioSource.PlayOneShot(VinesSound, 1f);
    }

    void StopShakeEffect()
    {
        shakeEffect.enabled = false;
    }
	void EquipementJiggle(float value = .8f)
	{


		SFXAudioSource.PlayOneShot (EquipJiggle, 0.8f);

	}

	void BigJumpToLedgeSound(float value = 1f)
	{

		VoiceAudioSource.PlayOneShot (Jump1, 1f);
		SFXAudioSource.PlayOneShot (Jump2, 1f);

	}

    void PlayFallDamageSound(float value = 1f)
    {
        shakeEffect.enabled = true;
        VoiceAudioSource.PlayOneShot(DamageSound, 1f);
        SFXAudioSource.PlayOneShot(RollWall, 1f);
    }

    void PlayCriticalVoice(float value = 1f)
    {
        VoiceAudioSource.PlayOneShot(CriticalVoice, 1f);
    }

}


