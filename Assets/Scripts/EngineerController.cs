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
    private Animator _anim;

    private bool _isRepairingWall = false;
    private bool _isStunned = false;

    private void Awake()
    {
        GameObject PC = GameObject.FindGameObjectWithTag("Player");
        GameObject[] wall = GameObject.FindGameObjectsWithTag("Wall");
        _player = PC.GetComponent<PlayerController>();
        _anim = GetComponent<Animator>();
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
                if (!IsStunned)
                {
                    Vector3 direction = _player.transform.position - transform.position;
                    _anim.SetBool("IsIdle", false);
                    _anim.SetFloat("MoveX", direction.x);
                    _anim.SetFloat("MoveY", direction.y);
                    _anim.SetBool("xIsGreater", Mathf.Abs(direction.x) > Mathf.Abs(direction.y));
                    transform.Translate(direction.normalized * Time.deltaTime * _moveSpeed);
                }
                break;
            case Objective.Wall:
                // If the target repair point becomes occupied, change objective;
                if (_targetRepairPoint.GetComponent<RepairPointController>().IsOccupied)
                    StartCoroutine(DecideObjective());
                if (!IsStunned)
                {
                    Vector3 direction = _targetRepairPoint.transform.position - transform.position;
                    _anim.SetBool("IsIdle", false);
                    _anim.SetFloat("MoveX", direction.x);
                    _anim.SetFloat("MoveY", direction.y);
                    _anim.SetBool("xIsGreater", Mathf.Abs(direction.x) > Mathf.Abs(direction.y));
                    transform.Translate(direction.normalized * Time.deltaTime * _moveSpeed);
                }
                break;
            case Objective.Player:
                // If the player has no more room, change objective;
                if (!_player.HasRoomForRepair)
                    StartCoroutine(DecideObjective());
                if(!IsStunned)
                {
                    Vector3 direction = _player.transform.position - transform.position;
                    _anim.SetBool("IsIdle", false);
                    _anim.SetFloat("MoveX", direction.x);
                    _anim.SetFloat("MoveY", direction.y);
                    _anim.SetBool("xIsGreater", Mathf.Abs(direction.x) > Mathf.Abs(direction.y));
                    transform.Translate(direction.normalized * Time.deltaTime * _moveSpeed);
                }
                break;
            case Objective.Stand:
                _anim.SetBool("IsIdle", true);
                StartCoroutine(DecideObjective());
                break;
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.collider.gameObject;

        if (obj.tag == "Projectile")
        {
            Destroy(obj);
            _anim.SetBool("IsHit", true);
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
        yield return new WaitForSeconds(1);
        _anim.SetBool("IsKO", true);    
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
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
                // Starting repairs
                CurrentObjective = Objective.Wall;
            }
            else
            {
                // Finished repairing
                CurrentObjective = Objective.New;
                _targetRepairPoint.GetComponent<RepairPointController>().IsOccupied = false;
            }
            _anim.SetBool("IsRepairing", value);
            _isRepairingWall = value;
        }
    }
    
    #endregion // Properties
}
