using UnityEngine;

public enum Raridade { Comum, Raro, Lendario }
public enum TipoItem
{
    FragmentoSombrio,
    MedalhaoFuriaAntiga,
    RelicarioIra,
    AmuletoGuerreiroAntigo,
    AnelEclipse
}

[CreateAssetMenu(fileName = "Novo Item", menuName = "Inventario/Item")]
public class ItemData : ScriptableObject
{
    public string nomeItem;
    [TextArea] public string descricao;
    public Raridade raridade;
    public TipoItem tipoItem;
    public Sprite icone; // Útil se for criar um inventário visual depois
}
