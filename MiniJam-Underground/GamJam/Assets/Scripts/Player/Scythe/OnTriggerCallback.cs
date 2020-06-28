using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTriggerCallback : MonoBehaviour
{
    public ScytheBehaviour scytheCore;
  
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!scytheCore)
            return;

        scytheCore.OnChildTriggerEnter(collision);
    }
}
