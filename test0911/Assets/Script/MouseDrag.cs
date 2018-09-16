using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDrag : MonoBehaviour
{
    public Terrain ter;
    //声明从鼠标发出一条射线clickRay
    Ray clickRay;
    //声明clickRay与游戏物体的碰撞
    RaycastHit clickPoint;
    //声明clickRay与地面的碰撞
    RaycastHit posPoint;
    //设置地面层，我的地面层是第8层，所以是8。
    LayerMask mask = 1 << 8;

    //是否碰到了地形
    bool isColliderTerr;

    //初始heightMap 用于还原地形
    float[,] heightMapBackup;


    void Start()
    {
        heightMapBackup = ter.terrainData.GetHeights(0, 0, ter.terrainData.heightmapWidth, ter.terrainData.heightmapHeight);
    }
    void Update()
    {
        clickRay = Camera.main.ScreenPointToRay(Input.mousePosition);

    }


    void OnCollisionStay(Collision collision)
    {
        Debug.Log("碰撞--" + collision.gameObject.name);
        if (collision.gameObject == ter.gameObject)
            isColliderTerr = true;
    }
    void OnMouseDown()
    {

        //如果射线与物体相碰，则调用OnMouseDrag()
        if (Physics.Raycast(clickRay, out clickPoint))
        {
            OnMouseDrag();
        }

    }


    void OnDestroy()
    {
        Debug.Log("销毁还原");
        ReSet();
    }

    public void ReSet()
    {
        ter.terrainData.SetHeights(0, 0, heightMapBackup);


    }
    float r;

    void OnMouseDrag()
    {
        //取射线与地面相碰的坐标，赋给mouseMove，再把mouseMove的X坐标和Z坐标赋给物体，
        //Y坐标不变（因为是贴在地面移动）
        r = transform.GetComponent<SphereCollider>().radius;
        //  Debug.Log(transform.GetComponent<SphereCollider>().radius);
        Physics.Raycast(clickRay, out posPoint, Mathf.Infinity, mask.value);
        Vector3 mouseMove = posPoint.point;
        transform.position = (new Vector3(mouseMove.x, transform.position.y, mouseMove.z));
        if (isColliderTerr)
        {
            Vector3 terrainLocalPos = transform.position - ter.transform.position;  //物件在地形上的相对位置
            Vector2 controlPos = new Vector2(terrainLocalPos.x / ter.terrainData.size.x * ter.terrainData.heightmapWidth, terrainLocalPos.z / ter.terrainData.size.z * ter.terrainData.heightmapHeight);
            Debug.LogError(ter.terrainData.size + "  " + ter.terrainData.heightmapScale.y);
            for (int x1 = 0; x1 < ter.terrainData.size.x; x1++)   //遍历所有点
            {
                for (int z1 = 0; z1 < ter.terrainData.size.z; z1++)
                {

                    if (Mathf.Abs(x1 - controlPos.x) <= r && Mathf.Abs(z1 - controlPos.y) <= r)  //首先缩小范围
                    {
                        float powDis = Mathf.Pow((x1 - controlPos.x), 2) - Mathf.Pow((z1 - controlPos.y), 2); ///当前点与原点的距离的平方
                        float powr = Mathf.Pow(r, 2);
                        if (powDis <= powr)    ///再次缩小范围if (Vector3.Distance(transform.position, new Vector3(x1, 6, z1)) < r)   //Vector3.Distance(transform.position, new Vector3(x1, 6, z1)) 
                        {
                            float sinkHeight; //要降低的高度
                            float oldHeight = ter.terrainData.GetHeight(x1, z1);//输出的是介于0到1之间的高度
                            sinkHeight = Mathf.Sqrt(powr - powDis);
                            float[,] newHeightData = new float[1, 1] { { (oldHeight - sinkHeight) / (ter.terrainData.heightmapScale.y) } };  //新的高度值
                            ter.terrainData.SetHeights(x1, z1, newHeightData);  //设定新的高度值
                            isColliderTerr = false;
                        }
                    }
                }
            }
        }
        return;
    }
}


/* //球体的半径为r，地形上的某一点为（x1,y1,z1),其中y1值不变。先判断点是不是在圆形区域,height为对应点的地形的高度
 
 
 for（int x1=0;x1< ter.terrainData.size.x; x1++）/遍历所有点
 { 
    for(int z1=0;z1< ter.terrainData.size.z; z1++）
 {  
    float r =  1.5f
    if(Mathf.Abs(x1-transform.position.x)<=r&&Mathf.Abs(z1-transform.position.z)<=r)/首先缩小范围
   {  
      float powDis = Mathf.Pow((x1-transform.position.x),2)-Mathf.Pow((z1-transform.position.z),2); /当前点与原点的距离的平方
      float powr = Mathf.Pow(r,2)
      if( powDis< = powr)/再次缩小范围
     {
       float sinkHeight; /要降低的高度
       float oldHeight = ter.terrainData.GetHeight( x1, z1 );/输出的是介于0到1之间的高度
       sinkHeight=Mathf.Sprt( powr - powDis);
       float[,] newHeightData = new float[1,1] { { ( oldHeight / terrainData.heightmapScale.y - sinkHeight)} };/新的高度值
       ter.terrainData.SetHeights( x1 , z1 , newHeightData );/设定新的高度值
    }
 }
 }
 }
*/
