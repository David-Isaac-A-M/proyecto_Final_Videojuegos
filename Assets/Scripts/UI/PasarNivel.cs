using UnityEngine;
using UnityEngine.SceneManagement;
using PurrNet;
using PurrLobby;
public class PasarNivel : NetworkBehaviour
{
    Jugador jugador;

    [PurrScene, SerializeField] private string nextScene;
    [SerializeField] private LobbyManager lobbyManager;
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
        //lobbyManager.SetLobbyStarted();
        SceneManager.LoadSceneAsync(nextScene);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
