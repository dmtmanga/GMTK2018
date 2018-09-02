using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepairPointController : MonoBehaviour {

    private bool _isOccupied = false;
    private GameObject _collider;

    private void Awake()
    {
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsOccupied)
            return;

        GameObject obj = collider.gameObject;

        if(obj.tag == "Engineer")
        {
            _collider = obj;
            IsOccupied = true;
            obj.GetComponent<EngineerController>().IsRepairingWall = true;
            obj.transform.position = new Vector2(transform.position.x, transform.position.y);
        }

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
            _isOccupied = value;
        }
    }
    #endregion // Properties
}
