using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraCtrl : MonoBehaviour
{

    public static CameraCtrl Instance;
    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);
    }
    private Camera thisCamera;
    private ControlsOverlord controlsOvr;
    private LevelGenerator myLevelGenerator;

    [SerializeField] private Transform oceanBackgroundTr;
    [SerializeField]
    private float camSpeed = 5f; // tiles per second
    [SerializeField]
    private float cameraTileHeight = 8; // tiles vertical

    private const int startCameraTileHeight = 8; // tiles vertical
    public const int minCameraTileHeight = 8; // tiles vertical
    public const int maxCameraTileHeight = 32; // tiles vertical
    

    [SerializeField]
    private float zoomSpeed = 5f; // tiles per second

    private float camSizePonder = 1f;

    private float maxCamX = -1;
    private float maxCamY = -1;

    public const int camLimitPonder = 2;


    void Start()
    {
        thisCamera = transform.GetComponent<Camera>();
        camSizePonder = 1 / 2f * 1920 / 1080 / Screen.width * Screen.height;
        if (thisCamera)
            thisCamera.orthographicSize = startCameraTileHeight * camSizePonder;
        cameraTileHeight = startCameraTileHeight;
    }

    void LateUpdate()
    {
        if(oceanBackgroundTr)
            oceanBackgroundTr.position = 
                new Vector3((int)transform.position.x, (int)transform.position.y, 0);

        if(!controlsOvr)
            if (ControlsOverlord.Instance)
                controlsOvr = ControlsOverlord.Instance;
        if (!controlsOvr)
            return;


        if (!myLevelGenerator && LevelGenerator.Instance)
            myLevelGenerator = LevelGenerator.Instance;
        if (!myLevelGenerator)
            return;

            if (controlsOvr.ZoomIn)
            cameraTileHeight -= zoomSpeed * Time.deltaTime;
        if (controlsOvr.ZoomOut)
            cameraTileHeight += zoomSpeed * Time.deltaTime;
        cameraTileHeight = Mathf.Clamp(cameraTileHeight, minCameraTileHeight, maxCameraTileHeight);
        thisCamera.orthographicSize = cameraTileHeight*camSizePonder;

        // todo make camera speed dependent on cam zoom
        if (controlsOvr.MoveLeft)
            transform.position += Vector3.left*camSpeed*Time.deltaTime;
        if (controlsOvr.MoveRight)
            transform.position += Vector3.right * camSpeed * Time.deltaTime;
        if (controlsOvr.MoveUp)
            transform.position += Vector3.up * camSpeed * Time.deltaTime;
        if (controlsOvr.MoveDown)
            transform.position += Vector3.down * camSpeed * Time.deltaTime;
        
        if(transform.position.x >= maxCamX)
            transform.position = new Vector3(maxCamX, transform.position.y, transform.position.z);
        else if (transform.position.x <= camLimitPonder)
            transform.position = new Vector3(camLimitPonder, transform.position.y, transform.position.z);

        if (transform.position.y >= maxCamY)
            transform.position = new Vector3(transform.position.x, maxCamY, transform.position.z);
        else if (transform.position.y <= camLimitPonder)
            transform.position = new Vector3(transform.position.x, camLimitPonder, transform.position.z);

        // todo maybe some treshold check
        myLevelGenerator.CameraMoved(transform.position.x, transform.position.y);
    }

    public void SetMapSize(int mapWidth, int mapHeight)
    {
        // da se vidi malo i vodica, da bude jasno da je kraj
        maxCamX = mapWidth - camLimitPonder; 
        maxCamY = mapHeight - camLimitPonder;
    }


}
