using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] private CustomButton tutorialButton;
    [SerializeField] private CustomButton startButton;

    private void Start()
    {
        tutorialButton.onClickCallback = () => { 
            StageStore.stageName = "Tutorial";
            SceneManager.LoadScene("Stage");
        };

        startButton.onClickCallback = () => { 
            StageStore.stageName = "Stage-1";
            SceneManager.LoadScene("Stage");
        };
    }

}
