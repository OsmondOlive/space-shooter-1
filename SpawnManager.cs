using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{

    [SerializeField] private GameObject _enemyOne;
    [SerializeField] private GameObject _enemyOneContainer;
    [SerializeField] private bool _stopSpawning = false;

    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        
    }

    //spawn an object every 9 seconds
    //Create a coroutine of type IEnumerator -- Yield Events
    //while loop

    IEnumerator SpawnRoutine()
    {
        //while loop(infinite loop) - parece q dentro da coroutine os loops infinitos são viáveis...
            //instantiate enemy prefab
            //yield wait for 3 seconds
        //yield return null; //wait 1 frame (example)
        //then we go for this line...
        //yield return new WaitForSeconds(3.69f); (example)

        while (_stopSpawning == false)
        {
            Vector3 posToSpawn = new Vector3(Random.Range(-81f, 81f), Random.Range(99f, 60f), -2.1f);
            GameObject enemyOneSpawn = Instantiate(_enemyOne, posToSpawn, Quaternion.Euler(0, 0, 0)); //Quaternion.identity is used when i dont care for rotation
            enemyOneSpawn.transform.parent = _enemyOneContainer.transform;
            yield return new WaitForSeconds(3.0f);
        }
    }

    public void WhenPlayerDies()
    {
        _stopSpawning = true;
    }

    public void WhenEnemyDies()
    {
        _stopSpawning = false;
    }

}
