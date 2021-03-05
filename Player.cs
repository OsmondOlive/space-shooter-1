using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _playerHP = 99.0f;
    [SerializeField] private GameObject _basicShot;
    [SerializeField] private GameObject _missile;
    [SerializeField] private float _missileRate = .9f;
    private float _canMissile = 0f;
    [SerializeField] private GameObject _missileII;
    [SerializeField] private float _missileRateII = 1.2f;
    private float _canMissileII = 0.3f;
    private bool _canShot = true;

    [SerializeField] private SpawnManager _spawnManager;

    //inspector vars
    [SerializeField] private float _baseXSpeed = 81f;
    [SerializeField] private float _baseYSpeed = 63f;
    [SerializeField] private float _spinSmooth = 1.8f;
    [SerializeField] private float _spinBoost = .9f;
    private enum Axis { x, y, z }
    [SerializeField] private Axis _spinConstraintAxis = Axis.y;
    // These are the stored angles 
    private Vector3 _originalRot;
    private Vector3 _targetSpinRot;
    private Vector3 _lastRot = new Vector3();
    private Vector3 _adjustedRot = new Vector3();
    private Vector3 _axis = new Vector3();
    private int _spinMove = 0;
    private float _xSpeed = 0;
    private float _ySpeed = 0;

    //concentration time hold variables
    private const float _minimumHeldDuration = .3f;
    private float _holdPressedTime = 0;
    private bool _hold = false;
    private bool _concentrationhold = false;

    [SerializeField] private ParticleSystem laser_BA;
    [SerializeField] private ParticleSystem lightSpot;
    [SerializeField] private AudioClip laserSound;
    [SerializeField] private ParticleSystem _concentration;
    [SerializeField] private ParticleSystem _concentrationII;
    [SerializeField] private AudioClip concentrationSound;
    [SerializeField] private AudioClip concentrationSound_II;
    [SerializeField] private ParticleSystem powerShot_I;
    [SerializeField] private ParticleSystem lightSpot_I;
    [SerializeField] private ParticleSystem powerShot_II;
    [SerializeField] private AudioClip superShotSound_I;
    [SerializeField] private AudioClip superShotSound_II;

    void Start()
    {
        _xSpeed = _baseXSpeed;
        _ySpeed = _baseYSpeed;
        _originalRot = transform.rotation.eulerAngles;
        _adjustedRot = _originalRot;
        _targetSpinRot = new Vector3(_originalRot.x + 360f, _originalRot.y + 360f, _originalRot.z + 360f);
        _spawnManager = GameObject.Find("Spawn-Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
            Debug.LogError("The Spawn Manager is NULL.");
    }

    void Update()
    {
        Moves();

        Shots();
    }

    float _lastTapLeftTime;
    float _lastTapRightTime;

    public void Moves()
    {
        if (Input.GetAxis("Vertical") > 0)
            transform.Translate(Vector3.up * _ySpeed * Time.deltaTime, Space.World);
        if (Input.GetAxis("Vertical") < 0)
            transform.Translate(-Vector3.up * _ySpeed * Time.deltaTime, Space.World);
        if (Input.GetAxis("Horizontal") > 0)
            transform.Translate(Vector3.right * _xSpeed * Time.deltaTime, Space.World);
        if (Input.GetAxis("Horizontal") < 0)
            transform.Translate(-Vector3.right * _xSpeed * Time.deltaTime, Space.World);

        if (transform.position.y >= 44.68f)
            transform.position = new Vector3(transform.position.x, 44.68f, 0);

        if (transform.position.y <= -51.9f)
            transform.position = new Vector3(transform.position.x, -51.9f, 0);

        if (transform.position.x > 96f)
            transform.position = new Vector3(-96f, transform.position.y, 0);

        if (transform.position.x < -96f)
            transform.position = new Vector3(96f, transform.position.y, 0);

        if (_spinMove == 0 && Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.UpArrow))
        {
            if ((Time.time - _lastTapLeftTime) < 0.3f)//DOUBLE TAP
                _spinMove = 1;

            _lastTapLeftTime = Time.time;
        }

        if (_spinMove == 0 && Input.GetKeyDown(KeyCode.D) ||
            Input.GetKeyDown(KeyCode.RightArrow))
        {
            if ((Time.time - _lastTapRightTime) < 0.3f)//DOUBLE TAP
                _spinMove = 2;

            _lastTapRightTime = Time.time;
        }

        if (_spinMove == 1)
            SpinMove(false);

        if (_spinMove == 2)
            SpinMove(true);
    }

    public void Shots()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            //Use has pressed the Space key. We don't know if they'll release or hold it, so keep track of when they started holding it.
            _holdPressedTime = Time.timeSinceLevelLoad;
            _hold = false;
            _concentrationhold = true;
        }

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Mouse0))
            if (Time.timeSinceLevelLoad - _holdPressedTime > _minimumHeldDuration)
                // Player has held the Space key for .3 seconds. Consider it "held"
                _hold = true;

        //============================================================================================================
        //concentration
        if (_concentrationhold == true && _hold == true &&
            Time.timeSinceLevelLoad - _holdPressedTime <= 4.5f)
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.PlayOneShot(concentrationSound);
            _concentration.Emit(1);

            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse0))
                _concentrationhold = false;
        }

        if (_concentrationhold == true && _hold == true &&
            Time.timeSinceLevelLoad - _holdPressedTime >= 3.6f)
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.PlayOneShot(concentrationSound_II);
            _concentrationII.Emit(1);

            if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Mouse0))
                _concentrationhold = false;
        }
        //============================================================================================================

        if (_hold == true && Time.timeSinceLevelLoad - _holdPressedTime < 3.9f && (Input.GetKeyUp(KeyCode.Mouse0)
            || _hold == true && Time.timeSinceLevelLoad - _holdPressedTime < 3.9f && Input.GetKeyUp(KeyCode.Space)))
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.Stop();
            audio.PlayOneShot(superShotSound_I);
            lightSpot_I.Emit(1);
            powerShot_I.Emit(1);
        }

        if (_hold == true && Time.timeSinceLevelLoad - _holdPressedTime > 3.9f && (Input.GetKeyUp(KeyCode.Mouse0)
            || _hold == true && Time.timeSinceLevelLoad - _holdPressedTime > 3.9f && Input.GetKeyUp(KeyCode.Space)))
        {
            AudioSource audio = GetComponent<AudioSource>();
            audio.Stop();
            audio.PlayOneShot(superShotSound_II);
            powerShot_II.Play();
            _canShot = false;
            // your particles can go on
            StartCoroutine(BurstTimer());
        }

        if (_hold == false && Input.GetKeyUp(KeyCode.Space) && _canShot ||
            _hold == false && (Input.GetKeyUp(KeyCode.Mouse0)) && _canShot)
        {
            _canShot = true;
            AudioSource audio = GetComponent<AudioSource>();
            audio.PlayOneShot(laserSound);
            laser_BA.Emit(1);
            lightSpot.Emit(1);
            Instantiate(_basicShot, transform.position, Quaternion.identity);
        }

        if (Time.time > _canMissile && Input.GetKeyDown(KeyCode.Mouse1) ||
            (Time.time > _canMissile && Input.GetKeyDown(KeyCode.AltGr)))
        {
            _canMissile = Time.time + _missileRate;
            Instantiate(_missile, transform.position + new Vector3(3.6f, -0.3f, 0), Quaternion.identity);
        }

        if (Time.time > _canMissileII && Input.GetKeyDown(KeyCode.Mouse1) ||
            (Time.time > _canMissileII && Input.GetKeyDown(KeyCode.AltGr)))
        {
            _canMissileII = Time.time + _missileRateII;
            Instantiate(_missile, transform.position + new Vector3(-3.6f, -0.3f, 0), Quaternion.identity);
        }
    }

    public void SpinMove(bool antiClockwise)
    {
        //Increase speed
        _xSpeed = _baseXSpeed * _spinBoost;
        _ySpeed = _baseYSpeed * _spinBoost;

        _adjustedRot[(int)_spinConstraintAxis] += _targetSpinRot[(int)_spinConstraintAxis]
            * Time.deltaTime * (antiClockwise ? -_spinSmooth : _spinSmooth);

        if (_adjustedRot[(int)_spinConstraintAxis] > _targetSpinRot[(int)_spinConstraintAxis])
        {//Rotation complete
         //restore speed
            _xSpeed = _baseXSpeed;
            _ySpeed = _baseYSpeed;

            //this is the axis we are modifying
            _axis[(int)_spinConstraintAxis] = _originalRot[(int)_spinConstraintAxis];

            //restore rotations
            _adjustedRot = _originalRot;
            _lastRot = _originalRot;

            //restore ship rotation
            transform.rotation = Quaternion.Euler(-90f, 0, 0);

            //reset the spinmove allowing for another 
            _spinMove = 0;

            //exit the void now we are all done
            return;
        }
        if (_adjustedRot[(int)_spinConstraintAxis] < -_targetSpinRot[(int)_spinConstraintAxis])
        {//Rotation complete
         //restore speed
            _xSpeed = _baseXSpeed;
            _ySpeed = _baseYSpeed;


            //this is the axis we are modifying
            _axis[(int)_spinConstraintAxis] = _originalRot[(int)_spinConstraintAxis];

            //restore rotations
            _adjustedRot = _originalRot;
            _lastRot = _originalRot;

            //restore ship rotation
            transform.rotation = Quaternion.Euler(_originalRot);

            //reset the spinmove allowing for another 
            _spinMove = 0;

            //exit the void now we are all done
            return;
        }

        //if we get to here we still need to rotate, lets do it

        float adjust = _adjustedRot[(int)_spinConstraintAxis] - _lastRot[(int)_spinConstraintAxis];

        _axis[(int)_spinConstraintAxis] = adjust;

        Quaternion localRotation = Quaternion.Euler(_axis);

        transform.rotation = transform.rotation * localRotation;

        _lastRot[(int)_spinConstraintAxis] = _adjustedRot[(int)_spinConstraintAxis];
    }

    public void DamageInPlayer()
    {
        _playerHP = _playerHP - Random.Range(9, 99);

        if (_playerHP <= 0)
        {
            //I can communicate with SpawManager to stop
            //Let them know to stop spawing when player is destroyed (for example)
            _spawnManager.WhenPlayerDies(); //find the GameObject. Then Get Component.
            Destroy(this.gameObject);
        }
    }

    IEnumerator BurstTimer()
    {
        yield return new WaitForSeconds(2.5f);
        _canShot = true;
    }
}