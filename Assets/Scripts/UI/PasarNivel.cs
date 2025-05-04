using UnityEngine;
using UnityEngine.SceneManagement;
using PurrNet;
public class PasarNivel : NetworkBehaviour
{
    Jugador jugador;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("jugador"))
        {
            jugador = other.GetComponent<Jugador>();
            CambiarEscena();
        }
    }

    [ObserversRpc]
    private void CambiarEscena()
    {

        PlayerPrefs.SetInt("municiones", jugador.municiones);
        PlayerPrefs.SetInt("botiquines", jugador.botiquines);
        PlayerPrefs.SetInt("salud", jugador.salud);
        Debug.Log("La municion del jugador es: " + jugador.municiones);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
