using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public GameObject[] Shooter;
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag =="Player")
        {
            for(int i =0;i<Shooter.Length;i++)
            {
                Shooter[i].GetComponent<Ballistics>().enabled = true;
            }
            
        }
    }
}
