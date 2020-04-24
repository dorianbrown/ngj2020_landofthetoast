using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followMouse : MonoBehaviour
{

    private Vector3 target;
    public GameObject item;
    private SpringJoint2D sj2d;
    [SerializeField] private Transform raycastStart;
    public float ropeDistance = 2.2f;

    private bool rope_connected = false;
    private GameObject lineObject;
    private LineRenderer line;
    private GameObject obj1;
    private GameObject obj2;
    [SerializeField] private Material lineMaterial;

    [SerializeField] private LayerMask m_anchorPoints;
    [SerializeField] private LayerMask m_hookPoints;
    [SerializeField] public GameObject tooltip;

    [SerializeField] private LayerMask m_light_button;
    [SerializeField] private GameObject orchestrator;

    void Start ()
    {
        sj2d = GetComponent<SpringJoint2D>();
    }

    void Update()
    {
        Vector3 vector = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f);
        target = Camera.main.ScreenToWorldPoint(vector);
        Vector2 difference = target - item.transform.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        item.transform.rotation = Quaternion.Euler(0f, 0f, rotationZ);

        ShootHook();

        // If close to hook and line is there, show tooltip for pressing E
        Collider2D[] hookpoints = Physics2D.OverlapCircleAll(transform.position, 1f, m_hookPoints);
        Collider2D[] lightbuttons = Physics2D.OverlapCircleAll(transform.position, 1f, m_light_button);

        if ((hookpoints.Length > 0 && rope_connected) | 
            (lightbuttons.Length > 0 && !GameObject.Find("Orchestrator").GetComponent<Orchestrator>().lights_enabled) |
            (hookpoints.Length > 0 && orchestrator.GetComponent<Orchestrator>().lights_enabled))
        {
            tooltip.GetComponent<SpriteRenderer>().enabled = true;
        } else
        {
            tooltip.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.E) && lightbuttons.Length > 0)
        {
            GameObject.Find("Orchestrator").GetComponent<Orchestrator>().lights_enabled = true;
            lightbuttons[0].gameObject.GetComponent<AudioSource>().Play(0);
        }
    }

    void ShootHook ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!rope_connected){
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = mousePosition - raycastStart.transform.position;
                RaycastHit2D hit = Physics2D.Raycast(raycastStart.transform.position, direction, ropeDistance);

                if (hit.collider.gameObject.name == "AnchorPoint")
                {
                    sj2d.enabled = true;
                    Debug.Log(hit.collider.gameObject.GetComponentInParent<Rigidbody2D>());
                    sj2d.connectedBody = hit.collider.gameObject.GetComponentInParent<Rigidbody2D>();

                    rope_connected = true;

                    lineObject = new GameObject();
                    line = lineObject.AddComponent<LineRenderer>();
                    line.material = lineMaterial;
                    line.widthMultiplier = 0.04f;
                    line.startColor = Color.gray;
                    line.endColor = Color.gray;

                    obj1 = hit.collider.gameObject;
                    obj2 = item;

                    line.SetPosition(0, hit.collider.gameObject.transform.position);
                    line.SetPosition(1, item.transform.position);
                }
            } else 
            {
                rope_connected = false;
                sj2d.enabled = false;
                Destroy(lineObject);
            }
        }

        if (line != null)
        {
            line.SetPosition(0, obj1.transform.position);
            line.SetPosition(1, obj2.transform.position);
        }

        Collider2D[] hookpoints = Physics2D.OverlapCircleAll(transform.position, 1f, m_hookPoints);

        // If E is down, line is connected, and close to hook
        if (Input.GetKeyDown(KeyCode.E) && rope_connected && hookpoints.Length > 0){
            // Cleanup old rope
            Rigidbody2D newRb2D = sj2d.connectedBody;
            rope_connected = false;
            sj2d.enabled = false;
            Destroy(lineObject);
            // Create new rope between anchorpoint and new hookpoint
            SpringJoint2D newSj = hookpoints[0].gameObject.AddComponent<SpringJoint2D>();
            newSj.connectedBody = newRb2D;
            newSj.frequency = 10f;
            newSj.autoConfigureDistance = false;
            newSj.distance = 5f;
        }

        if (Input.GetKeyDown(KeyCode.E) && orchestrator.GetComponent<Orchestrator>().active_phase == "quest3" && hookpoints.Length > 0)
        {
            GameObject hookpoint = hookpoints[0].gameObject;
            hookpoint.GetComponent<SpringJoint2D>().enabled = false;
            hookpoint.GetComponent<LineRenderer>().enabled = false;
            GameObject.Find("Launch").GetComponent<AudioSource>().Play(0);
        }
    }
}
