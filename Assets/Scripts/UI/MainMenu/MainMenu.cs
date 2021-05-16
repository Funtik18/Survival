using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    [SerializeField] private GameObject rabbit;
    [SerializeField] private GameObject wolf;
    [SerializeField] private GameObject loader;
    [SerializeField] private TMPro.TextMeshProUGUI loadText;
    [Space]
    [SerializeField] private MenuPage main;
    [SerializeField] private SurvivalModePage survivalMode;
    [SerializeField] private MenuPage chooseGender;
    [SerializeField] private MenuPage difficult;
    [Space]
    [SerializeField] private Button buttonBack;

    private MenuPage currentPage;

    private List<MenuPage> pages = new List<MenuPage>();
    private List<MenuPage> nest = new List<MenuPage>();

    private List<Data> datas;

    private void Awake()
    {
        pages.Add(main);
        pages.Add(survivalMode);
        pages.Add(chooseGender);
        pages.Add(difficult);

        for (int i = 0; i < pages.Count; i++)
        {
            pages[i].onOpened += AddNest;
            pages[i].onClossed += RemoveNest;
        }

        datas = SaveLoadManager.GetAllSaves();

        survivalMode.EnableContinueButton(datas.Count != 0);

        currentPage = main;
    }

    public void NewGame()
    {
        DataHolder.loadType = LoadType.NewGame;
        LoadGame();
    }
    public void Continue()
    {
        DataHolder.loadType = LoadType.Continue;

        DataHolder.Data = datas[datas.Count - 1];

        LoadGame();
    }

    public void CreatePlayer()
    {
        if (DataHolder.Data == null) { }
    }
    public void IsMale()
    {
        DataHolder.Data.playerData.statusData.gender = Gender.Male;
    }
    public void IsFemale()
    {
        DataHolder.Data.playerData.statusData.gender = Gender.Female;
    }
    public void IsNormal()
    {
        DataHolder.Data.difficult = Difficult.Normal;
    }

    private void LoadGame()
    {
        gameObject.SetActive(false);

        loader.SetActive(true);

        rabbit.SetActive(false);
        wolf.SetActive(true);

        ScenesManager.Instance.SetupLoad(progress : UpdateProgress, completedLoad : AllowLoad, 1.25f).LoadGameScene();
    }

    private void AllowLoad()
    {
        ScenesManager.Instance.Allow();
    }

    private void UpdateProgress(float progress)
    {
        loadText.text = (int)(progress * 100f) + "%";
    }

    private void AddNest(MenuPage page)
    {
        if (currentPage)
            currentPage.Close();

        currentPage = page;

        nest.Add(currentPage);

        buttonBack.onClick.RemoveAllListeners();
        buttonBack.onClick.AddListener(currentPage.ClosePage);

        buttonBack.gameObject.SetActive(true);
    }
    private void RemoveNest(MenuPage page)
    {
        nest.Remove(page);

        buttonBack.onClick.RemoveAllListeners();

        if (nest.Count > 0)
        {
            currentPage = nest[nest.Count - 1];
            currentPage.Open();

            buttonBack.onClick.AddListener(currentPage.ClosePage);
        }
        else
        {
            currentPage = main;
            currentPage.Open();

            buttonBack.gameObject.SetActive(false);
        }
    }
}