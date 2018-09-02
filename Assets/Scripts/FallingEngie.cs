using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingEngie : MonoBehaviour {

    public float _speed = 4;
    private Vector3 _direction;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start()
    {
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _direction = Random.insideUnitCircle;

        //Projectiles move towards mouse position once instantiated
        rb.velocity = new Vector2(_direction.x, _direction.y).normalized * _speed;
        StartCoroutine(WaitToDisappear());
    }

    // Update is called once per frame
    void Update()
    {
        // Slow down
        _speed -= 0.05f;
        if (_speed < 0f)
            _speed = 0f;
        rb.velocity = new Vector2(_direction.x, _direction.y).normalized * _speed;
    }


    IEnumerator WaitToDisappear()
    {
        yield return new WaitForSeconds(0.75f);
        Destroy(gameObject);
    }
}
