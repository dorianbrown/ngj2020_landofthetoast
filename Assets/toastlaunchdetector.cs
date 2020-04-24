using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toastlaunchdetector : MonoBehaviour
{
    
    [SerializeField] LayerMask detectors;
    private bool detected = false;

    // Update is called once per frame
    void Update()
    {
        Collider2D[] hookpoints = Physics2D.OverlapCircleAll(transform.position, 1f, detectors);

        if (!detected && hookpoints.Length > 0)
        {
            GameObject.Find("Orchestrator").GetComponent<Orchestrator>().toasts_launches_detected += 1;
            detected = true;
        }
    }
}
