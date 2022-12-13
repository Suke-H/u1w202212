using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{
    const string FirstLevel = "StageSelect";
    [SerializeField] private CustomButton startButton;
    [SerializeField] private CustomButton soundButton;

    // private SEController SE;

    private void Start()
    {
        // SE = GameObject.Find("SE").GetComponent<SEController>();

        startButton.onClickCallback = () => { 
            SceneManager.LoadScene(FirstLevel);
        };
        
        soundButton.onClickCallback = () =>
        {
            // SE.playSE("quote");
            SEController.instance.playSE("quote");
        };
    }

}
