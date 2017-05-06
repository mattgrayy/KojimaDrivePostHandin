using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CaptureTheFlag : MonoBehaviour
{
    private Kojima.GameController m_gameController;
    public List<GameObject> playerList;
    public List<int> playerScores;

    public GameObject heldByPlayer;
    // Use this for initialization
    void Start()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            playerList.Add(player);
            playerScores.Add(0);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitPlayers()
    {
        

        foreach (Kojima.CarScript player in m_gameController.m_players)
        {
            if (player != null)
            {
                //Do Player Shit here
            }

        }
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            this.transform.position = col.transform.position;
            this.transform.parent = col.transform;
            heldByPlayer = col.gameObject;
        }
    }

    public void Score(GameObject player)
    {
        for (int i = 0; i < playerList.Count; i++)
        {
            if (player.name == playerList[i].name)
            {
                playerScores[i]++;
            }
        }
    }

}
