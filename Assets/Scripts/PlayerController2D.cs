using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/// <summary>
/// Controlador 2D do jogador com movimentação horizontal, salto,
/// detecção de chão e simulação física via Rigidbody2D e Collider2D.
/// Compatível com o Unity 6 e o New Input System.
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerController2D : MonoBehaviour
{
    [Header("Movimentação")]
    [Tooltip("Velocidade horizontal de caminhada/corrida.")]
    [SerializeField] private float moveSpeed = 6f;

    [Tooltip("Força do salto vertical.")]
    [SerializeField] private float jumpForce = 10f;

    [Header("Detecção de Solo")]
    [Tooltip("Ponto de origem para a verificação de chão.")]
    [SerializeField] private Transform groundCheck;

    [Tooltip("Raio do círculo de verificação de chão.")]
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Tooltip("Camadas consideradas como solo sólido/plataformas.")]
    [SerializeField] private LayerMask groundLayer = ~0;

    private Rigidbody2D rb;
    private Collider2D col;
    private Rigidbody2D currentPlatformRb;
    private bool isGrounded;
    private float horizontalInput;
    private bool jumpRequested;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // Configurações do Rigidbody2D do jogador
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        if (groundCheck == null)
        {
            // Se não houver objeto filho atribuído para ground check, cria um dinamicamente
            GameObject gc = new GameObject("GroundCheck");
            gc.transform.SetParent(transform);
            gc.transform.localPosition = new Vector3(0f, -0.05f, 0f);
            groundCheck = gc.transform;
        }
    }

    private void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
    {
        CheckGrounded();
        ApplyMovement();
    }

    private void ReadInput()
    {
        horizontalInput = 0f;

        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                horizontalInput -= 1f;
            }
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                horizontalInput += 1f;
            }

            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                jumpRequested = true;
            }
        }
    }

    private void CheckGrounded()
    {
        currentPlatformRb = null;
        if (groundCheck != null)
        {
            // Ignora o collider do próprio jogador no check
            Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius, groundLayer);
            isGrounded = false;
            foreach (var c in colliders)
            {
                if (c != col && !c.isTrigger)
                {
                    isGrounded = true;
                    if (c.attachedRigidbody != null)
                    {
                        currentPlatformRb = c.attachedRigidbody;
                    }
                    break;
                }
            }
        }
    }

    private void ApplyMovement()
    {
        // Aplica velocidade horizontal mantendo a velocidade vertical atual da física.
        // Se estiver sobre uma plataforma móvel com Rigidbody2D, acompanha seu movimento perfeitamente.
        Vector2 currentVel = rb.linearVelocity;
        float platformVelX = (currentPlatformRb != null) ? currentPlatformRb.linearVelocity.x : 0f;
        rb.linearVelocity = new Vector2(horizontalInput * moveSpeed + platformVelX, currentVel.y);

        if (jumpRequested)
        {
            jumpRequested = false;
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
