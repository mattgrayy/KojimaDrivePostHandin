    using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarMaker : MonoBehaviour
{
    public List<Material> playerColours;


    //An instance of the cars that we will be using
    public GameObject carBase;
    //a collection of these cars for each player (cloned) 
    private List<GameObject> carList;
	//a list of 'levels' that have already been used
	private List<int> usedLevelList;
    //Sets so you can control the number of players
    static public int numberOfPlayers = 2;
	//Whether the level is random or pre-selected
	public bool randomlySelected;
	//If so, which level
	public int desiredLevel;
	//which round are we on?
	private int currentRound = 0;



    //These are the parient objects that are bound to the start locations in the world.
    //These should be in the format: Pairent (That is stored here) and 4 children
    //that are the corrosponding start locs 
    public Transform[] startLocs;
    public Transform[] endLocs;

    public GameObject endPointModel;
    private GameObject endPoint;
    //Thes are the childeren of the current selected start location
    //These will be swaped out each time a new round is started
    private Transform[] currentStartLocs;
    public RespawnManager respawnManager;

	//temp code
	public void spawnCars(){
        createSpawn();
		if (randomlySelected)
		{
			usedLevelList = new List<int>();
		}
        respawnManager = GetComponent<RespawnManager>();
        respawnManager.setUp();
		beginRound (randomlySelected, desiredLevel);		//if looking for a specfic level, set to true and pass in the desired level
	}
	//temp code

        //##Removed for external handeling
    
	//float fHackyBreatherTimer = 2.0f;
	//void Update()
	//{


		//if (GameManager.GameState == GameManager.gameStates_e.GAMESTATE_GAMEOVER)
		//{
		//	fHackyBreatherTimer -= Time.deltaTime;
		//	if (fHackyBreatherTimer <= 0.0f) {
		//		RestartRound(randomlySelected, desiredLevel);
		//		GameManager.GM.SetGameState(GameManager.gameStates_e.GAMESTATE_GAMEPLAY);
		//		GameManager.GM.ResetClock();

  //              //##TempRemove of sound effects 

		//		//AudioClip au = Soundbank.GetAudioclip("MUSIC", "GAME_MUSIC");
		//		//Soundbank.PlayOneShot("VOICE", "NEXTROUND");
		//		//Soundbank.GetStaticSoundbank("MUSIC").PlaySound("GAME_MUSIC");
		//		fHackyBreatherTimer = 2.0f;
		//		//GameManager.GM.
		//	}
		//}
	//}
    /// <summary>
    /// sets up a new round for the players, can randomly select from the current stages
    /// on the list or a spesific stage, requesting random with a requested stage will be
    /// overruled
    /// </summary>
    void beginRound(bool _randomStage, int _requestedStage=0)
    {
        if(carList != null)
        {
            foreach(GameObject go in carList)
            {
                Destroy(go);
            }
		}

		carList = new List<GameObject>();

		currentStartLocs = new Transform[4];

        if (_randomStage)
        {
			int currentStage = 0;
			do
			{
				currentStage = Random.Range(0, startLocs.Length);
			}
			while (usedLevelList.Contains(currentStage));
			usedLevelList.Add(currentStage);
			getCurrentStartLocs(currentStage);
            getCurrentEndLoc(currentStage);
        }
        else
        {
            getCurrentStartLocs(_requestedStage);
			getCurrentEndLoc(_requestedStage);
        }
        makeCars();

        foreach (GameObject go in carList)
        {
            go.GetComponent<Kojima.RespawnScript>().moveToStartPoint();
        }
        //##TempRemove of sound effects 
       // Soundbank.PlayOneShot("VOICE", "ROUND1");
	}

	static public int s_nMaxRounds = 3;

	void RestartRound(bool _randomStage, int _requestedStage = 0)
	{
		currentRound++;
        
		if (currentRound == s_nMaxRounds)
        { 
            //now handeld externaly 
            //SummaryTransitioner.StartSummary();
			return;
		}
		else if (_randomStage)
		{
			int currentStage = 0;
			do
			{
				currentStage = Random.Range(0, startLocs.Length);
			}
			while (usedLevelList.Contains(currentStage));
			usedLevelList.Add(currentStage);
			getCurrentStartLocs(currentStage);
			getCurrentEndLoc(currentStage);
		}
		else
		{
			getCurrentStartLocs(_requestedStage);
			getCurrentEndLoc(_requestedStage);
		}

		for (int i = 0; i < numberOfPlayers; i++)
		{
			carList[i].GetComponent<Kojima.RespawnScript>().setResetPoint(currentStartLocs[i]);
			carList[i].GetComponent<Kojima.RespawnScript>().moveToStartPoint();
		}

		if(currentRound == s_nMaxRounds - 1) {
            //##TempRemove of sound effects 
           // Soundbank.PlayOneShot("VOICE", "FINALROUND");
			return;
		}
        //##TempRemove of sound effects 
        //switch (currentRound + 1) {
        //	case 1:
        //		Soundbank.PlayOneShot("VOICE", "ROUND1");
        //		break;
        //	case 2:
        //		Soundbank.PlayOneShot("VOICE", "ROUND2");
        //		break;
        //	case 3:
        //		Soundbank.PlayOneShot("VOICE", "ROUND3");
        //		break;
        //	default:
        //		Soundbank.PlayOneShot("VOICE", "NEXTROUND");
        //		break;
        //}



    }

    /// <summary>
    /// Gets the current children of the currently selected start locations and placed them
    /// were the cars can acess them
    /// </summary>
    void getCurrentStartLocs(int _stageNo)
    {
        int i = 0;
        foreach(Transform child in startLocs[_stageNo])
        {
            currentStartLocs[i] = child;
            i++;
        }
    }

    void createSpawn()
    {
        endPoint = Instantiate(endPointModel);
    }

    void getCurrentEndLoc(int _stageNo)
    {
        endPoint.transform.position = endLocs[_stageNo].transform.position;
    }

    

    //spawn the cars and give them where they need to begin
    void makeCars()
    {
        for (int i = 0; i < numberOfPlayers; i++)
        {
            GameObject hold = Instantiate(carBase);
 
            //hold.GetComponent<motor>().setControl(i + 1);
            hold.GetComponent<Kojima.RespawnScript>().setResetPoint(currentStartLocs[i]);

            //Talk to Car maker about if they want to set the colour externaly
            //hold.GetComponent<Kojima.RespawnScript>().setBodyColour(playerColours[i]) ;

            //Cars and camera should auto settup from now on
            //hold.gameObject.transform.GetChild(0).gameObject.AddComponent<camMover>();
            //hold.gameObject.transform.GetChild(0).gameObject.GetComponent<camMover>().setTargetPos(hold.transform);
            //hold.gameObject.transform.GetChild(0).transform.SetParent(null);


            carList.Add(hold);
        }
    }


}
