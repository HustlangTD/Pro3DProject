using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.InputSystem;


public class Weapon : MonoBehaviour

{
    // public Camera playerCamera;

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

    public GameObject muzzleEffect;

    //Reloading
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public Vector3 spawnPosition;
    public Vector3 spawnRotation;
    
    public enum WeaponModel
    {
        Pistol, M4A1
        
    }

    public WeaponModel thisWeaponModel;

    private Animator animator;

    



    public enum shootingMode { Single, Burst, Auto };

    public shootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;
    }


    void Update()
    {
        if (bulletsLeft == 0 && isshooting)
        {
            SoundManager.Instance.emptyMagazineShoundPistol.Play();
        }


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

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false)
        {
            Reload();

        } 
        if (readyToShoot && isshooting == false && isReloading == false && bulletsLeft <= 0)
        {
            Reload();
            
        }


        if (readyToShoot && isshooting && !isReloading && bulletsLeft > 0)
        {
            burstBulletLeft = bulletsPerBurst;
            FireWeapon();
        }

        if (AmmoManager.Instance.ammoDisplay != null)
        {
            AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft / bulletsPerBurst}/{magazineSize / bulletsPerBurst}";
        }   

    }

    private void FireWeapon()
    {
        bulletsLeft--;

        muzzleEffect.GetComponent<ParticleSystem>().Play();
        animator.SetTrigger("RECOIL");
        
        SoundManager.Instance.playShootingSound(thisWeaponModel);
        
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

    private void Reload()
    {
        
        SoundManager.Instance.playReloadingSound(thisWeaponModel);

        animator.SetTrigger("RELOAD");

        isReloading = true;
        Invoke("ReloadCompleted", reloadTime);
    }
    private void ReloadCompleted()
    {
        bulletsLeft = magazineSize;
        isReloading = false;
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowreset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
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
// using UnityEngine;
// using System.Collections;

// public class Weapon : MonoBehaviour
// {
//     // Shooting
//     public bool isshooting, readyToShoot;
//     public float shootingdelay = 0.2f;

//     // Burst
//     public int bulletsPerBurst = 3;
//     private int burstBulletLeft;

//     // Spread
//     public float spreadIntensity = 0.1f;

//     // Bullet
//     public GameObject bulletPrefab;
//     public Transform bulletSpawn;
//     public float bulletVelocity = 30f;
//     public float bulletPrefabLifeTime = 3f;

//     public enum shootingMode { Single, Burst, Auto };
//     public shootingMode currentShootingMode;

//     private void Awake()
//     {
//         readyToShoot = true;
//         burstBulletLeft = bulletsPerBurst;
//     }

//     void Update()
//     {
//         if (currentShootingMode == shootingMode.Auto)
//             isshooting = Input.GetKey(KeyCode.Mouse0);
//         else
//             isshooting = Input.GetKeyDown(KeyCode.Mouse0);

//         if (readyToShoot && isshooting)
//         {
//             burstBulletLeft = bulletsPerBurst;
//             FireWeapon();
//         }
//     }

//     private void FireWeapon()
//     {
//         readyToShoot = false;

//         Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

//         GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.LookRotation(shootingDirection));

//         Rigidbody rb = bullet.GetComponent<Rigidbody>();
//         if (rb != null)
//             rb.linearVelocity = shootingDirection * bulletVelocity;

//         StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

//         // Reset shot sau delay
//         Invoke(nameof(ResetShot), shootingdelay);

//         // Burst mode
//         if (currentShootingMode == shootingMode.Burst && burstBulletLeft > 1)
//         {
//             burstBulletLeft--;
//             Invoke(nameof(FireWeapon), shootingdelay);
//         }
//     }

//     private void ResetShot()
//     {
//         readyToShoot = true;
//     }

//     private Vector3 CalculateDirectionAndSpread()
//     {
//         Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
//         Vector3 targetPoint;
//         if (Physics.Raycast(ray, out RaycastHit hit))
//             targetPoint = hit.point;
//         else
//             targetPoint = ray.GetPoint(1000);

//         Vector3 direction = targetPoint - bulletSpawn.position;
//         float x = Random.Range(-spreadIntensity, spreadIntensity);
//         float y = Random.Range(-spreadIntensity, spreadIntensity);
//         return direction + new Vector3(x, y, 0);
//     }

//     private IEnumerator DestroyBulletAfterTime(GameObject bullet, float delay)
//     {
//         yield return new WaitForSeconds(delay);
//         Destroy(bullet);
//     }
// }
