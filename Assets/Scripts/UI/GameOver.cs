using UnityEngine;

public class GameOver : MonoBehaviour
{
    [SerializeField] Canvas pantallaFin;
    [SerializeField] Canvas HUD;

    public void Start()
    {
        pantallaFin.enabled = false;
    }

    public void ActivarPantallaGameOver()
    {
        HUD.enabled = false;
        pantallaFin.enabled = true;
    }


}
