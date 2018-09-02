using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour {

    public GameObject[] _door = new GameObject[6];
    public float _spawnDelay;

    private GameController _gameController;

    private void Awake()
    {
        GameObject GC = GameObject.FindGameObjectWithTag("GameController");
        _gameController = GC.GetComponent<GameController>();
    }

    // Use this for initialization
    void Start () {
        StartCoroutine(SpawnEngineers());
    }
	
	// Update is called once per frame
	void Update () {
        
	}

    // Coroutine to handle the spawning of Engineer NPCs
    private IEnumerator SpawnEngineers()
    {
        int spawnIndex;

        // Begin spawn loop
        while (true)
        {
            // Stop the spawn loop if game ends
            if (_gameController.IsGameOver || _gameController.IsGameWin)
                Destroy(gameObject);

            // Select a door
            spawnIndex = Random.Range(0, 4);
            // Reroll index if the door is inactive
            while (_door[spawnIndex].GetComponent<EngineerSpawner>().IsActive == false)
                spawnIndex = Random.Range(0, 4);

            // Wait the necessary delay, and then spawn at the selected door
            yield return new WaitForSeconds(_spawnDelay);
            _door[spawnIndex].GetComponent<EngineerSpawner>().SpawnEngineer();
        } // while
    }



}
