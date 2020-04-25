using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenade : MonoBehaviour
{
    public ParticleSystem psPrefab;
    private ParticleSystem _ps;

    private Rigidbody _rb;

    public float smokeDuration;
    public AudioClip explodeSound;
    public AudioClip bounceSound;

    private AudioSource _as;

    private int _bounces = 0;
    private int max_bounces = 3;
    private float bounceForce = 5.0f;

    private bool exploded = false;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _as = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Launch()
    {
        Invoke("Explode", 0.5f);
        _as.Stop();
    }

    void Explode() {
        //_ps.gameObject.transform.rotation = Quaternion.Euler(-90, 0, 0);
        _ps = Instantiate(psPrefab, transform.position, Quaternion.Euler(-90, 0, 0));
        _ps.Play();

        exploded = true;

        _as.Stop();
        _as.clip = explodeSound;
        _as.Play();

        Invoke("CleanSmoke", smokeDuration);
    }

    void CleanSmoke() {
        _ps.Stop();
        Destroy(_ps.gameObject);
        Destroy(this.gameObject);
    }


    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            if (_bounces < max_bounces)
            {
                _bounces++;
                _rb.velocity /= 2;
                _rb.angularVelocity /= 2;

                _rb.AddForce(Vector3.up*bounceForce, ForceMode.Impulse);
                bounceForce /= 2;
            }
            else {
                _rb.angularVelocity = Vector3.zero;
                _rb.velocity = Vector3.zero;
                Explode();
            }
        }

        if (!exploded) { 
            _as.clip = bounceSound;
            _as.Play();
        }
    }

}
