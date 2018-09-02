using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineerSpawner : MonoBehaviour {

    public GameObject _engineerPrefab;

    private bool _isActive = true;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Spawn an Engineer
    public void SpawnEngineer()
    {
        Instantiate(_engineerPrefab, transform.position, Quaternion.identity);
    }

    public void LockDown()
    {
        IsActive = false;
    }


    #region Properties
    public bool IsActive
    {
        get
        {
            return _isActive;
        }

        private set
        {
            _isActive = value;
        }
    }
    #endregion // Properties

}
