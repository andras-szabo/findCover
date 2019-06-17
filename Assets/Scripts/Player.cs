using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Player : MonoBehaviour
{
	private Transform _cachedTransform;
	private Transform CachedTransform
	{
		get
		{
			return _cachedTransform ?? (_cachedTransform = this.transform);
		}
	}

	public Vector3 Position { get { return CachedTransform.position; } }
	public bool IsDirty { get; private set; }

	private NavMeshAgent _agent;
	private Camera _mainCam;

	private void OnDrawGizmos()
	{
		if (Application.isPlaying)
		{
			if (IsDirty)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(_agent.destination, 2f);
			}
		}
	}

	private void Awake()
	{
		_agent = GetComponent<NavMeshAgent>();
	}

	private void Start()
	{
		_mainCam = Camera.main;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			var ray = RectTransformUtility.ScreenPointToRay(_mainCam, Input.mousePosition);
			RaycastHit hitInfo;
			if (Physics.Raycast(ray, out hitInfo))
			{
				_agent.SetDestination(hitInfo.point);
			}
		}

		IsDirty = _agent.velocity.sqrMagnitude > 0f;
	}

}
