using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] private CustomButton tutorialButton;
    [SerializeField] private CustomButton startButton;

    [SerializeField] BGMController BGM;
    [SerializeField] SEController SE;

    private void Start()
    {
        BGM.BGMChange("Normal");

        tutorialButton.onClickCallback = () => {
            SE.playSE("click");
            StageStore.stageName = "Tutorial";
            SceneManager.LoadScene("Stage");
        };

        startButton.onClickCallback = () => { 
            SE.playSE("click");
            StageStore.stageName = "Stage-1";
            SceneManager.LoadScene("Stage");
        };
    }

}
