using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public GameObject wallPrefab;
    public GameObject[] propsLibrary;
    const float CHUNKSIZE = 10F;
    public GameObject chunkPrefab;
    public GameObject player;
    public Chunk[] chunkPool;
    public Vector2Int currentChunk;
    public int chunkRenderDist;
    public List<Transform> propsPool= new List<Transform>();
    public List<Transform> wallsPool = new List<Transform>();

    private void Start()
    {   //generating objects for pools chunks/props/walls
        //генерируем объекты для пуллов чанков/пропсов/стен
        for(int pr=0;pr<propsLibrary.Length;pr++)
            for(int i =0;i<30;i++)
            {
                Transform tr = GameObject.Instantiate(propsLibrary[pr]).transform;
                tr.parent = this.transform;
                tr.name = pr.ToString();
                tr.gameObject.SetActive(false);
                propsPool.Add(tr);
            }
        for(int w=0;w<20;w++)
        {
            Transform tr = GameObject.Instantiate(wallPrefab).transform;
            tr.parent = this.transform;
            tr.gameObject.SetActive(false);
            wallsPool.Add(tr);
        }

        chunkPool = new Chunk[chunkRenderDist * chunkRenderDist ];

        for (int i =0; i < chunkPool.Length; i++)
            chunkPool[i] = GameObject.Instantiate(chunkPrefab).GetComponent<Chunk>();

        //randoming  starting player position. Better to make more difference in values
        //рандомизация стартовой позиции игрока. По хорошему нужен бОльший разброс координат
        player.transform.position = new Vector3(Random.Range(-555, 555), 0, Random.Range(-555, 555));

        ReGenerateChunks(currentChunk);
    }

    private void Update()
    {
        //checking if player "moved" to another chunk; if yes - then starting re-generation
        //если игрок "перешёл" на другой чанк запускаем перегенарацию
        if(getPlayersChunk()!=currentChunk)
        {
            currentChunk = getPlayersChunk();
            ReGenerateChunks(currentChunk);
        }
    }
 private void ReGenerateChunks(Vector2Int _pos)
 {
      //generation chunks around player's position
    //генерация чанков вокруг нового положения игрока
     int _a=0;
     for(int i = -1; i < chunkRenderDist-1; i++)
         for (int j = -1; j < chunkRenderDist-1; j++)
         {
                
             bool chunkExist = false;
              //checking if chunk already existing - then we dont need to generate it again
             //если чанк с нужными координатами уже есть в пулле, то мы его не генерируем заново
             foreach (Chunk chunk in chunkPool)
             {
                    if (chunk.myCoordinates == new Vector2Int(_pos.x + i, _pos.y + j))
                    {
                        chunkExist = true;
                        break;
                    }
             }             
             if (!chunkExist)
             {
                 chunkPool[_a].transform.position = new Vector3(CHUNKSIZE * (_pos.x + i), 0, CHUNKSIZE * (_pos.y + j));
                 chunkPool[_a].GenerateChunk(_pos.x+i , _pos.y +j);
                 _a++;
             }
         }
 }
    //getting closest chunk to player
    //получение ближайшего чанка к позиции игрока и присвоение ему "текущего чанка"
    Vector2Int getPlayersChunk()
    {
         return new Vector2Int
         (
         Mathf.RoundToInt(player.transform.position.x/CHUNKSIZE),
         Mathf.RoundToInt(player.transform.position.z/CHUNKSIZE)
         );
    }

}
