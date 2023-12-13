using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VNMenuManager : MonoBehaviour
{
    public static VNMenuManager instance;

    private MenuPage activePage = null;
    [SerializeField] private CanvasGroup root;
    private CanvasGroupManager rootCG => new CanvasGroupManager(root, this);
    [SerializeField] private MenuPage[] pages;

    public enum MenuType{GameMenu = 0, MainMenu = 1}
    public MenuType menuType;

	public bool isOpen;

    private void Awake() {
        instance = this;
    }

    private MenuPage GetPage(MenuPage.PageType pageType) {
        return pages.FirstOrDefault(page => page.pageType == pageType);
    }

    public void OpenSavePage() {
        var page = GetPage(MenuPage.PageType.SaveAndLoad);
        var slm = page.anim.GetComponentInParent<SaveAndLoadMenu>();
        OpenPage(page);
    }

    public void OpenConfigPage(){
        var page = GetPage(MenuPage.PageType.Config);
        OpenPage(page);
    }

    private void OpenPage(MenuPage page) {
        if(page == null) return;

        Debug.Log($"Opening page '{page.name}'");

        if(activePage != null && activePage != page)
            activePage.Close();

        if(activePage != page){
            page.Open();
            activePage = page;
        }

        if(!isOpen) {
            OpenRoot();
        }
    }

    private void OpenRoot() {
        rootCG.Show();
        root.blocksRaycasts = true;
        root.interactable = true;
        isOpen = true;
    }

    public void CloseRoot() {
        rootCG.Hide();
        root.blocksRaycasts = false;
        root.interactable = false;
        isOpen = false;

        if(activePage != null){
            activePage.Close();
            activePage = null;
        }
    }

    public void OnHome() {
        VNConfiguration.activeConfig.Save();
        SceneManager.LoadScene("Menu");
    }

    public void OnQuit() {
        VNConfiguration.activeConfig.Save();
        Application.Quit();
    }
}
