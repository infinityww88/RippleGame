using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class RocketGlobal
{
	public static Action OnRightOperateDown;
	public static Action OnRightOperateUp;
	
	public static Action OnLeftOperateDown;
	public static Action OnLeftOperateUp;
	
	public static Action OnReloadScene;
	
	public static Action OnLandingSuccess;
	public static Action<bool, float> OnLandingResult;
	public static Action OnLandingFail;
	public static Action OnSunLightUp;
	public static Action OnGemMerged;
	public static Action OnRocketHit;
	public static Action OnNewZodiac;
	public static Action OnPause;
	public static Action OnResume;
	public static Action OnLaunch;
	public static Action<bool> OnGameOverlayActivated;
	public static Action<bool, bool> OnShowTrail;
	public static bool IsPaused { get; set; }
	public static bool InTutorial { get; set; }
	public static bool IsCompleted { get; set; }
	public static Action<bool> OnMusicSet;
	public static Action<bool> OnSoundSet;
}
