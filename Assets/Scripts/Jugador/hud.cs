using UnityEngine;
using UnityEngine.UI;
public class hud : MonoBehaviour
{
    [SerializeField] Slider barraVida;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActualizarVida(int salud)
    {
        barraVida.GetComponent<Slider>().value = salud;
    }

    public void ActualizarVidaMaxima(int salud)
    {
        barraVida.GetComponent <Slider>().maxValue = salud;
        barraVida.GetComponent<Slider>().value = salud;
    }
}
