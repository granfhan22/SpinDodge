using System;
using UnityEngine;

public class EnemySpinner : MonoBehaviour
{
    [SerializeField] private Rigidbody2D enemyRb;
    [SerializeField] private Vector3 rotateSpeed = new Vector3(0,0,100);
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateSpeed * Time.deltaTime, Space.Self);
    }
}
