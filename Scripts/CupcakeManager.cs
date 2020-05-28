using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CupcakeManager : MonoBehaviour
{
    public List<GameObject> cupcakeList;

    void Start()
    {
        cupcakeList = new List<GameObject>();
        InitCups();
    }

    void Update()
    {
        
    }

    public void InitCups()
    {
        for (int i = 1; i < 6; i++)
        {
            string name = "Cupcake" + i;
            GameObject c = GameObject.Find(name);
            c.GetComponent<CollisionManager>().enabled = false;
            c.tag = "Hidden";
            cupcakeList.Add(c);
        }
    }
}
