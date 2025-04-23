using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class hud : MonoBehaviour
{
    [SerializeField] Slider barraVida;
    [SerializeField] TextMeshProUGUI contadorBotiquines;
    [SerializeField] TextMeshProUGUI contadorMunicion;
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

    public void ActualizarBotiquines(int botiquines)
    {
        ActualizarTextContador(contadorBotiquines, botiquines);
    }

    public void ActualizarMuniciones(int municion)
    {
        ActualizarTextContador(contadorMunicion, municion);
    }

    public void ActualizarTextContador(TextMeshProUGUI contador, int cantidad)
    {
        if (cantidad < 10)
        {

            contador.text = "x0" + cantidad.ToString();
        }
        else
        {
            contador.text = "x" + cantidad.ToString();
        }
    }

    public void ActualizarVidaMaxima(int salud)
    {
        barraVida.GetComponent <Slider>().maxValue = salud;
        barraVida.GetComponent<Slider>().value = salud;
    }
}
