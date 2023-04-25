using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private List<GameObject> gamemodePanels;
    [SerializeField] private List<Button> gamemodePanelButtons;
    [SerializeField] private GameObject gamemodeButtons;
    [SerializeField] private Button playButton;
    private GameObject currentPanel;
    private int currentGamemodeIndex = 0;

    private bool holdingLeftRight = false;

    private void Start()
    {
        currentPanel = mainMenuPanel;
    }

    private void Update()
    {
        if (!mainMenuPanel.activeSelf)
        {
            float x = Input.GetAxis("Horizontal");
            if (x > 0.5f && !holdingLeftRight)
            {
                SwipeRight();
                holdingLeftRight = true;
            }
            else if (x < -0.5f && !holdingLeftRight)
            {
                SwipeLeft();
                holdingLeftRight = true;
            }
            else if (x < 0.5f && x > -0.5f)
            {
                holdingLeftRight = false;
            }
            
            if (Input.GetButtonDown("Fire2"))
            {
                BackToMainMenu();
            }
        }
    }

    public void BackToMainMenu()
    {
        mainMenuPanel.SetActive(true);
        currentPanel.SetActive(false);
        gamemodeButtons.SetActive(false);
        playButton.Select();
    }
    
    // Start is called before the first frame update
    public void ToGamemodeSelection()
    {
        mainMenuPanel.SetActive(false);
        gamemodePanels[currentGamemodeIndex].SetActive(true);
        currentPanel = gamemodePanels[currentGamemodeIndex];
        gamemodePanelButtons[currentGamemodeIndex].Select();
        gamemodeButtons.SetActive(true);
    }

    // Update is called once per frame
    public void SwipeRight()
    {
        gamemodePanels[currentGamemodeIndex].SetActive(false);
        if (currentGamemodeIndex + 1 >= gamemodePanels.Count)
            currentGamemodeIndex = -1;
        gamemodePanels[currentGamemodeIndex + 1].SetActive(true);
        currentGamemodeIndex++;
        currentPanel = gamemodePanels[currentGamemodeIndex];
        gamemodePanelButtons[currentGamemodeIndex].Select();
    }

    public void SwipeLeft()
    {
        gamemodePanels[currentGamemodeIndex].SetActive(false);
        if (currentGamemodeIndex - 1 < 0)
            currentGamemodeIndex = gamemodePanels.Count;
        gamemodePanels[currentGamemodeIndex - 1].SetActive(true);
        currentGamemodeIndex--;
        currentPanel = gamemodePanels[currentGamemodeIndex];
        gamemodePanelButtons[currentGamemodeIndex].Select();
    }

    public void PlayButton(GameObject prefab)
    {
        SceneManager.S.btn_PlayGame(prefab);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
