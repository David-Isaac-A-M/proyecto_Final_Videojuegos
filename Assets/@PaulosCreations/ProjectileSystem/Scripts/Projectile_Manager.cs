using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Paulos.Projectiles
{
    public class Projectile_Manager : MonoBehaviour
    {
        public static Projectile_Manager _Instance;

        [SerializeField]
        [Header("The name of the folder in Resources where the Projectile prefabs are located")]
        private string resourceFolderName = "Projectiles";
        //Everything inside the Resource folder will be included when you build the project/game.
        //make sure everything inside it is actually being used in your final game/project.

        [SerializeField]
        [Header("Unload from memory when clearing pools")]
        [Tooltip("If you are not sure 'yes' is probably best")]
        private UnloadResources unloadUnusedResources;

        //Loaded prefabs by name
        private Dictionary<string, GameObject> PrefabLoadedDict = new Dictionary<string, GameObject>();
        //projectile Pools by projectile name
        private Dictionary<string, List<Projectile_Controller>> ProjectilePoolsDict = new Dictionary<string, List<Projectile_Controller>>();

        private void Awake()
        {
            if (_Instance == null)
                _Instance = this;
            else Destroy(this);
        }

        //fire in a direction with projectile name and startpoint
        public void FireProjectileDirectional(string _projectileName, Transform _startPointTF, Vector3 _moveDirection)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)//pooled projectile found
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.forward = _moveDirection;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.forward;
                pooledProjectileController.InitiateProjectile(null);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.forward = _moveDirection;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.forward;

                projectileController.InitiateProjectile(null);
            }
        }

        //fire in a direction with projectile name and startpoint and custom damage to apply
        public void FireProjectileDirectional(string _projectileName, Transform _startPointTF, Vector3 _moveDirection, float _damageMultiplier)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.forward = _moveDirection;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.forward;
                pooledProjectileController.InitiateProjectile(null, _damageMultiplier);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.forward = _moveDirection;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.forward;

                projectileController.InitiateProjectile(null, _damageMultiplier);
            }
        }

        //fire forward with projectile name and startpoint
        public void FireProjectileForward(string _projectileName, Transform _startPointTF)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)//pooled projectile found
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.rotation = _startPointTF.rotation;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.forward;
                pooledProjectileController.InitiateProjectile(null);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.rotation = _startPointTF.rotation;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.forward;

                projectileController.InitiateProjectile(null);
            }
        }

        //fire forward with projectile name and startpoint and custom damage to apply
        public void FireProjectileForward(string _projectileName, Transform _startPointTF, float _damageMultiplier)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.rotation = _startPointTF.rotation;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.forward;
                pooledProjectileController.InitiateProjectile(null, _damageMultiplier);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.rotation = _startPointTF.rotation;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.forward;

                projectileController.InitiateProjectile(null, _damageMultiplier);
            }
        }

        //fire homing with projectile name, startpoint, and target
        public void FireProjectileHoming(string _projectileName, Transform _startPointTF, Transform _targetTF)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.rotation = _startPointTF.rotation;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.homing;
                pooledProjectileController.InitiateProjectile(null, _targetTF);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.rotation = _startPointTF.rotation;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.homing;

                projectileController.InitiateProjectile(null, _targetTF);
            }
        }

        //fire homing with projectile name, startpoint, target and custom damage to apply
        public void FireProjectileHoming(string _projectileName, Transform _startPointTF, Transform _targetTF, float _damageMultiplier)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.rotation = _startPointTF.rotation;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.homing;
                pooledProjectileController.InitiateProjectile(null, _targetTF, _damageMultiplier);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.rotation = _startPointTF.rotation;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.homing;

                projectileController.InitiateProjectile(null, _targetTF, _damageMultiplier);
            }
        }

        //fire aimend with projectile name, startpoint and target 
        public void FireProjectileAimed(string _projectileName, Transform _startPointTF, Transform _targetTF)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.rotation = _startPointTF.rotation;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.aimed;
                pooledProjectileController.InitiateProjectile(null, _targetTF);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.rotation = _startPointTF.rotation;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.aimed;

                projectileController.InitiateProjectile(null, _targetTF);
            }
        }

        //fire aimed with projectile name, startpoint, target and custom damage to apply
        public void FireProjectileAimed(string _projectileName, Transform _startPointTF, Transform _targetTF, float _damageMultiplier)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.rotation = _startPointTF.rotation;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.aimed;
                pooledProjectileController.InitiateProjectile(null, _targetTF, _damageMultiplier);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.rotation = _startPointTF.rotation;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.aimed;

                projectileController.InitiateProjectile(null, _targetTF, _damageMultiplier);
            }
        }


        //with reference to the attacker passed to the target
        //fire in a direction with projectile name and startpoint
        public void FireProjectileDirectional(string _projectileName, Transform _startPointTF, Vector3 _moveDirection, Transform _attacker)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.forward = _moveDirection;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.forward;
                pooledProjectileController.InitiateProjectile(_attacker);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.forward = _moveDirection;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.forward;

                projectileController.InitiateProjectile(_attacker);
            }
        }

        //fire in a direction with projectile name and startpoint and custom damage to apply
        public void FireProjectileDirectional(string _projectileName, Transform _startPointTF, Vector3 _moveDirection, Transform _attacker, float _damageMultiplier)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.forward = _moveDirection;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.forward;
                pooledProjectileController.InitiateProjectile(_attacker, _damageMultiplier);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.forward = _moveDirection;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.forward;

                projectileController.InitiateProjectile(_attacker, _damageMultiplier);
            }
        }

        //fire forward with projectile name and startpoint
        public void FireProjectileForward(string _projectileName, Transform _startPointTF, Transform _attacker)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.rotation = _startPointTF.rotation;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.forward;
                pooledProjectileController.InitiateProjectile(_attacker);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.rotation = _startPointTF.rotation;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.forward;

                projectileController.InitiateProjectile(_attacker);
            }
        }

        //fire forward with projectile name and startpoint and custom damage to apply
        public void FireProjectileForward(string _projectileName, Transform _startPointTF, Transform _attacker, float _damageMultiplier)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.rotation = _startPointTF.rotation;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.forward;
                pooledProjectileController.InitiateProjectile(_attacker, _damageMultiplier);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.rotation = _startPointTF.rotation;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.forward;

                projectileController.InitiateProjectile(_attacker, _damageMultiplier);
            }
        }

        //fire homing with projectile name, startpoint, and target
        public void FireProjectileHoming(string _projectileName, Transform _startPointTF, Transform _targetTF, Transform _attacker)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.rotation = _startPointTF.rotation;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.homing;
                pooledProjectileController.InitiateProjectile(_attacker, _targetTF);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.rotation = _startPointTF.rotation;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.homing;

                projectileController.InitiateProjectile(_attacker, _targetTF);
            }
        }

        //fire homing with projectile name, startpoint, target and custom damage to apply
        public void FireProjectileHoming(string _projectileName, Transform _startPointTF, Transform _targetTF, Transform _attacker, float _damageMultiplier)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.rotation = _startPointTF.rotation;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.homing;
                pooledProjectileController.InitiateProjectile(_attacker, _targetTF, _damageMultiplier);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.rotation = _startPointTF.rotation;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.homing;

                projectileController.InitiateProjectile(_attacker, _targetTF, _damageMultiplier);
            }
        }

        //fire aimend with projectile name, startpoint and target 
        public void FireProjectileAimed(string _projectileName, Transform _startPointTF, Transform _targetTF, Transform _attacker)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.rotation = _startPointTF.rotation;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.aimed;
                pooledProjectileController.InitiateProjectile(_attacker, _targetTF);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.rotation = _startPointTF.rotation;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.aimed;

                projectileController.InitiateProjectile(_attacker, _targetTF);
            }
        }

        //fire aimed with projectile name, startpoint, target and custom damage to apply
        public void FireProjectileAimed(string _projectileName, Transform _startPointTF, Transform _targetTF, Transform _attacker, float _damageMultiplier)
        {
            //look for available pooled projectile
            Projectile_Controller pooledProjectileController = GetPooledProjectile(_projectileName);

            if (pooledProjectileController != null)
            {
                //fire the projectile
                pooledProjectileController.transform.position = _startPointTF.position;
                pooledProjectileController.transform.rotation = _startPointTF.rotation;

                pooledProjectileController.projectileMovementType = ProjectileMovementTypes.aimed;
                pooledProjectileController.InitiateProjectile(_attacker, _targetTF, _damageMultiplier);

                return;
            }

            //Instantiate projectile if no pooled are found or available
            Transform SpawnedProjectileTF = GetSpawnedProjectile(_projectileName);//spawn new projectile

            if (SpawnedProjectileTF != null)
            {
                //setup and fire the projectile
                SpawnedProjectileTF.position = _startPointTF.position;
                SpawnedProjectileTF.rotation = _startPointTF.rotation;

                Projectile_Controller projectileController = SpawnedProjectileTF.GetComponent<Projectile_Controller>();

                projectileController.SetupProjectile();
                projectileController.projectileMovementType = ProjectileMovementTypes.aimed;

                projectileController.InitiateProjectile(_attacker, _targetTF, _damageMultiplier);
            }
        }


        //Utilities
        private Projectile_Controller GetPooledProjectile(string _projectileName)
        {
            Projectile_Controller pooledProjectileController = null;

            //look for a available pooled projectile
            ProjectilePoolsDict.TryGetValue(_projectileName, out List<Projectile_Controller> projectileList);

            if (projectileList != null)//if the pool exist
            {
                for (int i = 0; i < projectileList.Count; i++)//look for inactive projectile in pool
                {
                    if (projectileList[i].isFired == false)
                    {
                        pooledProjectileController = projectileList[i];
                        break;
                    }
                }
            }

            return pooledProjectileController;
        }

        private Transform GetSpawnedProjectile(string _projectileName)
        {
            GameObject objToSpawn = null;

            //check if the projectile is loaded from resources
            PrefabLoadedDict.TryGetValue(_projectileName, out GameObject projectilePF);

            if (projectilePF == null)//not loaded
            {
                //load from resource folder
                projectilePF = Resources.Load(resourceFolderName + "/" + _projectileName) as GameObject;

                if (projectilePF != null)
                {
                    PrefabLoadedDict.Add(_projectileName, projectilePF);
                }
                else
                {
                    Debug.LogWarning("Projectile prefab : '" + _projectileName + "' does not exist.");
                    return null;
                }
            }

            //spawn new projectile
            objToSpawn = Instantiate(projectilePF);
            objToSpawn.name = _projectileName;
            objToSpawn.transform.parent = transform;

            //create a pool for the projectile if it does not exist yet
            if (!ProjectilePoolsDict.ContainsKey(_projectileName))
            {
                ProjectilePoolsDict.Add(_projectileName, new List<Projectile_Controller>());
            }

            //add the spawned projectile to the pool
            ProjectilePoolsDict.TryGetValue(_projectileName, out List<Projectile_Controller> projectileList);

            if (projectileList != null)
            {
                projectileList.Add(objToSpawn.GetComponent<Projectile_Controller>());
            }

            return objToSpawn.transform;
        }

        public string GetPoolsInfo()
        {
            string poolsInfo = "Pools Info :\n";

            foreach (KeyValuePair<string, List<Projectile_Controller>> dictItem in ProjectilePoolsDict)
            {
                poolsInfo += string.Format("{0} x {1}\n", dictItem.Key, dictItem.Value.Count);
            }

            return poolsInfo;
        }

        //destroy all projectiles and clear all poolLists
        public void ClearAllPools()
        {
            foreach (List<Projectile_Controller> projectileList in ProjectilePoolsDict.Values)
            {
                foreach (Projectile_Controller controller in projectileList)
                {
                    Destroy(controller.gameObject);
                }

                projectileList.Clear();
            }

            ProjectilePoolsDict.Clear();

            if (unloadUnusedResources == UnloadResources.yes)
            {
                PrefabLoadedDict.Clear();
                Resources.UnloadUnusedAssets();
            }
        }

        //Destroy projectiles of one type and clear it`s poolList
        public void ClearPoolByName(string _projectileName)
        {
            //look for the pool list
            ProjectilePoolsDict.TryGetValue(_projectileName, out List<Projectile_Controller> projectileList);

            if (projectileList != null)
            {
                foreach (Projectile_Controller controller in projectileList)
                {
                    Destroy(controller.gameObject);
                }

                projectileList.Clear();

                ProjectilePoolsDict.Remove(_projectileName);
            }


            if (unloadUnusedResources == UnloadResources.yes)
            {
                PrefabLoadedDict.Remove(_projectileName);
                Resources.UnloadUnusedAssets();
            }
        }
    }

    //Custom Class
    public enum ProjectileMovementTypes { forward, homing, aimed}
    public enum UnloadResources { yes, no};
}
