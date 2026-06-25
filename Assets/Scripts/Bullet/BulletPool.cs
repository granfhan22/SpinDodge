using System;
using UnityEngine;
using System.Collections.Generic;

public class BulletPool : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 100;
    private List<GameObject> pool;

    // public event Action OnAllReturned;
    // public int ActiveCount { get; private set; } = 0;
    public static BulletPool Instance {get; private set;}
    private void Awake()
    {
        if( Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        pool = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < poolSize; i++)
        {
            tmp = Instantiate(bulletPrefab);
            tmp.SetActive(false);
            pool.Add(tmp);
        }
    }

    public GameObject GetPoolObject()
    {
       for(int i = 0; i < poolSize; i++)
        {
            if(!pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }
        return null;
    }

}