using UnityEngine;
using UnityEngine.SceneManagement;

public class ControlesHUD : MonoBehaviour
{
    [SerializeField] int saludInicial = 10;
    [SerializeField] int botiquinesIniciales = 5;
    [SerializeField] int MunicionInicial = 79;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Iniciar()
    {
        PlayerPrefs.SetInt("municiones", MunicionInicial);
        PlayerPrefs.SetInt("botiquines", botiquinesIniciales);
        PlayerPrefs.SetInt("salud", saludInicial);
        Debug.Log("La municion del jugador es: " + MunicionInicial);
        //lobbyManager.SetLobbyStarted();
        int escenaActual = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(escenaActual + 1);
    }
    public void ReIniciar()
    {
        Debug.Log("boton pulsado");
        SceneManager.LoadScene(0);
    }
}
