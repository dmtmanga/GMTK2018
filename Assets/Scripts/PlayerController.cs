﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Fields

    // Public
    public GameObject _fallingEngiePrefab;
    public Laser _laser;
    public float[] repairSpeed = new float[3];

    private float _repairProgress = 0f;
    private float _mouseCursorSpeedX;
    private float _mouseCursorSpeedY;
    private GameController _gameController;
    private int _numOfEngineers = 0;
    private bool _isDroppingEngie = false;

    #endregion // Fields


    #region Methods

    private void Awake()
    {
        GameObject GC = GameObject.FindGameObjectWithTag("GameController");
        _gameController = GC.GetComponent<GameController>();
    }

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        // If win condition has been met, the laser can stop shooting
        if (_gameController.IsGameWin)
        {
            return;
        }

        // Repair progress bar should update
        // Amount repaired will increase based on the number of engineers doing repairs
        if (_numOfEngineers > 0)
            Repair();

        // Check if mouse moved too fast
        _mouseCursorSpeedX = Input.GetAxis("Mouse X") / Time.deltaTime;
        _mouseCursorSpeedY = Input.GetAxis("Mouse Y") / Time.deltaTime;

        if ( !_isDroppingEngie && _numOfEngineers > 0 && (_mouseCursorSpeedX > 70f ||_mouseCursorSpeedY > 70f))
            LoseAnEngineer();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.collider.gameObject;

        if (obj.tag == "Engineer" && obj.GetComponent<EngineerController>().CurrentObjective == EngineerController.Objective.Player)
        {
            // If there's room, take on another engineer for repair
            if (_numOfEngineers < 3)
            {
                _numOfEngineers++;
                // Player sprite is updated and engineer object destroyed
                switch (_numOfEngineers) {
                    case 0:
                        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                        break;
                    case 1:
                        gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                        break;
                    case 2:
                        gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
                        break;
                    case 3:
                        gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                        break;
                }
                Destroy(obj);
            }
        }
    }

    private IEnumerator DropEngineer()
    {
        Instantiate(_fallingEngiePrefab, transform);

        yield return new WaitForSeconds(1);
        _isDroppingEngie = false;
    }

    // Animates the loss of an engineer and decrements counter
    private void LoseAnEngineer()
    {
        _isDroppingEngie = true;
        StartCoroutine(DropEngineer());

        _numOfEngineers--;
        switch (_numOfEngineers)
        {
            case 0:
                gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                break;
            case 1:
                gameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                break;
            case 2:
                gameObject.GetComponent<SpriteRenderer>().color = Color.cyan;
                break;
            case 3:
                gameObject.GetComponent<SpriteRenderer>().color = Color.green;
                break;
        }
    }

    private void Repair()
    {
        if (_numOfEngineers > 0)
            _repairProgress += repairSpeed[_numOfEngineers - 1];

        if (_repairProgress >= 100f)
        {
            _gameController.IsGameWin = true;
            GetComponent<SpriteRenderer>().color = Color.blue;
            Destroy(_laser);
        }
    }

    #endregion // Methods

    #region Properties
    public bool HasRoomForRepair
    {
        get
        {
            if (_numOfEngineers <3)
                return true;
            return false;
        }
    }
    #endregion // Properties

}
