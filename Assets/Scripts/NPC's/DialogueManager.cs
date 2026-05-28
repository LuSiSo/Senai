using UnityEngine;
using TMPro;
using System.Collections.Generic; // Permite o uso de cole��es como filas (Queue)

public class DialogueManager : MonoBehaviour
{
    // Singleton: Padr�o que permite que qualquer script chame o gerenciador usando DialogueManager.Instance
    public static DialogueManager Instance { get; private set; }

    [Header("Componentes de UI")]
    public GameObject caixaDeDi�logo;     // Objeto pai que cont�m o painel visual do di�logo
    public TMP_Text textoNomeNPC;          // Elemento TextMeshPro para o nome
    public TMP_Text textoDiscurso;         // Elemento TextMeshPro para as frases

    private Queue<string> frases;          // Fila l�gica de strings (Primeira a Entrar � a Primeira a Sair)
    public bool caixaAtiva { get; private set; } = false; // Estado se a janela de conversas est� ativa

    void Awake()
    {
        // Garante que s� exista um script desse rodando por cena
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        frases = new Queue<string>();
    }

    // Inicializa o fluxo de conversas vindo do NPC
    public void IniciarDialogo(string nomeNPC, string[] falasDoNPC)
    {
        caixaAtiva = true;
        caixaDeDi�logo.SetActive(true); // Liga o painel visual na tela
        textoNomeNPC.text = nomeNPC;

        frases.Clear(); // Limpa textos de conversas antigas

        // Adiciona as frases sequencialmente dentro da nossa fila
        foreach (string frase in falasDoNPC)
        {
            frases.Enqueue(frase);
        }

        ExibirProximaFrase();
    }

    public void ExibirProximaFrase()
    {
        // Se a fila esvaziou totalmente, fecha a janela
        if (frases.Count == 0)
        {
            EncerrarDialogo();
            return;
        }

        // Puxa e remove a primeira frase pendente na fila para plotar na tela
        string fraseAtual = frases.Dequeue();
        textoDiscurso.text = fraseAtual;
    }

    void EncerrarDialogo()
    {
        caixaAtiva = false;
        caixaDeDi�logo.SetActive(false); // Desliga o painel visual

        // Comunica o sistema de quests que o jogador concluiu uma conversa
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.VerificarObjetivos("Conversar");
        }
    }
}
