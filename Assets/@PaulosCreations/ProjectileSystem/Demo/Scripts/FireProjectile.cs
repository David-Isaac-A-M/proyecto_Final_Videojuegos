using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Paulos.Projectiles;

public class FireProjectile : MonoBehaviour
{
    [SerializeField]
    private Transform turretTF, firePointTF, targetTF, myTF;
    [SerializeField]
    private Text infoText, currentText;
    [SerializeField]
    private string[] projectileNames;
    [SerializeField]
    private ParticleSystem fireParticles;

    private Projectile_Manager projectileManager;
    private int projectileNR;

    private AudioSource fireAudio;

    private void Start()
    {
        myTF = transform;
        fireAudio = GetComponent<AudioSource>();
        projectileManager = Projectile_Manager._Instance;
        currentText.text = projectileNames[projectileNR];
        StartCoroutine(ShowPoolsInfo());
    }

    public void ChangeProjectile()
    {
        projectileNR += 1;

        if (projectileNR >= projectileNames.Length)
            projectileNR = 0;

        currentText.text = projectileNames[projectileNR];
    }

    public void FireForward()
    {
        transform.rotation = Quaternion.identity;
        TurretRecoil();

        projectileManager.FireProjectileForward(projectileNames[projectileNR], firePointTF);
    }

    public void FireHoming()
    {
        myTF.LookAt(targetTF);
        TurretRecoil();

        projectileManager.FireProjectileHoming(projectileNames[projectileNR], firePointTF, targetTF);
    }

    public void FireAimed()
    {
        myTF.LookAt(targetTF);
        TurretRecoil();

        projectileManager.FireProjectileAimed(projectileNames[projectileNR], firePointTF, targetTF);
    }

    public void FireDirectional()
    {
        Vector3 randDir = Random.insideUnitSphere;
        myTF.forward = randDir;
        TurretRecoil();

        projectileManager.FireProjectileDirectional(projectileNames[projectileNR], firePointTF, randDir);
    }

    private IEnumerator ShowPoolsInfo()
    {
        while (true)
        {
            infoText.text = projectileManager.GetPoolsInfo();

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void EmptyAllPools()
    {
        projectileManager.ClearAllPools();
    }

    public void EmptyPoolByName()
    {
        projectileManager.ClearPoolByName(projectileNames[projectileNR]);
    }

    private void TurretRecoil()
    {
        fireParticles.Play();
        fireAudio.pitch = Random.Range(0.95f, 1.05f);
        fireAudio.Play();
        turretTF.localPosition = new Vector3(0, 0, -0.6f);
    }

    private void Update()
    {
        turretTF.localPosition = Vector3.MoveTowards(turretTF.localPosition, Vector3.zero, 6f *Time.deltaTime);
    }
}
