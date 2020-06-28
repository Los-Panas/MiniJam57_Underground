﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Hit : MonoBehaviour
{
    public int hits_to_kill = 1;
    public GameObject soul;
    bool first = false;

    public bool KillEnemy()
    {
        if (!first)
        {
            gameObject.layer = 13;
            first = true;
            SendMessage("Die");
            // TO DO: Dissolve Shader
            // TO DO: Particle System
            Instantiate(soul, transform.position, Quaternion.identity);

            return true;
        }
        return false;
    }
}
