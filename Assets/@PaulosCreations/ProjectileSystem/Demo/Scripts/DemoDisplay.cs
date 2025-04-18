using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Paulos.Projectiles;

public class DemoDisplay : MonoBehaviour
{
    [SerializeField]
    private Transform camBaseTF, followTF;
    private bool chakeCam = true;

    // Start is called before the first frame update
    void Start()
    {
        Projectile_Controller[] projContrlrs = GetComponentsInChildren<Projectile_Controller>();

        for (int i = 0; i < projContrlrs.Length; i++)
        {
            projContrlrs[i].SetupProjectile();
            projContrlrs[i].projectileMovementType = ProjectileMovementTypes.forward;
            projContrlrs[i].InitiateProjectile(null);
        }
    }
    private void Update()
    {
        if (chakeCam)
            camBaseTF.position = followTF.position + (Random.insideUnitSphere * 0.02f);
    }

    public void OnImpact()
    {
        chakeCam = false;
    }
}
