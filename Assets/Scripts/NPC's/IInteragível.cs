using UnityEngine;

// Uma interface não herda de MonoBehaviour. ELa apenas dita uma regra de comportamento.
public interface IInteragível
{
    // Qualquer script que assinar esta interface DEVE implementar está função
    void Interagir()
    {

    }
}
