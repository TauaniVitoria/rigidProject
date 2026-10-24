using UnityEngine;

/// <summary>
/// Acompanha suavemente o alvo (normalmente o jogador) em uma cena 2D ortográfica.
/// </summary>
public class CameraFollow2D : MonoBehaviour
{
    [Header("Alvo")]
    [Tooltip("Transform do jogador ou objeto a ser seguido pela câmera.")]
    [SerializeField] private Transform target;

    [Header("Configurações de Suavização")]
    [Tooltip("Tempo aproximado para alcançar o alvo.")]
    [SerializeField] private float smoothTime = 0.2f;

    [Tooltip("Deslocamento relativo ao alvo (offset).")]
    [SerializeField] private Vector3 offset = new Vector3(2f, 1.5f, -10f);

    private Vector3 currentVelocity = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        desiredPosition.z = offset.z; // Mantém a profundidade fixa da câmera ortográfica

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
