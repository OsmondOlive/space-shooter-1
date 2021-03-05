using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicShot : MonoBehaviour
{
    [SerializeField] private float _basicShotSpeed = 150f;

    void Update()
    {
        transform.Translate(Vector3.up * _basicShotSpeed * Time.deltaTime, Space.World);

        if (transform.position.y >= 60)
        {
            Destroy(this.gameObject);
        }
    }
}
