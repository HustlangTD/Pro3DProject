using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace SlimUI.ModernMenu
{
    public class UIMenuManager : MonoBehaviour
    {
        private Animator CameraObject;

        [Header("MENUS")]
        public GameObject mainMenu;
        public GameObject firstMenu;
        public GameObject playMenu;
        public GameObject exitMenu;
        public GameObject extrasMenu;

        public enum Theme { custom1, custom2, custom3 };
        [Header("THEME SETTINGS")]
        public Theme theme;
        public ThemedUIData themeController;

        [Header("PANELS")]
        public GameObject mainCanvas;
        public GameObject PanelControls;
        public GameObject PanelVideo;
        public GameObject PanelGame;
        public GameObject PanelKeyBindings;
        public GameObject PanelMovement;
        public GameObject PanelCombat;
        public GameObject PanelGeneral;

        [Header("SETTINGS SCREEN")]
        public GameObject lineGame;
        public GameObject lineVideo;
        public GameObject lineControls;
        public GameObject lineKeyBindings;
        public GameObject lineMovement;
        public GameObject lineCombat;
        public GameObject lineGeneral;

        [Header("LOADING SCREEN")]
        public bool waitForInput = true;
        public GameObject loadingMenu;
        public Slider loadingBar;
        public TMP_Text loadPromptText;
        public KeyCode userPromptKey = KeyCode.Space;

        [Header("SFX")]
        public AudioSource hoverSound;
        public AudioSource sliderSound;
        public AudioSource swooshSound;

        private void Start()
        {
            CameraObject = GetComponent<Animator>();

            if (playMenu) playMenu.SetActive(false);
            if (exitMenu) exitMenu.SetActive(false);
            if (extrasMenu) extrasMenu.SetActive(false);
            if (firstMenu) firstMenu.SetActive(true);
            if (mainMenu) mainMenu.SetActive(true);

            SetThemeColors();
        }

        // =================== THEMES ===================
        void SetThemeColors()
        {
            if (themeController == null)
            {
                Debug.LogWarning("ThemeController chưa được gán!");
                return;
            }

            switch (theme)
            {
                case Theme.custom1:
                    themeController.currentColor = themeController.custom1.graphic1;
                    themeController.textColor = themeController.custom1.text1;
                    break;
                case Theme.custom2:
                    themeController.currentColor = themeController.custom2.graphic2;
                    themeController.textColor = themeController.custom2.text2;
                    break;
                case Theme.custom3:
                    themeController.currentColor = themeController.custom3.graphic3;
                    themeController.textColor = themeController.custom3.text3;
                    break;
            }
        }

        // =================== MENU CONTROL ===================
        public void PlayCampaign()
        {
            exitMenu?.SetActive(false);
            extrasMenu?.SetActive(false);
            playMenu?.SetActive(true);
        }

        public void PlayCampaignMobile()
        {
            exitMenu?.SetActive(false);
            extrasMenu?.SetActive(false);
            playMenu?.SetActive(true);
            mainMenu?.SetActive(false);
        }

        public void ReturnMenu()
        {
            playMenu?.SetActive(false);
            extrasMenu?.SetActive(false);
            exitMenu?.SetActive(false);
            mainMenu?.SetActive(true);
        }

        public void DisablePlayCampaign()
        {
            playMenu?.SetActive(false);
        }

        public void Position2()
        {
            DisablePlayCampaign();
            if (CameraObject) CameraObject.SetFloat("Animate", 1);
        }

        public void Position1()
        {
            if (CameraObject) CameraObject.SetFloat("Animate", 0);
        }

        // =================== PANEL HANDLING ===================
        void DisablePanels()
        {
            PanelControls?.SetActive(false);
            PanelVideo?.SetActive(false);
            PanelGame?.SetActive(false);
            PanelKeyBindings?.SetActive(false);
            lineGame?.SetActive(false);
            lineControls?.SetActive(false);
            lineVideo?.SetActive(false);
            lineKeyBindings?.SetActive(false);
            PanelMovement?.SetActive(false);
            lineMovement?.SetActive(false);
            PanelCombat?.SetActive(false);
            lineCombat?.SetActive(false);
            PanelGeneral?.SetActive(false);
            lineGeneral?.SetActive(false);
        }

        public void GamePanel()
        {
            DisablePanels();
            PanelGame?.SetActive(true);
            lineGame?.SetActive(true);
        }

        public void VideoPanel()
        {
            DisablePanels();
            PanelVideo?.SetActive(true);
            lineVideo?.SetActive(true);
        }

        public void ControlsPanel()
        {
            DisablePanels();
            PanelControls?.SetActive(true);
            lineControls?.SetActive(true);
        }

        public void KeyBindingsPanel()
        {
            DisablePanels();
            MovementPanel();
            PanelKeyBindings?.SetActive(true);
            lineKeyBindings?.SetActive(true);
        }

        public void MovementPanel()
        {
            DisablePanels();
            PanelKeyBindings?.SetActive(true);
            PanelMovement?.SetActive(true);
            lineMovement?.SetActive(true);
        }

        public void CombatPanel()
        {
            DisablePanels();
            PanelKeyBindings?.SetActive(true);
            PanelCombat?.SetActive(true);
            lineCombat?.SetActive(true);
        }

        public void GeneralPanel()
        {
            DisablePanels();
            PanelKeyBindings?.SetActive(true);
            PanelGeneral?.SetActive(true);
            lineGeneral?.SetActive(true);
        }

        // =================== SOUND ===================
        public void PlayHover() => hoverSound?.Play();
        public void PlaySFXHover() => sliderSound?.Play();
        public void PlaySwoosh() => swooshSound?.Play();

        // =================== EXIT / EXTRA ===================
        public void AreYouSure()
        {
            exitMenu?.SetActive(true);
            extrasMenu?.SetActive(false);
            DisablePlayCampaign();
        }

        public void AreYouSureMobile()
        {
            exitMenu?.SetActive(true);
            extrasMenu?.SetActive(false);
            mainMenu?.SetActive(false);
            DisablePlayCampaign();
        }

        public void ExtrasMenu()
        {
            playMenu?.SetActive(false);
            extrasMenu?.SetActive(true);
            exitMenu?.SetActive(false);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        // =================== LOAD SCENE ===================
        public void LoadScene(string sceneName)
        {
            if (!string.IsNullOrEmpty(sceneName))
            {
                // 🟢 Dùng chế độ SINGLE để xóa hoàn toàn scene cũ, tránh trùng EventSystem
                SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            }
        }
    }
}
