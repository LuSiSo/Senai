using UnityEngine;
using UnityEngine.Rendering;

public class Enemy : MonoBehaviour
{
    [Header("Atributos e Vida")]
    public int health = 100; // Vida total do inimigo

    [Header("Configuração de Aqtaque")]
    public Transform enemyAttackPoint; // Ponro de origem do ataque do inimigo
    public float attackRange = 0.6f; // Alvance do golpe do inimigo
    public LayerMask playerLayer; // Camada onde o Player está (ex: "Player")
    public float danoParaCausar = 10f; // Quantidade de dano que o inimigo causa
    public float ataqueCooldown = 1.5f; // Tempo de espera entre um ataque e outro (em segundos)

    public float tempoUltimoAtaque = 0f; // Rastreador do cronômetro de ataque
    private Rigidbody2D rb; // Guarda o componente físico do inimigo

    // Função pública chamada pelo script 'PlayerAttack' para reduzir a vida do inimigo
    public void TakeDamage(int damage)
    {
        health -= damage;

        // Se a vida zerar ou negativar, executa a lógica de morte
        if (health <= 0) Die();
    }

    // Função pública chamada pelo script 'PlayerAttack' para empurrar o inimigo fisicamente
    public void TomarKnockback(Vector2 direcao, float forca)
    {
        if (rb != null)
        {
            // Reseta a velocidade atual do inimigo para que o empurrão não seja afetado 
            // por movimentos anteriores, tornando o impacto consistente.
            rb.linearVelocity = Vector2.zero;

            // Adiciona força física instantânea usando o modo de Impulso
            rb.AddForce(direcao * forca, ForceMode2D.Impulse);
        }
    }

    void Die()
    {
        Debug.Log("Inimigo morreu!");
        Destroy(gameObject); // Remove o inimigo da cena completamente
    }

    void Start()
    {
        // Pega automaticamente o componente Rigidbody2D anexado a este inimigo
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        TentarAtacar();
    }

    // Detecta colisões físicas sólidas (usado quando o inimigo esbarra no jogador)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Tenta pegar o componente PlayerController do objeto que colidiu
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        // Se o objeto for realmente o Player
        if (player != null)
        {
            // O inimigo diz para o player receber dano
            player.TomarDano(danoParaCausar);
        }
    }

    void TentarAtacar()
    {
        if (enemyAttackPoint == null) return;

        // Verifica se o tempo atual do jogo já passou do tempo do último ataque + o cooldown necessário
        if (Time.time >= tempoUltimoAtaque + ataqueCooldown)
        {
            Collider2D playerAtingido = Physics2D.OverlapCircle(enemyAttackPoint.position, attackRange, playerLayer);

            if (playerAtingido != null)
            {
                // Executar o ataque
                Atacar(playerAtingido);
            }
        }
    }

    void Atacar(Collider2D playerCollider)
    {
        tempoUltimoAtaque = Time.time;

        Debug.Log("Inimigo atacou o Player!");

        PlayerController player = playerCollider.GetComponent<PlayerController>();

        if (player != null)
        {

            player.TomarDano(danoParaCausar);
        }
    }
}
