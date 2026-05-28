using System.Globalization;
using System.Runtime.ExceptionServices;
using UnityEngine;

public class ItensInteragíveis : MonoBehaviour, IInterag�vel
{
    [Header("Perfil do NPC")]
    public string nomeDoObjeto;// Nome exibido na intreface gr�fica

    [TextArea(3, 5)] // Aumenta o tamanho da caixinha de digita��o na Unity
    public string[] descri��o; // Lista de frases estruturadas da conversa

    [Header("Cofigura��o de Miss�o (Opcional")]
    public bool temMiss�oParaDar = false; // Flag se este NPC concede uma miss�o
    public string idDaMiss�o; // O ID exato cadastrado no QuestManager

    public void Interagir()
    {
        if (DialogueManager.Instance.caixaAtiva)
        {
            DialogueManager.Instance.ExibirProximaFrase();
        }
        else
        {
            DialogueManager.Instance.IniciarDialogo(nomeDoObjeto, descri��o);

            if (temMiss�oParaDar)
            {
                QuestManager.Instance.AtivarMissao(idDaMiss�o);
            }
        }
    }
}
