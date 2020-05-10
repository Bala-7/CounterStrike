using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashGreanade : MonoBehaviour
{
    private AudioSource asrc;
    private Transform cam;
    private Transform cam2;
    private Image flashTexture;
    private float alpha;
    private RawImage secondCamImage;
    
    [Range(0, 180)]
    public int MAX_FLASH_ANGLE;

    [Range(0, 180)]
    public int TOTAL_FLASH_ANGLE;

    [Range(0, 30)]
    public int MAX_FLASH_DISTANCE;

    [Range(0.5f, 3.0f)]
    public float flashDelay;

    public float baseFlashDuration;
    private float reduceFlashSpeed;

    // Start is called before the first frame update
    void Start()
    {
        asrc = GetComponent<AudioSource>();
        
        cam = GameManager.s.GetCamTransform();
        cam2 = GameManager.s.GetFlashCamTransform();
        flashTexture = GameManager.s.GetFlashTexture();
        secondCamImage = GameManager.s.GetFlashCamImage();

        secondCamImage.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        //Debug.Log(angle);

        /*if (Input.GetKeyDown(KeyCode.Q)) {

            Launch();
        }*/       
    }

    // Esta función se llama al lanzar la granada, y simplemente llamará a la función Flash() después de 'flashDelay' segundos
    public void Launch() {
        Invoke("Flash", flashDelay);
    }

    // Función que activa el flash de la granada
    void Flash() {
        asrc.Play();

        // Miramos el ángulo respecto a la granada
        Vector3 grDirection = transform.position - cam.position;
        float angle = Vector3.Angle(grDirection, cam.forward);
        float initialFlash = (angle < TOTAL_FLASH_ANGLE) ? 1.0f : 1.0f - angle / MAX_FLASH_ANGLE;   // Si estás en un ángulo pequeño con la granada, te flashea totalmente
        
        // Miramos la distancia a la granada
        float distance = Vector3.Distance(transform.position, cam.position);
        reduceFlashSpeed = baseFlashDuration *  (distance / MAX_FLASH_DISTANCE);
        
        // Miramos si hay objetos en medio, y comprobamos otras condiciones
        RaycastHit hit;
        bool grenadeHidden = Physics.Raycast(cam.position, grDirection, out hit, distance - 1.0f);
        bool noAngle = (angle > MAX_FLASH_ANGLE);
        bool tooFar = (distance > MAX_FLASH_DISTANCE);

        // Si se dan todas las condiciones, se inicia el flash
        if(!grenadeHidden && !noAngle && !tooFar) { 
            // Se activa la textura blanca que nos flashea
            Color c = flashTexture.color;
            c.a = initialFlash;
            flashTexture.color = c;

            // Se sitúa la segunda cámara en la posición actual de la cámara principal
            cam2.transform.position = cam.transform.position;
            cam2.transform.rotation = cam.transform.rotation;

            // Se activa la textura que renderiza la imagen de la segunda cámara
            secondCamImage.gameObject.SetActive(true);
            Color c2 = secondCamImage.color;
            c2.a = initialFlash;
            secondCamImage.color = c2;

            // Se inician las llamadas a la función que reduce el flash progresivamente
            InvokeRepeating("ReduceFlash", initialFlash, 0.1f);
        } 

        
    }


    // Función que reduce progresivamente el flash provocado por la granada
    void ReduceFlash()
    {
        Color c = flashTexture.color;
        c.a = c.a - reduceFlashSpeed;
        flashTexture.color = c;

        Color c2 = secondCamImage.color;
        c2.a = c2.a - reduceFlashSpeed;
        secondCamImage.color = c2;

        if (c.a <= 0.0f && c2.a <= 0.0f) {
            secondCamImage.gameObject.SetActive(true);
            CancelInvoke();
            Destroy(this.gameObject);
        }
    }
}
