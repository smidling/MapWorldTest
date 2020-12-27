using UnityEngine;

public class ControlsOverlord : MonoBehaviour
{
    public static ControlsOverlord Instance;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else Destroy(this);
    }

    [SerializeField] private bool moveLeft = false;
    [SerializeField] private bool moveRight = false;
    [SerializeField] private bool moveUp = false;
    [SerializeField] private bool moveDown = false;
    [SerializeField] private bool zoomIn = false;
    [SerializeField] private bool zoomOut = false;

    public bool MoveLeft
    {
        get { return moveLeft; }
    }

    public bool MoveRight
    {
        get { return moveRight; }
    }

    public bool MoveUp
    {
        get { return moveUp; }
    }

    public bool MoveDown
    {
        get { return moveDown; }
    }

    public bool ZoomIn
    {
        get { return zoomIn; }
    }

    public bool ZoomOut
    {
        get { return zoomOut; }
    }

    public bool destroyActive = false;


    public void SetInputs(bool left, bool right, bool up, bool down, bool zIn, bool zOut)
    {
        // reset inputs
        moveLeft = left == true || moveLeft;
        moveRight = right == true || moveRight;
        moveUp = up == true || moveUp;
        moveDown = down == true || moveDown;
        zoomIn = zIn == true || zoomIn;
        zoomOut = zOut == true || zoomOut;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        moveLeft = false;
        moveRight = false;
        moveUp = false;
        moveDown = false;
        zoomIn = false;
        zoomOut = false;

        // keyboard for testing
#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveLeft = true;
            moveRight = false;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveLeft = false;
            moveRight = true;
        }
        else
        {
            moveLeft = false;
            moveRight = false;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveUp = true;
            moveDown = false;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveUp = false;
            moveDown = true;
        }
        else
        {
            moveUp = false;
            moveDown = false;
        }

        if (Input.GetKey(KeyCode.KeypadPlus))
        {
            zoomIn = true;
            zoomOut = false;
        }
        else if (Input.GetKey(KeyCode.KeypadMinus))
        {
            zoomIn = false;
            zoomOut = true;
        }
        else
        {
            zoomIn = false;
            zoomOut = false;
        }
#endif
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            if (LevelGenerator.Instance && GameMenuCtrl.Instance)
            {
                // todo disable if over UI
                Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (destroyActive)
                {
                    // try to destroy house
                    if (LevelGenerator.Instance.DestroyBuilding((int) worldPosition.x, (int) worldPosition.y))
                    {
                        destroyActive = false;
                        GameMenuCtrl.Instance.DestroyCommandEnd();
                    }
                }
                string name = LevelGenerator.Instance.GetTileName((int) worldPosition.x, (int) worldPosition.y);
                GameMenuCtrl.Instance.ShowWidget(name);
            }
    }


}
