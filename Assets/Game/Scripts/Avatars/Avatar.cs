using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Avatar : StateMachine
{
    public const int STATE_NULL = -1;

    public int InitialLife = 100;
    public float Speed = 2;
    public GameObject Model3d;

    protected int m_life = 100;
    protected float m_timeToDisappear = 0;
    protected PatrolWaypoints m_patrolComponent;
    protected RotateToTarget m_rotateComponent;
    protected AreaVisionDetection m_areaVisionDetection;
    protected Animator m_animatorController;

    public int Life
    {
        get { return m_life; }
    }
    
    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_life = InitialLife;
        m_patrolComponent = GetComponent<PatrolWaypoints>();
        m_rotateComponent = GetComponent<RotateToTarget>();
        m_areaVisionDetection = GetComponent<AreaVisionDetection>();
        if (Model3d != null) m_animatorController = Model3d.GetComponent<Animator>();

        // Debug.Log(this.gameObject.name + " INITIAL LIFE IS=" + m_life);
    }

    protected int m_currentAnimationID = -1;

    protected void ChangeAnimation(int _animationID)
    {
        if (m_animatorController != null)
        {
            if (m_currentAnimationID != _animationID)
            {
                m_currentAnimationID = _animationID;
                m_animatorController.SetInteger("animationID", _animationID);
            }            
        }
    }

    public abstract void InitLogic();
    public abstract void FreezeLogic();

    public virtual void DecreaseLife(int _damage)
    {
        m_life = m_life - _damage;
        if (m_life < 0)
        {
            m_life = 0;
        }
        // Debug.Log(this.gameObject.name +  " DECREASE LIFE TO=" + m_life);
    }

    public virtual void IncreaseLife(int _life)
    {
        m_life = m_life + _life;
        if (m_life > 100)
        {
            m_life = 100;
        }
        // Debug.Log(this.gameObject.name + " INCREASED LIFE TO=" + m_life);
    }

    protected void MoveToPosition(Vector3 _targetPosition)
    {
        Vector3 increment = _targetPosition;
        transform.GetComponent<Rigidbody>().MovePosition(transform.position + increment);
    }

    void OnTriggerEnter(Collider _collision)
    {
        if (_collision.gameObject.GetComponent<Spikes>() != null)
        {
            // Debug.Log(this.gameObject.name + " HAS COLLIDED WITH A SPIKES INSTANCE");
            int damageSpikes = _collision.gameObject.GetComponent<Spikes>().DamageLife;
            DecreaseLife(damageSpikes);
        }
        if (_collision.gameObject.GetComponent<Portal>() != null)
        {
            // Debug.Log(this.gameObject.name + " HAS COLLIDED WITH A SPIKES INSTANCE");
            int moreLife = _collision.gameObject.GetComponent<Portal>().MoreLife;
            IncreaseLife(moreLife);
        }
    }

    protected void RotateToTarget(Vector3 _target)
    {
        if (m_rotateComponent != null)
        {
            m_rotateComponent.ActivateRotation(_target);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
