//using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemigo_CQC : MonoBehaviour
{
    public int rutina;
    public float cronometro;
    public Animator animator;
    public Quaternion angulo;
    public float grado;
    public Enemigo_Rango rango;

    private bool verificacionRage;
    
    public GameObject jugador;
    public bool atacando;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        jugador = GameObject.FindWithTag("jugador");
        verificacionRage = false;
        atacando = false;
    }

    // Update is called once per frame
    void Update()
    {
        Comportamiento_Enemigo();
    }

    public void Comportamiento_Enemigo()
    {
        if(Vector3.Distance(transform.position, jugador.transform.position)>5)
        {
            animator.SetBool("rage", false);
            verificacionRage=false;
            cronometro += 1 * Time.deltaTime;
            if (cronometro > 4)
            {
                rutina = Random.Range(0, 2);
                cronometro = 0;
            }

            switch (rutina)
            {
                case 0:
                    animator.SetBool("walk", false);
                    break;
                case 1:
                    grado = Random.Range(0, 360);
                    angulo = Quaternion.Euler(0, grado, 0);
                    rutina++;
                    break;
                case 2:
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, angulo, 0.5f);
                    transform.Translate(Vector3.forward * 1 * Time.deltaTime);
                    animator.SetBool("walk", true);
                    break;
            }
        }
        else
        {
            var lookpos = jugador.transform.position - transform.position;
            lookpos.y = 0;
            var rotation = Quaternion.LookRotation(lookpos);
            if (verificacionRage==false)
            {
                animator.SetBool("rage", true);
            }
            else
            {
                if(Vector3.Distance(transform.position, jugador.transform.position) > 1 && !atacando)
                {
                    
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 3);
                    animator.SetBool("walk", true);
                    transform.Translate(Vector3.forward * 2 * Time.deltaTime);
                    animator.SetBool("attack", false);
                }
                else
                {
                    transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, 3);
                    animator.SetBool("walk", false );
                }
            }
            
        }
    }

    public void DesactivarRage()
    {
        verificacionRage = true;
        animator.SetBool("rage", false);
    }

    public void DesactivarAttack()
    {
        animator.SetBool("attack", false) ;
        atacando = false;
        rango.GetComponent<CapsuleCollider>().enabled = true;
    }
}
