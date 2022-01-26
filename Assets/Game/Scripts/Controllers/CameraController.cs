using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : StateMachine
{
    public const int CAMERA_FROZEN = 0;
    public const int CAMERA_1ST_PERSON = 1;
    public const int CAMERA_3RD_PERSON = 2;

    public const float SPEED_ROTATION = 10f;

    private static CameraController _instance;

    public static CameraController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType<CameraController>();
            }
            return _instance;
        }
    }

    public Camera GameCamera;
    public Player GamePlayer;
    public Vector3 Offset = new Vector3(0, 3, 5);

    public float Sensitivity = 7F;

    private float m_rotationY = 0F;
    
    void Start()
    {
        m_lastState = CAMERA_1ST_PERSON;
        ChangeState(CAMERA_FROZEN);
    }

    public bool IsFirstPersonCamera()
    {
        return (m_state == CAMERA_1ST_PERSON);
    }

    private void CameraFollowAvatar()
    {
        Offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * SPEED_ROTATION, Vector3.up) * Offset;
        GameCamera.transform.position = GamePlayer.transform.position + Offset;
        GameCamera.transform.LookAt(GamePlayer.transform.position + new Vector3(0, 1, 0));
    }

    public void FreezeCamera()
    {
        ChangeState(CAMERA_FROZEN);
    }

    public void RestorePreviousCamera()
    {
        RestoreState();
    }

    private void SwitchCameraState()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            if (m_state == CAMERA_1ST_PERSON)
            {
                ChangeState(CAMERA_3RD_PERSON);
            }
            else
            {
                ChangeState(CAMERA_1ST_PERSON);
            }
        }
    }

    protected override void ChangeState(int newState)
    {
        base.ChangeState(newState);

        switch (m_state)
        {
            case CAMERA_1ST_PERSON:
                GameCamera.transform.SetParent(GamePlayer.transform);
                GameCamera.transform.forward = new Vector3(0, 0, 0);
                GameCamera.transform.localPosition = new Vector3(0, 1.26f, 0);
                SystemEventController.Instance.DispatchSystemEvent(SystemEventController.EVENT_CAMERA_SWITCHED_TO_1ST_PERSON);
                break;

            case CAMERA_3RD_PERSON:
                GameCamera.transform.parent = null;
                SystemEventController.Instance.DispatchSystemEvent(SystemEventController.EVENT_CAMERA_SWITCHED_TO_3RD_PERSON);
                break;
        }
    }
    private void RotateCamera()
    {
        float rotationX = GameCamera.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * Sensitivity;

        m_rotationY += Input.GetAxis("Mouse Y") * Sensitivity;
        m_rotationY = Mathf.Clamp(m_rotationY, -60, 60);

        GameCamera.transform.localEulerAngles = new Vector3(-m_rotationY, rotationX, 0);
    }

    void Update()
    {
        SwitchCameraState();

        switch (m_state)
        {
            case CAMERA_FROZEN:
                break;

            case CAMERA_1ST_PERSON:
                RotateCamera();
                break;

            case CAMERA_3RD_PERSON:
                CameraFollowAvatar();
                break;
        }
    }
}
