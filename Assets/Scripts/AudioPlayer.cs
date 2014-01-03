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

	void Init()
	{
		AddEventListeners();
		var playerMoveSourceGO = new GameObject("PlayerMoveAudioSource");
		playerMoveSourceGO.transform.parent = Camera.main.transform;
		playerMoveSource = playerMoveSourceGO.AddComponent<AudioSource>();
		playerMoveSource.clip = playerMoveSound;

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
	}

	void RemoveEventListeners()
	{
		Messenger.RemoveListener(LevelStateMessage.LevelStarted.ToString(), LevelStarted);
		Messenger<Colour>.RemoveListener(ColourCollisionMessage.PlayerChangedColour.ToString(), PlayerChangedColour);
		Messenger<Colour>.RemoveListener(ColourCollisionMessage.FloorPiecesChangedColour.ToString(), FloorPieceChangedColour);
		Messenger.RemoveListener(CheckpointMessage.CheckpointPressed.ToString(), CheckpointPressed);
	}

	void LevelStarted()
	{
		Init();
		//Play Music.

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

	void Update()
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
			else
				Debug.LogWarning("Player char is null!");
		}
	}
}
