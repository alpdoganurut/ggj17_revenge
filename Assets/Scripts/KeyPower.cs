﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KeyPower : MonoBehaviour
{
	[SerializeField]KeyCode keyCode;
	public int Index{
		get{
			return index;
		}
	}
	[SerializeField]int index = 0;
	[SerializeField]float power = 0;

	/// <summary>
	/// Moved all the way up.
	/// </summary>
	bool isSatisfied = false;

	LTDescr currentTween;

	public void Init(int index)
	{
		print ("New key Power!");

		this.index = index;
		this.keyCode = GameControlManager2.Instance.keys[index];

		// Block move up
		currentTween = LeanTween.value (power, 1, GameControlManager2.Instance.moveUpTime)
			.setEase (GameControlManager2.Instance.moveUpEasing)
//			.setOnComplete (() => {
//				
//			})
			.setOnUpdate((value)=>{
				
//				print ("Top Bounce");
				power = value;
				if(power >= GameControlManager2.Instance.topBouncePercent && !isSatisfied)
				{
					BouncePlayer();
					isSatisfied = true;
				}
			});

		StartCoroutine (ListenForRelease () );
	}



	IEnumerator ListenForRelease()
	{
		while(!Input.GetKeyUp(keyCode)) 
		{
			yield return true;
		}


		print ("Key Power Released, " + keyCode.ToString());
		Released ();
	}

	private void BouncePlayer()
	{
		foreach (var reactor in GameControlManager2.Instance.dynamicReactors) 
		{
			Vector3? force = GetForceForPosition (reactor.transform.position);

			if(force != null && !reactor.verticallyStatic)
			{
				Debug.DrawRay (reactor.transform.position, (Vector3)force * .1f, Color.red, .4f);
				reactor.Bounced((Vector3)force);
			}
		}

//		Player2 player = GameManager.Instance.player;
//		Vector3? force = GetForceForPosition (player.transform.position);
//
//		if(force != null)
//		{
//			GameManager.Instance.player.Bounced((Vector3)force);
//		}

	}

	public float GetKeyPos()
	{
//		float 	curH = Camera.main.orthographicSize * 2;
		float 	curW = GameManager.Instance.CameraBounds.size.x;
		float 	curH = GameManager.Instance.CameraBounds.size.y;


		// this pos is local to camera
		float 	keyDownPosLocal = -(curW / 2) + (index * GameManager.Instance.blockCountForKey) + (GameManager.Instance.blockCountForKey / 2);
		float 	keyDownPosGlobal = keyDownPosLocal += Camera.main.transform.position.x;

		return 	keyDownPosGlobal * 1.25f;
	}

	public float GetImpactForPos(Vector3 pos)
	{
//		float 	curH = Camera.main.orthographicSize * 2;
//		float 	curW = Camera.main.aspect * curH;
//
//		// this pos is local to camera
//		float 	keyDownPosLocal = -(curW / 2) + (index * GameManager.Instance.blockCountForKey) - 0.5f;
//		float 	keyDownPosGlobal = keyDownPosLocal += Camera.main.transform.position.x;

		float 	keyDownPosGlobal = GetKeyPos();

		float 	dif = Mathf.Abs (keyDownPosGlobal - pos.x);

		float 	influenceRadius = GameControlManager2.Instance.influenceRadius;
		float 	impactPercent = 1 - (dif / influenceRadius);

		return 	impactPercent;
	}

	public float GetElevationForPosition(Vector3 pos)
	{
		float impactPercent = GetImpactForPos (pos);

		float _targetElevation = 0;

		// Getting impacted
		//		if(isAnyKeyDown && impactPercent <= 1 && impactPercent > 0)
		if(impactPercent <= 1 && impactPercent > 0)
		{
			float maxInfluenceElevation = GameControlManager2.Instance.maxInfluenceElevation;
			_targetElevation = impactPercent * maxInfluenceElevation;
//			print ("Elevation position for posX: " + posX + " is " + _targetElevation * power);
		}

		return _targetElevation * power; 
	} 

	public Vector3? GetForceForPosition(Vector3 pos)
	{
//		float 	curH = Camera.main.orthographicSize * 2;
//		float 	curW = Camera.main.aspect * curH;
//
//		// this pos is local to camera
//		float 	keyDownPosLocal = -(curW / 2) + (index * GameManager.Instance.blockCountForKey) - 0.5f;
//		float 	keyDownPosGlobal = keyDownPosLocal += Camera.main.transform.position.x;
//		float 	dif = Mathf.Abs (keyDownPosGlobal - posX);
//
//		float 	influenceRadius = GameControlManager2.Instance.influenceRadius;
//		float 	impactPercent = 1 - (dif / influenceRadius);

		//		print ("keyDownPos = " + keyDownPos);
		//		print ("posX = " + posX);
		//		print ("dif = " + dif);
		//		print ("impactPercent = " + impactPercent);
		Vector3? force = null;

		float impactPercent = GetImpactForPos (pos);

		float maxMagn = GameControlManager2.Instance.bounceForceMaxMagnitude;
		float angle = (1 - impactPercent) * 90;


		if (impactPercent <= 1 && impactPercent > 0) 
		{
			// If at right
			if(pos.x > GetKeyPos())
			{
				angle = -angle;
			}

			force = (Vector2.up).Rotate (angle);
			
			force = ((Vector3)force).normalized * impactPercent * maxMagn * power;
		}


		return force;

//		return GameControlManager2.Instance.maxBounceForce * impactPercent;
	} 


	public void Released()
	{
		if(!isSatisfied)
		{
			print ("Release Boucne");
			BouncePlayer ();
//			GameManager.Instance.player.Bounced( GetElevationForPositionX(GameManager.Instance.player.gameObject.transform.position.x) * Vector3.up * GameControlManager2.Instance.bounceForceMult);
			// TODO: Bounce
		}

		GameControlManager2.Instance.KeyReleased(this);

		LeanTween.cancel (currentTween.uniqueId);

		// Block move down
		LeanTween.value (power, 0, GameControlManager2.Instance.moveDownTime)
//			.setDelay (.2f)
			.setEase (GameControlManager2.Instance.moveDownEasing)
			.setOnComplete (() => {
				GameControlManager2.Instance.KeyPowerDied(this);
				// TODO: Might need clean up
			}).setOnUpdate((value)=>{
				power = value;
			});
	}
}