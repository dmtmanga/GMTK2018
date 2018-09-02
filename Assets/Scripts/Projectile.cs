using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public float _speed;
    public float _power;
    private Vector3 _direction;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start () {
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _direction = target - transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        // Projectiles move towards mouse position once instantiated
        rb.velocity = new Vector2(_direction.x, _direction.y).normalized * _speed;
    }


    public void OnDestroy()
    {
        // Do something here. Maybe cause an explosion animation?
    }

}
