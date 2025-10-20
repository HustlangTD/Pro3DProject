using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{

    public Camera playerCamera;
    //shooting
    public bool isshooting, readyToShoot;
    bool allowreset = true;
    public float shootingdelay = 2f;
    //Burst
    public int bulletsPerBurst = 3;
    public int burstBulletLeft;

    //Spread
    public float spreadIntensity;

    //Bullet
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f;
    public enum shootingMode { Single, Burst, Auto };

    public shootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletLeft = bulletsPerBurst;
    }


    void Update()
    {
        if (currentShootingMode == shootingMode.Auto)
        {
            //Holding left mouse button 
            isshooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if (currentShootingMode == shootingMode.Single || currentShootingMode == shootingMode.Burst)
        {
            //Pressing left mouse button
            isshooting = Input.GetKeyDown(KeyCode.Mouse0);
        }
        if (readyToShoot && isshooting)
        {
            burstBulletLeft = bulletsPerBurst;
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        // readyToShoot = false;
        // Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;


        // GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);
        // //pointing the bullet to face the 
        // bullet.transform.forward = shootingDirection;

        // bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        // StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        // if (allowreset)
        // {
        //     Invoke("ResetShot", shootingdelay);
        //     allowreset = false;
        // }

        // //Burst Mode
        // if (currentShootingMode == shootingMode.Burst && burstBulletLeft > 1)
        // {
        //     burstBulletLeft--;
        //     Invoke("FireWeapon", shootingdelay);
        // }
        readyToShoot = false;

        // 1. Tính hướng bắn từ camera + spread
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // 2. Tạo viên đạn theo hướng của camera luôn, không Quaternion.identity
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation(shootingDirection));

        // 3. Bắn viên đạn
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = shootingDirection * bulletVelocity;

        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        // 4. Xử lý delay + burst
        if (allowreset)
        {
            Invoke(nameof(ResetShot), shootingdelay);
            allowreset = false;
        }

        if (currentShootingMode == shootingMode.Burst && burstBulletLeft > 1)
        {
            burstBulletLeft--;
            Invoke(nameof(FireWeapon), shootingdelay);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowreset = true;
    }
    
    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000); // A far point in the direction of the ray
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        return direction + new Vector3(x, y, 0);
    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}