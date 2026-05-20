using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering; // Importante para o novo Input System

public class PlayerAttack : MonoBehaviour
{
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
        // Medida de segurança: se esquecer de arrastar o AttackPoint na Unity, o código não trava o jogo
        if (attackPoint == null) return;

        // Toca animação (descomente quando tiver)
        // animator.SetTrigger("Attack");

        // Cria uma esfera invisível que detecta todos os colisores da "enemyLayers" dentro do raio
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayers);

        // Varre a lista de tudo o que foi atingido pelo ataque
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            // SEGURANÇA: Tenta pegar o script Enemy
            Enemy enemy = enemyCollider.GetComponent<Enemy>();

            // Só dá dano se o objeto realmente tiver o script Enemy
            if (enemy != null)
            {
                // 1. Aplica o dano à vida do inimigo
                enemy.TakeDamage(damage);

                // 2. CALCULO DO KNOCKBACK: Subtrai a posição do Player da posição do Inimigo.
                // Isso gera um vetor/seta apontando exatamente na direção oposta ao Player.
                Vector2 direcao = (enemyCollider.transform.position - transform.position).normalized;

                // Ajuste opcional: força o empurrão a ser predominantemente horizontal e adiciona 
                // um leve valor em Y (0.2f) para fazer o inimigo levantar um pouco do chão ao ser atingido
                direcao = new Vector2(Mathf.Sign(direcao.x), 0.2f).normalized; // 0.2f dá um leve empurrão no inimigo

                // 3. Envia a direção calculada e a intensidade da força para o script do inimigo
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
