using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int ammoAmount = 200;
    public AmmpType ammotype;

    public enum AmmpType
    {
        PistolAmmo,
        M4A1Ammo
    }
}
