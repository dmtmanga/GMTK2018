using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EngineerController : MonoBehaviour {

    // Declare enum for the possible objectives Engineers can have
    public enum Objective { New, Player, Rod, Stand, Wall, Walk};

    public float _moveSpeed = 0f;
    public float _decisionDelay = 0f;

    private PlayerController _player;
    //private SpawnController _spawnController;
    private Objective _objective = Objective.New;
    private WallController[] _wallControllers = new WallController[6];
    private GameObject _targetRepairPoint;

    private bool _isRepairingWall = false;
    private bool _isStunned = false;

    private void Awake()
    {
        GameObject PC = GameObject.FindGameObjectWithTag("Player");
        //GameObject SC = GameObject.FindGameObjectWithTag("SpawnController");
        GameObject[] wall = GameObject.FindGameObjectsWithTag("Wall");
        _player = PC.GetComponent<PlayerController>();
        //_spawnController = SC.GetComponent<SpawnController>();
        for (int i = 0; i < 6; i++)
            _wallControllers[i] = wall[i].GetComponent<WallController>();
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {

        // If currently repairing a wall, stay still
        if (IsRepairingWall)
        {
            if (_targetRepairPoint.transform.parent.gameObject.GetComponent<WallController>().IsRepaired)
            {
                IsRepairingWall = false;
            }
            return;
        }
        switch (CurrentObjective)
        {
            case Objective.New:
                // Engineer walks in towards the player a little before stopping to decide
                StartCoroutine(WalkIn());
                break;
            case Objective.Walk:
                // Currently walking towards player while thinking
                if(!IsStunned)
                    transform.Translate((_player.transform.position - transform.position).normalized * Time.deltaTime * _moveSpeed);
                break;
            case Objective.Wall:
                // If the target repair point becomes occupied, change objective;
                if (_targetRepairPoint.GetComponent<RepairPointController>().IsOccupied)
                    StartCoroutine(DecideObjective());
                if (!IsStunned)
                    transform.Translate((_targetRepairPoint.transform.position - transform.position).normalized * Time.deltaTime * _moveSpeed);
                break;
            case Objective.Player:
                // If the player has no more room, change objective;
                if (!_player.HasRoomForRepair)
                    StartCoroutine(DecideObjective());
                if(!IsStunned)
                    transform.Translate((_player.transform.position - transform.position).normalized * Time.deltaTime * _moveSpeed);
                break;
            case Objective.Stand:
                StartCoroutine(DecideObjective());
                break;
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collider = collision.collider.gameObject;

        if (collider.tag == "Projectile")
        {
            Destroy(collider);
            StartCoroutine(Stunned());
        }
    }

    private GameObject ChooseRepairPoint( GameObject[] walls)
    {
        Dictionary<float, GameObject> potentialWalls = new Dictionary<float, GameObject>();
        foreach (GameObject wall in walls)
            if (wall.GetComponent<WallController>().HasRoomForRepair)
                potentialWalls.Add(Mathf.Abs(Vector3.Distance(transform.position, wall.transform.position)), wall);

        // Find closest of the potential walls
        GameObject closestWall = FindClosestWall(potentialWalls);

        return closestWall.GetComponent<WallController>().GetAvailableRepairPoint();
    }

    private IEnumerator DecideObjective()
    {
        // Engineer needs some time to decide!
        CurrentObjective = Objective.Stand;
        yield return new WaitForSeconds(_decisionDelay);

        // First check if there are any walls that need tending
        GameObject[] damagedWalls = GetDamagedWalls();
        if (damagedWalls.Length > 0)
        {
            _targetRepairPoint = ChooseRepairPoint(damagedWalls);
            CurrentObjective = Objective.Wall;
        }
        // If walls aren't an option, there's the player
        else if (_player.HasRoomForRepair)
            CurrentObjective = Objective.Player;
        // If all else fails, drop a rod and attract some shots! (TO BE IMPLEMENTED)
        else
            CurrentObjective = Objective.Stand;
    }

    // Takes a dictionary of keys: distance to wall and values: wall object
    // Returns closest wall
    private GameObject FindClosestWall(Dictionary<float, GameObject> walls)
    {
        float minDistance = Mathf.Min(walls.Keys.ToArray());
        GameObject wall;
        bool success = walls.TryGetValue(minDistance, out wall);

        return success ? wall : null;
    }

    private GameObject[] GetDamagedWalls()
    {
        GameObject[] walls = new GameObject[6];
        int count = 0;

        // Look up and find damaged walls
        for (int i = 0; i < 6; i++)
            if (!_wallControllers[i].DoorIsLockedDown && 
                _wallControllers[i].IsDamaged 
                && _wallControllers[i].HasRoomForRepair)
            {
                walls[count] = _wallControllers[i].gameObject;
                count++;
            }

        // Return an array of the damaged walls
        GameObject[] ret = new GameObject[count];
        for (int i = 0; i < count; i++)
            ret[i] = walls[i];
        return ret;
    }

    private IEnumerator Stunned()
    {
        IsStunned = true;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().color = Color.red;
        // Some animation stuff here for stun

        yield return new WaitForSeconds(1);
        Destroy(gameObject);
        // Need some animation stuff here
    }

    private IEnumerator WalkIn()
    {
        CurrentObjective = Objective.Walk;
        yield return new WaitForSeconds(1);
        CurrentObjective = Objective.Stand;
    }

    #region Properties
    public bool IsStunned
    {
        get
        {
            return _isStunned;
        }
        private set
        {
            _isStunned = value;
        }
    }

    public Objective CurrentObjective
    {
        get
        {
            return _objective;
        }

        private set
        {
            _objective = value;
        }
    }

    public bool IsRepairingWall
    {
        get
        {
            return _isRepairingWall;
        }

        set
        {
            if (value == true)
            {
                CurrentObjective = Objective.Wall;
                gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;
                _isRepairingWall = value;
            }
            else
            {
                CurrentObjective = Objective.New;
                gameObject.GetComponent<SpriteRenderer>().color = Color.grey;
                _isRepairingWall = false;
            }
        }
    }
    
    #endregion // Properties
}
