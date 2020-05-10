using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreanadeLaunch : MonoBehaviour
{
    #region Attributes
    
    #region Prefabs
    public FlashGreanade flashPrefab;
    public Molotov molotovPrefab;
    public SmokeGrenade smokePrefab;
    #endregion

    #region Camera
    public Transform cam;
    #endregion


    #region Audio
    private AudioSource asrc;

    public AudioClip flashRadioSound;
    public AudioClip molotovRadioSound;
    public AudioClip smokeRadioSound;
    #endregion

    #region Animations
    public Animator handsAnim;  // Animator to perform hand animations
    #endregion

    #region Other Settings
    [Range(10,30)]
    public int hardLaunchForce;

    [Range(1, 10)]
    public int softLaunchForce;
    #endregion

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        asrc = GetComponent<AudioSource>();
        
        // I made this to avoid one annoying bug with the camera. No idea why is this needed
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
            // Creating the nade
            FlashGreanade fg = Instantiate(flashPrefab, cam.position + cam.forward * 1, Quaternion.identity);

            // Activate the mesh so the grenade is visible
            fg.gameObject.GetComponent<MeshRenderer>().enabled = true;

            // Grenade needs to have a Rigidbody component, so we can apply forces to it
            Rigidbody rb = fg.gameObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;  // This is to avoid the grenade go through walls

            // Apply a force to throw the nade
            float launchForce = (hardFlashLaunch) ? hardLaunchForce : softLaunchForce;    // Con un hardLaunch lanzamos la granada con fuerza = 20. Con un softLaunch, la lanzamos con fuerza = 2
            fg.GetComponent<Rigidbody>().AddForce(cam.forward * launchForce, ForceMode.Impulse);
            fg.Launch();    // Activate the nade script (see Launch method in FlashGrenade.cs)
            
            // Play sounds
            asrc.clip = flashRadioSound;
            asrc.Play();
        }
        else if (molotov)
        {
            // Creating the nade
            Molotov m = Instantiate(molotovPrefab, cam.position + cam.forward * 1, Quaternion.identity);
            m.gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = true;


            // Grenade needs to have a Rigidbody component, so we can apply forces to it
            Rigidbody rb = m.gameObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;  // Esto se hace para que la granada no atraviese las paredes

            // Apply a force to throw the nade
            rb.AddForce(cam.forward * hardLaunchForce, ForceMode.Impulse);
            rb.angularVelocity = new Vector3(0, 3, 10);
            m.Launch();

            // Play sounds
            asrc.clip = molotovRadioSound;
            asrc.Play();
        }
        else if (smoke) {   // This one is a little bit different. The throw is performed in the method 'ThrowSmoke()' 0.9sec after the key is pressed instead of inside the 'else if' branch. This is made to play the hand animation before the grenade is throwed

            Invoke("ThrowSmoke", 0.9f);
            handsAnim.Play("My_Grenade_Throw");

            asrc.clip = smokeRadioSound;
            asrc.Play();
        }


    }

    void ThrowSmoke() {
        SmokeGrenade s = Instantiate(smokePrefab, cam.position + cam.forward * 1, Quaternion.identity);

        Rigidbody rb = s.gameObject.AddComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;  
        //rb.mass = 1.5f;

        rb.AddForce(cam.forward * hardLaunchForce, ForceMode.Impulse);
        rb.angularVelocity = new Vector3(0, 3, 10);
        //s.Launch();
    }

}


