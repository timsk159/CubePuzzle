using UnityEngine;
using System.Collections;

public class AudioPlayer : MonoBehaviour 
{
	bool checkPlayerMovement;

	PlayerCharacter playerChar;

	public AudioClip playerMoveSound;
	public AudioClip playerChangedColourSound;
	public AudioClip floorChangedColourSound;
	public AudioClip checkpointSound;

	public AudioClip hitWallSound;

	public AudioClip menuMusic;
	public AudioClip inGameMusic;

	//For performance, the players sound source isn't pooled
	AudioSource playerMoveSource;

	void Awake()
	{
		Messenger.AddListener(LevelStateMessage.LevelStarted.ToString(), LevelStarted);
	}

	void OnDestroy()
	{
		RemoveEventListeners();
	}

	void Start()
	{
		if(Application.loadedLevelName == "FrontMenu")
			PooledAudioController.Instance.PlayMusic(menuMusic);
	}

	void OnDeserialized()
	{
		Debug.Log("+++++----- Deserialized called!");
	}

	void Init()
	{
		AddEventListeners();
		if(playerMoveSource == null)
		{
			var playerMoveSourceGO = new GameObject("PlayerMoveAudioSource");
			playerMoveSourceGO.transform.parent = Camera.main.transform;
			playerMoveSourceGO.transform.localPosition = Vector3.zero;
			playerMoveSource = playerMoveSourceGO.AddComponent<AudioSource>();
			playerMoveSource.clip = playerMoveSound;
		}

		if(playerChar == null)
		{
			playerChar = LevelController.Instance.playerChar;
		}
		checkPlayerMovement = true;
	}

	void AddEventListeners()
	{
		Messenger<Colour>.AddListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
		Messenger<Colour>.AddListener(ColourCollisionMessage.FloorPiecesChangedColour.ToString(), FloorPieceChangedColour);
		Messenger.AddListener(CheckpointMessage.CheckpointPressed.ToString(), CheckpointPressed);
		Messenger.AddListener(PlayerMessage.HitWall.ToString(), PlayerHitWall);
		Messenger.AddListener(PlayerMessage.HitWallHard.ToString(), PlayerHitWallHard);
	}

	void RemoveEventListeners()
	{
		Messenger.RemoveListener(LevelStateMessage.LevelStarted.ToString(), LevelStarted);
		Messenger<Colour>.RemoveListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
		Messenger<Colour>.RemoveListener(ColourCollisionMessage.FloorPiecesChangedColour.ToString(), FloorPieceChangedColour);
		Messenger.RemoveListener(CheckpointMessage.CheckpointPressed.ToString(), CheckpointPressed);
		Messenger.RemoveListener(PlayerMessage.HitWall.ToString(), PlayerHitWall);
		Messenger.RemoveListener(PlayerMessage.HitWallHard.ToString(), PlayerHitWallHard);
	}

	void LevelStarted()
	{
		Init();
		//Play Music.
		PooledAudioController.Instance.PlayMusic(inGameMusic);
	}

	void PlayerChangedColour(Colour colourToChangeTo)
	{
		PooledAudioController.Instance.PlaySound(playerChangedColourSound);
	}

	void FloorPieceChangedColour(Colour colourToChangeTo)
	{
		PooledAudioController.Instance.PlaySound(floorChangedColourSound);
	}

	void CheckpointPressed()
	{
		PooledAudioController.Instance.PlaySound(checkpointSound);
	}

	void PlayerHitWall()
	{
		//Play sound quietly.
		PooledAudioController.Instance.PlaySound(hitWallSound, 0.8f);
	}

	void PlayerHitWallHard()
	{
		//Play sound loudly.
		PooledAudioController.Instance.PlaySound(hitWallSound);
	}

	void Update()
	{
		if(!LevelSerializer.IsDeserializing)
		{
			if(checkPlayerMovement)
			{
				if(playerChar != null)
				{
					//Play player movement sound if player is moving
					if(playerChar.playerMovement.isMoving)
					{
						if(!playerMoveSource.isPlaying)
							playerMoveSource.Play();
					}
					else if(playerMoveSource.isPlaying)
					{
						playerMoveSource.Pause();
					}
				}
			}
		}
	}
}
