using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
// using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.InputSystem;


public class Weapon : MonoBehaviour

{
    // public Camera playerCamera;
    public bool isActiveWeapon;

    public int weaponDamage;

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

    internal Animator animator;

    



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
        if (isActiveWeapon)
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

            if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && isReloading == false && WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0)
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

            // if (AmmoManager.Instance.ammoDisplay != null)
            // {
            //     AmmoManager.Instance.ammoDisplay.text = $"{bulletsLeft / bulletsPerBurst}/{magazineSize / bulletsPerBurst}";
            // }
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

        Bullet bul = bullet.GetComponent<Bullet>();
        bul.bulletDamage = weaponDamage;

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
        if (WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > magazineSize)
        {
            bulletsLeft = magazineSize;
            WeaponManager.Instance.DecreasedTotalAmmo(bulletsLeft, thisWeaponModel);
        }
        else
        {
            bulletsLeft = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
            WeaponManager.Instance.DecreasedTotalAmmo(bulletsLeft, thisWeaponModel);
        }
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

