using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering; // Importante para o novo Input System

public class PlayerAttack : MonoBehaviour
{
    private PlayerController playerController;

    [Header("Configurações de Ataque")]
    public Transform attackPoint; // Objeto vazio (filho do player) que define o centro do golpe
    public float attackRange = 0.5f; // Raio do círculo de alcance do golpe
    public LayerMask enemyLayers; // Layer correspondente aos inimigos do jogo
    public int damage = 20; // Quantidade de dano que o ataque causará ao inimigo

    [Header("Configuração de KnockBack")]
    public float knockbackForca = 8f; // Força do Knockback

    [Header("Configurações de Cooldown")]
    public float ataqueCooldown = 0.5f;   // Tempo de espera entre os ataques (ex: meio segundo)
    private float tempoUltimoAtaque = 0f; // Guarda o tempo em que o jogador atacou pela última vez

    void Start()
    {
        playerController = GetComponent<PlayerController>();
    }


    // Esta função será chamada automaticamente pelo Player Input (adicione a ação "Attack" lá)
    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
        {
            if (Time.time >= tempoUltimoAtaque + ataqueCooldown)
            {
                Attack();
            }
        }
    }

    void Attack()
    {
        if (attackPoint == null) return;

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider2D enemyCollider in hitEnemies)
        {
            Enemy enemy = enemyCollider.GetComponent<Enemy>();

            if (enemy != null)
            {
                // CALCULA O DANO COM BUFFS CONDICIONAIS
                int danoFinal = damage;

                if (playerController != null)
                {
                    // Checa se tem o Amuleto do Guerreiro Antigo E a vida está cheia
                    if (playerController.TemItemEquipado(TipoItem.AmuletoGuerreiroAntigo) && playerController.TemVidaCheia())
                    {
                        // O item diz: "Aumenta o dano de 20 para 30". 
                        // Se o player já tiver outros buffs (ex: Medalhão), adicionamos a diferença (+10) 
                        // ou forçamos o valor. Somar +10 preserva os outros buffs acumulados!
                        danoFinal += 10;
                    }
                }

                Debug.Log($"Player causa {danoFinal} de dano ao inimigo '{enemy.name}'.");
                // 1. Aplica o dano calculado à vida do inimigo
                enemy.TakeDamage(danoFinal);

                // 2. CÁLCULO DO KNOCKBACK (Mantenha o seu código original aqui...)
                Vector2 direcao = (enemyCollider.transform.position - transform.position).normalized;
                direcao = new Vector2(Mathf.Sign(direcao.x), 0.2f).normalized;
                enemy.TomarKnockback(direcao, knockbackForca);
            }
        }
    }

    // Desenha uma linha guia vermelha no editor da Unity para ajudar a ajustar o alcance visualmente
    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red; // Deixa o círculo vermelho no editor
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
