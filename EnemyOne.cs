using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum States {moveUp, moveDown};

public class EnemyOne : MonoBehaviour
{

    [SerializeField] private float _enemyHP = 99f;
    [SerializeField] private float _enemySpeed = 33f;
    [Tooltip("rotation speed Angles per second")]
    [SerializeField] private float rotationSpeed = 150f;

    [SerializeField] private float topY = 99f;
    [SerializeField] private float bottomY = 0f;

    private Vector3 targetPoint;
    private States state = States.moveDown;
    private SpawnManager _spawnManager;

    void Start()
    {
        float random_XSpawn = Random.Range(-81f, 81f);
        transform.position = new Vector3(random_XSpawn, 56.5f, -2.1f);

        _spawnManager = GameObject.Find("Spawn-Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
            Debug.LogError("The Spawn Manager is NULL.");

        //init target point
        targetPoint = new Vector3(Random.Range(-81f, 81f), bottomY, -2.1f);
    }


    //podemos fazer asteroides de diferentes tamanhos surgindo aleatoriamente 
    //sendo spawnados em diferentes direções e velocidades.

    void Update()
    {
        MoveShip();
        //transform.Translate(Vector3.down * _enemySpeed * Time.deltaTime, Space.World);

        //float random_XSpawn = Random.Range(-81f, 81f);
        //float random_YSpawn = Random.Range(99f, 60f);

        //if (transform.position.y < -60f)
        //{
        //    transform.position = new Vector3(random_XSpawn, random_YSpawn, -2.1f);
        //}
        if (transform.position.y >= 90 || transform.position.y <= -90 || transform.position.x <= -99 || transform.position.x >= 99)
        {
            Destroy(this.gameObject);
        }
    }

    private void MoveShip()
    {
        //create direction to target point
        Quaternion tRot = Quaternion.LookRotation(targetPoint - transform.position, Vector3.back);
        //and rotate ship direction to target direction
        transform.rotation = Quaternion.RotateTowards(transform.rotation, tRot, rotationSpeed * Time.deltaTime);
        //rotate ship only around 1 axis
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z);

        //move ship forward
        transform.position += transform.up * _enemySpeed * Time.deltaTime;

        //simple state machine
        switch (state)
        {
            case States.moveUp:
                //while we are moving up, check condition to change state
                if(transform.position.y >= topY)
                {
                    //set random bottom point
                    targetPoint = new Vector3(Random.Range(-81f, 81f), bottomY, -2.1f);
                    state = States.moveDown;
                }
                break;
            case States.moveDown:
                //while we are going down, check condition to change state
                if(transform.position.y <= bottomY)
                {
                    //set random bottom point
                    targetPoint = new Vector3(Random.Range(-81f, 81f), topY, -2.1f);
                    state = States.moveUp;
                }
                break;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            // make something
            DamageLaserMissile();
            Destroy(other.gameObject);
        }
        // If your ennemy could collision with the player
        if (other.tag == "Player")
        {
            other.transform.GetComponent<Player>().DamageInPlayer();
            Destroy(this.gameObject);
        }
        // If your ennemy could collision with laser
        if (other.tag == "Laser")
        {
            DamageLaserMissile();
            Destroy(other.gameObject);
        }
    }

    public void DamageLaserMissile()
    {
        _enemyHP = _enemyHP - Random.Range(9, 99);

        if (_enemyHP <= 0)
        {
            //I can communicate with SpawManager to stop
            //Let them know to stop spawing when player is destroyed (for example)
            _spawnManager.WhenEnemyDies(); //find the GameObject. Then Get Component.
            Destroy(this.gameObject);
        }
    }

    public void DamageSuperShot_I()
    {
        _enemyHP = _enemyHP - Random.Range(69, 99);

        if (_enemyHP <= 0)
        {
            //I can communicate with SpawManager to stop
            //Let them know to stop spawing when player is destroyed (for example)
            _spawnManager.WhenEnemyDies(); //find the GameObject. Then Get Component.
            Destroy(this.gameObject);
        }
    }

    public void DamageSuperShot_II()
    {
        _enemyHP = _enemyHP - Random.Range(0, 3);

        if (_enemyHP <= 0)
        {
            //I can communicate with SpawManager to stop
            //Let them know to stop spawing when player is destroyed (for example)
            _spawnManager.WhenEnemyDies(); //find the GameObject. Then Get Component.
            Destroy(this.gameObject);
        }
    }
}