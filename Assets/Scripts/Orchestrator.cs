using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orchestrator : MonoBehaviour
{
    // Tracking Gamestate here
    public int hooks_attached = 0;
    public bool lights_enabled = false;
    public int toasts_launched = 0;
    public int toasts_launches_detected = 0;

    public string active_phase = "intro";

    [SerializeField] private GameObject intro_text;
    [SerializeField] private GameObject quest1_text;
    [SerializeField] private GameObject quest2_text;
    [SerializeField] private GameObject quest3_text;
    [SerializeField] private GameObject outro_text;
    [SerializeField] private GameObject light_button;

    [SerializeField] private GameObject switch_group;

    void Start()
    {
        intro_text.GetComponent<SpriteRenderer>().enabled = true;
    }

    void Update()
    {
        // Intro
        if (Input.GetMouseButtonDown(0) && active_phase == "intro")
        {
            switch_group.GetComponent<Animator>().Play("SwitchAnimation");
            intro_text.GetComponent<SpriteRenderer>().enabled = false;

            foreach (Rigidbody2D rb2d in GameObject.Find("Toast Group").GetComponentsInChildren<Rigidbody2D>())
            {
                rb2d.WakeUp();
            }
        }

        // Quest1
        if (active_phase == "intro" && GameObject.Find("Toast Group").GetComponentInChildren<SpriteRenderer>().isVisible && switch_group.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Default"))
        {
            active_phase = "quest1";
            quest1_text.GetComponent<SpriteRenderer>().enabled = true;
        }
        if (Input.GetMouseButtonDown(0) && active_phase == "quest1" && quest1_text.GetComponent<SpriteRenderer>().enabled)
        {
            quest1_text.GetComponent<SpriteRenderer>().enabled = false;
        }

        // Quest2
        if (hooks_attached == 2 && active_phase == "quest1") {
            active_phase = "quest2";
            quest2_text.GetComponent<SpriteRenderer>().enabled = true;
        }
        if (Input.GetMouseButtonDown(0) && active_phase == "quest2" && quest2_text.GetComponent<SpriteRenderer>().enabled)
        {
            quest2_text.GetComponent<SpriteRenderer>().enabled = false;
            light_button.SetActive(true);
        }

        // Quest3
        if (lights_enabled && active_phase == "quest2")
        {
            Animator[] light_animators = GameObject.Find("Lights").GetComponentsInChildren<Animator>();
            foreach (Animator anim in light_animators)
            {
                anim.Play("LightOn");
            }
            active_phase = "quest3";
            quest3_text.GetComponent<SpriteRenderer>().enabled = true;
        }
        if (Input.GetMouseButtonDown(0) && active_phase == "quest3" && quest3_text.GetComponent<SpriteRenderer>().enabled)
        {
            quest3_text.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (toasts_launches_detected == 2 && active_phase == "quest3")
        {
            active_phase = "outro";
            outro_text.GetComponent<SpriteRenderer>().enabled = true;
            outro_text.GetComponent<Animator>().Play("Outro");
        }
    }
}
