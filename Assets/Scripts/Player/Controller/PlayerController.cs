using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movimento")]
    public float jumpForce = 6f;
    public float speed = 6f;
    private float moveInput;
    public float facingDirection = 1f;
    private bool facingRight = true;

    [Header("Ajustes de Inércia")]
    public float acceleration = 10f; // Quanto maior, mais rápido atinge a velocidade máxima
    public float deceleration = 5f; // Quanto maior, mais rápido ele para

    //Limitador de velocidade
    [SerializeField] private bool limitarVelocidadeManual = true;

    [Header("Corrida")]
    public float runMultiplier = 2f; // Multiplicador de velocidade
    private float currentRunValue = 1f; // Valor atual (1 ou 1.5)

    [Header("Dash")]
    public float dashForce = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 3f;
    private bool canDash = true;
    private bool isDashing = false;

    [Header("Coyote Time")]
    public float coyoteTime = 0.2f; // Tempo de tolerância
    private float coyoteTimeCounter;

    [Header("Vida")]
    public float vidaMaxima = 100f; // Vida máxima do jogador
    private float vidaAtual; // Vida atual do jogador

    [Header("Dano")]
    public float danoBase = 20f; 
    public float invicibilityTime = 0.5f; // Tempo que o player fica imune após tomar dano
    private bool isInvincible = false; // Controla se o player está invencivel no momento 

    [Header("Interação com parede")]
    public Transform wallCheck; // Um objeto vazio na frente do player
    public LayerMask wallLayer;          // Layer das paredes
    public float wallSlidingSpeed = 2f;  // Velocidade que ele escorrega na parede
    public Vector2 wallJumpForce = new Vector2(5f, 10f); // Força X e Y do pulo
    private bool isWallTouch;
    private bool isWallSliding;

    [Header("Checagem do chão")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    private bool isGrounded;

    [Header("UI")]
    public TMP_Text lifeText; // Componente de texto para mostrar a vida
    public BarraDeVida barra; // Referência ao script da barra de vida (Slider)

    [Header("Inventário de Buffs")]
    // Lista contendo os itens que o jogador coletou e estão ativos
    public List<ItemData> itensEquipados = new List<ItemData>(); 
    private PlayerAttack playerAttack;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Inicializa a vida do player com o valor máximo (do Script 1)
        playerAttack = GetComponent<PlayerAttack>(); // Garante o acesso ao dano

        vidaAtual = vidaMaxima;

        if (barra != null)
        {
            barra.ColocarVidaMaxima(vidaMaxima);
        }

        UpdateLifeUI();
    }

    // FUNÇÃO QUE O ITEMCOLETAVEL VAI CHAMAR
    public void ColetarItem(ItemData item)
    {
        itensEquipados.Add(item);
        Debug.Log($"Coletou: {item.nomeItem} ({item.raridade})");

        // Aplica o efeito baseado no tipo do item
        switch (item.tipoItem)
        {
            case TipoItem.FragmentoSombrio:
                Curar(30f);
                break;

            case TipoItem.MedalhaoFuriaAntiga:
                // Aumenta o dano base de 20 para 25 (ou simplesmente soma +5)
                if (playerAttack != null) playerAttack.damage += 5;
                break;

            case TipoItem.RelicarioIra:
                // Diminui o cooldown do dash de 3f para 1.5f
                dashCooldown = 1.5f;
                break;

            case TipoItem.AmuletoGuerreiroAntigo:
                // A lógica condicional será checada direto no script PlayerAttack (veja abaixo)
                break;

            case TipoItem.AnelEclipse:
                // Sobe a vida máxima de 100 para 130
                vidaMaxima += 30f;
                if (barra != null) barra.ColocarVidaMaxima(vidaMaxima);

                // Cura o jogador em 30 pelo aumento da vida máxima (opcional)
                Curar(30f);

                // Fica com 30 de ataque base (se o dano base era 20, soma +10)
                if (playerAttack != null) playerAttack.damage += 10;
                break;
        }

        UpdateLifeUI();
    }

    // Função auxiliar para curar o jogador
    public void Curar(float quantidade)
    {
        vidaAtual += quantidade;
        if (vidaAtual > vidaMaxima) vidaAtual = vidaMaxima;

        if (barra != null) barra.AlterarVida(vidaAtual);
        UpdateLifeUI();
    }

    // Função auxiliar para o script de ataque saber se a vida está cheia
    public bool TemVidaCheia()
    {
        return vidaAtual >= vidaMaxima;
    }

    // Função auxiliar para verificar se o player tem um item específico equipado
    public bool TemItemEquipado(TipoItem tipo)
    {
        return itensEquipados.Exists(i => i.tipoItem == tipo);
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        isWallTouch = Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);

        // Lógica do Coyote Time
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime; // Reseta o contador enquanto estiver no chão
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Começa a diminuir quando sai do chão
        }

        // Lógica para deslizar na parede
        if (isWallTouch && !isGrounded && moveInput != 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }

        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }

    }

    void FixedUpdate()
    {
        // Se estiver no meio do dash, não executa a lógica de movimento normal
        if (isDashing) return;

        float targetVelocityX = moveInput * speed * currentRunValue;
        float lerpAmount = (Mathf.Abs(moveInput) > 0.01f) ? acceleration : deceleration;
        float newX = Mathf.Lerp(rb.linearVelocity.x, targetVelocityX, lerpAmount * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(newX, rb.linearVelocity.y);

        // Limitador de velocidade 
        if (limitarVelocidadeManual)
        {
            // Vai calcular a velocidade máxima, multiplicando a velocidade normal base pelo de corrida
            float maxVelocidadeAtual = speed * currentRunValue;
            // Evita que a velocidade do eixo X exceda o limite da velocidade.
            float xLimitado = Mathf.Clamp(rb.linearVelocity.x, -maxVelocidadeAtual, maxVelocidadeAtual);
            // Limita a velocidade do eixo X, mantendo a velocidade vertical intacta 
            rb.linearVelocity = new Vector2(xLimitado, rb.linearVelocity.y);
        }

        if (isWallSliding)
        {
            // Se a velocidade de queda (y) for maior que a velocidade de slide, limita ela
            if (rb.linearVelocity.y < -wallSlidingSpeed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, -wallSlidingSpeed);
            }
        }
    }

    // FUNÇÃO NOVA: O inimigo vai chamar isso aqui
    public void TomarDano(float quantidadeDano)
    {
        // Se estiver invencível, ignora o dano
        if (isInvincible) return;

        vidaAtual -= quantidadeDano;

        // Garante que a vida não fique abaixo de zero
        if (vidaAtual < 0) vidaAtual = 0;

        // Atualiza os elementos visuais
        barra.AlterarVida(vidaAtual);
        UpdateLifeUI();

        if (vidaAtual <= 0)
        {
            Morrer();
        }
        else
        {
            // Ativa rotina de invencibilidade temporária
            StartCoroutine(GerenciarInvencibilidade());
        }
    }

    System.Collections.IEnumerator GerenciarInvencibilidade()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invicibilityTime);
        isInvincible = false;
    }

    void Morrer()
    {
        Debug.Log("O Player Morreu!");
        // Adicione sua lógica de Game Over aqui
        gameObject.SetActive(false);
    }


    // MECÂNICA DE PULO
    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            if (coyoteTimeCounter > 0f)
            {
                Jump();
                coyoteTimeCounter = 0f;
            }
            else if (isWallSliding)
            {
                WallJump();
            }
        }
    }

    // MECÂNICA DE VIDA
    void UpdateLifeUI()
    {
        if (lifeText != null)
        {
            lifeText.text = "Vida: " + vidaAtual;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;

        // Inverte a escala X do Player (isso vira ele e o attackPoint juntos!)
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
    }

    void WallJump()
    {
        // Empurra para o lado oposto (facingDirection * -1) e para cima
        rb.linearVelocity = new Vector2(-facingDirection * wallJumpForce.x, wallJumpForce.y);

    }


    // MECÂNICA DE MOVIMENTO
    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();
        moveInput = input.x;
        if (moveInput > 0)
        {
            facingDirection = 1f;
        }
        else if (moveInput < 0)
        {
            facingDirection = -1f;
        }

        // Inverter o WallCheck para o lado que o player está olhando
        wallCheck.localPosition = new Vector3(Mathf.Abs(wallCheck.localPosition.x) * facingDirection, wallCheck.localPosition.y, 0);
    }


    // MECÂNICA DE CORRIDA
    public void OnRun(InputValue value)
    {
        // Se o valor for maior que 0, o botão está sendo segurado
        if (value.isPressed)
        {
            currentRunValue = runMultiplier;
            Debug.Log("Posso correr");
        }
        else
        {
            currentRunValue = 1f;
            Debug.Log("Não posso correr");
        }
    }


    // MECÂNICA DE DASH
    public void OnSprint(InputValue value)
    {
        if (value.isPressed && canDash)
        {
            StartCoroutine(ExecuteDash());
        }
    }

    IEnumerator ExecuteDash()
    {
        canDash = false; // Fica falsa ao começar
        isDashing = true;

        // Aplica a força do Dash na direção que está olhando
        rb.linearVelocity = new Vector2(facingDirection * dashForce, rb.linearVelocity.y);
        // Espera o tempo de "impulso" terminar
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;

        // Espera o tempo de recarga de 3 segundos
        yield return new WaitForSeconds(dashCooldown);
        canDash = true; // Volta a ser verdadeira
    }
}

