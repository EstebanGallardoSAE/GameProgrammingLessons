using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemEventController : MonoBehaviour
{
    public const string EVENT_ENEMY_DEAD = "EVENT_ENEMY_DEAD";
    public const string EVENT_CAMERA_SWITCHED_TO_1ST_PERSON = "EVENT_CAMERA_SWITCHED_TO_1ST_PERSON";
    public const string EVENT_CAMERA_SWITCHED_TO_3RD_PERSON = "EVENT_CAMERA_SWITCHED_TO_3RD_PERSON";

    private static SystemEventController _instance;

    public static SystemEventController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType<SystemEventController>();
            }
            return _instance;
        }
    }

    public delegate void SystemEvent(string _nameEvent, params object[] _parameters);

    public event SystemEvent Event;

    public void DispatchSystemEvent(string _nameEvent, params object[] _parameters)
    {
        if (Event != null) Event(_nameEvent, _parameters);
    }

}
