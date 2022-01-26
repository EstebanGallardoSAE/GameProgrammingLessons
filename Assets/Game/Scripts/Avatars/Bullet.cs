using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Avatar
{
    public const int TYPE_BULLET_PLAYER = 0;
    public const int TYPE_BULLET_ENEMY = 1;

    public int Damage;
    public int Type;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    public override void InitLogic()
    {
    }
    public override void FreezeLogic()
    {
    }

    public void Shoot(int _type, Vector3 _position, Vector3 _direction)
    {
        Type = _type;
        this.transform.position = _position;
        this.transform.forward = _direction;

        if (Type == TYPE_BULLET_PLAYER)
        {
            Physics.IgnoreCollision(this.transform.GetComponent<Collider>(), GameController.Instance.MyPlayer.GetComponent<Collider>());
        }        
    }

    void OnCollisionEnter(Collision _collision)
    {
        if (Type == TYPE_BULLET_PLAYER)
        {
            if (_collision.gameObject.GetComponent<Enemy>() != null)
            {
                // Debug.Log(this.gameObject.name + " COLLISION AGAINST ENEMY");
                _collision.gameObject.GetComponent<Enemy>().DecreaseLife(Damage);
                GameObject.Destroy(this.gameObject);
            }
            if (_collision.gameObject.GetComponent<NPC>() != null)
            {
                // Debug.Log(this.gameObject.name + " COLLISION AGAINST NPC");
                _collision.gameObject.GetComponent<NPC>().DecreaseLife(Damage);
                GameObject.Destroy(this.gameObject);
            }
        }
        if (Type == TYPE_BULLET_ENEMY)
        {
            if (_collision.gameObject.GetComponent<Player>() != null)
            {
                // Debug.Log(this.gameObject.name + " COLLISION AGAINST PLAYER");
                _collision.gameObject.GetComponent<Player>().DecreaseLife(Damage);
                GameObject.Destroy(this.gameObject);
            }
        }

        if (_collision.gameObject.GetComponent<Bullet>() != null)
        {
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            GameObject.Destroy(this.gameObject);
        }        
    }

    void OnTriggerEnter(Collider _collision)
    {
        // Debug.Log("-- " + this.gameObject.name + " COLLISION AGAINST " + _collision.gameObject.name + "!!!!!");
        GameObject.Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        MoveToPosition(this.gameObject.transform.forward * Speed * Time.deltaTime);
    }

    public override void RunLogic()
    {
    }
}
