using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public abstract class Enemy : MonoBehaviour
{

    protected SpriteRenderer sr;
    protected Animator anim;
    protected int health;

    [SerializeField] private int maxHealth = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
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

    public virtual void TakeDamage(int damageValue, DamageType damageType = DamageType.Default)
    {
        health -= damageValue;
        if (health <= 0)
        {
            anim.SetTrigger("Death");
        }
    }

}

public enum DamageType
{
    Default,
    JumpedOn
}