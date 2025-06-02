using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.Rendering.DebugUI;

public class Enemigo_Taliban : MonoBehaviour
{
    enum EstadoEnemigo { Patrullando, kaboom ,PersiguiendoJugador, Regresando }
    public event Action<Vector3> OnDeath;


    [SerializeField] int salud;
    [SerializeField] private NavMeshAgent agente;
    [SerializeField] float distanciaExplosion = 2f;
    [SerializeField] float tiempoHastaExplosión = 3f;
    [SerializeField] AudioClip[] sonidoDanio;
    [SerializeField] AudioClip sonidoMuerte;
    [SerializeField] GameObject explosion;

    

    private GameObject jugador;
    private bool muelto;
    private bool alarma;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agente = GetComponent<NavMeshAgent>();

    }

    // Update is called once per frame
    void Update()
    {
        BuscarJugador();
        agente.SetDestination(new Vector3(jugador.transform.position.x, 1.5f, jugador.transform.position.z));
        if (Vector3.Distance(transform.position, jugador.transform.position) < distanciaExplosion)
        {
            StartCoroutine(DetonarBomba());
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

    IEnumerator DetonarBomba()
    {
        if (sonidoDanio != null && !alarma)
        {
            InvocarSonido(sonidoDanio[0]);
            alarma = true;
        }
        yield return new WaitForSeconds(tiempoHastaExplosión);

        Debug.Log("¡BOOM! Enemigo explotó.");
        RecibirDano();
    }

    public void RecibirDano()
    {
        salud -= 1;
        if (sonidoDanio != null && salud > 0) RepSonidoAleatorio(sonidoDanio);
        if (salud <= 0 && !muelto)
        {
            Debug.Log("Me mori");
            //if (sonidoDanio != null) InvocarSonido(sonidoMuerte);
            if (explosion != null) InvocarExplosion();
            OnDeath?.Invoke(transform.position);
            muelto = true;
            DestuirEnemigo();
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

    private void InvocarExplosion()
    {
        GameObject explotion = Instantiate (explosion,transform.position,transform.rotation);
        explotion.transform.parent = null;

        InvocarSonido(sonidoMuerte);
        Destroy(explotion,3f);
    }

    void RepSonidoAleatorio(AudioClip[] sonidos)
    {
        int index = UnityEngine.Random.Range(0, sonidos.Length);
        InvocarSonido(sonidos[index]);
    }
    void DestuirEnemigo()
    {
        Debug.Log("Me desintegro");
        Destroy(gameObject);
    }

}
