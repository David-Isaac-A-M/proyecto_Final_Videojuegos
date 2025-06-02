//using Unity.Mathematics;
using System;
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
    public event Action<Vector3> OnDeath;

    private bool verificacionRage;

    public NavMeshAgent agente;
    [SerializeField] int salud;
    [SerializeField] int rangoVision;
    [SerializeField] AudioSource bocinaEnemigo;
    [SerializeField] AudioClip[] sonidoDanio;
    [SerializeField] AudioClip sonidoMuerte;
    private bool sonidoRep = false;


    private GameObject jugador;
    public bool atacando;
    private bool muelto=false;
    private bool efInstaKill = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        DM.OnPowerUpActivado += AplicarEfectoPowerUp;
        DM.OnPowerUpDesactivado += LimpiarEfectoPowerUp;
    }
    private void OnDisable()
    {
        DM.OnPowerUpActivado -= AplicarEfectoPowerUp;
        DM.OnPowerUpDesactivado -= LimpiarEfectoPowerUp;
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        verificacionRage = false;
        atacando = false;
    }
    public void AplicarEfectoPowerUp(TipoPowerUp efecto)
    {
        switch (efecto)
        {
            case TipoPowerUp.Instakill:
                efInstaKill = true;
                break;
            case TipoPowerUp.Nuke:
                salud = 0;
                RecibirDano();
                break;
            default:
                break;
        }
    }
    public void LimpiarEfectoPowerUp(TipoPowerUp efecto)
    {
        if (efecto==TipoPowerUp.Instakill)
        {
            efInstaKill = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        BuscarJugador();
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
                rutina = UnityEngine.Random.Range(0, 2);
                cronometro = 0;
            }

            switch (rutina)
            {
                case 0:
                    animator.SetBool("walk", false);
                    break;
                case 1:
                    grado = UnityEngine.Random.Range(0, 360);
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
            Debug.Log("ataque desactivado" + UnityEngine.Random.value);
        animator.SetBool("attack", false);
        atacando = false;
        agente.enabled = true;
        rango.GetComponent<CapsuleCollider>().enabled = true;
    }

    public void RecibirDano()
    {
        salud = (!efInstaKill) ? salud - 1 : 0;
        if(sonidoDanio!=null && salud>0) RepSonidoAleatorio(sonidoDanio);
        if (salud <= 0&&!muelto) 
        {
            Debug.Log("Me mori");
            if (sonidoDanio != null) InvocarSonido(sonidoMuerte);
            animator.SetBool("death", true );
            OnDeath?.Invoke(transform.position);
            muelto = true;
        }
    }
    private void InvocarSonido(AudioClip pistaAudio)
    {
        GameObject objetoSonido = new GameObject("Sonido_Invocado");
        AudioSource audio = objetoSonido.AddComponent<AudioSource>();
        audio.clip = pistaAudio;
        audio.Play();
        Destroy(objetoSonido, pistaAudio.length);
    }

    void RepSonidoAleatorio(AudioClip[] sonidos)
    {
        int index = UnityEngine.Random.Range(0, sonidos.Length);
        InvocarSonido(sonidos[index]);
    }


    void BuscarJugador()
    {
        // Solo buscar jugadores si somos el dueño del enemigo (MasterClient)
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag("jugador");
        float menorDistancia = float.MaxValue;

        foreach (var j in jugadores)
        {
            float distancia = Vector3.Distance(transform.position, j.transform.position);
            if (distancia < menorDistancia)
            {
                menorDistancia = distancia;
                jugador = j;
            }
        }
    }

    public void DestruirEnemigo()
    {
        OnDeath = null;

        Destroy(gameObject);
    }
}
