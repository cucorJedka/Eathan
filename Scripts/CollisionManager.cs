using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    public bool collided = false;
    public bool collisionAvailable = false;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Ethan" && collisionAvailable)
        {
            collided = true;
        }
    }
}
