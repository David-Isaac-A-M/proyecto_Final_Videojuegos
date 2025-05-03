using Paulos.Projectiles;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;

public class Enemigo_Distancia : MonoBehaviour
{
    enum EstadoEnemigo { Patrullando, PersiguiendoJugador, Regresando }

    
    [SerializeField] int salud;
    [SerializeField] float maxDistance;
    [SerializeField] float altura;
    [SerializeField] float velMovimiento;
    [SerializeField] float velRotación;
    [SerializeField] float distanciaCambio;
    [SerializeField] float rangoVision;
    [SerializeField] float anguloVision;
    [SerializeField] Transform spawnBulletPoint;
    [SerializeField] Transform puntoA;
    [SerializeField] Transform puntoB;
    [SerializeField] Transform playerPosition;

    private bool enfriamiento;
    private bool girando;
    private bool fijarAnimacion;
    private bool set;
    private bool vivo;
    private float tiempoSinVerJugador = 0f;
    private float tiempoMaximoSinVerJugador = 2f; // segundos
    private Transform objetivoActual;
    private Vector3 posicionAntesDePerseguir;
    private Vector3 posicionAnterior;
    private Animator animator;
    private EstadoEnemigo estadoActual;

    void Start()
    {
        enfriamiento = false;
        fijarAnimacion = false;
        set = false; 
        objetivoActual = puntoB;
        girando = true;
        vivo = true;
        estadoActual = EstadoEnemigo.Patrullando;
        animator = GetComponent<Animator>();
        posicionAnterior = transform.position;
    }

    void Update()
    {
        if (vivo) 
        {
            Vector3 origenRayo = spawnBulletPoint.position;
            Vector3 direccion = spawnBulletPoint.forward;
            //Este codigo sirve para ver el raycast que se genra para el disparo, descomente si va a cambiar la maxDistance u otro aspecto
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
        ActualizarAnimaciones();



    }

    void DetectarJugador()
    {
        Vector3 direccionJugador = playerPosition.position - transform.position;
        float distancia = direccionJugador.magnitude;
        bool jugadorDetectado = false;


        if (distancia <= rangoVision)
        {
            //Debug.Log("Te veo, creo");
            float angulo = Vector3.Angle(transform.forward, direccionJugador.normalized);
            if (angulo <= anguloVision)
            {
                // Comprobar si hay obstáculo con raycast
                //Debug.Log("Si te veo");
                Ray ray = new Ray(transform.position, direccionJugador.normalized);
                if (Physics.Raycast(ray, out RaycastHit hit, rangoVision))
                {
                    //Debug.Log("Algo choco en mi cono de vision");
                    if (hit.collider.CompareTag("jugador") || hit.collider.CompareTag("disparo_enemigo"))
                    {
                        jugadorDetectado = true;
                        tiempoSinVerJugador = 0;
                        string etiqueta = hit.collider.tag;
                        //Debug.Log("el collider del Raycast es: " + etiqueta);
                        if (estadoActual != EstadoEnemigo.PersiguiendoJugador)
                        {
                            posicionAntesDePerseguir = transform.position;
                        }

                        estadoActual = EstadoEnemigo.PersiguiendoJugador;
                        DispararJugador();
                    }
                }
            }
        }

        if (!jugadorDetectado) 
        {
            if(estadoActual == EstadoEnemigo.PersiguiendoJugador && (tiempoSinVerJugador > tiempoMaximoSinVerJugador))
            {
                //Debug.Log("Ya se fue el jugador");
                estadoActual = EstadoEnemigo.Regresando;
            }
            else
            {
                tiempoSinVerJugador += 1 * Time.deltaTime;
            }
        }
    }

    

    void Patrullar()
    {
        Vector3 destino = new Vector3(objetivoActual.position.x, altura, objetivoActual.position.z);

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
        Vector3 destino = new Vector3(posicionAntesDePerseguir.x, altura, posicionAntesDePerseguir.z);

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
            objetivoActual = (Vector3.Distance(destino, puntoA.position) < Vector3.Distance(destino, puntoB.position)) ? puntoB : puntoA;
        }
    }

    void PerseguirJugador()
    {
        
        Vector3 direccionAlJugador = playerPosition.position - transform.position;

        // Ignorar la diferencia en Y
        direccionAlJugador.y = altura;
        direccionAlJugador = direccionAlJugador.normalized;

        // Offset hacia atrás (mantener distancia)
        float distanciaDeseada = maxDistance / 2;
        Vector3 posicionObjetivo = playerPosition.position - direccionAlJugador * distanciaDeseada;
        posicionObjetivo.y = altura; // asegurar altura constante
        
        GirarHacia(playerPosition.position); // mirar al jugador con Y ignorado en rotación también si necesario
        MoverHacia(posicionObjetivo);
        

        /*
        Vector3 destino = new Vector3(playerPosition.position.x, altura, playerPosition.position.z);
        GirarHacia(destino);
        MoverHacia(destino);
        */
    }

    void DispararJugador()
    {
        Vector3 origenRayo = spawnBulletPoint.position;

        // Dirección horizontal hacia el jugador (ignorando la diferencia en Y)
        Vector3 direccionJugador = playerPosition.position - origenRayo;
        direccionJugador.y = 0f; // Eliminar inclinación vertical
        direccionJugador = direccionJugador.normalized;

        // Hacer que el spawnBulletPoint mire al jugador horizontalmente
        if (direccionJugador != Vector3.zero)
        {
            spawnBulletPoint.rotation = Quaternion.LookRotation(direccionJugador);
        }

        // Verificar si el jugador está dentro del rango y ángulo de visión
        float distanciaAlJugador = Vector3.Distance(origenRayo, playerPosition.position);
        float angulo = Vector3.Angle(spawnBulletPoint.forward, direccionJugador);

        if (distanciaAlJugador <= maxDistance && angulo <= anguloVision / 2f)
        {
            // Realizar raycast en la dirección corregida
            if (Physics.Raycast(origenRayo, spawnBulletPoint.forward, out RaycastHit hit, maxDistance))
            {
                if (hit.collider.CompareTag("jugador") && !enfriamiento)
                {
                    //Debug.Log("Debo disparar");
                    enfriamiento = true;
                    Projectile_Manager._Instance.FireProjectileForward("Projectile_Bullet_L", spawnBulletPoint);
                    Invoke("DesactivarEnfriamiento", 1f);
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
        
        salud -= 1;
        Debug.Log("Salud enemigo actual: " + salud);
        if (salud <= 0)
        {
            vivo = false;
            animator.SetBool("death", true);
            
        }
    }

    void DestuirEnemigo()
    {
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
