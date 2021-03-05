using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileBehavior : MonoBehaviour
{
    public Transform EnemyOne;
    [SerializeField] private float _missileSpeed = 99f;
    [SerializeField] private float _rotSpeed = 99f;
    
    void Start()
    {
        
    }

    void Update()
    {
        GameObject follow = GameObject.FindGameObjectWithTag("Enemy");
                
        if (follow != null)
        {
            EnemyOne = follow.transform;
        }

        if (EnemyOne == null)
        {
            transform.Translate(Vector3.up * _missileSpeed * Time.deltaTime);

            if (transform.position.y >= 60 || transform.position.y <= -60 || transform.position.x <= -90 || transform.position.x >= 90)
            {
                Destroy(this.gameObject);
            }
        }

        //here we know for sure we have a Player
        if (EnemyOne != null)
        {
            Vector3 dir = EnemyOne.position - transform.position;
            dir.Normalize();

            float zAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

            Quaternion desiredRotation = Quaternion.Euler(0, 0, zAngle);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, desiredRotation, _rotSpeed * Time.deltaTime);

            transform.Translate(Vector3.up * _missileSpeed * Time.deltaTime);
        }
    }
}
