using UnityEngine;
using Paulos.Projectiles;


public class Jugador : MonoBehaviour
{
    [SerializeField] Transform boquilla;
    [SerializeField] Transform mira;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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
            Debug.Log("Estas apuntando al disparar");
            if (mira)
            {
                Projectile_Manager._Instance.FireProjectileForward("Projectile_Fire", mira);
            }
        }
        else
        if (boquilla)
        {
            Debug.Log("Estas disparando sin apuntar");
            Projectile_Manager._Instance.FireProjectileForward("Projectile_Fire", boquilla);
        }
    }
}
