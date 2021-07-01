using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour
{
    [System.Serializable]
    public class Cell
    {
        public bool visited;
        public GameObject north; //1
        public GameObject south; //4
        public GameObject east;  //2
        public GameObject west;  //3
    }

    public GameObject wall;
    public float wallLegth = 1.0f;
    public int xSize = 5;
    public int ySize = 5;
    private Vector3 initialPos;
    private GameObject wallHolder;
    //CreateCells
    public Cell[] cells;
    //Neighbour
    public int currentCell = 0;
    private int totalCells;
    //CreateMaze
    private int visitedCells = 0;
    private bool start = false;
    private int currentNeighbour = 0;
    private List<int> lastCells;
    private int backingUp;
    private int wallToBreak = 0;


    void Start()
    {
        CreateWalls();
    }

    void CreateWalls()
    {
        wallHolder = new GameObject();
        wallHolder.name = "Maze";

        initialPos = new Vector3((-xSize / 2) + wallLegth / 2, 0, (-ySize / 2) + wallLegth / 2);
        Vector3 myPos = initialPos;
        GameObject tempWall;

        //For X axis
        for (int i = 0; i < ySize; i++)
        {
            for (int j = 0; j <= xSize; j++)
            {
                myPos = new Vector3(initialPos.x + (j * wallLegth) - wallLegth / 2, 0, initialPos.z + (i * wallLegth) - wallLegth / 2);
                tempWall = Instantiate(wall, myPos, Quaternion.identity) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        //For Y axis
        for (int i = 0; i <= ySize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                myPos = new Vector3(initialPos.x + (j * wallLegth), 0, initialPos.z + (i * wallLegth) - wallLegth);
                tempWall = Instantiate(wall, myPos, Quaternion.Euler(0, 90, 0)) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        CreateCells();
    }

    void CreateCells()
    {
        lastCells = new List<int>();
        lastCells.Clear();
        totalCells = xSize * ySize;

        //GameObject[] allWalls;
        GameObject[] allwalls;
        int chidren = wallHolder.transform.childCount;
        allwalls = new GameObject[chidren];
        cells = new Cell[xSize * ySize];

        int EW = 0;
        int childProcess = 0;
        int termCount = 0;

        //Get all childern to allwalls and name it
        for (int i = 0; i < chidren; i++)
        {
            allwalls[i] = wallHolder.transform.GetChild(i).gameObject;
            //allwalls[i].transform.GetChild(0).GetComponent<TextMesh>().text = i.ToString();
        }

        //Assign walls to Cells
        for (int i = 0; i < cells.Length; i++)
        {
            if (termCount == xSize)
            {
                EW++;
                termCount = 0;
            }
            cells[i] = new Cell();
            cells[i].west = allwalls[EW];
            cells[i].south = allwalls[childProcess + (xSize + 1) * ySize];

            EW++;
            termCount++;
            childProcess++;

            cells[i].east = allwalls[EW];
            cells[i].north = allwalls[(childProcess + (xSize + 1) * ySize) + xSize - 1];
        }

        CreateMaze();
    }

    void CreateMaze()
    {
        if (visitedCells < totalCells)
        {
            if (start)
            {
                Neighbour();
                if (cells[currentNeighbour].visited == false && cells[currentCell].visited == true)
                {
                    BreakWall();
                    cells[currentNeighbour].visited = true;
                    visitedCells++;
                    lastCells.Add(currentCell);
                    currentCell = currentNeighbour;
                    if (lastCells.Count > 0)
                    {
                        backingUp = lastCells.Count - 1;
                    }
                }
            }
            else
            {
                currentCell = Random.Range(0, totalCells);
                cells[currentCell].visited = true;
                visitedCells++;
                start = true;
            }

            Invoke("CreateMaze", 0.2f);
        }
    }

    void BreakWall()
    {
        switch (wallToBreak)
        {
            case 1: Destroy(cells[currentCell].north); break;
            case 2: Destroy(cells[currentCell].east); break;
            case 3: Destroy(cells[currentCell].west); break;
            case 4: Destroy(cells[currentCell].south); break;
        }
    }

    void Neighbour()
    {
        totalCells = xSize * ySize;
        int length = 0;
        int[] neighbours = new int[4];
        int[] connectingWall = new int[4];
        int check = (currentCell + 1) / xSize;
        check -= 1;
        check *= xSize;
        check += xSize;

        //WEST
        if (currentCell - 1 >= 0 && currentCell != check)
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbours[length] = currentCell - 1;
                connectingWall[length] = 3;
                length++;
            }
        }
        //EAST
        if (currentCell + 1 < totalCells && (currentCell + 1) != check)
        {
            if (cells[currentCell + 1].visited == false)
            {
                neighbours[length] = currentCell + 1;
                connectingWall[length] = 2;
                length++;
            }
        }
        //NORT
        if (currentCell + xSize < totalCells)
        {
            if (cells[currentCell + xSize].visited == false)
            {
                neighbours[length] = currentCell + xSize;
                connectingWall[length] = 1;
                length++;
            }
        }
        //SOUTH
        if (currentCell - xSize >= 0)
        {
            if (cells[currentCell - xSize].visited == false)
            {
                neighbours[length] = currentCell - xSize;
                connectingWall[length] = 4;
                length++;
            }
        }

        //for (int i = 0; i < length; i++)
        //print(neighbours[i]);

        if (length != 0)
        {
            int theChosen = Random.Range(0, length);
            currentNeighbour = neighbours[theChosen];
            wallToBreak = connectingWall[theChosen];
        }
        else
        {
            if (backingUp > 0)
            {
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }
    }
}
