using System.Collections.Generic;
using UnityEngine;

public class CoverManager : MonoBehaviour
{
	public static List<CoverPoint> GetCoverPointsSortedByDistanceTo(Vector3 position)
	{
		return Instance.GetCoverPointsSortedByDistance(position);
	}

	public static CoverManager Instance { get; private set; }

	public CoverPoint[] coverPoints;
	private List<CoverPoint> _coversSorted;

	private void Awake()
	{
		Instance = this;

		_coversSorted = new List<CoverPoint>(capacity: coverPoints.Length);
		_coversSorted.AddRange(coverPoints);
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public List<CoverPoint> GetCoverPointsSortedByDistance(Vector3 origin)
	{
		_coversSorted.Sort((CoverPoint a, CoverPoint b) =>
		{
			var dstA = Vector3.SqrMagnitude(a.Position - origin);
			var dstB = Vector3.SqrMagnitude(b.Position - origin);

			if (dstA < dstB) { return -1; }
			if (dstA > dstB) { return 1; }
			return 0;
		});

		return _coversSorted;
	}
}
