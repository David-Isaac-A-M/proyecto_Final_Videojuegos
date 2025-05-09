using UnityEngine;
using Paulos.Projectiles;
using PurrNet;
using System.Collections.Generic;

public class Jugador : MonoBehaviour
{
    public static List<Jugador> TodosLosJugadores = new List<Jugador>();

    [SerializeField] Transform boquilla;
    [SerializeField] Transform mira;
    public int salud;
    [SerializeField] int saludMax;
    public int botiquines;
    public int municiones;
    public bool derribado;
    public bool vivo;
    [SerializeField] hud hud;
    public float tiempoInvulnerabilidad;
    public float tiempoParaReanimar;
    public float rangoInteraccion;

    private float tiempoReanimando;
    private Jugador objetivoReanimacion;
    private bool invulneravilidad = false;

    void Awake()
    {
        TodosLosJugadores.Add(this);
    }

    void OnDestroy()
    {
        TodosLosJugadores.Remove(this);
    }

    void Start()
    {
        municiones = PlayerPrefs.GetInt("municiones", 10);
        botiquines = PlayerPrefs.GetInt("botiquines", 3);
        salud = PlayerPrefs.GetInt("salud", saludMax);

        invulneravilidad = false;
        vivo = true;
        derribado = false;
        tiempoReanimando = 0;

        hud.ActualizarVidaMaxima(saludMax);
        hud.ActualizarMuniciones(municiones);
        hud.ActualizarBotiquines(botiquines);
        hud.ActualizarVida(salud);
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && municiones > 0)
        {
            hud.CambiarMensajeEstado("Se disparo el arma");
            Disparar();
        }

        if (Input.GetButtonDown("Heal") && botiquines > 0)
        {
            botiquines -= 1;
            Curar(saludMax - salud);
            hud.ActualizarBotiquines(botiquines);
        }

        if(Input.GetButtonDown("Interact"))
        {
            hud.CambiarMensajeEstado("El jugador actual es: " + TodosLosJugadores[0].name);
        }

        RevisarJugadoresCercanosYReanimar();
    }

    void RevisarJugadoresCercanosYReanimar()
    {
        /*
        if (!Input.GetButton("Interact"))
        {
            tiempoReanimando = 0;
            objetivoReanimacion = null;
            return;
        }
        */

        if (objetivoReanimacion == null)
        {
            foreach (var j in TodosLosJugadores)
            {
                if (j != this && j.derribado)
                {
                    hud.CambiarMensajeEstado("Encuentro un jugador derribado");
                    float distancia = Vector3.Distance(transform.position, j.transform.position);
                    if (distancia <= rangoInteraccion)
                    {
                        objetivoReanimacion = j;
                        //tiempoReanimando = 0;
                        hud.CambiarMensajeEstado("A rango de reanimación a " + j.name);
                        Debug.Log("A rango de reanimación a " + j.name);
                        break;
                    }
                }
            }
        }


        if (objetivoReanimacion != null)
        {
            float distancia = Vector3.Distance(transform.position, objetivoReanimacion.transform.position);
            if (distancia > rangoInteraccion && objetivoReanimacion.derribado || !Input.GetButton("Interact"))
            {
                hud.CambiarMensajeEstado("Cancelando reanimación");
                Debug.Log("Cancelando reanimación");
                objetivoReanimacion = null;
                tiempoReanimando = 0;
                return;
            }
            
            if(distancia <= rangoInteraccion && objetivoReanimacion.derribado && Input.GetButton("Interact"))
            {
                tiempoReanimando += Time.deltaTime;
                hud.CambiarMensajeEstado("Reanimando... " + tiempoReanimando.ToString() + "s");
                Debug.Log("Reanimando... " + tiempoReanimando.ToString() + "s");
            }

            if (tiempoReanimando >= tiempoParaReanimar)
            {
                objetivoReanimacion.Revivir();
                hud.CambiarMensajeEstado("Jugador reanimado");
                Debug.Log("Jugador reanimado completamente");
                objetivoReanimacion = null;
                tiempoReanimando = 0;
            }

        }
    }

    void Disparar()
    {
        municiones -= 1;
        hud.ActualizarMuniciones(municiones);
        if (Input.GetButton("Aim") && mira)
        {
            Projectile_Manager._Instance.FireProjectileForward("Projectile_Bullet_S", mira);
        }
        else if (boquilla)
        {
            Projectile_Manager._Instance.FireProjectileForward("Projectile_Bullet_S", boquilla);
        }
    }

    void Curar(int curacion)
    {
        salud += curacion;
        hud.ActualizarVida(salud);
    }

    public void RecibirDaño()
    {
        if (invulneravilidad) return;

        invulneravilidad = true;
        salud -= 1;
        hud.ActualizarVida(salud);

        if (salud <= 0)
        {
            hud.CambiarMensajeEstado("Derribado");
            Debug.Log("Jugador " + name + " ha sido derribado");
            derribado = true;
        }

        Invoke("DesactivarInvulnerabilidad", 2);
    }

    public void Revivir()
    {
        derribado = false;
        salud = saludMax;
        hud.ActualizarVida(salud);
        hud.CambiarMensajeEstado("Jugador reanimado");
        Debug.Log("Jugador " + name + " reanimado completamente");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("enemigo") && !invulneravilidad)
        {
            RecibirDaño();
        }

        if (collision.gameObject.CompareTag("botiquin"))
        {
            botiquines += 1;
            hud.ActualizarBotiquines(botiquines);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("municion"))
        {
            municiones += 20;
            hud.ActualizarMuniciones(municiones);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("basura"))
        {
            Destroy(collision.gameObject);
        }
    }

    public void DesactivarInvulnerabilidad()
    {
        invulneravilidad = false;
    }
}
