using System.Collections;
using UnityEngine;

public class lighting : MonoBehaviour
{
    public Light lightning;
    public AudioClip thunderSound;// tieng sam
    public AudioSource audioSource;

    public float minTimeBetweenStrikes = 5f; //thoi gian toi thieu giua cac lan sam
    public float maxTimeBetweenStrikes = 25f; //thoi gian toi da giua cac lan sam
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Inlighting());
    }

    // Update is called once per frame
    IEnumerator Inlighting()
    {
        while (true)
        {
            var waitTime = Random.Range(minTimeBetweenStrikes, maxTimeBetweenStrikes);
            yield return new WaitForSeconds(waitTime);
            StartCoroutine(MakeLighting());
        }
    }

    IEnumerator MakeLighting()
    {
        var originalIntensity = lightning.intensity;
        lightning.intensity = Random.Range(6f, 8f);
        audioSource.clip = thunderSound;
        audioSource.Play();
        yield return new WaitForSeconds(Random.Range(0.3f, 0.5f));
        lightning.intensity = originalIntensity;

    }
}
