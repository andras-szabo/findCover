using System.Collections;
using UnityEngine;

public class FindCover : MonoBehaviour
{
	public const float COVER_UPDATE_INTERVAL_SECS = 0.2f;
	private Transform _cachedTransform;
	public Transform CachedTransform
	{
		get
		{
			return _cachedTransform ?? (_cachedTransform = transform);
		}
	}

	private UnityEngine.AI.NavMeshAgent _agent;

	private bool _hasTarget;
	private Vector3 _targetPosition;
	
	public bool log;
	public bool isStopped;

	private void L(string msg)
	{
		if (log)
		{
			Debug.LogFormat("{0} | {1} | {2}", Time.frameCount, gameObject.name, msg);
		}
	}

	private void OnDrawGizmos()
	{
		if (_hasTarget)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(CachedTransform.position, _targetPosition);
			Gizmos.DrawWireSphere(_targetPosition, 0.5f);
		}
	}

	private void Awake()
	{
		_agent = GetComponent<UnityEngine.AI.NavMeshAgent>();	
	}

	private IEnumerator Start()
	{
		// Small random delay to distribute cover calculations among
		// agents (i.e. that not everyone calculates covers at the same
		// exact frame)
		yield return new WaitForSeconds(Random.Range(0f, COVER_UPDATE_INTERVAL_SECS));
		StartCoroutine(UpdateCoverRoutine());
	}

	private IEnumerator UpdateCoverRoutine()
	{
		var waitDelay = new WaitForSeconds(COVER_UPDATE_INTERVAL_SECS);
		var hasUpdatedEver = false;
		while (!isStopped)
		{
			if (!hasUpdatedEver || GameManager.IsPlayerDirty)
			{
				TryGoToCover();
				hasUpdatedEver = true;
			}

			yield return waitDelay;
		}
	}

	private void TryGoToCover()
	{
		var playerPos = GameManager.PlayerPosition;
		if (TryFindPositionnBehindCover(_agent.radius, 1f, (CachedTransform.position - playerPos).normalized,
										out _targetPosition))
		{
			_hasTarget = true;
			_agent.SetDestination(_targetPosition);
		}
		else
		{
			L("Couldn't find cover");
		}
	}

	private bool TryFindPositionnBehindCover(float agentRadius, float agentVerticalPosition,
											 Vector3 playerToEnemyNormalised,
											 out Vector3 positionBehindCover)
	{
		var success = false;
		positionBehindCover = Vector3.zero;

		CoverPoint cover;

		if (TryFindCoverPoint(playerToEnemyNormalised, out cover))
		{
			// Find the wall with a normal closer to the player->enemy direction;
			// that is the wall behind which the enemy will try to hide.
			var playerDotNormalA = Vector3.Dot(playerToEnemyNormalised, cover.wallNormalA);
			var playerDotNormalB = Vector3.Dot(playerToEnemyNormalised, cover.wallNormalB);

			var sign = playerDotNormalA > playerDotNormalB ? 1f : -1f;

			positionBehindCover = cover.Position + (sign * agentRadius * cover.wallNormalA)
												 - (sign * agentRadius * cover.wallNormalB);

			positionBehindCover.y = agentVerticalPosition;
			success = true;
		}

		return success;
	}

	private bool TryFindCoverPoint(Vector3 playerToEnemyNormalised, out CoverPoint coverFound)
	{
		coverFound = null;
		var success = false;

		var coverPoints = CoverManager.GetCoverPointsSortedByDistanceTo(transform.position);

		if (coverPoints != null)
		{
			var playerPosition = GameManager.PlayerPosition;
			var myPosition = CachedTransform.position;

			for (int i = 0; !success && (i < coverPoints.Count); ++i)
			{
				L(coverPoints[i].name);
				if (IsSuitableCover(playerPosition, coverPoints[i], myPosition, playerToEnemyNormalised))
				{
					success = true;
					coverFound = coverPoints[i];
				}
				L("------------------");
			}
		}

		return success;
	}

	private bool IsSuitableCover(Vector3 playerPosition, CoverPoint c, Vector3 myPosition, Vector3 playerToMe)
	{
		return IsCoverInFrontOfPlayer(playerPosition, c, playerToMe) &&
			   DoesCoverHideMyAss(playerPosition, c);
	}

	// "In front of" here is to be taken relatively: the idea is that
	// if I have to get past the player to get to cover, that's not a 
	// good cover.
	private bool IsCoverInFrontOfPlayer(Vector3 playerPosition, CoverPoint c, Vector3 playerToMe)
	{
		//TODO - make this condition stricter maybe?
		//		 or if 0f is fine, discard the normalization
		var isInFront = Vector3.Dot(playerToMe, (c.Position - playerPosition).normalized) > 0f;
		return isInFront;
	}

	private bool DoesCoverHideMyAss(Vector3 playerPosition, CoverPoint c)
	{
		var playerToCover = (c.Position - playerPosition).normalized;

		var pcDotA = Vector3.Dot(playerToCover, c.wallNormalA);
		var pcDotB = Vector3.Dot(playerToCover, c.wallNormalB);

		var doesHide = pcDotA > pcDotB ? (pcDotA > 0f) : (pcDotB > 0f);

		if (log) { L(string.Format("DoesHide? {0}", doesHide)); }

		return doesHide;
	}
}
