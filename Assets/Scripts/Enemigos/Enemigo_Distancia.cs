using Paulos.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public class Enemigo_Distancia : MonoBehaviour
{
    enum EstadoEnemigo { Patrullando, PersiguiendoJugador, Regresando }
    public event Action<Vector3> OnDeath;


    [SerializeField] int salud;
    [SerializeField] float maxDistance;
    [SerializeField] float alturaMax;
    [SerializeField] float alturaMin;
    [SerializeField] float velMovimiento;
    [SerializeField] float velRotación;
    [SerializeField] float distanciaCambio;
    [SerializeField] float rangoVision;
    [SerializeField] float anguloVision;
    [SerializeField] Transform spawnBulletPoint;    
    [SerializeField] float distanciaBusquedaPuntos = 5f;
    [SerializeField] LayerMask terreno;

    private bool enfriamiento;
    private bool girando;
    private bool fijarAnimacion;
    private bool set;
    private bool vivo;
    private Vector3 puntoA;
    private Vector3 puntoB;
    private float tiempoSinVerJugador = 0f;
    private float tiempoMaximoSinVerJugador = 2f; // segundos
    private float tiempoMuerto;
    private Transform playerPosition;
    private Vector3 objetivoActual;
    private Vector3 posicionAntesDePerseguir;
    private Vector3 posicionAnterior;
    private Animator animator;
    private EstadoEnemigo estadoActual;
    private bool efInstaKill = false;
    private bool muelto = false;


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
    public void AplicarEfectoPowerUp(TipoPowerUp efecto)
    {
        switch (efecto)
        {
            case TipoPowerUp.Instakill:
                efInstaKill = true;
                break;
            case TipoPowerUp.Nuke:
                salud = 0;
                RecibirDaño();
                break;
            default:
                break;
        }
    }
    public void LimpiarEfectoPowerUp(TipoPowerUp efecto)
    {
        if (efecto == TipoPowerUp.Instakill)
        {
            efInstaKill = false;
        }
    }
    void Start()
    {
        GenerarPuntosdeRuta();
        enfriamiento = false;
        fijarAnimacion = false;
        set = false; 
        objetivoActual = puntoB;
        girando = true;
        vivo = true;
        estadoActual = EstadoEnemigo.Patrullando;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (vivo) 
        {
            Vector3 origenRayo = spawnBulletPoint.position;
            Vector3 direccion = spawnBulletPoint.forward;
            //Este codigo sirve para ver el raycast que se genra para el disparo, descomentar si se va a cambiar la maxDistance u otro aspecto
            Debug.DrawRay(origenRayo, direccion * maxDistance, Color.red);
            //playerPosition = GameObject.FindWithTag("jugador").transform;


            DetectarJugador();
            switch (estadoActual)
            {
                case EstadoEnemigo.Patrullando:
                    //Debug.Log("patrullando");
                    Patrullar();
                    break;
                case EstadoEnemigo.PersiguiendoJugador:
                    //Debug.Log("Persiguiendo");
                    PerseguirJugador();
                    break;
                case EstadoEnemigo.Regresando:
                    //Debug.Log("Reiniciando patrulla");
                    VolverAPatrullar();
                    break;
            }
        }
        else
        {
            tiempoMuerto += Time.deltaTime;
            if(tiempoMuerto == 2)
            {
                DestuirEnemigo();
            }
        }
        ActualizarAnimaciones();



    }

    void GenerarPuntosdeRuta()
    {
        Dictionary<Vector3, float> posiblesPuntos = new Dictionary<Vector3, float>();

        // Direcciones en las que lanzaremos los Raycasts
        Vector3[] direcciones = { transform.right, -transform.right, transform.forward, -transform.forward };

        foreach (Vector3 dir in direcciones)
        {
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, distanciaBusquedaPuntos, terreno))
            {
                posiblesPuntos.Add(hit.point - (dir * 1f), hit.distance);
            }
            else
            {
                posiblesPuntos.Add(transform.position + (dir * distanciaBusquedaPuntos), distanciaBusquedaPuntos);
            }
        }

        // Seleccionar los dos puntos más alejados para patrullar
        List<KeyValuePair<Vector3, float>> ordenados = new List<KeyValuePair<Vector3, float>>(posiblesPuntos);
        ordenados.Sort((a, b) => b.Value.CompareTo(a.Value)); // Ordenar por distancia

        if (ordenados.Count >= 2)
        {
            puntoA = ordenados[0].Key;
            puntoB = ordenados[1].Key;
        }

        Debug.Log($" puntos de ruta: A={puntoA}, B={puntoB}");
    }

    void DetectarJugador()
    {
        playerPosition = BuscarJugadorEnRango();

        if (playerPosition != null)
        {
            tiempoSinVerJugador = 0;

            if (estadoActual != EstadoEnemigo.PersiguiendoJugador)
                posicionAntesDePerseguir = transform.position;

            estadoActual = EstadoEnemigo.PersiguiendoJugador;
            DispararJugador();
        }
        else
        {
            if (estadoActual == EstadoEnemigo.PersiguiendoJugador && tiempoSinVerJugador > tiempoMaximoSinVerJugador)
                estadoActual = EstadoEnemigo.Regresando;
            else
                tiempoSinVerJugador += Time.deltaTime;
        }
    }
    Transform BuscarJugadorEnRango()
    {
        GameObject[] jugadores = GameObject.FindGameObjectsWithTag("jugador");

        foreach (GameObject jugador in jugadores)
        {
            Vector3 direccion = jugador.transform.position - transform.position;
            float distancia = direccion.magnitude;

            if (distancia <= rangoVision)
            {
                float angulo = Vector3.Angle(transform.forward, direccion.normalized);

                if (angulo <= anguloVision)
                {
                    if (Physics.Raycast(transform.position, direccion.normalized, out RaycastHit hit, rangoVision))
                    {
                        if (hit.collider.CompareTag("jugador"))
                        {
                            Debug.Log("Encontre un jugador");
                            return jugador.transform;
                        }
                    }
                }
            }
        }

        return null;
    }

    void Patrullar()
    {
        Vector3 destino = new Vector3(objetivoActual.x, UnityEngine.Random.Range(alturaMin,alturaMax), objetivoActual.z);

        if (girando)
        {
            GirarHacia(destino);
        }
        else
        {
            MoverHacia(destino);

            if (Vector3.Distance(transform.position, destino) < distanciaCambio)
            {
                objetivoActual = (objetivoActual == puntoA) ? puntoB : puntoA;
                girando = true;
            }
        }
    }

    void VolverAPatrullar()
    {
        Vector3 destino = new Vector3(posicionAntesDePerseguir.x, UnityEngine.Random.Range(alturaMin, alturaMax), posicionAntesDePerseguir.z);

        if (Vector3.Distance(transform.position, destino) > distanciaCambio)
        {
            GirarHacia(destino);
            MoverHacia(destino);
        }
        else
        {
            // Volvió al punto de patrulla, retomar rutina
            estadoActual = EstadoEnemigo.Patrullando;
            girando = true;
            objetivoActual = (Vector3.Distance(destino, puntoA) < Vector3.Distance(destino, puntoB)) ? puntoB : puntoA;
        }
    }

    void PerseguirJugador()
    {
        if (playerPosition == null) return;

        Vector3 direccion = playerPosition.position - transform.position;
        direccion.y = 0;
        direccion.Normalize();

        Vector3 destino = playerPosition.position - direccion * (maxDistance / 2);
        destino.y = alturaMax;

        GirarHacia(playerPosition.position);
        MoverHacia(destino);
    }

    void DispararJugador()
    {
        if (playerPosition == null) return;

        Vector3 origen = spawnBulletPoint.position;
        Vector3 direccion = playerPosition.position - origen;
        direccion.y = 0f;
        direccion.Normalize();

        if (direccion != Vector3.zero)
            spawnBulletPoint.rotation = Quaternion.LookRotation(direccion);

        float distancia = Vector3.Distance(origen, playerPosition.position);
        float angulo = Vector3.Angle(spawnBulletPoint.forward, direccion);

        if (distancia <= maxDistance && angulo <= anguloVision / 2f)
        {
           
            if (Physics.Raycast(origen, spawnBulletPoint.forward, out RaycastHit hit, maxDistance))
            {
                Debug.Log("Algo entro en mi rango de vision");
                if (hit.collider.CompareTag("jugador") && !enfriamiento)
                {
                    Debug.Log("Disparar al jugador");
                    enfriamiento = true;
                    Projectile_Manager._Instance.FireProjectileForward("Projectile_Bullet_L", spawnBulletPoint);
                    Invoke(nameof(DesactivarEnfriamiento), 1f);
                }
            }
        }
    }

    void ActualizarAnimaciones()
    {
        Vector3 desplazamiento = transform.position - posicionAnterior;
        Vector3 direccionLocal = transform.InverseTransformDirection(desplazamiento);

        float velocidadZ = direccionLocal.z;
        float velocidadX = direccionLocal.x;

        bool moviendoAdelante = velocidadZ > 0.01f;
        bool moviendoAtras = velocidadZ < -0.01f;
        bool moviendoLateral = Mathf.Abs(velocidadX) > 0.01f;

        if (moviendoAdelante)
        {
            if (!set)
            {
                animator.SetBool("forward", true);
                animator.SetBool("backward", false);
                set = true;
            }
        }
        else if (moviendoAtras)
        {
            if (!set)
            {
                animator.SetBool("forward", false);
                animator.SetBool("backward", true);
                set = true;
            }
        }
        else if (moviendoLateral)
        {
            // Si deseas agregar animaciones laterales, aquí puedes extender con "left" y "right"
            // Por ahora, si solo tienes forward/backward, las ignoramos o tratamos como forward
            if (!set)
            {
                animator.SetBool("forward", true);
                animator.SetBool("backward", false);
                set = true;
            }
        }
        else
        {
            if (set)
            {
                animator.SetBool("forward", false);
                animator.SetBool("backward", false);
                animator.SetBool("set", false);
                set = false;
            }
        }

        posicionAnterior = transform.position;
    }

    void DesactivarEnfriamiento()
    {
        enfriamiento = false;
    }

    void AlternarFijacionAnimacion()
    {
        animator.SetBool("set", true);
    }

    void GirarHacia(Vector3 destino)
    {
        Vector3 direccion = (destino - transform.position).normalized;
        direccion.y = 0f; // Eliminar la influencia vertical en la rotación

        if (direccion == Vector3.zero) return;

        Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
        Quaternion rotacionActual = transform.rotation;

        // Solo actualizar el ángulo en Y
        Quaternion rotacionSoloY = Quaternion.Euler(0f, rotacionDeseada.eulerAngles.y, 0f);

        transform.rotation = Quaternion.Slerp(rotacionActual, rotacionSoloY, velRotación * Time.deltaTime);

        float angulo = Quaternion.Angle(transform.rotation, rotacionSoloY);
        if (estadoActual == EstadoEnemigo.Patrullando && angulo < 5f)
        {
            girando = false;
        }
    }

    void MoverHacia(Vector3 destino)
    {
        transform.position = Vector3.MoveTowards(transform.position, destino, velMovimiento * Time.deltaTime);
    }

    public void RecibirDaño()
    {
        salud = (!efInstaKill) ? salud - 1 : 0;
        salud -= 1;
        Debug.Log("Salud enemigo actual: " + salud);
        if (salud <= 0 && !muelto)
        {
            Debug.Log("Me mori");
            vivo = false;
            tiempoMuerto = 0f;
            animator.SetBool("death", true);
            OnDeath?.Invoke(transform.position);
            muelto = true;
        }
    }

    void DestuirEnemigo()
    {
        Debug.Log("Me desintegro");
        Destroy(gameObject);
    }
    //Esto es exclusivamente para pruebas y puede ser comentado o borrado cuando no se necesite.
    /*
    void OnDrawGizmosSelected()
    {
        if (playerPosition == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoVision);

        Vector3 forward = transform.forward;
        Vector3 leftLimit = Quaternion.Euler(0, -anguloVision / 2, 0) * forward;
        Vector3 rightLimit = Quaternion.Euler(0, anguloVision / 2, 0) * forward;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + leftLimit * rangoVision);
        Gizmos.DrawLine(transform.position, transform.position + rightLimit * rangoVision);
    }
    */
    
}
