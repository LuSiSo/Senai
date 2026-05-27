using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Variável que guarda a referência X, Y e Z da posição do target que vamos seguir, ou seja, nosso player.
    public Transform target;
    // A velocidade de suavização do movimento da câmera.
    // Valor maior: câmera acompanha mais rápido.
    // Valor menor: câmera acompanha mais suavemente.
    public float smoothSpeed = 5f;
    // A distância entre a câmera e o alvo.
    public Vector3 offset;

    [Header("Eixos que a câmera acompanha")]
    // Define se a câmera vai seguir o eixo X.
    public bool followX = true;
    // Define se a câmera vai seguir o eixo Y.
    public bool followY = false;

    void Start()
    {

    }  

    // Update is called once per frame
    void Update()
    {

    }

    private void LateUpdate()
    {
        // Verifica se o target (ou seja, o Player) está vazio. Se estiver, a função é encerrada, para evitar erros de referência nula.
        if (target == null) return;

        // Estamos criando uma variáel chamada desiredPosition que armazena a posição atual da câmera.
        Vector3 desiredPosition = transform.position;

        if (followX)
        {
            desiredPosition.x = target.position.x + offset.x;
        }

        if (followY)
        {
            desiredPosition.y = target.position.y + offset.y;
        }

        // O Eixo Z é fixo, ou seja, a câmera não vai seguir o alvo no Eixo Z.
        desiredPosition.z = transform.position.z; // A posição Z da câmera é definida pelo offset, para manter a distância correta do alvo.

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }
}
