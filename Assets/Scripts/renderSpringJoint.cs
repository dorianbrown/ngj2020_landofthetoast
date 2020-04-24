using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class renderSpringJoint : MonoBehaviour
{

    private bool has_sj = false;

    private SpringJoint2D sj;
    private LineRenderer line;
    [SerializeField] private Material lineMaterial;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!has_sj)
        {
            sj = GetComponent<SpringJoint2D>();
            if (sj != null)
            {
                // Add 1 to global state for this
                GetComponent<AudioSource>().Play(0);
                GameObject.Find("Orchestrator").GetComponent<Orchestrator>().hooks_attached += 1;
                has_sj = true;
                line.enabled = true;
                line.material = lineMaterial;
                line.widthMultiplier = 0.04f;
                line.startColor = Color.gray;
                line.endColor = Color.gray;
            }
        }

        if (has_sj)
        {
            line.SetPosition(0, transform.position);
            line.SetPosition(1, sj.connectedBody.gameObject.transform.GetChild(5).position);
        }
    }
}
