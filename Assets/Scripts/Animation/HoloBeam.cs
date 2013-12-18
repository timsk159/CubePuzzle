using UnityEngine;
using System.Collections;

public class HoloBeam : MonoBehaviour 
{
	Material mat;
	Light beamLight;

	//Brightness & RimPower
	float initialBrightness;
	float initialRimPower;
	float initialLightIntensity;

	void Start()
	{
		mat = renderer.material;
		beamLight = GetComponentInChildren<Light>();

		initialBrightness = mat.GetFloat("_Brightness");
		initialRimPower = mat.GetFloat("_RimPower");

		if(beamLight != null)
		{
			initialLightIntensity = beamLight.intensity;
		}

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

	void OnBrightnessTweenUpdate(float newVal)
	{
		var deltaBrightness = newVal - initialBrightness;
		beamLight.intensity = initialLightIntensity + (deltaBrightness);
		mat.SetFloat("_Brightness", newVal);
	}

	void OnRimTweenUpdate(float newVal)
	{
		mat.SetFloat("_RimPower", newVal);
	}

	public void IntensifyRimAndBrightness()
	{
		iTween.Stop(gameObject);
		var oldRim = initialRimPower;
		var oldBrightness = initialBrightness;
		iTween.ValueTo(gameObject, iTween.Hash("from", initialRimPower, "to", initialRimPower * 2, "time", 0.2f, "onupdate", "OnRimTweenUpdate"));
		iTween.ValueTo(gameObject, iTween.Hash("from", initialBrightness, "to", initialBrightness * 5.0f, "time", 0.2f, "onupdate", "OnBrightnessTweenUpdate",
		                                       "oncomplete", "IntensifyBrightnessComplete"));

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
		iTween.ValueTo(gameObject, iTween.Hash("from", currentBrightness, "to", initialBrightness, "time", 0.1f, "onupdate", "OnBrightnessTweenUpdate",
		                                       "oncomplete", "DeIntensifyBrightnessComplete"));
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
