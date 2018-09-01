using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    #region Fields

    // Public
    public Laser _laser;

    private float _repairProgress = 0f;

    // Private
    private int _numOfEngineers = 0;

    #endregion // Fields


    #region Methods

    // Use this for initialization
    void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject obj = collider.gameObject;

        if (obj.tag == "Engineer")
        {
            // If there's room, take on another engineer for repair
            if (_numOfEngineers <= 3)
            {
                _numOfEngineers++;
                // Engineer should go into repair state
            }
        }
    }

    #endregion // Methods





}
