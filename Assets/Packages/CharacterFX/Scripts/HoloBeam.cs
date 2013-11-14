using UnityEngine;
using System.Collections;

public class HoloBeam : MonoBehaviour 
{
	Material mat;
	Light beamLight;

	//Brightness & RimPower
	float brightness;
	float rimPower;
	float initialLightIntensity;

	void Start()
	{
		mat = renderer.material;
		beamLight = GetComponentInChildren<Light>();

		brightness = mat.GetFloat("_Brightness");
		rimPower = mat.GetFloat("_RimPower");

		if(beamLight != null)
		{
			initialLightIntensity = beamLight.intensity;
		}

		DoBrightnessTween();
		DoRimTween();
	}

	void DoBrightnessTween()
	{
		var oldBrightness = brightness;
		brightness -= 0.5f;

		iTween.ValueTo(gameObject, iTween.Hash("from", oldBrightness, "to", brightness, "onupdate", "OnBrightnessTweenUpdate", "looptype", iTween.LoopType.pingPong));
	}

	void DoRimTween()
	{
		var oldRim = rimPower;
		rimPower += 0.5f;

		iTween.ValueTo(gameObject, iTween.Hash("from", oldRim, "to", rimPower, "onupdate", "OnRimTweenUpdate", "looptype", iTween.LoopType.pingPong));
	}

	void OnBrightnessTweenUpdate(float newVal)
	{
		var deltaBrightness = newVal - brightness;
		beamLight.intensity = initialLightIntensity + (deltaBrightness);
		mat.SetFloat("_Brightness", newVal);
	}

	void OnRimTweenUpdate(float newVal)
	{
		mat.SetFloat("_RimPower", newVal);
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
