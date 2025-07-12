using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostitionCheck : MonoBehaviour
{
    public bool occupied;

    RaycastHit hit;
    void Update()
    {
        if(Physics.Raycast(transform.position - transform.up * 4,transform.up,out hit, 4))
        {
            Debug.DrawRay(transform.position - transform.up * 4, transform.up,Color.yellow);
            occupied = true;
            if (!RoundSystem.RS.posCheck.Contains(gameObject))
            {
                RoundSystem.RS.posCheck.Add(gameObject);
            }
        }
        else
        {
            occupied = false;
            if (RoundSystem.RS.posCheck.Contains(gameObject))
            {
                RoundSystem.RS.posCheck.Remove(gameObject);
            }
        }
    }
}
