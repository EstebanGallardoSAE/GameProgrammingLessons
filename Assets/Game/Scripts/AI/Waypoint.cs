using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Waypoint
{
    public enum ActionsPatrol { GO = 0, STAY }

    public GameObject Target;
    public Vector3 Position;
    public float Duration;
    public ActionsPatrol Action;
}
