using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour {

    public GameObject _door;
    public GameObject[] _repairPoint = new GameObject[2];
    public float _wallMaxHP = 100f;

    private GameController _gameController;
    private float _wallHP;
    private float _repairSpeed = 0.1f;
    private float[] _wallDamageThreshold = {75f, 50f, 25f};

    private void Awake()
    {
        GameObject GC = GameObject.FindGameObjectWithTag("GameController");
        _gameController = GC.GetComponent<GameController>();
    }

    // Use this for initialization
    void Start () {
        _wallHP = _wallMaxHP;
	}
	
	// Update is called once per frame
	void Update () {
        float percentHP = 100 * _wallHP / _wallMaxHP;

        foreach (GameObject rp in _repairPoint)
            if (rp.GetComponent<RepairPointController>().IsOccupied)
                RepairDamage(_repairSpeed);
	}


    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject obj = collision.collider.gameObject;

        if (obj.tag == "Projectile")
        {
            Destroy(obj);
            if (!DoorIsLockedDown)
                TakeDamage(obj.GetComponent<Projectile>()._power);
        }
    }


    public GameObject GetAvailableRepairPoint()
    {
        if (HasRoomForRepair)
        {
            GameObject result = null;
            foreach (GameObject repairPoint in _repairPoint)
            {
                if (!repairPoint.GetComponent<RepairPointController>().IsOccupied)
                {
                    result = repairPoint;
                    break;
                }
            }
            return result;
        }
        return null;
    }

    // Logic for wall getting repaired
    // TO be called externally by a repairing Engineer
    public void RepairDamage(float repairedHP)
    {
        float newHP = _wallHP + repairedHP;
        if (newHP > 100f)
            _wallHP = 100f;
        else
            _wallHP = newHP;
    }

    // Logic for wall taking damage
    private void TakeDamage(float damage)
    {
        float newHP = _wallHP - damage;
        // Check if wall is to be destroyed
        if (newHP < 0f)
        {
            _wallHP = 0f;
            _door.GetComponent<EngineerSpawner>().LockDown();
            Debug.Log(gameObject.name + " is destroyed! Door locked down!");
        }
        // Otherwise, check if a threshold was breached
        else
        {
            float newPercentHP = 100 * (newHP / _wallMaxHP);
            float currPercentHP = 100 * (_wallHP / _wallMaxHP);

            if (newPercentHP < _wallDamageThreshold[0] && 
                currPercentHP >= _wallDamageThreshold[0])
                Debug.Log(gameObject.name + " is damaged!  :  " + _wallHP.ToString());
            else if (newPercentHP < _wallDamageThreshold[1] &&
                currPercentHP >= _wallDamageThreshold[1])
                Debug.Log(gameObject.name + " is highly damaged!!  :  " + _wallHP.ToString());
            else if (newPercentHP < _wallDamageThreshold[2] &&
                currPercentHP >= _wallDamageThreshold[2])
                Debug.Log(gameObject.name + " is in critical condition!!!  :  " + _wallHP.ToString());
            
            // finally, make sure wall hp is updated
            _wallHP = newHP;
        }
    }

    #region Properties
    public bool IsDamaged
    {
        get
        {
            // Check for % HP and compare to damage threshold
            if ((_wallHP/_wallMaxHP) * 100 < _wallDamageThreshold[0])
                return true;
            return false;
        }

    }
    public bool IsRepaired
    {
        get
        {
            // Check for % HP and compare to damage threshold
            if ((_wallHP / _wallMaxHP) * 100 > 90f)
                return true;
            return false;
        }

    }
    public bool HasRoomForRepair
    {
        get
        {
            int count = 0;
            foreach (GameObject rp in _repairPoint)
                if (rp.GetComponent<RepairPointController>().IsOccupied)
                    count++;
            if (count >= 2)
                return false;
            return true;
        }
    }
    public bool DoorIsLockedDown
    {
        get
        {
            if (_wallHP == 0f)
                return true;
            return false;
        }

    }
    #endregion // Properties
}
