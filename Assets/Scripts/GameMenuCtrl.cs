using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMenuCtrl : MonoBehaviour
{

    public static GameMenuCtrl Instance;
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);
    }

    [SerializeField]
    private Button buttonMenu;
    [SerializeField]
    private LongButtonPress buttonLeft;
    [SerializeField]
    private LongButtonPress buttonRight;
    [SerializeField]
    private LongButtonPress buttonUp;
    [SerializeField]
    private LongButtonPress buttonDown;
    [SerializeField]
    private LongButtonPress buttonZoomIn;
    [SerializeField]
    private LongButtonPress buttonZoomOut;
    [SerializeField]
    private Text houseNumText;
    [SerializeField]
    private Image objectNameWidget;
    [SerializeField]
    private Text objectNameText;
    [SerializeField]
    private Button destroyButton;

    [SerializeField]
    private Color buttonActiveColor;
    [SerializeField]
    private Color buttonIdleColor;

    private ControlsOverlord myControlsOverlord;
    private LevelGenerator myLevelGenerator;


    [SerializeField]
    private float objectNameTimeout = 5;
    private float startTime = 0;


    void Start()
    {
        objectNameWidget.gameObject.SetActive(false);
        buttonMenu.onClick.AddListener(delegate { ButtonClickedMenu(); });
        destroyButton.onClick.AddListener(delegate { ButtonClickedDestroy(); });
    }

    private void FixedUpdate()
    {
        if (!myControlsOverlord && ControlsOverlord.Instance)
            myControlsOverlord = ControlsOverlord.Instance;
        if (myControlsOverlord)
            myControlsOverlord.SetInputs(
                buttonLeft.isPressed, buttonRight.isPressed,
                buttonUp.isPressed, buttonDown.isPressed,
                buttonZoomIn.isPressed, buttonZoomOut.isPressed);

        if (!myLevelGenerator && LevelGenerator.Instance)
            myLevelGenerator = LevelGenerator.Instance;
        if (myLevelGenerator)
            houseNumText.text = myLevelGenerator.LevelHousesNum.ToString("F0");
    }


    public void ButtonClickedMenu()
    {
        // reset game, its easier
        SceneManager.LoadScene(0);
    }

    public void ButtonClickedDestroy()
    {
        // next house we tap, its destroyed
        myControlsOverlord.destroyActive = true;
        destroyButton.targetGraphic.color = buttonActiveColor;
    }

    public void DestroyCommandEnd()
    {
        destroyButton.targetGraphic.color = buttonIdleColor;
    }

    public void ShowWidget(string objectName)
    {
        if (!gameObject.activeInHierarchy)
            return;
        // todo movable widget
        objectNameText.text = objectName;
        objectNameWidget.gameObject.SetActive(true);
        startTime = Time.time;
        StartCoroutine(DisableWidget());
    }

    IEnumerator DisableWidget()
    {
        yield return new WaitForSeconds(objectNameTimeout);
        if(startTime + objectNameTimeout * 0.9f <= Time.time)
            objectNameWidget.gameObject.SetActive(false);
    } 
    
}
