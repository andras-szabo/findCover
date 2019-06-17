using UnityEngine;

public class CoverPoint : MonoBehaviour
{
	public Vector3 wallNormalA;
	public Vector3 wallNormalB;

	private Transform _cachedTransform;
	public Transform CachedTransform
	{
		get
		{
			return _cachedTransform ?? (_cachedTransform = transform);
		}
	}

	public Vector3 Position
	{
		get
		{
			return CachedTransform.position;
		}
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		var pos = transform.position;
		Gizmos.DrawLine(pos, pos + wallNormalA);
		Gizmos.DrawLine(pos, pos + wallNormalB);
	}
}
