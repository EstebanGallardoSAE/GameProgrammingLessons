using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    public int DamageLife = 10;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<PatrolWaypoints>()!=null) GetComponent<PatrolWaypoints>().ActivatePatrol(2);
        
        SystemEventController.Instance.Event += ProcessSystemEvent;
    }

    void OnDestroy()
    {
        SystemEventController.Instance.Event -= ProcessSystemEvent;
    }

    private void ProcessSystemEvent(string _nameEvent, object[] _parameters)
    {
        if (_nameEvent == SystemEventController.EVENT_ENEMY_DEAD)
        {
            this.transform.localScale *= 2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
