using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    protected int m_lastState = 0;
    protected int m_state = 0;

    protected virtual void RestoreState()
    {
        ChangeState(m_lastState);
    }

    protected virtual void ChangeState(int newState)
    {
        if (m_state != newState)
        {
            m_lastState = m_state;
        }        
        m_state = newState;
    }
}
