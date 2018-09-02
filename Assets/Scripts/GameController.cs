using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {


    private bool _gameOver = false;
    private bool _gameWin = false;
    private PlayerController _player;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    #region Properties
    public bool IsGameOver
    {
        get
        {
            return _gameOver;
        }

        private set
        {
            _gameOver = value;
        }
    }

    public bool IsGameWin
    {
        get
        {
            return _gameWin;
        }

        set
        {
            _gameWin = value;
        }
    }
    #endregion // Properties
}
