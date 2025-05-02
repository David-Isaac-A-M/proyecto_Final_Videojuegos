//using Unity.Mathematics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemigo_CQC : MonoBehaviour
{
    public int rutina;
    public float cronometro;
    public Animator animator;
    public Quaternion angulo;
    public float grado;
    public Enemigo_Rango rango;

    private bool verificacionRage;

    public NavMeshAgent agente;
    [SerializeField] int salud;
    [SerializeField] int rangoVision;
    
    
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
        if(Vector3.Distance(transform.position, jugador.transform.position)> rangoVision) //Modo de patrulla del enemigo (movimiento aleatorio por el escenario)
        {
            agente.enabled = false;
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
                if(agente.enabled)
                    agente.SetDestination(jugador.transform.position);

                if (Vector3.Distance(transform.position, jugador.transform.position) > 1 && !atacando)
                {
                    
                    animator.SetBool("walk", true);
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
        agente.enabled = true;
    }

    public void DesactivarAttack()
    {
        if (Vector3.Distance(transform.position, jugador.transform.position) > 5 + 0.2f)
        {
            animator.SetBool("attack", false);
        }
        animator.SetBool("attack", false) ;
        atacando = false;
        agente.enabled = true;
        rango.GetComponent<CapsuleCollider>().enabled = true;
    }

    public void RecibirDano()
    {
        salud -= 1;
        if (salud <= 0) 
        {
            Debug.Log("Me mori");
            animator.SetBool("death", true );
        }
    }

    public void DestruirEnemigo()
    {
        Destroy(gameObject);
    }
}
