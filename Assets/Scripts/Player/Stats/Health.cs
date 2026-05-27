using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI; // Necessário para controlar os componentes de UI clássicos (como Sliders)


public class BarraDeVida : MonoBehaviour
{
    public Slider slider; // Referência para o componente Slider do Canvas da Unity

    // Configura os limites máximos do Slider no início do jogo
    public void ColocarVidaMaxima(float vida)
    {
        slider.maxValue = vida; // Define o limite máximo que barra pode encher
        slider.value = vida; // Começa com a barra totalmente cheia
    }

    // Altera a posição visual da barra sempre que o jogador ganha ou perde vida
    public void AlterarVida(float vida)
    {
        slider.value = vida;  // Modifica o preenchimento atual do Slider
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
