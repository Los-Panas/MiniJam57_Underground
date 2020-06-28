using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Hit : MonoBehaviour
{
    public int hits_to_kill = 1;
    public GameObject soul;
    public void KillEnemy()
    {
        // TO DO: Dissolve Shader
        // TO DO: Particle System
        Instantiate(soul);
    }
}
