using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMissileBehavior_II : MonoBehaviour
{
    public MeshRenderer _baseMissileRenderer;
    [SerializeField] private float _missileRate = 1.2f;
    private float _canMissile = 0.3f;

    public AudioClip _missileSound;
    [SerializeField] private AudioSource _missileSource;

    void Start()
    {
        _missileSource = GetComponent<AudioSource>();

        if (_missileSource == null)
        {
            Debug.LogError("Audiosource on the missile = NULL!");
        }

        else
        {
            _missileSource.clip = _missileSound;
        }
    }

    public void OnBecameInvisible()
    {
        _baseMissileRenderer.enabled = false;
    }

    public void OnBecameVisible()
    {
        _baseMissileRenderer.enabled = true;
    }

    void Update()
    {
        //when prefab instantiate mesh renderer off

        if (Time.time > _canMissile && Input.GetKeyDown(KeyCode.Mouse1) || (Time.time > _canMissile && Input.GetKeyDown(KeyCode.AltGr)))
        {
            _canMissile = Time.time + _missileRate;
            OnBecameInvisible();
            StartCoroutine(cooldown());
            _missileSource.Play();
        }
    }

    public IEnumerator cooldown()
    {
        yield return new WaitForSeconds(.9f);
        OnBecameVisible();
    }
}
