using UnityEngine;

public class ItemColetavel : MonoBehaviour
{
    public ItemData dadosDoItem; // Arraste o Scriptable Object correspondente aqui pelo Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ColetarItem(dadosDoItem);
                Destroy(gameObject); // Remove o item do mapa após coletado
            }
        }
    }
}
