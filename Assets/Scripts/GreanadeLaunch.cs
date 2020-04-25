using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreanadeLaunch : MonoBehaviour
{
    public FlashGreanade flashPrefab;
    public Molotov molotovPrefab;
    public SmokeGrenade smokePrefab;

    public Transform cam;
    private AudioSource asrc;
    public AudioClip flashRadioSound;
    public AudioClip molotovRadioSound;
    public AudioClip smokeRadioSound;

    public Animator handsAnim;

    [Range(10,30)]
    public int hardLaunchForce;

    [Range(1, 10)]
    public int softLaunchForce;
    
    // Start is called before the first frame update
    void Start()
    {
        asrc = GetComponent<AudioSource>();
        cam.gameObject.SetActive(false);
        cam.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        bool hardFlashLaunch = Input.GetKeyDown(KeyCode.Q);
        bool softFlashLaunch = Input.GetKeyDown(KeyCode.E);

        bool molotov = Input.GetKeyDown(KeyCode.Z);
        bool smoke = Input.GetKeyDown(KeyCode.X);


        if (hardFlashLaunch || softFlashLaunch)
        {
            // Creamos la granada
            FlashGreanade fg = Instantiate(flashPrefab, cam.position + cam.forward * 1, Quaternion.identity);

            // La granada tiene que tener un Rigidbody, para que podamos aplicar una fuerza sobre ella
            Rigidbody rb = fg.gameObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;  // Esto se hace para que la granada no atraviese las paredes

            // Aplicamos una fuerza sobre ella para lanzarla, y activamos su mecanismo
            float launchForce = (hardFlashLaunch) ? hardLaunchForce : softLaunchForce;    // Con un hardLaunch lanzamos la granada con fuerza = 20. Con un softLaunch, la lanzamos con fuerza = 2
            fg.GetComponent<Rigidbody>().AddForce(cam.forward * launchForce, ForceMode.Impulse);
            fg.Launch();
            asrc.clip = flashRadioSound;
            asrc.Play();
        }
        else if (molotov)
        {
            Molotov m = Instantiate(molotovPrefab, cam.position + cam.forward * 1, Quaternion.identity);

            Rigidbody rb = m.gameObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;  // Esto se hace para que la granada no atraviese las paredes

            rb.AddForce(cam.forward * hardLaunchForce, ForceMode.Impulse);
            rb.angularVelocity = new Vector3(0, 3, 10);
            m.Launch();

            asrc.clip = molotovRadioSound;
            asrc.Play();
        }
        else if (smoke) {

            Invoke("ThrowSmoke", 0.9f);
            handsAnim.Play("My_Grenade_Throw");

            asrc.clip = smokeRadioSound;
            asrc.Play();
        }


    }

    void ThrowSmoke() {
        SmokeGrenade s = Instantiate(smokePrefab, cam.position + cam.forward * 1, Quaternion.identity);

        Rigidbody rb = s.gameObject.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;  // Esto se hace para que la granada no atraviese las paredes
        //rb.mass = 1.5f;

        rb.AddForce(cam.forward * hardLaunchForce, ForceMode.Impulse);
        rb.angularVelocity = new Vector3(0, 3, 10);
        //s.Launch();
    }

}


