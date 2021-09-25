using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    public GameObject LevelsUI;
    public GameObject StartMenu;
    public GameObject HowToPlayMenu;


    private GameController controller;

    private void Start()
    {
        controller = FindObjectOfType<GameController>();
    }

    public void ShowLevelUI()
    {
        LevelsUI.SetActive(true);
        HighlightChildren(controller.currentLevel - 1);
        StartMenu.SetActive(false);
    }

    public void ShowStartMenu()
    {
        HowToPlayMenu.SetActive(false);
        LevelsUI.SetActive(false);
        StartMenu.SetActive(true);
    }
    public void ShowHowToPlayMenu()
    {
        StartMenu.SetActive(false);
        HowToPlayMenu.SetActive(true);
    }

    private void HighlightChildren(int currentLevel)
    {
        for (int i = 0; i < LevelsUI.transform.childCount - 1; i++)
        {
            if (i > currentLevel)
            {
                LevelsUI.transform.GetChild(i).GetComponent<LeanButton>().interactable = false;
            }
        }
    }
}
