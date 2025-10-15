using UnityEngine;

public class SUN : MonoBehaviour
{
    // hieu ung mat troi xoay vong 24h trong ...phut
    public Light sunLight;
    public float rotationSpeed = 360f / 120f; // 360do trong 2phut

    void Awake()
    {
        if (sunLight == null )
        {
            sunLight = GetComponent<Light>();
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.right, rotationSpeed * Time.deltaTime);

        //dieu chinh cuong do danh sang dua tren goc quay
        var angle = Vector3.Dot(transform.forward, Vector3.down);
        sunLight.intensity = Mathf.Clamp01(angle);

        //dieu chinh mau sac anh sang dua tren goc quay
        sunLight.color = Color.Lerp(Color.red, Color.white, Mathf.Clamp01(angle * 2));
    }
}
