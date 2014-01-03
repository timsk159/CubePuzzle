using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PoolableAudioSource
{
	public GameObject go;
	public AudioSource source;
}

public class AudioController : MonoSingleton<AudioController>
{
	public AudioSource musicAudioSource;
	public AudioClip currentMusic;

	List<PoolableAudioSource> audioSourcePool;
	int maxPoolSize = 5;

	void Start()
	{
		if(musicAudioSource == null)
		{
			musicAudioSource = Camera.main.audio;
		}
		if(musicAudioSource == null)
		{
			//add audio listener.
			musicAudioSource =	Camera.main.gameObject.AddComponent<AudioSource>();
		}
	}

	public void PlayMusic(AudioClip clip)
	{
		if(musicAudioSource.isPlaying)
		{
			TransitionAudio(clip, musicAudioSource);
			return;
		}
		else
		{
			musicAudioSource.clip = clip;
			musicAudioSource.Play();
		}
	}

	public void PlaySound(AudioClip clip)
	{
		var availableSource = audioSourcePool.Where(s => !s.source.isPlaying).FirstOrDefault();
		if(availableSource == null)
		{
			availableSource = CreateNewPoolSource();
		}

		PlaySound(clip, availableSource.source);

	}

	public void PlaySound(AudioClip clip, AudioSource source)
	{
		source.clip = clip;
		source.Play();

		CheckPoolSize();
	}

	public void StopSound(AudioClip clip)
	{
		var source = audioSourcePool.Where(s => s.source.clip == clip).FirstOrDefault();
		if(source == null)
			Debug.LogError("Tried to stop a sound that wasn't playing");
		else
		{
			StopSound(clip, source.source);
		}
	}

	public void StopSound(AudioClip clip, AudioSource source)
	{
		if(source.clip != clip)
			Debug.LogError("Tried to stop a clip from playing on a source that wasn't playing the specified clip");

		source.Stop();
	}

	PoolableAudioSource CreateNewPoolSource()
	{
		var returnSource = new PoolableAudioSource();
		var go = new GameObject("PooledAudioSource " + (audioSourcePool.Count + 1));
		var source = go.AddComponent<AudioSource>();

		go.transform.parent = Camera.main.transform;
		go.transform.localPosition = Vector3.zero;

		returnSource.go = go;
		returnSource.source = source;

		audioSourcePool.Add(returnSource);
		return returnSource;
	}

	void CheckPoolSize()
	{
		if(audioSourcePool.Count > maxPoolSize)
		{
			Debug.LogError("Too many audio sources pooled, this many are currently pooled: " + audioSourcePool.Count + " The maximum is: " + maxPoolSize);
		}
	}

	void TransitionAudio(AudioClip newClip, AudioSource source)
	{
		throw new System.NotImplementedException();
	}

}
