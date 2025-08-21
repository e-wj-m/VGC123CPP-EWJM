using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class EnemyScript : MonoBehaviour
{

    protected SpriteRenderer sr;
    protected Animator anim;
    protected int health;

    [SerializeField] private int maxHealth = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void Start()
    {
       sr = GetComponent<SpriteRenderer>();
       anim = GetComponent<Animator>();

        if (maxHealth <= 0)
        {
            Debug.LogError("Max Health has to be greater than zero! Setting the default Health value to 5.");
            maxHealth = 5;
        }

        health = maxHealth;

    }

}
