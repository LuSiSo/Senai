using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryManager : MonoBehaviour
{
    [Header("UI do Inventário")]
    public GameObject inventoryPanel; // Seu painel de madeira

    // Deixe como tamanho 9 no Inspector e arraste o Slot_1, Slot_2... até o Slot_9
    public InventorySlot[] slots;

    [Header("Referências")]
    public PlayerController player;

    private bool isPaused = false;
    private PlayerInput playerInput;

    void Start()
    {
        if (inventoryPanel != null) inventoryPanel.SetActive(false);
        LimparPainelVisual();

        if (player != null)
        {
            playerInput = player.GetComponent<PlayerInput>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isPaused = !isPaused;

        if (inventoryPanel != null) inventoryPanel.SetActive(isPaused);

        // Pausa ou despausa o jogo (física / time)
        Time.timeScale = isPaused ? 0f : 1f;

        // Bloqueia o input do Player (novo Input System) para evitar callbacks enquanto pausado
        if (playerInput != null)
        {
            playerInput.enabled = !isPaused;
        }
        else if (player != null)
        {
            // alternativa: desabilitar o script PlayerController
            player.enabled = !isPaused;
        }

        // Mostrar cursor quando aberto (opcional)
        Cursor.visible = isPaused;
        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;

        if (isPaused)
        {
            AtualizarVisualInventario();
        }
    }

    public void AtualizarVisualInventario()
    {
        LimparPainelVisual();

        if (player == null) return;

        // Passa pelos itens coletados pelo player (máximo 9)
        for (int i = 0; i < player.itensEquipados.Count; i++)
        {
            if (i < slots.Length)
            {
                slots[i].DefinirItem(player.itensEquipados[i]);
            }
        }
    }

    private void LimparPainelVisual()
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot != null) slot.LimparSlot();
        }
    }
}
