using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class NhanAudioSetting : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider volumeSlider;

    void Start()
    {
        // Lấy giá trị lưu trước đó nếu có
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
        volumeSlider.value = savedVolume;
        SetVolume(savedVolume);
    }

    public void SetVolume(float value)
    {
        // Chuyển giá trị (0–1) thành dB (-80 đến 0)
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(value) * 20);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }
}
