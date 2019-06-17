using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static Vector3 PlayerPosition
	{
		get
		{
			return Instance.GetPlayerPosition();
		}
	}

	public static bool IsPlayerDirty
	{
		get
		{
			return Instance.GetPlayerDirty();
		}
	}

	public static GameManager Instance { get; private set; }

	public Player player;

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public bool GetPlayerDirty()
	{
		return player.IsDirty;
	}

	public Vector3 GetPlayerPosition()
	{
		return player.Position;
	}
}
