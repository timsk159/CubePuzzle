using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PoolableAudioSource
{
	public GameObject go;
	public AudioSource source;
}

public class PooledAudioController : MonoSingleton<PooledAudioController>
{
	public AudioSource musicAudioSource;
	public AudioClip currentMusic;

	List<PoolableAudioSource> audioSourcePool;
	int initialPoolSize = 2;
	int maxPoolSize = 5;

	void Start()
	{
#if DEBUG_AUDIO
		Debug.Log("Pooled Audio Start");
#endif
		if(musicAudioSource == null)
		{
			musicAudioSource = Camera.main.audio;
		}
		if(musicAudioSource == null)
		{
			//add audio listener.
			musicAudioSource =	Camera.main.gameObject.AddComponent<AudioSource>();
		}
		if(audioSourcePool == null)
		{
			InitPool();
		}
	}

	public void PlayMusic(AudioClip clip)
	{
#if DEBUG_AUDIO
		Debug.Log("Play Music clip " + clip.name);
#endif
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
#if DEBUG_AUDIO
		Debug.Log("Requested sound to be played: " + clip.name);
#endif
		if(audioSourcePool == null)
		{
			InitPool();
		}

		var availableSource = GetSourceWithClip(clip);
		if(availableSource == null)
		{
			availableSource = CreateNewPoolSource();
		}

		PlaySound(clip, availableSource.source);

	}

	public void PlaySound(AudioClip clip, AudioSource source)
	{
#if DEBUG_AUDIO
		Debug.Log("Playing sound: " + clip.name);
#endif

		source.clip = clip;
		source.Play();

		CheckPoolSize();
	}

	public void PlaySoundLoop(AudioClip clip)
	{
#if DEBUG_AUDIO
		Debug.Log("Requested sound loop to be played: " + clip.name);
#endif

		var source = GetSourceWithClip(clip);
		if(source != null && !source.source.isPlaying)
		{
			source.source.Play();
		}
		else if(source == null)
		{
			PlaySound(clip);
		}
	}

	public void PauseSoundLoop(AudioClip clip)
	{
#if DEBUG_AUDIO
		Debug.Log("Pausing sound loop: " + clip.name);
#endif

		var source = GetSourceWithClip(clip);
		if(source != null && source.source.isPlaying)
			source.source.Pause();

	}

	public bool IsPlaying(AudioClip clip)
	{
		var source = GetSourceWithClip(clip);
		if(source != null && source.source.isPlaying)
		{
			return true;
		}
		else
			return false;
	}

	public void StopSound(AudioClip clip)
	{
		var source = GetSourceWithClip(clip);
		if(source == null)
			Debug.LogError("Tried to stop a sound that wasn't playing");
		else
		{
			StopSound(clip, source.source);
		}
	}

	public void StopSound(AudioClip clip, AudioSource source)
	{
#if DEBUG_AUDIO
		Debug.Log("Stopping audio clip: " + clip.name);
#endif

		if(source.clip != clip)
			Debug.LogError("Tried to stop a clip from playing on a source that wasn't playing the specified clip");

		source.Stop();
	}

	void TransitionAudio(AudioClip newClip, AudioSource source)
	{
#if DEBUG_AUDIO
		Debug.Log("Transitioning music from: " + source.clip.name + " To: " + newClip.name);
#endif
		//Check docs for AudioSource.PlaySchedueled();

		throw new System.NotImplementedException();
	}

	PoolableAudioSource CreateNewPoolSource()
	{
#if DEBUG_AUDIO
		Debug.Log("Creating new Audio Pool Source");
#endif
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

	PoolableAudioSource GetSourceWithClip(AudioClip clip)
	{
		var returnSource = audioSourcePool.Where(s => s.source.clip == clip).FirstOrDefault();

		return returnSource;
	}

	void CheckPoolSize()
	{
		if(audioSourcePool.Count > maxPoolSize)
		{
			Debug.LogError("Too many audio sources pooled, this many are currently pooled: " + audioSourcePool.Count + " The maximum is: " + maxPoolSize);
		}
	}

	void InitPool()
	{
#if DEBUG_AUDIO
		Debug.Log("Initializing Audio Pool");
#endif

		CleanPool();
		audioSourcePool = new List<PoolableAudioSource>(initialPoolSize);
		for(int i = 1; i <= initialPoolSize; i++)
		{
			var newPoolSource = new PoolableAudioSource();
			var go = new GameObject("PooledAudioSource " + i);
			var source = go.AddComponent<AudioSource>();

			go.transform.parent = Camera.main.transform;
			go.transform.localPosition = Vector3.zero;

			newPoolSource.go = go;
			newPoolSource.source = source;

			audioSourcePool.Add(newPoolSource);
		}
	}

	void CleanPool()
	{
#if DEBUG_AUDIO
		Debug.Log("Cleaning audio pool");
#endif

		if(audioSourcePool == null || audioSourcePool.Count == 0)
			return;

		foreach(var pooledSource in audioSourcePool)
		{
			Destroy(pooledSource.go);
		}
		audioSourcePool.Clear();
	}

}
