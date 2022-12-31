using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] private CustomButton tutorialButton;
    [SerializeField] private CustomButton startButton;

    [SerializeField] BGMController BGM;

    private void Start()
    {
        BGM.BGMChange("Normal");

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
