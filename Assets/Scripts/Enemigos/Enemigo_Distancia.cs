using Paulos.Projectiles;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Enemigo_Distancia : MonoBehaviour
{

    [SerializeField] Transform spawnBulletPoint;

    private Transform playerPosition;

    [SerializeField] float maxDistance;


    private bool enfriamiento;


    void Start()
    {
        enfriamiento = false;
    }

    void Update()
    {
        Vector3 origenRayo = spawnBulletPoint.position;
        Vector3 direccion = spawnBulletPoint.forward;
        //Este codigo sirve para ver el raycast que se genra para el disparo, descomente si va a cambiar la maxDistance u otro aspecto
        Debug.DrawRay(origenRayo, direccion * maxDistance, Color.red);
        playerPosition = GameObject.FindWithTag("jugador").transform;


        RaycastHit hit;
        if(Physics.Raycast(origenRayo,direccion,out hit, maxDistance))
        {
            if(hit.collider.CompareTag("jugador") && !enfriamiento)
            {
                ShootPlayer();
            }
            Debug.Log("Impactó con: " + hit.collider.name);
        }
        if (Input.GetButtonDown("Fire1")) //En este if es donde debe de estar la verificacion al raycast para llamar al metodo
        {
            ShootPlayer();
        }
    }

    void ShootPlayer()
    {
        //Vector3 playerDirection = playerPosition.position - transform.position;
        enfriamiento= true;
        Projectile_Manager._Instance.FireProjectileForward("Projectile_Bullet_L", spawnBulletPoint);
        Invoke("DesactivarEnfriamiento", 1);

    }

    void DesactivarEnfriamiento()
    {
        enfriamiento = false;
    }
}
