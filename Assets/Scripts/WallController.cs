using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour {

    private float _wallHP;


	// Use this for initialization
	void Start () {
        _wallHP = 100f;
	}
	
	// Update is called once per frame
	void Update () {
		// Will need to handle any animation changes during certain damage thresholds
	}


    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        GameObject obj = collider.gameObject;

        if (obj.tag == "Projectile")
        {
            Destroy(obj);
            TakeDamage();
        }
    }

    // Logic for wall getting repaired
    // TO be called externally by a repairing Engineer
    public void RepairDamage()
    {

    }

    // Logic for wall taking damage
    private void TakeDamage()
    {
        
    }

}
