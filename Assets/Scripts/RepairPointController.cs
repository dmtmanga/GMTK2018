using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairPointController : MonoBehaviour {

    private bool _isOccupied = false;
    private ParticleSystem _sparks;

    private void Awake()
    {
        _sparks = GetComponent<ParticleSystem>();
    }

    // Use this for initialization
    void Start () {
        _sparks.Stop();
	}
	
	// Update is called once per frame
	void Update () {
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsOccupied)
            return;

        GameObject obj = collider.gameObject;

        // Stuff happens if the obj is an engie who is here to do work on the wall
        if(obj.tag == "Engineer" && obj.GetComponent<EngineerController>().CurrentObjective == EngineerController.Objective.Wall)
        {
            IsOccupied = true;
            obj.GetComponent<EngineerController>().IsRepairingWall = true;
            Vector3 direction = transform.parent.position - obj.transform.position;
            obj.GetComponent<Animator>().SetFloat("DiffX", direction.x);
            obj.GetComponent<Animator>().SetFloat("DiffY", direction.y);
            obj.GetComponent<Animator>().SetBool("xIsGreater", Mathf.Abs(direction.x) > Mathf.Abs(direction.y));
            obj.transform.position = new Vector2(transform.position.x, transform.position.y);
        }
    }

    public void GameOverSparks()
    {
        // Game is over

        // Disable the collider component
        GetComponent<BoxCollider2D>().enabled = false;

        // Pump up the sparks
        _sparks.emissionRate = 200;
        _sparks.startLifetime = 2;
        _sparks.startSpeed = 10;
    }

    #region Properties
    public bool IsOccupied
    {
        get
        {
            return _isOccupied;
        }

        set
        {
            // Set sparks on/off based on if occupied
            if (value)
            {
                _sparks.Play();
            }
            else
            {
                _sparks.Stop();
            }
            _isOccupied = value;
        }
    }
    #endregion // Properties
}
