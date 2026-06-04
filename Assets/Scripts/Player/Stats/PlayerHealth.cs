using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TomarDano(float dano)
    {
        currentHealth -= dano;

        Debug.Log($"Player tomou {dano} de dano. Vida: {currentHealth}");

        if (currentHealth <= 0)
        {
            Morrer();
        }
    }

    void Morrer()
    {
        Debug.Log("Player morreu!");
    }
}
