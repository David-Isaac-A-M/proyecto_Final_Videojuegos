using UnityEngine;

public class CargadorEscena : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject personaje;
    [SerializeField] private GameObject puntoSpawn;
    private void Awake()
    {
        Instantiate(personaje,puntoSpawn.transform);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
