using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paulos.Projectiles
{
    public class Projectile_Controller : MonoBehaviour
    {
        [Header("Parameters")]
        public float speed;
        [Tooltip("Can be multiplied with when fired.")]
        public float baseDamage;
        [Tooltip("Time between impact and pooling. (for the effects to play out.")]
        public float poolDelay = 1f;
        [Tooltip("Time till repool, when nothing is hit.")]
        public float maxLifeTime = 10f;
        public bool playAudioOnImpact;

        [Header("References")]
        [Space(10)]
        public GameObject meshObject;
        public GameObject effectsObject;
        public AudioSource impactAudio;

        [HideInInspector]
        public ProjectileMovementTypes projectileMovementType;
        public bool isFired { get; private set; }

        private bool targetHit;
        private float damageMultiplier = 1, originalPitch;
        private Transform currentTargetTF, currentAttackerTF;

        private Transform myTransform;
        private GameObject myObject;
        private Rigidbody myRigidBody;
        private Collider myCollider;
        private Coroutine homingMovementCor, lifeTimerCor;
        private bool homingCorRunning, lifeTimerRunning;

        //only called ones when spawned/instantiated by projectile manager
        public void SetupProjectile()
        {
            myTransform = transform;
            myObject = gameObject;
            myRigidBody = GetComponent<Rigidbody>();
            myCollider = GetComponent<Collider>();
            originalPitch = impactAudio.pitch;

            effectsObject.SetActive(false);

            if (!playAudioOnImpact)
                impactAudio.gameObject.SetActive(false);

            myObject.SetActive(false);
        }


        //fire forward
        public void InitiateProjectile(Transform _attacker)
        {
            currentAttackerTF = _attacker;
            damageMultiplier = 1;

            FireProjectile();
        }

        //fire forward with custom damage amount
        public void InitiateProjectile(Transform _attacker, float _damageMultiplier)
        {
            currentAttackerTF = _attacker;
            damageMultiplier = _damageMultiplier;

            FireProjectile();
        }

        //fire with target
        public void InitiateProjectile(Transform _attacker, Transform _target)
        {
            currentAttackerTF = _attacker;
            damageMultiplier = 1;
            currentTargetTF = _target;

            FireProjectile();
        }

        //fire with target and custom damage amount
        public void InitiateProjectile(Transform _attacker, Transform _target, float _damageMultiplier)
        {
            currentAttackerTF = _attacker;
            damageMultiplier = _damageMultiplier;
            currentTargetTF = _target;

            FireProjectile();
        }


        //start projectile movement
        private void FireProjectile()
        {
            isFired = true;
            targetHit = false;

            myObject.SetActive(true);

            if (projectileMovementType == ProjectileMovementTypes.forward)
            {
                myRigidBody.linearVelocity = myTransform.forward * speed;
            }
            else if (projectileMovementType == ProjectileMovementTypes.homing)
            {
                if (!homingCorRunning)
                    homingMovementCor = StartCoroutine(HomingMovement());
            }
            else if (projectileMovementType == ProjectileMovementTypes.aimed)
            {
                Vector3 dir = (currentTargetTF.position - myTransform.position).normalized;
                myTransform.forward = dir;
                myRigidBody.linearVelocity = dir * speed;
            }

            if (!lifeTimerRunning)
                lifeTimerCor = StartCoroutine(LifeTimer());

            myCollider.enabled = true;
            meshObject.SetActive(true);
        }

        private IEnumerator HomingMovement()
        {
            homingCorRunning = true;

            bool targetValid = true;
            GameObject targetObj = currentTargetTF.gameObject;

            while (!targetHit && targetValid)
            {
                //Check if the target has been Disabled or Destroyed.
                if (currentTargetTF == null || targetObj.activeInHierarchy == false)
                {
                    myRigidBody.linearVelocity = myTransform.forward * speed;

                    targetValid = false;
                }
                else
                {
                    Vector3 moveDirection = (currentTargetTF.position - myTransform.position).normalized;
                    myRigidBody.linearVelocity = moveDirection * speed;
                    myTransform.LookAt(currentTargetTF);
                }

                yield return null;
            }

            homingCorRunning = false;
        }

        //when nothing is hit for to long, pool the projectile.
        private IEnumerator LifeTimer()
        {
            lifeTimerRunning = true;

            yield return new WaitForSeconds(maxLifeTime);

            lifeTimerRunning = false;

            if (projectileMovementType == ProjectileMovementTypes.homing)
            {
                if (homingCorRunning)
                {
                    StopCoroutine(homingMovementCor);
                    homingCorRunning = false;
                }
            }

            if (!targetHit)
            {
                myRigidBody.linearVelocity = Vector3.zero;

                myCollider.enabled = false;
                meshObject.SetActive(false);

                Pool();
            }
        }
       
        private void OnTriggerEnter(Collider other)
        {
            if (targetHit)
                return;

            targetHit = true;

            if (projectileMovementType == ProjectileMovementTypes.homing)
            {
                if (homingCorRunning)
                {
                    StopCoroutine(homingMovementCor);
                    homingCorRunning = false;
                }
            }

            if (lifeTimerRunning)
            {
                StopCoroutine(lifeTimerCor);
                lifeTimerRunning = false;
            }

            myRigidBody.linearVelocity = Vector3.zero;

            myCollider.enabled = false;
            meshObject.SetActive(false);

            if (playAudioOnImpact)
                impactAudio.pitch = originalPitch + Random.Range(-0.1f, 0.1f);

            effectsObject.SetActive(true);

            //call the IProjectileDamageable interface on the target we hit, if it has IProjectileDamageable implemented.
            other.transform.GetComponent<IProjectileDamageable>()?.OnProjectileDamage(currentAttackerTF, baseDamage * damageMultiplier);

            Invoke("Pool", poolDelay);//delayed so the effects can play out.
        }

        private void Pool()
        {
            effectsObject.SetActive(false);
            myObject.SetActive(false);

            isFired = false;
        }
    }
}
