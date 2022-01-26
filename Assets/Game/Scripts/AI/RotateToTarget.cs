using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToTarget : MonoBehaviour
{
    public Vector3 Target;
    public float RotationSpeed = 1;
    private bool m_activated = false;

    public void ActivateRotation(Vector3 _target)
    {
        m_activated = true;
        Target = _target;
    }

    public void DeactivateRotation()
    {
        m_activated = false;
    }

    void Update()
    {
        if (!m_activated) return;

        Vector3 direction = (new Vector3(Target.x, 0, Target.z) - new Vector3(transform.position.x, 0, transform.position.z)).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * RotationSpeed);
    }
}
