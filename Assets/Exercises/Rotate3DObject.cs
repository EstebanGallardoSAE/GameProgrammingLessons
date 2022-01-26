using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate3DObject : MonoBehaviour
{
    private float m_rotationSpeed = 25;
    private bool m_direction = false;
    
    private int m_state = 0;

    public float RotationSpeed
    {
        get { return m_rotationSpeed; }
        set { m_rotationSpeed = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_state = (m_state + 1)%4;
        }
        
        switch (m_state)
        {
            case 0:
                this.transform.Rotate(new Vector3(0, m_rotationSpeed * Time.deltaTime, 0));
                break;

            case 1:
                this.transform.Rotate(new Vector3(0, -m_rotationSpeed * Time.deltaTime, 0));
                break;

            case 2:
                this.transform.Rotate(new Vector3(0, 4 * m_rotationSpeed * Time.deltaTime, 0));
                break;

            case 3:
                this.transform.Rotate(new Vector3(4 * m_rotationSpeed * Time.deltaTime, 0, 0));
                break;

        }

        /*
        if (Input.GetKey(KeyCode.Space))
        {
            m_direction = !m_direction;
        }

        if (m_direction)
        {
            this.transform.Rotate(new Vector3(0, m_rotationSpeed * Time.deltaTime, 0));
        }
        else
        {
            this.transform.Rotate(new Vector3(0, -m_rotationSpeed * Time.deltaTime, 0));
        }
        */
        
    }
}
