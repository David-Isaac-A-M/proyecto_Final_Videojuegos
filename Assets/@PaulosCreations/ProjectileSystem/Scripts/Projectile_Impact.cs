using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Paulos.Projectiles
{
    public class Projectile_Impact : MonoBehaviour, IProjectileDamageable
    {
        [Header("Called when a Projectile hits this target.")]
        public ImpactEvent OnProjectileImpact;//passes the attacker and damage amount when invoked.

        //Implementation of the IProjectileDamageable Interface
        public void OnProjectileDamage(Transform _attacker, float _damageAmount)
        {
            OnProjectileImpact?.Invoke(_attacker, _damageAmount);
        }
    }

    //custom event
    [System.Serializable]
    public class ImpactEvent : UnityEvent<Transform, float> { }

    //Custom Interface
    public interface IProjectileDamageable
    {
        void OnProjectileDamage(Transform _attacker, float _damageAmount);
    }
}
