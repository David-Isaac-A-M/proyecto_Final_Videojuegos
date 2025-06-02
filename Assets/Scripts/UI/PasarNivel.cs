using UnityEngine;
using UnityEngine.SceneManagement;
public class PasarNivel : MonoBehaviour
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

    private void CambiarEscena()
    {

        PlayerPrefs.SetInt("municiones", jugador.municiones);
        PlayerPrefs.SetInt("botiquines", jugador.botiquines);
        PlayerPrefs.SetInt("salud", jugador.salud);
        Debug.Log("La municion del jugador es: " + jugador.municiones);
        //lobbyManager.SetLobbyStarted();
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(escenaActual+1);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
