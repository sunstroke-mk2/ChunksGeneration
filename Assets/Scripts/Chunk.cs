using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public ChunkManager myChunkManager;
    public Vector2Int myCoordinates;
    public bool realtimeGen = false;
    public float scale = 5F;
    public float wallChance = 25F;
    Renderer myRenderer;
    List<Transform> myProps = new List<Transform> ();
    List<Transform>myWalls= new List<Transform> ();
    void Start()
    { 
        myChunkManager = GameObject.FindAnyObjectByType<ChunkManager> ();
    }
    public void GenerateChunk(float xOrg,float yOrg)
    {
        //тут мы генерируем перлинов шум по переданным координатам и используем его в качестве альфа канала декаль-текстуры
        ClearProps();
        xOrg = xOrg * scale;
        yOrg = yOrg * scale;
        myRenderer = transform.GetComponent<Renderer>();
        Texture2D baseTexture = myRenderer.material.GetTexture("_DecalTex") as Texture2D;
        int _size=baseTexture.width;
        Texture2D newTexture = new Texture2D(_size, _size);

        Color[] pixels = baseTexture.GetPixels(0);

        float y = 0.0F;

        while (y < newTexture.height)
        {
            float x = 0.0F;
            while (x < newTexture.width)
            {
                float xCoord = xOrg + x / newTexture.width * scale;
                float yCoord = yOrg + y / newTexture.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pixels[(int)y * newTexture.width + (int)x].a = sample *2F;
                x++;
            }
            y++;
        }
        myCoordinates = new Vector2Int((int)xOrg,(int) yOrg);
        newTexture.SetPixels(pixels);
        newTexture.Apply();
        myRenderer.material.SetTexture("_DecalTex",newTexture);
        PlaceProps(xOrg, yOrg);
    }

    void PlaceProps(float seedA,float seedB)
    {
        //из координат чанка формируем сид генерации и рандомим пропсы (забираем их из заранее созданного пула и размещаем на чанке
        string _seed = Mathf.Abs(seedA).ToString() + Mathf.Abs(seedB).ToString();
        Debug.Log("props generated with seed "+_seed);
        Random.InitState(int.Parse(_seed));
        int propsCount = Random.Range(0, 5);
        for (int i = 0; i < propsCount; i++) 
        {
            string _type   = Random.Range(0, myChunkManager.propsLibrary.Length).ToString();
            float _posX = Random.Range(-5, 5);
            float _posY = Random.Range(-5, 5);
            float _angle= Random.Range(0, 180);

            //поиск пропса в пулле и установка
            foreach(Transform prop in myChunkManager.propsPool)
            {
                if(prop.name ==_type)
                {  
                    prop.parent = this.transform;
                    prop.gameObject.SetActive(true);
                    prop.localPosition = new Vector3(_posX, 0, _posY);
                    prop.eulerAngles = new Vector3(0, _angle, 0);
                    myProps.Add(prop);
                    myChunkManager.propsPool.Remove(prop);
                    
                    break;
                }
            }
        }
        if (myCoordinates.sqrMagnitude % 2 == 0)//только у чЄтных чанков генерим стены, иначе они будут дублироватьс€
        {
            for(int i=0;i<=270;i+=90)
                if(Random.Range(0,100)<=wallChance)
                    InstallWall(i);
        }
    }
    void ClearProps()
    { //метод вызываем при ре√≈Ќ≈–ј÷»» чанков. ”бираем присутствующие стеночки и пропсы обратно в общий пулл объектов
        foreach(Transform prop in myProps)
        {
            myChunkManager.propsPool.Add(prop);
            prop.parent=myChunkManager.transform;
            prop.gameObject.SetActive(false);
        }
        myProps.Clear();

        foreach(Transform wall in myWalls)
        {
            myChunkManager.wallsPool.Add(wall);
            wall.parent = myChunkManager.transform;
            wall.gameObject.SetActive(false);
        }
        myWalls.Clear();
    }
    void InstallWall(float _wallAngle)
    {
        //достаЄм стенку из пулла и поворачиваем еЄ на нужный градус(префаб стены должен иметь центр в центре чанка)
        Transform wall = myChunkManager.wallsPool[0];
        myChunkManager.wallsPool.Remove(wall);
        myWalls.Add(wall);
        wall.gameObject.SetActive(true);
        wall.parent = transform;
        wall.localPosition = Vector3.zero;
        wall.eulerAngles = new Vector3(0, _wallAngle, 0);
    }
    private void OnDrawGizmos()
    {//сугубо дебаг дл€ визуального представлени€ границ чанков
        if (myCoordinates.sqrMagnitude % 2 == 0)
            Gizmos.color = Color.white;
        else
            Gizmos.color = Color.yellow;

        Gizmos.DrawCube(transform.position, new Vector3(10F,0.1F,10F));
    }
}