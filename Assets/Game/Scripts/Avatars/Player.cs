using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Avatar
{
    public const int INITIAL = 0;
    public const int IDLE = 1;
    public const int WALK = 2;
    public const int DIE = 3;
    public const int JUMP = 4;

    public const int ANIMATION_IDLE = 0;
    public const int ANIMATION_RUN = 1;
    public const int ANIMATION_DIE = 2;

    public Vector3 JumpPower;
    public bool isGrounded = true;
    Rigidbody rb;

    public bool InPlatform = false;
    public GameObject BulletPlayer;
    public Text DisplayScore;
    public Text DisplayLife;
    public Text DisplayDanger;

    private int m_score;
    private Vector3 m_initialPosition;

    public int Score
    {
        get { return m_score; }
        set { m_score = value;
            DisplayScore.text = m_score.ToString();
        }
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        rb = this.GetComponent<Rigidbody>();
        JumpPower = new Vector3(0.0f, 2.0f, 0.0f);

        Score = 0;
        m_state = IDLE;
        DisplayLife.text = m_life.ToString();
        m_initialPosition = this.transform.position;

        SystemEventController.Instance.Event += new SystemEventController.SystemEvent(OnSystemEvent);
        
        if (m_animatorController != null) m_animatorController.gameObject.SetActive(false);

        ChangeState(INITIAL);
    }

    void OnDestroy()
    {
        SystemEventController.Instance.Event -= OnSystemEvent;
    }

    private void OnSystemEvent(string _nameEvent, object[] _parameters)
    {
        if (_nameEvent == SystemEventController.EVENT_CAMERA_SWITCHED_TO_1ST_PERSON)
        {
            m_animatorController.gameObject.SetActive(false);
        }
        if (_nameEvent == SystemEventController.EVENT_CAMERA_SWITCHED_TO_3RD_PERSON)
        {
            m_animatorController.gameObject.SetActive(true);
        }
    }


    public override void InitLogic()
    {
        m_life = InitialLife;
        DisplayLife.text = m_life.ToString();
        this.transform.position = m_initialPosition;
        ChangeState(IDLE);
    }

    public void SetIdleState()
    {
        ChangeState(IDLE);
    }

    public override void FreezeLogic()
    {
        ChangeState(INITIAL);
    }

    private void Move()
    {
        float axisVertical = Input.GetAxis("Vertical");
        float axisHorizontal = Input.GetAxis("Horizontal");

        if (CameraController.Instance.IsFirstPersonCamera())
        {
            Vector3 forward = axisVertical * CameraController.Instance.GameCamera.transform.forward;
            Vector3 lateral = axisHorizontal * CameraController.Instance.GameCamera.transform.right;

            MoveToPosition((forward + lateral) * Speed * Time.deltaTime);
        }
        else
        {
            Vector2 directionJoystick = new Vector2(axisHorizontal, axisVertical);
            Vector3 fwdAvatar = new Vector3(directionJoystick.x, 0, directionJoystick.y);
            Vector3 realForward = CameraController.Instance.GameCamera.transform.TransformDirection(fwdAvatar);
            realForward = new Vector3(realForward.x, 0, realForward.z).normalized;

            MoveToPosition(realForward * Speed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            isGrounded = true;
        }
        
        if(collision.gameObject.tag == "Platform")
        {
            InPlatform = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Platform")
        {
            InPlatform = false;
        }
    }




    private void jump()
    {     
        if (isGrounded == true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(JumpPower * 3, ForceMode.Impulse);
                ChangeAnimation(JUMP);              
                isGrounded = false;
            }
        }
    }

    public void ShootBullet()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GameObject bullet = Instantiate(BulletPlayer);
            int type = Bullet.TYPE_BULLET_PLAYER;
            Vector3 position = Vector3.zero;
            Vector3 direction = Vector3.zero;
            if (CameraController.Instance.IsFirstPersonCamera())
            {
                position = CameraController.Instance.GameCamera.transform.position;
                direction = CameraController.Instance.GameCamera.transform.forward;
            }
            else
            {
                position = this.transform.position;
                direction = new Vector3(CameraController.Instance.GameCamera.transform.forward.x, 0, CameraController.Instance.GameCamera.transform.forward.z);
            }
            SoundsController.Instance.PlaySoundFX(SoundsController.FX_SHOOT, 1);
            bullet.GetComponent<Bullet>().Shoot(type, position, direction);
        }
    }

    public override void DecreaseLife(int _damage)
    {
        base.DecreaseLife(_damage);
        DisplayLife.text = m_life.ToString();
    }

    public override void IncreaseLife(int _life)
    {
        base.IncreaseLife(_life);
        DisplayLife.text = m_life.ToString();
    }

    private bool ArrowKeyPressed()
    {
        if (
            Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.LeftArrow)
            || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)
            )
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    protected override void ChangeState(int newState)
    {
        base.ChangeState(newState);

        switch (m_state)
        {
            case IDLE:
                ChangeAnimation(ANIMATION_IDLE);
                // Debug.Log("CAMBIADO A ESTADO IDLE");
                break;

            case WALK:
                ChangeAnimation(ANIMATION_RUN);
                // Debug.Log("CAMBIADO A ESTADO WALK");
                break;

            case DIE:
                ChangeAnimation(ANIMATION_DIE);
                // Debug.Log("CAMBIADO A ESTADO DIE");
                break;
        }
    }

    private void CheckToDisplayDanger()
    {
        if (LevelController.Instance != null)
        {
            if (LevelController.Instance.AlarmEnemyNearby(6))
            {
                DisplayDanger.text = "Enemy nearby";
            }
            else
            {
                DisplayDanger.text = "";
            }
        }
    }

    private void RotateToCamera()
    {
        if (m_animatorController != null)
        {
            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(CameraController.Instance.GameCamera.transform.forward.x, 0, CameraController.Instance.GameCamera.transform.forward.z));
            m_animatorController.gameObject.transform.rotation = Quaternion.Slerp(m_animatorController.gameObject.transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    public override void RunLogic()
    {
        
        switch (m_state)
        {
            case INITIAL:
                break;

            case IDLE:
                
                jump();
                RotateToCamera();
                ShootBullet();
                CheckToDisplayDanger();
                if (ArrowKeyPressed() == true)
                {
                    ChangeState(WALK);
                }
                if (m_life == 0)
                {
                    ChangeState(DIE);
                }
                break;

            case WALK:
                jump();
                RotateToCamera();
                Move();
                ShootBullet();
                CheckToDisplayDanger();
                if (ArrowKeyPressed() == false)
                {
                    ChangeState(IDLE);
                }
                if (m_life == 0)
                {
                    ChangeState(DIE);
                }
                break;
            case DIE:
                break;
        }
    }
}
