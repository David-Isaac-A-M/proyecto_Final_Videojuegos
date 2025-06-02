using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class hud : MonoBehaviour
{
    [SerializeField] Slider barraVida;
    [SerializeField] TextMeshProUGUI contadorBotiquines;
    [SerializeField] TextMeshProUGUI contadorMunicion;
    [SerializeField] TextMeshProUGUI mensajeError;
    [SerializeField] TextMeshProUGUI powerUp;
    [SerializeField] TextMeshProUGUI objetivo;
    [SerializeField] RawImage imgObjetivo;

    private Dictionary<TipoPowerUp, float> powerUpsActivos = new Dictionary<TipoPowerUp, float>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (powerUpsActivos.Count > 0)
        {
            string textoUI = "Power-Ups activos:\n";

            // Crear una lista separada con las claves antes de modificar el Dictionary
            List<TipoPowerUp> claves = new List<TipoPowerUp>(powerUpsActivos.Keys);

            foreach (var key in claves)
            {
                float nuevoTiempo = powerUpsActivos[key] - Time.deltaTime;
                if (nuevoTiempo <= 0)
                {
                    powerUpsActivos.Remove(key);
                }
                else
                {
                    textoUI += $"{key}: {nuevoTiempo:F1}s\n";
                    powerUpsActivos[key] = nuevoTiempo;
                }
            }

            powerUp.text = textoUI;
        }
        else
        {
            powerUp.text = "";
        }

    }

    public void ActivarContador(TipoPowerUp tipo, float duracion)
    {
        if (!powerUpsActivos.ContainsKey(tipo))
        {
            powerUpsActivos.Add(tipo, duracion);
        }
        else
        {
            powerUpsActivos[tipo] = duracion;
        }
    }

    private void OnEnable()
    {
        DM.OnObjetivoDefinido += DefinirObjetivo;
    }
    private void OnDisable()
    {
        DM.OnObjetivoDefinido -= DefinirObjetivo;
    }

    private void DefinirObjetivo(string objetivoAct)
    {
        imgObjetivo.enabled = true;
        objetivo.text = objetivoAct;
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

    public void CambiarMensajeEstado(string texto)
    {
        mensajeError.text = texto;
    }

    public void ActualizarVidaMaxima(int salud)
    {
        barraVida.GetComponent <Slider>().maxValue = salud;
        barraVida.GetComponent<Slider>().value = salud;
    }
}
