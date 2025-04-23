using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo_Rango : MonoBehaviour
{
    public Animator animator;
    public Enemigo_CQC enemigo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("jugador"))
        {
            animator.SetBool("walk", false);
            animator.SetBool("attack", true);
            enemigo.atacando = true;
            enemigo.agente.enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
