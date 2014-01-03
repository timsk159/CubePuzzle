using UnityEngine;
using System.Collections;

public class AudioPlayer : MonoBehaviour 
{
	PlayerCharacter playerChar;

	public AudioClip playerMoveSound;
	public AudioClip playerChangedColourSound;
	public AudioClip floorChangedColourSound;
	public AudioClip checkpointSound;

	void Start()
	{
		AddEventListeners();
		if(playerChar == null)
		{
			playerChar = LevelController.Instance.playerChar;
		}
	}

	void OnDestroy()
	{
		RemoveEventListeners();
	}

	void AddEventListeners()
	{
		Messenger.AddListener(LevelStateMessage.LevelStarted.ToString(), LevelStarted);
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
		//Play Music.

	}

	void PlayerChangedColour(Colour colourToChangeTo)
	{
		AudioController.Instance.PlaySound(playerChangedColourSound);
	}

	void FloorPieceChangedColour(Colour colourToChangeTo)
	{
		AudioController.Instance.PlaySound(floorChangedColourSound);
	}

	void CheckpointPressed()
	{
		AudioController.Instance.PlaySound(checkpointSound);
	}

	void Update()
	{
		if(playerChar != null)
		{
			//Play player movement sound if player is moving
			if(playerChar.playerMovement.isMoving)
			{
				AudioController.Instance.PlaySound(playerMoveSound);
			}
			else
			{
				AudioController.Instance.StopSound(playerMoveSound);
			}
		}
	}
}
