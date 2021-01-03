using UnityEngine;

public class MapTileCtrl : MonoBehaviour
{
    public enum TileType
    {
        Terrain = 0,
        Water,
        House
    }
    public TileType type = TileType.Terrain;
    public int x = -1;
    public int y = -1;
    public string tileName = "";
    public int tileLvl = 0;
    
    [SerializeField]
    private SpriteRenderer mySpriteRenderer;

    private int cameraDistancePonder = 0;
    private Transform cameraTr;
    
    
    void Start()
    {
        if (!mySpriteRenderer)
            mySpriteRenderer = transform.GetComponent<SpriteRenderer>();
        cameraDistancePonder = CameraCtrl.maxCameraTileHeight + CameraCtrl.camLimitPonder * 2;
        cameraTr = CameraCtrl.Instance.transform;
    }

    void Update()
    {
        if (type == TileType.House)
            return;
        // check if too far away from the cam
        if ((int) transform.position.x < cameraTr.position.x - cameraDistancePonder
            || (int) transform.position.x > cameraTr.position.x + cameraDistancePonder
            || (int) transform.position.y < cameraTr.position.y - cameraDistancePonder
            || (int) transform.position.y > cameraTr.position.y + cameraDistancePonder)
        {
            gameObject.SetActive(false);
            Destroy(this);
            //        Destroy(gameObject);
        }
    }
    

    public void SetSprite(Sprite newSprite)
    {
        if (!mySpriteRenderer)
            mySpriteRenderer = transform.GetComponent<SpriteRenderer>();
        mySpriteRenderer.sprite = newSprite;
    }

}
