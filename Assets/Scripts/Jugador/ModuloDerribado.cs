using UnityEngine;
using PurrNet;

public class ModuloDerribado : NetworkBehaviour
{
    [SerializeField] private SyncVar<bool> derribado;

    public bool Derribado => derribado;

   [ObserversRpc(bufferLast: true)]
    public void CambiarDerrobado (bool valor)
    {
        derribado.value = valor;
    }
}
