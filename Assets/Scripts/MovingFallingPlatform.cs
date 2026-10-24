using System.Collections;
using UnityEngine;

/// <summary>
/// Controla uma plataforma 2D interativa com três estados:
/// 1. Idle: Imóvel e sem gravidade até o primeiro contato físico com o jogador.
/// 2. Moving: Desloca-se horizontalmente para a direita com velocidade constante durante 5 segundos.
/// 3. Falling: Interrompe o movimento horizontal e cai naturalmente sob ação da gravidade.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class MovingFallingPlatform : MonoBehaviour
{
    private enum PlatformState
    {
        Idle,
        Moving,
        Falling
    }

    [Header("Configurações de Movimento")]
    [Tooltip("Velocidade de deslocamento horizontal para a direita (unidades por segundo).")]
    [SerializeField] private float moveSpeed = 3f;

    [Tooltip("Duração do deslocamento horizontal em segundos antes de iniciar a queda.")]
    [SerializeField] private float movementDuration = 5f;

    [Header("Configurações de Queda")]
    [Tooltip("Intensidade da gravidade aplicada à plataforma durante o estado de queda.")]
    [SerializeField] private float fallGravityScale = 2f;

    [Header("Identificação do Jogador")]
    [Tooltip("Tag utilizada para identificar o GameObject do jogador.")]
    [SerializeField] private string playerTag = "Player";

    // Componentes e controle de estado
    private Rigidbody2D rb;
    private PlatformState currentState = PlatformState.Idle;
    private Coroutine movementCoroutine;

    private Rigidbody2D Rb
    {
        get
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody2D>();
            }
            return rb;
        }
    }

    /// <summary>
    /// Propriedade pública para consulta do estado atual (útil para testes e debug).
    /// </summary>
    public string CurrentStateName => currentState.ToString();

    /// <summary>
    /// Indica se a plataforma já foi ativada.
    /// </summary>
    public bool HasBeenActivated => currentState != PlatformState.Idle;

    /// <summary>
    /// Método público para acionamento controlado em testes unitários/validação.
    /// </summary>
    public void TriggerActivationForTest()
    {
        if (currentState == PlatformState.Idle)
        {
            StartMovingSequence();
        }
    }

    /// <summary>
    /// Método público para transição de queda em testes de validação.
    /// </summary>
    public void TriggerFallingForTest()
    {
        TransitionToFalling();
    }

    /// <summary>
    /// Reseta o estado da plataforma para o estado inicial Idle (utilizado para testes).
    /// </summary>
    public void ResetToIdleForTest()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }

        currentState = PlatformState.Idle;
        if (Rb != null)
        {
            Rb.gravityScale = 0f;
            Rb.linearVelocity = Vector2.zero;
            Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void Awake()
    {
        InitializePlatform();
    }

    private void Start()
    {
        InitializePlatform();
    }

    private void InitializePlatform()
    {
        if (Rb != null && currentState == PlatformState.Idle)
        {
            Rb.bodyType = RigidbodyType2D.Dynamic;
            Rb.gravityScale = 0f;
            Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            Rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Responde apenas ao primeiro contato e apenas se estiver no estado Idle
        if (currentState != PlatformState.Idle)
        {
            return;
        }

        // Verifica se a colisão foi com o jogador
        if (collision.gameObject.CompareTag(playerTag))
        {
            StartMovingSequence();
        }
    }

    /// <summary>
    /// Inicia a sequência de movimentação horizontal.
    /// Garante ativação única por instância da plataforma.
    /// </summary>
    private void StartMovingSequence()
    {
        currentState = PlatformState.Moving;
        if (Rb != null)
        {
            Rb.gravityScale = 0f;
        }
        
        // Inicia a corrotina responsável pelo tempo e velocidade constante
        movementCoroutine = StartCoroutine(MovementRoutine());
    }

    /// <summary>
    /// Mantém o movimento horizontal constante durante o tempo determinado
    /// e realiza a transição para a queda ao final dos 5 segundos.
    /// </summary>
    private IEnumerator MovementRoutine()
    {
        float timer = 0f;

        // Loop executado no ciclo de física para manter a precisão temporal e estabilidade física
        while (timer < movementDuration)
        {
            // Aplica velocidade constante para a direita (Unity 6 linearVelocity)
            if (Rb != null)
            {
                Rb.linearVelocity = new Vector2(moveSpeed, 0f);
            }

            yield return new WaitForFixedUpdate();
            timer += Time.fixedDeltaTime;
        }

        // Transição para o estado Falling
        TransitionToFalling();
    }

    /// <summary>
    /// Interrompe qualquer movimento horizontal e ativa a gravidade para queda natural.
    /// </summary>
    private void TransitionToFalling()
    {
        currentState = PlatformState.Falling;

        if (Rb != null)
        {
            // 1. Interrompe imediatamente qualquer velocidade horizontal ou residual
            Rb.linearVelocity = Vector2.zero;

            // 2. Ativa a gravidade com o valor positivo configurado
            Rb.gravityScale = fallGravityScale;

            // Mantém restrição de rotação Z ativa para que a plataforma caia estável
            Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
    }

    private void OnDisable()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }
    }
}
