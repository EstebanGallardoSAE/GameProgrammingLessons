using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolWaypoints : StateMachine
{
	public delegate void MovingEvent();
	public delegate void StandingEvent();

	public event MovingEvent MoveEvent;
	public event StandingEvent StandEvent;

	public void DispatchMovingEvent()
	{
		if (MoveEvent != null)
			MoveEvent();
	}

	public void DispatchStandingEvent()
	{
		if (StandEvent != null)
			StandEvent();
	}

	public const int SYNCHRONIZATION = 0;
	public const int UPDATE_WAYPOINT = 1;
	public const int GO_TO_WAYPOINT = 2;
	public const int STAY_IN_WAYPOINT = 3;


	public Waypoint[] Waypoints;
    public int CurrentWaypoint = 0;
    public NavMeshAgent Agent;

    private bool m_activated = false;
	private float m_timeDone = 0;
	private float m_speed;
	private RotateToTarget m_rotateComponent;
	private bool m_hasRigidBody;

	// Start is called before the first frame update
	void Start()
    {
		m_rotateComponent = GetComponent<RotateToTarget>();
		Agent = GetComponent<NavMeshAgent>();

		if (GetComponent<Rigidbody>() != null)
        {
			m_hasRigidBody = true;
		}
		else
        {
			m_hasRigidBody = false;
		}

		for (int i = 0; i < Waypoints.Length; i++)
        {
            Waypoints[i].Position = Waypoints[i].Target.transform.position;
        }

		for (int i = 0; i < Waypoints.Length; i++)
		{
			GameObject.Destroy(Waypoints[i].Target);
		}
	}

	private Vector3 GetPreviousPositionWaypoint(int _waypointIndex)
	{
		int finalIndexCheck = _waypointIndex; 
		do {
			finalIndexCheck--; 
			if (finalIndexCheck < 0)
			{
				finalIndexCheck = Waypoints.Length - 1;
			}
		} while (Waypoints[finalIndexCheck].Action != Waypoint.ActionsPatrol.GO);
		return Waypoints[finalIndexCheck].Position;
	}

	private void WalkToCurrentWaypoint()
    {
		m_timeDone += Time.deltaTime;
		float duration = Waypoints[CurrentWaypoint].Duration;
		Vector3 origin = GetPreviousPositionWaypoint(CurrentWaypoint);
		Vector3 forwardTarget = (Waypoints[CurrentWaypoint].Position - origin);
		float increaseFactor = m_timeDone/duration;
		Vector3 nextPosition = origin + (increaseFactor * forwardTarget);
		if (Agent != null)
        {
			//transform.GetComponent<Rigidbody>().MovePosition(new Vector3(nextPosition.x, transform.position.y, nextPosition.z));

			Agent.SetDestination(nextPosition);
        }
		else
        {
			transform.position = nextPosition;
		}		
	}

	private bool ReachedCurrentWaypoint()
    {
		if (Vector3.Distance(transform.position, Waypoints[CurrentWaypoint].Position) < 0.5f)
        {
			return true;
        }
		else
        {
			return false;
		}		
    }

	private void WalkWithSpeedToWaypoint()
    {
		Vector3 forwardTarget = (Waypoints[CurrentWaypoint].Position - transform.position).normalized;
		Vector3 nextPosition = transform.position + (forwardTarget * m_speed * Time.deltaTime);
		if (Agent != null)
		{
			//transform.GetComponent<Rigidbody>().MovePosition(new Vector3(nextPosition.x, transform.position.y, nextPosition.z));
			Agent.SetDestination(nextPosition);
		}
		else
        {
			transform.position = nextPosition;

		}
	}

	public void ActivatePatrol(float _speed)
    {
		m_speed = _speed;
		m_activated = true;
		ChangeState(SYNCHRONIZATION);
    }

	public void DeactivatePatrol()
    {
		m_activated = false;
	}

    protected override void ChangeState(int newState)
    {
        base.ChangeState(newState);

		switch (m_state)
		{
			case SYNCHRONIZATION:
				DispatchMovingEvent();
				break;

			case UPDATE_WAYPOINT:
				break;

			case GO_TO_WAYPOINT:
				DispatchMovingEvent();
				break;

			case STAY_IN_WAYPOINT:
				DispatchStandingEvent();
				break;
		}
	}

	// Update is called once per frame
	void Update()
    {
		if (m_activated == false) return;

		switch (m_state)
        {
			case SYNCHRONIZATION:
				if (m_rotateComponent != null)
				{
					m_rotateComponent.ActivateRotation(Waypoints[CurrentWaypoint].Position);
				}
				WalkWithSpeedToWaypoint();

				if (ReachedCurrentWaypoint() == true)
				{
					ChangeState(UPDATE_WAYPOINT);
				}
				break;

			case UPDATE_WAYPOINT:
				CurrentWaypoint++;
				if (CurrentWaypoint > Waypoints.Length - 1)
                {
					CurrentWaypoint = 0;
				}
				m_timeDone = 0;
				if (m_rotateComponent != null)
                {
					m_rotateComponent.ActivateRotation(Waypoints[CurrentWaypoint].Position);
				}					
				if (Waypoints[CurrentWaypoint].Action == Waypoint.ActionsPatrol.GO)
                {
					ChangeState(GO_TO_WAYPOINT);
				}
				else
                {
					ChangeState(STAY_IN_WAYPOINT);
				}				
				break;

			case GO_TO_WAYPOINT:
				WalkToCurrentWaypoint();

				if (m_timeDone > Waypoints[CurrentWaypoint].Duration)
				{
					ChangeState(UPDATE_WAYPOINT);
				}
				break;

			case STAY_IN_WAYPOINT:
				m_timeDone += Time.deltaTime;
				if (m_timeDone > Waypoints[CurrentWaypoint].Duration)
                {
					ChangeState(UPDATE_WAYPOINT);
				}
				break;
		}
	}
}
