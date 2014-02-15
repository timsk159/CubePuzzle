using UnityEngine;
using System.Collections;

public class HoloBeam : MonoBehaviour 
{
	Material mat;

	//Brightness & RimPower
	float initialBrightness;
	float initialRimPower;

	void Start()
	{
		mat = renderer.material;

		initialBrightness = mat.GetFloat("_Brightness");
		initialRimPower = mat.GetFloat("_RimPower");

		DoBrightnessTween();
	//	DoRimTween();
	}

	void DoBrightnessTween()
	{
		var oldBrightness = initialBrightness;
		var newBrightness = initialBrightness + 0.25f;

		iTween.ValueTo(gameObject, iTween.Hash("from", oldBrightness, "to", newBrightness, "onupdate", "OnBrightnessTweenUpdate", "looptype", iTween.LoopType.pingPong));
	}

	void DoRimTween()
	{
		var oldRim = initialRimPower;
		var newRimPower = initialRimPower + 0.5f;

		iTween.ValueTo(gameObject, iTween.Hash("from", oldRim, "to", newRimPower, "onupdate", "OnRimTweenUpdate", "looptype", iTween.LoopType.pingPong));
	}

	void OnRimTweenUpdate(float newVal)
	{
		mat.SetFloat("_RimPower", newVal);
	}

	public void IntensifyRimAndBrightness()
	{
		iTween.Stop(gameObject);
		iTween.ValueTo(gameObject, iTween.Hash("from", initialRimPower, "to", initialRimPower * 2, "time", 0.2f, "onupdate", "OnRimTweenUpdate"));
	}

	void IntensifyBrightnessComplete()
	{
		DeIntensifyBrightnessAndRim();
	}

	void DeIntensifyBrightnessAndRim()
	{
		var currentBrightness = mat.GetFloat("_Brightness");
		var currentRim = mat.GetFloat("_RimPower");

		iTween.ValueTo(gameObject, iTween.Hash("from", currentRim, "to", initialRimPower, "time", 0.1f, "onupdate", "OnRimTweenUpdate",
		                                       "oncomplete", "DeIntensifyRimComplete"));
	}

	void DeIntensifyBrightnessComplete()
	{
		mat.SetFloat("_Brightness", initialBrightness);
		DoBrightnessTween();
	}

	void DeIntensifyRimComplete()
	{
		mat.SetFloat("_RimPower", initialRimPower);
		//DoRimTween();
	}

	/*
	void OnRimTweenComplete()
	{
		DoRimTween();
	}

	void OnBrightnessTweenComplete()
	{
		DoBrightnessTween();
	}
	*/
}
