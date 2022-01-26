using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    private static LevelController _instance;

    public static LevelController Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = GameObject.FindObjectOfType<LevelController>();
            }
            return _instance;
        }
    }


    public Enemy[] Enemies;
    public NPC[] NPCs;

    public void Destroy()
    {
        if (_instance != null)
        {
            _instance = null;
            Bullet[] bullets = GameObject.FindObjectsOfType<Bullet>();
            for (int i = 0; i < bullets.Length; i++)
            {
                if (bullets[i] != null) GameObject.Destroy(bullets[i].gameObject);
            }
            for (int i = 0; i < Enemies.Length; i++)
            {
                if (Enemies[i] != null) GameObject.Destroy(Enemies[i].gameObject);
            }
            for (int i = 0; i < NPCs.Length; i++)
            {
                if (NPCs[i] != null) GameObject.Destroy(NPCs[i].gameObject);
            }            
            GameObject.Destroy(this.gameObject);
        }
    }

    public bool HasKilledAllEnemies()
    {
        for (int i = 0; i < Enemies.Length; i++)
        {
            if (Enemies[i] != null)
            {
                return false;
            }
        }
        return true;
    }

    public void InitializeLogicGameElements()
    {
        for (int i = 0; i < Enemies.Length; i++)
        {
            if (Enemies[i] != null) Enemies[i].InitLogic();
        }
        for (int i = 0; i < NPCs.Length; i++)
        {
            if (NPCs[i] != null) NPCs[i].InitLogic();
        }
    }

    public void FreezeLogicGameElements()
    {
        for (int i = 0; i < Enemies.Length; i++)
        {
            if (Enemies[i] != null) Enemies[i].FreezeLogic();
        }
        for (int i = 0; i < NPCs.Length; i++)
        {
            if (NPCs[i] != null) NPCs[i].FreezeLogic();
        }
    }

    public bool AlarmEnemyNearby(float _distanceDetection)
    {
        for (int i = 0; i < Enemies.Length; i++)
        {
            if (Enemies[i] != null)
            {
                if (Vector3.Distance(Enemies[i].transform.position, GameController.Instance.MyPlayer.transform.position) < _distanceDetection)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void RunLogic()
    {
        for (int i = 0; i < Enemies.Length; i++)
        {
            if (Enemies[i] != null) Enemies[i].RunLogic();
        }
        for (int i = 0; i < NPCs.Length; i++)
        {
            if (NPCs[i] != null) NPCs[i].RunLogic();
        }
    }
}
