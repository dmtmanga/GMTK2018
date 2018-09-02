using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

    public Projectile _projectilePrefab;
    public float _fireDelay;
    public float _fireSpread;


	// Use this for initialization
	void Start () {
        StartCoroutine(Shoot());
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    // Logic for shooting Projectiles
    private IEnumerator Shoot ()
    {
        while (true)
        {
            yield return new WaitForSeconds(_fireDelay);
            OneShot();
        }
    }

    private void OneShot()
    {
        Instantiate(_projectilePrefab, gameObject.transform);
    }
}
