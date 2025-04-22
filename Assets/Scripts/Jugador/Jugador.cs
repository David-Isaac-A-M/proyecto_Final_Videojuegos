using UnityEngine;
using Paulos.Projectiles;


public class Jugador : MonoBehaviour
{
    [SerializeField] Transform boquilla;
    [SerializeField] Transform mira;
    [SerializeField] int salud;

    public float tiempoInvulnerabilidad = 1;

    private bool invulneravilidad = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        invulneravilidad = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Disparar();
        }
    }

    void Disparar()
    {
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
            Debug.Log("Mi salud actual es: " + salud.ToString());
            Invoke("DesactivarInvulnerabilidad", 2);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "enemigo" && invulneravilidad == false)
        {
            Debug.Log("Impacto recibido en el colision enter");
            //Invoke("DesactivarInvulnerabilidad",tiempoInvulnerabilidad);
        }
    }

    public void DesactivarInvulnerabilidad()
    {
        invulneravilidad=false;
    }
}
