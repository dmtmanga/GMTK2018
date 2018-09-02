using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {

    public Projectile _projectilePrefab;
    public AudioSource _audioSource;
    public float _fireDelay;
    public float _fireSpread;
    public AudioClip _fireSfx;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

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
            _audioSource.PlayOneShot(_fireSfx);
            OneShot();
        }
    }

    private void OneShot()
    {
        Instantiate(_projectilePrefab, gameObject.transform);
    }
}
