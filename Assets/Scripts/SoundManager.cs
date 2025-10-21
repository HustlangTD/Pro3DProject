using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }

    public AudioSource shootingShoundPistol;
    public AudioSource reloadingShoundPistol;

    public AudioClip M4A1Shot;

    public AudioSource shootingShoundM4A1;

    public AudioSource emptyMagazineShoundPistol;

    public AudioClip zombieWalking;
    public AudioClip zombieChase;
    public AudioClip zombieAttack;
    public AudioClip zombieHurt;
    public AudioClip zombieDeath;

    public AudioSource zombieChannel;

    public AudioSource zombieChannel2;

    public AudioSource playerChannel;
    public AudioClip playerHurt;
    public AudioClip playerDie;

    public AudioClip gameOverMusic;







    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;

        }

    }

    public void playShootingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol:
                shootingShoundPistol.Play();
                break;
            case WeaponModel.M4A1:
                shootingShoundM4A1.PlayOneShot(M4A1Shot);
                break;
        }
    }
    public void playReloadingSound(WeaponModel weapon)
    {
        switch (weapon)
        {
            case WeaponModel.Pistol:
                reloadingShoundPistol.Play(); 
                break;
            case WeaponModel.M4A1:
                reloadingShoundPistol.Play();
                break;
        }
    }
    
    
}
