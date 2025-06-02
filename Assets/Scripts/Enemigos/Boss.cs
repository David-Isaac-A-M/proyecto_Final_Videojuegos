
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Boss : MonoBehaviour
{
    public static event Action<GameObject> SolicitarAsignacionLoot;
    public static event Action<Transform> OnBossMuerto;


    [SerializeField] Animator animator;
    [SerializeField] NavMeshAgent agente;
    [SerializeField] int SaludMax;
    [SerializeField] AudioClip[] sonidoDanio;
    [SerializeField] AudioClip[] sonidoMuerte;
    [SerializeField] GameObject[] enemigos;
    [SerializeField] GameObject explosion;
    [SerializeField] float intervaloInvocacion = 5f;
    [SerializeField] float enfriamientoataqueCQC = 5f;
    [SerializeField] float distanciaInvocacion = 15f;
    [SerializeField] int enemigosInvocados = 3;
    [SerializeField] float desvioInvocacion = 3f;
    [SerializeField] float desvioExplosion = 5f;
    [SerializeField] float distanciaPeligro = 10;
    [SerializeField] float distanciaSegura = 12f;
    [SerializeField] GameObject loot;

    private Coroutine invocacion;
    private bool peligro = false;

    private int saludAct;
    private GameObject jugador;
    private float tiempoPaso;
    private float tiempoPasoCQC;
    private Rigidbody rb;
    private Vector3 ultimaVel;
    public float acceleracion;
    private bool muelto;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ultimaVel = rb.linearVelocity;
        animator = GetComponent<Animator>();
        agente = GetComponent<NavMeshAgent>();
        saludAct = SaludMax;
        tiempoPaso = enfriamientoataqueCQC;
        invocacion = StartCoroutine(InvocarEnemigos());
    }

    void AsignacionLoot(GameObject instancia)
    {
        Debug.Log("definiendo objetivo : " + instancia);
        SolicitarAsignacionLoot?.Invoke(instancia);
    }
    void AnunciarMuerte(Transform posicion)
    {
        Debug.Log("Muriendo en : " + posicion);
        OnBossMuerto?.Invoke(posicion);
    }

    private IEnumerator InvocarEnemigos()
    {
        yield return new WaitForSeconds(intervaloInvocacion);

        while (true)
        {
            animator.SetBool("ataque", true);
            SpawnearEnemigos();
            animator.SetBool("ataque", false);
            yield return new WaitForSeconds(intervaloInvocacion);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!muelto)
        {
            BuscarJugador();
            acceleracion = (rb.linearVelocity - ultimaVel).magnitude / Time.deltaTime;
            ultimaVel = rb.linearVelocity;
            animator.SetFloat("speed", acceleracion);
            agente.SetDestination(jugador.transform.position);
            if (Vector3.Distance(transform.position, jugador.transform.position) >= distanciaInvocacion)
            {
                if (tiempoPaso <= 0)
                {
                    animator.SetBool("ataque", true);
                    SpawnearEnemigos();
                    animator.SetBool("ataque", true);
                    tiempoPaso = intervaloInvocacion;

                }
                tiempoPaso -= Time.deltaTime;
                tiempoPasoCQC = 0;
            }
            else
            {
                if (tiempoPasoCQC <= 0)
                {

                    animator.SetBool("ataque", true);
                    Egspoooosion();
                    animator.SetBool("ataque", false);
                    tiempoPasoCQC = enfriamientoataqueCQC;

                }
                tiempoPasoCQC -= Time.deltaTime;
                tiempoPaso = 0;
            }

        }
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

    public void RecibirDaño()
    {
        if (saludAct>0)
        {
            Debug.Log("le dieron al Boss");
            saludAct -= 1;
            if (sonidoDanio != null && saludAct > 0) InvocarSonido(sonidoDanio);
            if (saludAct<=0)
            {
                muelto = true;
                Debug.Log("el Boss se murio");
                //morir
                if (sonidoDanio != null) InvocarSonido(sonidoMuerte);
                animator.SetBool("muelto", true);
                AnunciarMuerte(transform);
                Destroy(gameObject,3);

            }
        }
    }

    void SpawnearEnemigos()
    { 
        for (int i = 0; i < enemigosInvocados; i++)
        {
            Vector3 desviacion = UnityEngine.Random.insideUnitSphere * desvioInvocacion;
            desviacion.y = .5f;
            Vector3 spawn = jugador.transform.position + desviacion;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(spawn,out hit,desvioInvocacion,NavMesh.AllAreas))
            {
                spawn = hit.position;
            }
            int index = UnityEngine.Random.Range(0, enemigos.Length);
            GameObject enemigoInstanciado = Instantiate(enemigos[index], spawn, Quaternion.identity);
            AsignacionLoot(enemigoInstanciado);

        }
    }
    void Egspoooosion()
    {
        Vector3 desviacion = UnityEngine.Random.insideUnitSphere * desvioExplosion;
        desviacion.y = .5f;
        Vector3 spawn = jugador.transform.position + desviacion;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawn, out hit, desvioInvocacion, NavMesh.AllAreas))
        {
            spawn = hit.position;
        }        
        GameObject enemigoInstanciado = Instantiate(explosion, spawn, Quaternion.identity);
        AsignacionLoot(enemigoInstanciado);
    }

    private void InvocarSonido(AudioClip[] pistaAudio)
    {
        GameObject objetoSonido = new GameObject("Sonido_Invocado");
        AudioSource audio = objetoSonido.AddComponent<AudioSource>();
        int index = UnityEngine.Random.Range(0, pistaAudio.Length);
        audio.clip = pistaAudio[index];
        audio.Play();
        Destroy(objetoSonido, pistaAudio[index].length);
    }
}
