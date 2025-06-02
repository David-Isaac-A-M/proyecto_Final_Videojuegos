using System;
using System.Collections;
using UnityEngine;

public enum TipoPowerUp //colocarlo fuera de la clase permite que sea accesible por cualquier otro script
{
    Nada,
    Tri_Shot,
    Instakill,
    Shieldward,
    Nuke,
}
public class DM : MonoBehaviour
{
    [SerializeField] private GameObject personaje;
    [SerializeField] private Transform spawnJugador;
    [SerializeField] Transform[] spawnersEnemigos;
    [SerializeField] GameObject[] enemigos;
    [SerializeField] int oleadas;
    [SerializeField] bool oleadasInfinitas;
    [SerializeField] float intervaloSpawn=0f;
    [SerializeField] private GameObject[] dropsComun;
    [SerializeField] private GameObject[] dropsRaro;
    [SerializeField] private GameObject[] dropsUltraRaro;
    [SerializeField] private float probDropRar = .3f;
    [SerializeField] private float probDropUltraRar = .05f;
    [SerializeField] private float probComun = .7f;
    [SerializeField] private string Objetivo;

    private int oleadaAct = 0;
    private int enemigosAct = 0;

    //eventos
    
    public static event Action<TipoPowerUp> OnPowerUpActivado;
    public static event Action<TipoPowerUp> OnPowerUpDesactivado;
    public static event Action<string> OnObjetivoDefinido;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(GenerarOleadas());
        ObjetivoDefinido(Objetivo);
    }
    private void Awake()
    {
        if (personaje != null && spawnJugador != null)
        {
            Instantiate(personaje, spawnJugador);
        }
    }

    private void OnEnable()
    {
        Jugador.powerUpRecogido += ActivarPowerUp;
        Jugador.powerUpTerminado += DesactivarPowerUp;
        Boss.SolicitarAsignacionLoot += AsignarLoot;
        Boss.OnBossMuerto += SpawnearMeta;

    }
    private void OnDisable()
    {
        Jugador.powerUpRecogido -= ActivarPowerUp;
        Jugador.powerUpTerminado -= DesactivarPowerUp;
        Boss.SolicitarAsignacionLoot -= AsignarLoot;
        Boss.OnBossMuerto -= SpawnearMeta;
    }

    public void ObjetivoDefinido(string objetivo)
    {
        Debug.Log("definiendo objetivo : " + objetivo);
        OnObjetivoDefinido?.Invoke(objetivo);
    }

    public void ActivarPowerUp(TipoPowerUp tipo)
    {
        Debug.Log("Activando Power-Up: " + tipo);
        OnPowerUpActivado?.Invoke(tipo); // Disparar el evento
    }

    public void DesactivarPowerUp(TipoPowerUp tipo)
    {
        Debug.Log("Desactivando Power-Up");
        OnPowerUpDesactivado?.Invoke(tipo); // Disparar el evento de desactivación
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator GenerarOleadas()
    {
        while (oleadaAct < oleadas || oleadasInfinitas)
        {
            foreach (Transform spawner in spawnersEnemigos)
            {
                int index = UnityEngine.Random.Range(0, enemigos.Length);
                GameObject enemigoInstanciado = Instantiate(enemigos[index], spawner.position, spawner.rotation);
                enemigosAct += 1;

                // Suscribirse al evento de muerte del enemigo
                if (dropsComun != null && dropsRaro != null && dropsUltraRaro != null)
                {

                    switch (enemigoInstanciado.tag)
                    {
                        case "enemigo":
                            enemigoInstanciado.GetComponent<Enemigo_CQC>().OnDeath += SoltarPowerUp;
                            break;
                        case "volador":
                            enemigoInstanciado.GetComponent<Enemigo_Distancia>().OnDeath += SoltarPowerUp;
                            break;
                        case "Taliban":
                            enemigoInstanciado.GetComponent<Enemigo_Taliban>().OnDeath += SoltarPowerUp;
                            break;
                        default:
                            break;
                    }
                }
                yield return new WaitForSeconds(intervaloSpawn);
            }
            oleadaAct++;            
        }
    }

    private void AsignarLoot(GameObject instancia)
    {
        switch (instancia.tag)
        {
            case "enemigo":
                instancia.GetComponent<Enemigo_CQC>().OnDeath += SoltarPowerUp;
                break;
            case "volador":
                instancia.GetComponent<Enemigo_Distancia>().OnDeath += SoltarPowerUp;
                break;
            case "Taliban":
                instancia.GetComponent<Enemigo_Taliban>().OnDeath += SoltarPowerUp;
                break;
            default:
                break;
        }
    }

    private void SpawnearMeta(Transform posicon)
    {
        Instantiate(dropsUltraRaro[0], posicon.position, Quaternion.identity);
    }


    private void SoltarPowerUp(Vector3 posicionEnemigo)
    {
        enemigosAct -= 1;
        float rareza = UnityEngine.Random.value;

        if (rareza <= probDropUltraRar)
        {
            int index = UnityEngine.Random.Range(0, dropsUltraRaro.Length);
            Instantiate(dropsUltraRaro[index], posicionEnemigo, Quaternion.identity);
        }
        else if (rareza <= probDropRar)
        {
            int index = UnityEngine.Random.Range(0, dropsRaro.Length);
            Instantiate(dropsRaro[index], posicionEnemigo, Quaternion.identity);
        }
        else if (rareza<= probComun) 
        {
            int index = UnityEngine.Random.Range(0, dropsComun.Length);
            Instantiate(dropsComun[index], posicionEnemigo, Quaternion.identity);
        }
    }
}
