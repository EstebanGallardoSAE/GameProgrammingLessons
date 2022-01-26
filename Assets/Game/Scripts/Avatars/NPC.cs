using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : Avatar
{
    public const int INITIAL = 0;
    public const int ROTATE_AND_PATROL = 1;
    public const int GO_TO_PLAYER = 2;
    public const int TALK_TO_PLAYER = 3;
    public const int DIE = 4;

    public float DetectionDistance = 10;
    public float TalkingDistance = 5;

    private bool m_playerHasBeenDetected = false;
    private Vector3 m_initialPosition;

    private int m_enemiesKilled = 0;

    private const int ANIMATION_IDLE = 0;
    private const int ANIMATION_RUN = 1;
    private const int ANIMATION_DEATH = 2;
    private const int ANIMATION_TALK = 3;

    protected override void Start()
    {
        base.Start();

        m_initialPosition = this.transform.position;
        // Debug.Log("Enemy has been initialized");

        if (m_areaVisionDetection != null)
        {
            m_areaVisionDetection.DetectionEvent += PlayerDetectionEvent;
            m_areaVisionDetection.LostEvent += PlayerLostEvent;
        }
        if (m_patrolComponent != null)
        {
            m_patrolComponent.MoveEvent += OnMoveEvent;
            m_patrolComponent.StandEvent += OnStandEvent;
        }
        SystemEventController.Instance.Event += ProcessSystemEvent;

        ChangeState(INITIAL);
    }

    private void OnStandEvent()
    {
        ChangeAnimation(ANIMATION_IDLE);
    }

    private void OnMoveEvent()
    {
        ChangeAnimation(ANIMATION_RUN);
    }

    void OnDestroy()
    {
        SystemEventController.Instance.Event -= ProcessSystemEvent;
    }

    private void ProcessSystemEvent(string _nameEvent, object[] _parameters)
    {
        if (_nameEvent == SystemEventController.EVENT_ENEMY_DEAD)
        {
            m_enemiesKilled++;
        }
    }

    private void PlayerLostEvent(GameObject _objectDetected)
    {
        m_playerHasBeenDetected = false;
    }

    private void PlayerDetectionEvent(GameObject _objectDetected)
    {
        m_playerHasBeenDetected = true;
    }

    public override void InitLogic()
    {
        ChangeState(ROTATE_AND_PATROL);
    }

    public override void FreezeLogic()
    {
        ChangeState(INITIAL);
    }

    public override void DecreaseLife(int _damage)
    {
        base.DecreaseLife(_damage);
        if (m_life == 0)
        {
            GameObject.FindObjectOfType<Player>().Score -= 10;
            SoundsController.Instance.PlaySoundFX(SoundsController.FX_DEAD_NPC, 1);
        }
    }

    private Vector3 GetDirection(Vector3 target, Vector3 origin)
    {
        return (target - origin).normalized;
    }

    private void GoToInitialPosition()
    {
        if (Vector3.Distance(this.transform.position, m_initialPosition) > 1)
        {
            Vector3 directionToInitialPosition = GetDirection(m_initialPosition, this.transform.position);
            MoveToPosition(directionToInitialPosition * Speed * Time.deltaTime);
            RotateToTarget(m_initialPosition);
            ChangeAnimation(ANIMATION_RUN);
        }
        else
        {
            ChangeAnimation(ANIMATION_IDLE);
        }
    }

    private bool IsInsideDetectionRange()
    {
        if (m_areaVisionDetection != null)
        {
            return m_playerHasBeenDetected;
        }
        else
        {
            if (Vector3.Distance(this.transform.position, GameController.Instance.MyPlayer.transform.position) < DetectionDistance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private bool IsInsideTalkingRange()
    {
        if (m_areaVisionDetection != null)
        {
            if (m_areaVisionDetection)
            {
                if (Vector3.Distance(this.transform.position, GameController.Instance.MyPlayer.transform.position) < TalkingDistance)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (Vector3.Distance(this.transform.position, GameController.Instance.MyPlayer.transform.position) < TalkingDistance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private void RestoreLife()
    {
        m_life = 100;
    }

    private void WalkToPlayer()
    {
        Vector3 directionToPlayer = GetDirection(GameController.Instance.MyPlayer.transform.position, this.transform.position);
        MoveToPosition(directionToPlayer.normalized * Speed * Time.deltaTime);
    }

    private void TalkToPlayer()
    {
        switch (m_enemiesKilled)
        {
            case 0:
                Debug.Log("NECESITAMOS TU AYUDA, DEFIENDENOS DEL ENEMIGO!!!!");
                break;

            case 1:
                Debug.Log("NO ESTA MAL. HAS MATADO A 1 ENEMIGO");
                break;

            case 2:
                Debug.Log("BUEN TRABAJO. HAS MATADO A 2 ENEMIGOS");
                break;

            default:
                Debug.Log("ERES UN CRACK. HAS MATADO A 3 ENEMIGOS O MAS");
                break;
        }
    }

    private void UpdateRandomInitialPosition()
    {
        m_initialPosition = new Vector3(UnityEngine.Random.Range(-20, 20), transform.position.y, UnityEngine.Random.Range(-20, 20));

        // Debug.Log("NEW POSITION=" + m_initialPosition.ToString());
        // CREATE A TEMPORAL SPHERE TO SHOW THE NEW INITIAL POSITION FOR 3 SECONDS
        // GameObject newPosition = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // newPosition.transform.position = m_initialPosition;
        // GameObject.Destroy(newPosition, 3);
    }

    protected override void ChangeState(int newState)
    {
        base.ChangeState(newState);

        switch (m_state)
        {
            case INITIAL:
                break;

            case ROTATE_AND_PATROL:
                if ((m_patrolComponent == null) || (m_patrolComponent.Waypoints.Length == 0))
                {
                    UpdateRandomInitialPosition();
                }
                else
                {
                    m_patrolComponent.ActivatePatrol(Speed);
                }
                // Debug.Log("NPC A ESTADO [CHECK_FOR_PLAYER]");
                break;

            case GO_TO_PLAYER:
                ChangeAnimation(ANIMATION_RUN);
                if ((m_patrolComponent != null) && (m_patrolComponent.Waypoints.Length != 0))
                {
                    m_patrolComponent.DeactivatePatrol();
                }
                // Debug.Log("NPC A ESTADO [GO_TO_PLAYER]");
                break;

            case TALK_TO_PLAYER:
                ChangeAnimation(ANIMATION_TALK);
                // Debug.Log("NPC A ESTADO [TALK_TO_PLAYER]");
                break;

            case DIE:
                ChangeAnimation(ANIMATION_DEATH);
                // Debug.Log("ENEMY A ESTADO [DIE]");
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (m_state)
        {
            case ROTATE_AND_PATROL:
                if ((m_patrolComponent == null) || (m_patrolComponent.Waypoints.Length == 0))
                {
                    GoToInitialPosition();
                }
                RestoreLife();
                if (IsInsideDetectionRange() == true)
                {
                    ChangeState(GO_TO_PLAYER);
                }
                if (IsInsideTalkingRange() == true)
                {
                    ChangeState(TALK_TO_PLAYER);
                }
                if (m_life == 0)
                {
                    ChangeState(DIE);
                }
                break;

            case GO_TO_PLAYER:
                RotateToTarget(GameController.Instance.MyPlayer.transform.position);
                WalkToPlayer();

                if (IsInsideDetectionRange() == false)
                {
                    ChangeState(ROTATE_AND_PATROL);
                }
                if (IsInsideTalkingRange() == true)
                {
                    ChangeState(TALK_TO_PLAYER);
                }
                if (m_life == 0)
                {
                    ChangeState(DIE);
                }
                break;

            case TALK_TO_PLAYER:
                RotateToTarget(GameController.Instance.MyPlayer.transform.position);
                TalkToPlayer();

                if (IsInsideDetectionRange() == false)
                {
                    ChangeState(ROTATE_AND_PATROL);
                }
                if (IsInsideTalkingRange() == false)
                {
                    ChangeState(GO_TO_PLAYER);
                }
                if (m_life == 0)
                {
                    ChangeState(DIE);
                }
                break;

            case DIE:
                m_timeToDisappear += Time.deltaTime;
                if (m_timeToDisappear > 3)
                {
                    GameObject.Destroy(this.gameObject);
                    ChangeState(STATE_NULL);
                }
                break;
        }

    }
}
