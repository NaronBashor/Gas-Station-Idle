using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial Panels")]
    [SerializeField] private List<GameObject> tutorialPanels; // List of tutorial panels to show in sequence
    [SerializeField] private Button nextButton; // Button to proceed to the next panel
    [SerializeField] private Button skipButton; // Button to skip the tutorial

    private int currentPanelIndex = 0;

    private const string TutorialCompletedKey = "TutorialCompleted";

    private void Start()
    {
        // Check if the tutorial has already been completed
        if (PlayerPrefs.HasKey(TutorialCompletedKey) && PlayerPrefs.GetInt(TutorialCompletedKey) == 1) {
            // Tutorial has already been completed, so skip it
            CompleteTutorial();
            return;
        }

        // Initialize the tutorial by showing the first panel
        ShowPanel(0);

        // Add button listeners
        //nextButton.onClick.AddListener(NextPanel);
        //skipButton.onClick.AddListener(SkipTutorial);
    }

    private void ShowPanel(int index)
    {
        // Hide all panels initially
        foreach (GameObject panel in tutorialPanels) {
            panel.SetActive(false);
        }

        // Show only the current panel
        if (index >= 0 && index < tutorialPanels.Count) {
            tutorialPanels[index].SetActive(true);
        }

        // Hide the "Next" button on the last panel
        //nextButton.gameObject.SetActive(index < tutorialPanels.Count - 1);
    }

    public void NextPanel()
    {
        currentPanelIndex++;
        SoundManager.Instance.PlaySound("buttonClick", 1f, false);

        // If we have reached the end of the tutorial, complete it
        if (currentPanelIndex >= tutorialPanels.Count) {
            CompleteTutorial();
        } else {
            // Show the next panel
            ShowPanel(currentPanelIndex);
        }
    }

    private void SkipTutorial()
    {
        // Complete the tutorial directly, skipping all panels
        CompleteTutorial();
    }

    private void CompleteTutorial()
    {
        // Hide all tutorial panels
        foreach (GameObject panel in tutorialPanels) {
            panel.SetActive(false);
        }

        // Mark the tutorial as completed in PlayerPrefs
        PlayerPrefs.SetInt(TutorialCompletedKey, 1);
        PlayerPrefs.Save(); // Save PlayerPrefs to ensure the setting is saved to disk

        // Optionally disable the TutorialManager if it's no longer needed
        gameObject.SetActive(false);
    }
}
