using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    private Image imagemSlot;

    void Awake()
    {
        // Pega o componente de imagem do próprio objeto
        imagemSlot = GetComponent<Image>();
    }

    // Esconde o slot se não houver item
    public void LimparSlot()
    {
        if (imagemSlot != null)
        {
            imagemSlot.sprite = null;
            imagemSlot.enabled = false; // Desliga a imagem para não aparecer o quadrado branco
        }
    }

    // Mostra o ícone do item coletado
    public void DefinirItem(ItemData item)
    {
        if (imagemSlot != null && item != null && item.icone != null)
        {
            imagemSlot.sprite = item.icone;
            imagemSlot.enabled = true; // Ativa a imagem para exibir o ícone
        }
    }
}
