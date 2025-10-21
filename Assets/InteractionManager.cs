using Unity.VisualScripting;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; set; }

    public Weapon hoveredWeapon = null;
    public AmmoBox hoveredAmmoBox = null;

    public Throwable hoveredThrowable = null;


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
    private void Update()
{
    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit))
    {
        Debug.Log("Ray hit: " + hit.collider.gameObject.name);
        GameObject objectHitByRaycast = hit.transform.gameObject;
        
        //  Dùng GetComponentInParent
        Weapon weaponInParent = objectHitByRaycast.GetComponentInParent<Weapon>();

            if (weaponInParent != null) // Nếu tìm thấy Weapon script ở cha hoặc chính nó
            {
                // Nếu ta đang nhìn vào một khẩu súng mới, hãy tắt outline của khẩu súng cũ
                if (hoveredWeapon != null && hoveredWeapon != weaponInParent)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false;
                }

                hoveredWeapon = weaponInParent;
                hoveredWeapon.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    WeaponManager.Instance.PickupWeapon(objectHitByRaycast.gameObject);
                }
            }
            else // Nếu không nhìn vào súng nữa
            {
                if (hoveredWeapon)
                {
                    hoveredWeapon.GetComponent<Outline>().enabled = false;
                    hoveredWeapon = null; // Reset hoveredWeapon
                }
            }
            //AmmoBox
            if (objectHitByRaycast.GetComponent<AmmoBox>())
            {
                hoveredAmmoBox = objectHitByRaycast.gameObject.GetComponent<AmmoBox>();
                hoveredAmmoBox.GetComponent<Outline>().enabled = true;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    WeaponManager.Instance.PickupAmmo(hoveredAmmoBox);
                    Destroy(objectHitByRaycast.gameObject);
                }
            }
            else
            {
                if (hoveredAmmoBox)
                {
                    hoveredAmmoBox.GetComponent<Outline>().enabled = false;

                }
            }
            //Throwable
            if (objectHitByRaycast.GetComponent<Throwable>())
            {
                hoveredThrowable = objectHitByRaycast.gameObject.GetComponent<Throwable>();
                hoveredThrowable.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(KeyCode.E))
                {
                    WeaponManager.Instance.PickupThrowable(hoveredThrowable);
                    Destroy(objectHitByRaycast.gameObject);
                }
            }
            else
            {
                if (hoveredThrowable)
                {
                    hoveredThrowable.GetComponent<Outline>().enabled = false;
                    
                }
            }
    }
    else // Nếu raycast không trúng bất cứ thứ gì (nhìn ra trời)
    {
        if (hoveredWeapon)
        {
            hoveredWeapon.GetComponent<Outline>().enabled = false;
            hoveredWeapon = null; // Reset hoveredWeapon
        }
    }
}
    
    // private void Update()
    // {
    //     Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    //     RaycastHit hit;
    //     if (Physics.Raycast(ray, out hit))
    //     {
    //         GameObject objectHitByRaycast = hit.transform.gameObject;
    //         if (objectHitByRaycast.GetComponent<Weapon>())
    //         {
    //             hoveredWeapon = objectHitByRaycast.gameObject.GetComponent<Weapon>();
    //             hoveredWeapon.GetComponent<Outline>().enabled = true;
    //         }
    //         else
    //         {
    //             if (hoveredWeapon)
    //             {
    //                 hoveredWeapon.GetComponent<Outline>().enabled = false;
                    
    //             }
    //         }
    //     }
        
    // }
}
