using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molotov : MonoBehaviour
{
    public GameObject firePrefab;

    [Range(1, 10)]
    public int expansionWidth;

    [Range(1, 10)]
    public int expansionHeight;

    [Range(0.5f, 3.5f)]
    public float gridCellDistance;

    [Range(5, 20)]
    public int molotovDurationSeconds;

    private MeshRenderer _mr;
    
    private Rigidbody _rb;
    private List<GameObject> fires;

    private List<Vector3> expansionLevel1;
    private List<Vector3> expansionLevel2;
    private List<Vector3> expansionLevel3;

    public bool drawDebugLines;
    private bool drawingLines = false;

    private AudioSource asrc;

    // Start is called before the first frame update
    void Start()
    {
        asrc = GetComponent<AudioSource>();
        _rb = GetComponent<Rigidbody>();
        _mr = transform.GetChild(0).GetComponent<MeshRenderer>();
        fires = new List<GameObject>();
        expansionLevel1 = new List<Vector3>();
        expansionLevel2 = new List<Vector3>();
        expansionLevel3 = new List<Vector3>();
    }

    // Update is called once per frame
    void Update(){}


    public void Launch()
    {
        //Invoke("Explode", 1.0f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground") {
            _rb.velocity = Vector3.zero;
            Invoke("Explode", 0.3f);
        }
    }


    void Explode() {
        float rayLayerY = transform.position.y + 1; // Altura a la que situamos la capa que va a trazar los rayos
        Vector3 rayLayerCenter = new Vector3(transform.position.x, rayLayerY, transform.position.z);
        drawingLines = true;

        for (int i = -expansionWidth; i <= expansionWidth; i++) {
            for (int j = -expansionHeight; j <= expansionHeight; j++)
            {
                Vector3 pointPosition = new Vector3(rayLayerCenter.x + i * gridCellDistance, rayLayerY, rayLayerCenter.z + j * gridCellDistance);
                RaycastHit hit;
                bool floorHit = Physics.Raycast(pointPosition, Vector3.down, out hit, 3.5f);
                floorHit = ((floorHit && hit.transform.tag == "Ground") || (i == 0 && j == 0));
                
                
                if (floorHit)
                {
                    int d = Mathf.Abs(i) + Mathf.Abs(j);
                    if (d < 2) {
                        GameObject go = Instantiate(firePrefab, hit.point, Quaternion.identity);
                        fires.Add(go);
                        expansionLevel1.Add(hit.point);
                    }
                    if (d >= 2 && d < 4) expansionLevel2.Add(hit.point);
                    else if (d >= 4) expansionLevel3.Add(hit.point);

                    
                }
            }
        }

        Invoke("ExpandLevel2", 2.0f);
        Invoke("CleanFire", molotovDurationSeconds);
        _mr.gameObject.SetActive(false);
        asrc.Play();
    }

    void ExpandLevel2() {
        foreach (Vector3 p in expansionLevel2) {
            GameObject go = Instantiate(firePrefab, p, Quaternion.identity);
            fires.Add(go);
        }

        Invoke("ExpandLevel3", 2.0f);
    }


    void ExpandLevel3()
    {
        foreach (Vector3 p in expansionLevel3)
        {
            GameObject go = Instantiate(firePrefab, p, Quaternion.identity);
            fires.Add(go);
        }


    }

    void CleanFire() {
        foreach (GameObject go in fires) {
            Destroy(go);
        }
        Destroy(this.gameObject);
        drawingLines = false;
    }


    private void OnDrawGizmos()
    {
        if (drawingLines && drawDebugLines) {
            foreach (Vector3 p in expansionLevel1)
            {
                Debug.DrawRay(p + Vector3.up, Vector3.down * 1.5f, Color.red);
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(p, 0.5f);
            }
            foreach (Vector3 p in expansionLevel2)
            {
                Debug.DrawRay(p + Vector3.up, Vector3.down * 1.5f, Color.blue);
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(p, 0.5f);
            }
            foreach (Vector3 p in expansionLevel3)
            {
                Debug.DrawRay(p + Vector3.up, Vector3.down * 1.5f, Color.green);
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(p, 0.5f);
            }

        }
    }
}
