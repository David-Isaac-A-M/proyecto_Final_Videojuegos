using UnityEngine;
using Paulos.Projectiles;
using System.Collections.Generic;
using System;
using UnityEngine.Audio;
using System.Collections;
using System.Linq;
using UnityEngine.InputSystem;


public class Jugador : MonoBehaviour
{
    public static List<Jugador> TodosLosJugadores = new List<Jugador>();
    public static event Action<TipoPowerUp> powerUpRecogido; //informa al DM el power Up recojido por el jugador
    public static event Action<TipoPowerUp> powerUpTerminado; //informa al DM el power Up recojido por el jugador
    Dictionary<TipoPowerUp, float> powerUpsActivos = new Dictionary<TipoPowerUp, float>();

    [SerializeField] Transform[] boquillas;
    [SerializeField] Transform[] miras;
    [SerializeField] GameObject escudos;
    public int salud;
    [SerializeField] int saludMax;
    public int botiquines;
    public int municiones;

    [SerializeField] Animator animador;

    [SerializeField] private bool derribado;
    public bool Derribado => derribado;
    public bool vivo;
    [SerializeField] hud hud;
    [SerializeField] GameOver pantallaGameOver;
    [SerializeField] BasicBehaviour basic;

    //sonidos
    [SerializeField] AudioSource patas;
    private float tiempoPaso = 0f;
    [SerializeField] float intervaloPasos = 0.33f;
    [SerializeField] float intervaloPasosCorriendo = 0.33f;
    [SerializeField] float intervaloPasosApuntando = .5f;
    [SerializeField] float duracionEfecto = 15f; //tiempo en segundos
    [SerializeField] AudioSource prota;
    [SerializeField] AudioSource arma;
    [SerializeField] AudioClip sonidoDisparo;
    [SerializeField] AudioClip[] sonidosDanio;
    [SerializeField] AudioClip[] sonidosPasos;
    [SerializeField] AudioClip sonidoBotiquin;
    [SerializeField] AudioClip sonidoCuraRapida;
    [SerializeField] AudioClip sonidoCuracion;
    [SerializeField] AudioClip sonidoMunicion;
    [SerializeField] AudioClip sonidoPowerUp;
    [SerializeField] AudioClip[] sonidosAtaques;



    public float tiempoInvulnerabilidad;
    public float tiempoParaReanimar;
    public float rangoInteraccion;

    private float tiempoReanimando;
    private Jugador objetivoReanimacion;
    private bool invulneravilidad = false;
    private bool gameOver = true;
    private bool altMouse = false;

    void Awake()
    {
        TodosLosJugadores.Add(this);
        SecuestrarCursor(altMouse);

        StartCoroutine(GestionarPowerUpsActivos());
    }

    void OnDestroy()
    {
        TodosLosJugadores.Remove(this);
    }

    //Gestion de power ups

    public void AnunciarPowerUp(TipoPowerUp powerUp)
    {
        Debug.Log("recogi " +  powerUp);
        powerUpRecogido?.Invoke(powerUp);
    }
    public void AnunciarPowerUpTerminado(TipoPowerUp powerUp)
    {
        Debug.Log("recogi " +  powerUp);
        powerUpTerminado?.Invoke(powerUp);
    }

    private void OnEnable()
    {
        DM.OnPowerUpActivado += AplicarEfecto;
    }
    private void OnDisable()
    {
        DM.OnPowerUpActivado -= AplicarEfecto;
    }

    private IEnumerator GestionarPowerUpsActivos()
    {       
        Debug.Log(powerUpsActivos.Count);
        while (true)
        {
            Debug.Log(powerUpsActivos.Count);
            Debug.Log(powerUpsActivos.Keys);
            foreach (var key in powerUpsActivos.Keys.ToList())
            {
                float nuevoTiempo = powerUpsActivos[key] - Time.deltaTime;

                if (nuevoTiempo <= 0)
                {
                    LimpiarEfecto(key);
                    Debug.Log("eliminar " + key);
                    powerUpsActivos.Remove(key);
                    
                }
                else
                {
                    powerUpsActivos[key] = nuevoTiempo;
                }
            }


            yield return null;

        }
    }

    private void AplicarEfecto(TipoPowerUp efecto)
    {
        
        switch (efecto) //activar o desactivar efectos de power ups
        {
            case TipoPowerUp.Tri_Shot:
                powerUpsActivos[efecto] = duracionEfecto;
                break;
            case TipoPowerUp.Shieldward:
                escudos.SetActive(true);
                powerUpsActivos[efecto] = duracionEfecto;
                break;
            case TipoPowerUp.Instakill:
                powerUpsActivos[efecto] = duracionEfecto;
                break;
            default:
                break;
        }
    }

    private void LimpiarEfecto(TipoPowerUp efecto)
    {
        switch (efecto) //activar o desactivar efectos de power ups
        {
            case TipoPowerUp.Nada:
                break;
            case TipoPowerUp.Tri_Shot:
                break;
            case TipoPowerUp.Shieldward:
                escudos.SetActive(false);
                break;
            case TipoPowerUp.Instakill:
                AnunciarPowerUpTerminado(efecto);
                break;
            case TipoPowerUp.Nuke:
                break;
            default:
                break;
        }
    }




    //controles
    void Start()
    {
        municiones = PlayerPrefs.GetInt("municiones", 10);
        botiquines = PlayerPrefs.GetInt("botiquines", 3);
        salud = PlayerPrefs.GetInt("salud", saludMax);

        invulneravilidad = false;
        vivo = true;
        CambiarDerribado(false);
        tiempoReanimando = 0;
        basic.CambiarDerribadoBasic(Derribado);

        hud.ActualizarVidaMaxima(saludMax);
        hud.ActualizarMuniciones(municiones);
        hud.ActualizarBotiquines(botiquines);
        hud.ActualizarVida(salud);
        


    }

    void Update()
    {
        gameOver = true;
        foreach (var j in TodosLosJugadores)
        {
            if (j.derribado == false)
            {
                gameOver = false;
                //break;
            }
        }
        if (gameOver)
        {
            pantallaGameOver.ActivarPantallaGameOver();
        }

        if (Input.GetButtonDown("Fire1") && municiones > 0 && !derribado)
        {
            hud.CambiarMensajeEstado("Se disparo el arma");
            if (sonidoDisparo != null) InvocarSonido(sonidoDisparo,"fogonazo");
            Disparar();
        }

        if (Input.GetButtonDown("Heal") && botiquines > 0 && !derribado)
        {
            botiquines -= 1;
            Curar(saludMax - salud);
            hud.ActualizarBotiquines(botiquines);
            if(sonidoCuracion!=null) InvocarSonido(sonidoCuracion,"curacion");
        }

        if(Input.GetButtonDown("Interact"))
        {
            hud.CambiarMensajeEstado("El estado del jugador 1 de derribado es " + TodosLosJugadores[0].Derribado);

        }
        if (Input.GetButtonDown("AltMouse") && !derribado)
        {
            altMouse = !altMouse;
            SecuestrarCursor(altMouse);

        }

        if (animador.GetBool("Grounded")&&animador.GetFloat("Speed")>0.1&& tiempoPaso <= 0)
        {
            ReproducirPasos();
            if (animador.GetFloat("Speed") > 1.5)
            {
                tiempoPaso = intervaloPasosCorriendo;
            }
            else
            {
                tiempoPaso = intervaloPasos;
            }
        }
        if (animador.GetBool("Grounded") && animador.GetFloat("Speed") > 0.1 && animador.GetBool("Aim") && tiempoPaso <= 0)
        {
            ReproducirPasos();
            tiempoPaso = intervaloPasosApuntando;

        }
        tiempoPaso -=Time.deltaTime;

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
                if (j != this && j.Derribado)
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
            }

            if (tiempoReanimando >= tiempoParaReanimar)
            {
                SolicitarRevivir(objetivoReanimacion);
                hud.CambiarMensajeEstado("Jugador reanimado");
                objetivoReanimacion = null;
                tiempoReanimando = 0;
            }

        }
    
    }

    public void SolicitarRevivir(Jugador J)
    {
        RevivirServer(J);
    }

    void Disparar()
    {
        municiones -= 1;
        hud.ActualizarMuniciones(municiones);
        if (Input.GetButton("Aim") && miras[0]!=null)
        {
            if (powerUpsActivos.ContainsKey(TipoPowerUp.Tri_Shot) &&
                miras[1] != null && miras[2] != null)
            {
                Projectile_Manager._Instance.FireProjectileForward("Projectile_Bullet_S", miras[1]);
                Projectile_Manager._Instance.FireProjectileForward("Projectile_Bullet_S", miras[2]);
            }
            Projectile_Manager._Instance.FireProjectileForward("Projectile_Bullet_S", miras[0]);
        }
        else if (boquillas[0])
        {
            if (powerUpsActivos.ContainsKey(TipoPowerUp.Tri_Shot) &&
                boquillas[1] != null && boquillas[2] != null)  
            {
                Projectile_Manager._Instance.FireProjectileForward("Projectile_Bullet_S", boquillas[1]);
                Projectile_Manager._Instance.FireProjectileForward("Projectile_Bullet_S", boquillas[2]);
            }
            Projectile_Manager._Instance.FireProjectileForward("Projectile_Bullet_S", boquillas[0]);
        }
    }

    void Curar(int curacion)
    {
        if (curacion+salud>saludMax)
        {
            salud = saludMax;
        }
        else
        {
            salud += curacion;
        }
        hud.ActualizarVida(salud);
    }

    public void RecibirDaño(int danio=1)
    {
        if (salud > 0)
        {
            Debug.Log("Mando a llamar recibirDaño");
            if (invulneravilidad || powerUpsActivos.ContainsKey(TipoPowerUp.Shieldward)) return;

            invulneravilidad = true;
            salud -= danio;
            hud.ActualizarVida(salud);
            ReproducirDanio();

            if (salud <= 0)
            {
                hud.CambiarMensajeEstado("Derribado");
                Debug.Log("Jugador " + name + " ha sido derribado");
                SecuestrarCursor(true);
                CambiarDerribado(true);
                basic.CambiarDerribadoBasic(true);
            }

            Invoke("DesactivarInvulnerabilidad", 2);
        }
    }
    public void RevivirServer(Jugador J)
    {
        J.CambiarDerribado(false);
        J.basic.CambiarDerribadoBasic(false);
        J.salud = saludMax;
        J.hud.ActualizarVida(salud);
        J.hud.CambiarMensajeEstado("Jugador reanimado");
    }
    public void Revivir(Jugador J)
    {
        J.CambiarDerribado(false);
        J.basic.CambiarDerribadoBasic(false);
        J.salud = saludMax;
        J.hud.ActualizarVida(salud);
        J.hud.CambiarMensajeEstado("Jugador reanimado");
    }

    private void OnTriggerEnter(Collider collision)
    {
        if ((collision.gameObject.CompareTag("enemigo") || collision.gameObject.CompareTag("volador")) && !invulneravilidad) 
        {
            if (sonidosAtaques!=null)
            {
                RepSonidoAleatorio(sonidosAtaques);
            }            
            RecibirDaño();
        }
        if (collision.gameObject.CompareTag("Explosion"))
        {
            Debug.Log("explosioné");
            RecibirDaño(2);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        

        if (collision.gameObject.CompareTag("botiquin"))
        {
            botiquines += 1;
            if (sonidoBotiquin != null) InvocarSonido(sonidoBotiquin,"botiquin_recogido");
            hud.ActualizarBotiquines(botiquines);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("municion"))
        {
            municiones += 20;
            if (sonidoMunicion != null) InvocarSonido(sonidoMunicion,"municion_recogida");
            hud.ActualizarMuniciones(municiones);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("tri_attack"))
        {
            AnunciarPowerUp(TipoPowerUp.Tri_Shot);
            InvocarSonido(sonidoPowerUp, "Invocar_Power_Up");
            hud.ActivarContador(TipoPowerUp.Tri_Shot, duracionEfecto);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Shield"))
        {
            AnunciarPowerUp(TipoPowerUp.Shieldward);
            InvocarSonido(sonidoPowerUp, "Invocar_Power_Up");
            hud.ActivarContador(TipoPowerUp.Shieldward, duracionEfecto);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("InstaKill"))
        {
            AnunciarPowerUp(TipoPowerUp.Instakill);
            InvocarSonido(sonidoPowerUp, "Invocar_Power_Up");
            hud.ActivarContador(TipoPowerUp.Instakill, duracionEfecto);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("Nuke"))
        {
            AnunciarPowerUp(TipoPowerUp.Nuke);
            InvocarSonido(sonidoPowerUp, "Invocar_Power_Up");
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Cura"))
        {
            Curar(2);
            InvocarSonido(sonidoCuraRapida, "Cura_Rapida");
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Recarga"))
        {
            municiones += 10;
            InvocarSonido(sonidoMunicion, "Recarga");
            hud.ActualizarMuniciones(municiones);
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.CompareTag("basura"))
        {
            Destroy(collision.gameObject);
        }
    }

    private void SecuestrarCursor(bool estado=false)
    {
        Cursor.visible = estado;
        if (!estado)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState= CursorLockMode.None;
        }
    }
    void ReproducirPasos()
    {
        int index = UnityEngine.Random.Range(0, sonidosPasos.Length); 
        patas.PlayOneShot(sonidosPasos[index]); 
    }
    void ReproducirDanio()
    {
        int index = UnityEngine.Random.Range(0, sonidosDanio.Length);
        prota.PlayOneShot(sonidosDanio[index]);
    }
    void RepSonidoAleatorio(AudioClip[] sonidos)
    {
        int index = UnityEngine.Random.Range(0, sonidos.Length);
        InvocarSonido(sonidos[index]);
    }

    private void InvocarSonido(AudioClip pistaAudio,string nombre_sonido="sonido_invocado")
    {
        GameObject objetoSonido = new GameObject(nombre_sonido);
        AudioSource audio =objetoSonido.AddComponent<AudioSource>();
        audio.clip = pistaAudio;
        audio.Play();
        Destroy(objetoSonido,pistaAudio.length);
    }


    public void DesactivarInvulnerabilidad()
    {
        invulneravilidad = false;
    }

    public void CambiarDerribado(bool valor)
    {
        derribado = valor;
    }
}
