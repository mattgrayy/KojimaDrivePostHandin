using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour {
    //These values will dictate what is toggled on and off in the world zone
    //A is the zone currently containing a player 
    //B is 8 zones arround A
    //C further out still
    //D everything else  
    public enum ZONEVAL
    {
        A = 0,
        B = 1,
        C = 2,
        D = 3
    }

    public List<int> newCellsWithPlayers;
    public List<int> currentCellsWithPlayers;

    public int numberOfChunksLength;

    public float cooldown;
    private float currentCoolDown;
    public WorldObject[] spawnGrid;
    private WorldObject[,] compleateGrid;


    //spawn us at the center 
    int currentPlayerZoneX = 1 ;
    int currentPlayerZoneY = 1;
    void Start () {
        newCellsWithPlayers = new List<int>();

        compleateGrid = new WorldObject[numberOfChunksLength, numberOfChunksLength];
        createStructure();

    }

	
    //Creates the new structure 
    void createStructure()
    {
        int posCount = 0;
        for(int i = 0; i < numberOfChunksLength; i++)
        {
            for(int j = 0; j < numberOfChunksLength; j++)
            {
                compleateGrid[i, j] = spawnGrid[posCount++];
                if (i != currentPlayerZoneX || j != currentPlayerZoneY)
                {
                    compleateGrid[i, j].setNewLevel(ZONEVAL.D);
                }

            }
        }

    }


            // Update is called once per frame
    void Update () {
        if(currentCoolDown > 0 )
        {
            currentCoolDown = currentCoolDown - Time.deltaTime;
            return;
        }
        currentCoolDown = cooldown;

        findPlayers();
        int record=0;
        if (!testIfchange(record))
        {
            if (record == 0)
            {
                updateCurrentList();
                updateReigions();
            }


        }

        foreach (WorldObject Wobj in compleateGrid)
        {
            Wobj.manualUpdate();
        }



    }

    void updateReigions()
    {
        foreach(WorldObject Wobj in compleateGrid)
        {
            //Set them all to D
            Wobj.setNewLevel(ZONEVAL.D);
        }
        //Set all surrounding Zones to B 
        foreach (int i in newCellsWithPlayers)
        {
            updateSurround(i);

        }
        //Set all found Zones To A
        foreach (int i in newCellsWithPlayers)
        {
            setMain(i);
        }

    }

    void setMain(int cell)
    {
        int Xval = cell / 10;
        int Yval = cell - (Xval*10);

        compleateGrid[Xval, Yval].setNewLevel(ZONEVAL.A);

    }

    void updateSurround(int center)
    {
        int Xval = center / 10;
        int Yval = center - (Xval*10);

    

        if (Xval - 1 >= 0 && Yval + 1 < numberOfChunksLength)
        {
            compleateGrid[Xval - 1, Yval + 1].setNewLevel(ZONEVAL.B);
        }
        if ( Yval + 1 < numberOfChunksLength)
        {
            compleateGrid[Xval, Yval + 1].setNewLevel(ZONEVAL.B);
        }
        if (Xval + 1 < numberOfChunksLength && Yval + 1 < numberOfChunksLength)
        {
            compleateGrid[Xval + 1, Yval + 1].setNewLevel(ZONEVAL.B);
        }


        if (Xval - 1 >= 0)
        {
            compleateGrid[Xval - 1, Yval].setNewLevel(ZONEVAL.B);
        }
        if (Xval + 1 < numberOfChunksLength)
        {
            compleateGrid[Xval + 1, Yval].setNewLevel(ZONEVAL.B);
        }



        if (Xval -1 >= 0 && Yval - 1 >= 0)
        {
            compleateGrid[Xval - 1, Yval - 1].setNewLevel(ZONEVAL.B);
        }
        if ( Yval - 1 >= 0)
        {
            compleateGrid[Xval, Yval - 1].setNewLevel(ZONEVAL.B);
        }
        if (Xval + 1 < numberOfChunksLength && Yval - 1 >= 0)
        {
            compleateGrid[Xval + 1, Yval - 1].setNewLevel(ZONEVAL.B);
        }


    }

    void updateCurrentList()
    {
        currentCellsWithPlayers = new List<int>();
        foreach(int Wobj in newCellsWithPlayers )
        {
            currentCellsWithPlayers.Add(Wobj);
        }

    }

    bool testIfchange(int _in)
    {
        if(currentCellsWithPlayers.Count != newCellsWithPlayers.Count)
        {
            _in = -1;
            return false;
        }
        for(int i = 0; i < currentCellsWithPlayers.Count; i ++)
        {
            if(currentCellsWithPlayers[i] != newCellsWithPlayers[i])
            {
                _in = -2;
                return false;
            }
        }
        _in = 0;
        return true;
    }

    void findPlayers()
    {
        newCellsWithPlayers = new List<int>();

        for(int i = 0; i < numberOfChunksLength; i++)
        {
            for (int j = 0; j < numberOfChunksLength; j++)
            {
                if (compleateGrid[i,j].getHasPlayer())
                {
                    newCellsWithPlayers.Add((i*10)+ j);
                }
            }
        }
    }



    void toggleSurroundingZones()
    {
        if (currentPlayerZoneX < numberOfChunksLength - 1)
        {
            compleateGrid[currentPlayerZoneX + 1, currentPlayerZoneY].setNewLevel(ZONEVAL.B);

            if (currentPlayerZoneY < numberOfChunksLength - 1)
            {
                compleateGrid[currentPlayerZoneX + 1, currentPlayerZoneY + 1].setNewLevel(ZONEVAL.B);
            }
        }

    }
}
