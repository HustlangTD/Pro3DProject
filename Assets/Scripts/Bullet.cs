using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int bulletDamage;

    private void OnCollisionEnter(Collision objectWeHit)
    {
        if (objectWeHit.gameObject.CompareTag("Enemy"))
        {
            print("Hit " + objectWeHit.gameObject.name + "!");
            
            // Gây sát thương cho Enemy
            Enemy enemyScript = objectWeHit.gameObject.GetComponent<Enemy>();
            if (enemyScript != null && !enemyScript.isDead)
            {
                enemyScript.TakeDamage(bulletDamage);
            }

            // Tạo hiệu ứng và hủy đạn
            CreateBullerImpactEffect(objectWeHit);
            CreateBloodSprayEffect(objectWeHit);
            Destroy(gameObject);
        }
        else if (objectWeHit.gameObject.CompareTag("Wall"))
        {
            print("Hit a wall");
            CreateBullerImpactEffect(objectWeHit);
            Destroy(gameObject);
        }
        
        else if (objectWeHit.gameObject.CompareTag("Boss"))
        {
            print("Hit the BOSS!");

            // 1. Lấy component BossBeetleAI từ đối tượng va chạm
            BossBeetleAI boss = objectWeHit.gameObject.GetComponent<BossBeetleAI>();

            // 2. Nếu tìm thấy script, gọi hàm TakeDamage của Boss
            if (boss != null)
            {
                boss.TakeDamage(bulletDamage);
            }

            // 3. Tạo hiệu ứng (tương tự Enemy) và hủy đạn
            CreateBullerImpactEffect(objectWeHit);
            CreateBloodSprayEffect(objectWeHit); 
            Destroy(gameObject);
        }
    }

    private void CreateBloodSprayEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];

        GameObject bloodSprayPrefab = Instantiate(
            GlobalReferences.Instance.bloodSprayEffect,
            contact.point,
            Quaternion.LookRotation(contact.normal)

        );
        bloodSprayPrefab.transform.SetParent(objectWeHit.gameObject.transform);
    }

    void CreateBullerImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];

        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)

        );
        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }
}
