using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PoolableAudioSource
{
	public GameObject go;
	public AudioSource source;
}

public class PooledAudioController : MonoBehaviour
{
	static float musicVolume;
	static float effectsVolume;

	public AudioSource musicAudioSource;
	public AudioClip currentMusic;

	List<PoolableAudioSource> audioSourcePool;
	Dictionary<AudioClip, PoolableAudioSource> activeClips;
	int initialPoolSize = 2;
	int maxPoolSize = 5;

	private static PooledAudioController _instance;

	public static PooledAudioController Instance 
	{
		get
		{
			if(_instance == null)
			{
				_instance = (PooledAudioController)FindObjectOfType(typeof(PooledAudioController));

				if(_instance == null)
				{
					_instance = new GameObject("(Singleton)" + typeof(PooledAudioController).Name).AddComponent<PooledAudioController>();
				}
			}
			return _instance;
		}
	}

	public void SetMusicVolume(float newMusicVolume)
	{
		musicVolume = (newMusicVolume / 2.5f);
		if(musicAudioSource != null)
			musicAudioSource.volume = musicVolume;
	}

	public void SetEffectsVolume(float newEffectsVolume)
	{
		effectsVolume = newEffectsVolume;
		if(audioSourcePool != null)
		{
			foreach(var pooledSource in audioSourcePool)
			{
				pooledSource.source.volume = effectsVolume;
			}
		}
	}

	void Awake()
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

	void OnLevelWasLoaded(int levelID)
	{
		Awake();
	}

	public void PlayMusic(AudioClip clip)
	{
#if DEBUG_AUDIO
		Debug.Log("Play Music clip " + clip.name);
#endif
		if(musicAudioSource.clip == clip)
		{
			musicAudioSource.loop = true;
			musicAudioSource.Play();
			return;
		}

		if(musicAudioSource.isPlaying)
		{
			TransitionAudio(clip, musicAudioSource);
			return;
		}
		else
		{
			musicAudioSource.loop = true;
			musicAudioSource.volume = musicVolume;
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

		var availableSource = FindSource(clip);
		PlaySound(clip, availableSource.source);
	}

	public void PlaySound(AudioClip clip, float volume)
	{
#if DEBUG_AUDIO
		Debug.Log("Requested sound to be played: " + clip.name);
#endif
		if(audioSourcePool == null)
		{
			InitPool();
		}

		var availableSource = FindSource(clip);

		PlaySound(clip, availableSource.source, volume);
	}

	public void PlaySound(AudioClip clip, AudioSource source)
	{
#if DEBUG_AUDIO
		Debug.Log("Playing sound: " + clip.name);
#endif
		AddClipToActiveClips(clip, source);

		source.volume = effectsVolume;
		source.clip = clip;
		source.Play();

		CheckPoolSize();
	}

	public void PlaySound(AudioClip clip, AudioSource source, float volume)
	{
		#if DEBUG_AUDIO
		Debug.Log("Playing sound: " + clip.name);
		#endif

		AddClipToActiveClips(clip, source);

		source.volume = volume;
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
		PoolableAudioSource returnSource = null;

		if(activeClips.ContainsKey(clip))
			returnSource = activeClips[clip];

		return returnSource;
	}

	PoolableAudioSource GetFreeSource()
	{
		var returnSource = audioSourcePool.Where(s => s.source.clip == null).FirstOrDefault();

		return returnSource;
	}

	PoolableAudioSource FindSource(AudioClip clip)
	{
		var availableSource = GetSourceWithClip(clip);
		if(availableSource == null)
		{
			availableSource = GetFreeSource();
			if(availableSource == null)
				availableSource = CreateNewPoolSource();
		}

		return availableSource;
	}

	void AddClipToActiveClips(AudioClip clip, AudioSource source)
	{
		var pooledSource = audioSourcePool.Where(s => s.source == source).FirstOrDefault();
		if(activeClips.ContainsKey(clip))
			activeClips[clip] = pooledSource;
		else
			activeClips.Add(clip, pooledSource);
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
		activeClips = new Dictionary<AudioClip, PoolableAudioSource>();
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
