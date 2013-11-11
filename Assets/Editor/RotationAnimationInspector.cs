using UnityEditor;
using UnityEngine;
using System.Collections;

[ExecuteInEditMode()]
[CustomEditor(typeof(Rotation))]
public class RotationAnimationInspector : Editor 
{
	Rotation _target;
	bool previousRandomValue;

	void Awake()
	{
		_target = target as Rotation;
		previousRandomValue = _target.random;
		//if(_target.scale == null)
		//	_target.scale = new Vector3();
	}

	public override void OnInspectorGUI()
	{
		//scale, speed, time, easetype, playonstart
		_target.scale = EditorGUILayout.Vector3Field("Scale", _target.scale);
		_target.speed = EditorGUILayout.FloatField("Speed", _target.speed);
		_target.time = EditorGUILayout.FloatField("Time", _target.time);
		_target.easeType = (iTween.EaseType)EditorGUILayout.EnumPopup("Ease type", _target.easeType);
		_target.playOnStart = EditorGUILayout.Toggle("Play on start", _target.playOnStart);

		_target.random = EditorGUILayout.Toggle("Random animation?", _target.random);

		if(previousRandomValue != _target.random)
		{
			ResetVariables();
			previousRandomValue = _target.random;
		}

		if(!_target.random)
		{
			//looptype
			_target.loopType = (iTween.LoopType)EditorGUILayout.EnumPopup("Loop type", _target.loopType);
		}
		else
		{
			//randommin, randommax
			_target.randomMin = EditorGUILayout.FloatField("Random Min", _target.randomMin);
			_target.randomMax = EditorGUILayout.FloatField("Random Max", _target.randomMax);
		}
	}

	void ResetVariables()
	{
		_target.loopType = iTween.LoopType.none;
		_target.randomMin = 0;
		_target.randomMax = 0;
	}
}
