using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    public WallController[] _wallControllers = new WallController[6];
    public AudioClip _bgmIntro;
    public AudioClip _bgmLoop;

    private AudioSource _audioSource;
    private bool _gameOver = false;
    private bool _gameWin = false;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization
    void Start () {
        _audioSource.clip = _bgmLoop;
        _audioSource.Play();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        if (IsGameOver || IsGameWin)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                SceneManager.LoadScene("Level01");
        }

        // Check if Game Over
        int count = 0;
        foreach (WallController wall in _wallControllers)
            if (wall.DoorIsLockedDown)
                count++;
        if (count >= 6)
            IsGameOver = true;
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
