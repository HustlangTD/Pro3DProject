using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision objectWeHit)
    {
        if (objectWeHit.gameObject.CompareTag("Enemy"))
        {
            print("Hit " + objectWeHit.gameObject.name + "!");
            CreateBullerImpactEffect(objectWeHit);
            Destroy(gameObject);
            
        }
        if (objectWeHit.gameObject.CompareTag("Wall"))
        {
            print("Hit a wall");
            CreateBullerImpactEffect(objectWeHit);
            Destroy(gameObject);
        }
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
