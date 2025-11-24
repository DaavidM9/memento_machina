using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;

public class AttackArea : MonoBehaviour
{
    private float damageAfterTime = 0.05f;
    private List<IDamageable> EnemiesInRange { get; } = new();
    public Animator animator;
    

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<IDamageable>();
            EnemiesInRange.Add(enemy);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<IDamageable>();
            EnemiesInRange.Remove(enemy);
        }
    }
    public IEnumerator TriggerAttack(int dmg)
    {
        yield return new WaitForSeconds(damageAfterTime);

        List<IDamageable> enemiesToDamage = new List<IDamageable>(EnemiesInRange);

        foreach (var enemy in enemiesToDamage)
        {
            enemy.TakeDamage(dmg, this.transform.position);
            OrbScript.Instance.Chargebattery(7.5f);
        }

    }

    public void ToggleFacing()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            transform.SetLocalPositionAndRotation(new Vector2(-0.005f, 0.12f), Quaternion.Euler(0, 0, 90));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            transform.SetLocalPositionAndRotation(new Vector2(-0.005f, -0.12f), Quaternion.Euler(0, 0, -90));
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.S))
        {
            transform.SetLocalPositionAndRotation(new Vector2(-0.005f, 0), Quaternion.Euler(0, 0, 0));
        }
    }

    public void TriggerSlash()
    {
        animator.SetTrigger("Slash");
    }
}
