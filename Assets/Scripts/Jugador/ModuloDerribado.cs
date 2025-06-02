using UnityEngine;

public class ModuloDerribado : MonoBehaviour
{
    [SerializeField] private bool derribado;

    public bool Derribado => derribado;

    public void CambiarDerrobado (bool valor)
    {
        derribado = valor;
    }
}
