using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    [SerializeField] private CustomButton tutorialButton;
    [SerializeField] private CustomButton startButton;

    BGMController BGM;
    SEController SE;

    private void Start()
    {
        Debug.Log(GameObject.Find("BGM"));
        Debug.Log(GameObject.Find("BGM").GetComponent<BGMController>());

        BGM = GameObject.Find("BGM").GetComponent<BGMController>();
        SE = GameObject.Find("SE").GetComponent<SEController>();

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
