using UnityEngine;
using Paulos.Projectiles;


public class Jugador : MonoBehaviour
{
    [SerializeField] Transform boquilla;
    [SerializeField] Transform mira;
    [SerializeField] int salud;
    [SerializeField] int saludMax;
    [SerializeField] int botiquines;
    [SerializeField] int municiones;
    [SerializeField] hud hud;
    public float tiempoInvulnerabilidad = 1;

    private bool invulneravilidad = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        invulneravilidad = false;
        hud.ActualizarVidaMaxima(salud);
        hud.ActualizarMuniciones(municiones);
        hud.ActualizarBotiquines(botiquines);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")&municiones>0)
        {
            Disparar();
        }
        if(Input.GetButtonDown("Heal") && botiquines>0)
        {
            Debug.Log("curando");
            botiquines -= 1;
            Curar(saludMax-salud);
            hud.ActualizarBotiquines(botiquines);
        }
    }

    void Disparar()
    {
        municiones -= 1;
        hud.ActualizarMuniciones(municiones);
        if (Input.GetButton("Aim"))
        {
            if (mira)
            {
                Projectile_Manager._Instance.FireProjectileForward("Projectile_Bullet_S", mira);
            }
        }
        else
        if (boquilla)
        {
            Projectile_Manager._Instance.FireProjectileForward("Projectile_Bullet_S", boquilla);
        }
    }

    void Curar(int curacion)
    {
        salud += curacion;
        hud.ActualizarVida(salud);
    }

    private void OnTriggerEnter(Collider colision)
    {
        Debug.Log("Algo entro en mi triger");
        Debug.Log("El tag del objeto que entro en el triger es: " + colision.gameObject.tag);
        //Debug.Log("El valor de invulneravilidad es: " + invulneravilidad.ToString());
        if (colision.tag=="enemigo" && !invulneravilidad)
        {
            Debug.Log("Impacto recibido en el Triger enter");
            invulneravilidad = true;
            salud -= 1;
            if(salud <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                hud.ActualizarVida(salud);
            }
            Debug.Log("Mi salud actual es: " + salud.ToString());
            Invoke("DesactivarInvulnerabilidad", 2);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Si choque");
        if (collision.gameObject.tag == "enemigo" && invulneravilidad == false)
        {
            Debug.Log("Impacto recibido en el colision enter");
            //Invoke("DesactivarInvulnerabilidad",tiempoInvulnerabilidad);
        }
        if(collision.gameObject.CompareTag("botiquin"))
        {
            botiquines += 1;
            hud.ActualizarBotiquines(botiquines);
            Destroy(collision.gameObject);
        }
        if(collision.gameObject.CompareTag("municion"))
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
        invulneravilidad=false;
    }
}
