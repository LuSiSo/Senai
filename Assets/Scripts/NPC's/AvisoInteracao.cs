using UnityEngine;

public class AvisoInteracao : MonoBehaviour
{
    [Header("Componente Visual")]
    public GameObject iconeIndicativo; // Arraste o seu objeto 'IndicativoVisual' para c�

    void Start()
    {
        // Garante que o �cone comece desligado quando o jogo iniciar
        if (iconeIndicativo != null)
        {
            iconeIndicativo.SetActive(false);
        }
    }

    // Disparado AUTOMATICAMENTE quando o Player entra na �rea do Collider (que deve ser Trigger)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se quem entrou na �rea de alcance foi o Player
        if (collision.CompareTag("Player"))
        {
            if (iconeIndicativo != null)
            {
                iconeIndicativo.SetActive(true); // Liga o bal�ozinho visual
                Debug.Log("Player aproximou-se de: " + gameObject.name);
            }
        }
    }

    // Disparado AUTOMATICAMENTE quando o Player sai da �rea do Collider
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Verifica se quem saiu da �rea foi o Player
        if (collision.CompareTag("Player"))
        {
            if (iconeIndicativo != null)
            {
                iconeIndicativo.SetActive(false); // Desliga o bal�ozinho visual
                Debug.Log("Player afastou-se de: " + gameObject.name);
            }
        }
    }
}
