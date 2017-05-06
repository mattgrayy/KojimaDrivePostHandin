using System.Collections.Generic;
using UnityEngine;

public class CarColourise : MonoBehaviour
{

    public enum Colours
    {
        RED,
        BLUE,
        GREEN,
    }

    public Colours colour;
    

    public List<int> m_matIDs;
    private int m_matID;
    public GameObject[] m_players;
    public GameObject player;
    private int m_matNum;
    public Material m_initMat, m_newMat;
    private GCSharp.MaterialChanger m_matChanger;
    public List<Material> ColourList;
    public int selectedColour = 11;
    public GameObject colourWheelPrefab;
    private GameObject colourWheel;
    private GameObject t_body;
    private Kojima.CarScript cs;
    private Renderer colWheelRend;
    private bool occupied = false;
    
    private void Start()
    {

        colourWheel = Instantiate(colourWheelPrefab, (this.transform.position + new Vector3(0,2.5f,0)) , this.transform.rotation) as GameObject;
        colourWheel.SetActive(false);
        
        
    }

    private void Init()
    {
        m_matIDs = new List<int>();
        m_players = GameObject.FindGameObjectsWithTag("Player");


        for (int i = 0; i < m_players.Length; i++)
        {
            int t_id = m_players[i].GetComponent<MaterialID>().GetID();
            m_matIDs.Add(t_id);
        }

        for (int i = 0; i < m_players.Length; i++)
        {
           
            
            t_body = m_players[i].GetComponent<Kojima.CarScript>().GetCarBody();
            m_players[i].GetComponent<GCSharp.MaterialChanger>().SetBody(t_body);
            m_players[i].GetComponent<GCSharp.MaterialChanger>().SetMatID(m_matIDs[i]);
            m_players[i].GetComponent<GCSharp.MaterialChanger>().SetBHMat(m_newMat);
            //m_players[i].GetComponent<GCSharp.MaterialChanger>().UpdateMatToInitMat();
            

            //colourWheel.transform.parent = m_players[i].transform;

            //colWheelRend = colourWheel.GetComponent<Renderer>();
            //colWheelRend.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player" && !occupied)
        {
            occupied = true;

            col.gameObject.AddComponent<GCSharp.MaterialChanger>();
            Init();
            col.transform.position = this.transform.position;
            col.transform.rotation = this.transform.rotation;
         
            colourWheel.SetActive(true);
            cs = col.GetComponent<Kojima.CarScript>();
            cs.ResetCar();
        }
        //colWheelRend.enabled = true;

        //
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.GetComponent<GCSharp.MaterialChanger>() == null)
        {
            col.gameObject.AddComponent<GCSharp.MaterialChanger>();
            Init();
        }
        
        if (col.gameObject.tag == "Player")
        {
            player = col.gameObject;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                
                //m_players[i].GetComponent<GCSharp.MaterialChanger>().SetBHMat(m_newMat);
                player.GetComponent<GCSharp.MaterialChanger>().UpdateMatToBHMat();
            }
        }
        
       

    }

    private void OnTriggerExit(Collider col)
    {
        Destroy(col.gameObject.GetComponent<GCSharp.MaterialChanger>());
        colourWheel.SetActive(false);
        occupied = false;
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.RightArrow) && selectedColour < ColourList.Count)
        {
            if (selectedColour == 11)
            {
                selectedColour = 0;
                m_newMat = ColourList[selectedColour];
                colourWheel.transform.Rotate(0, 30, 0);
                for (int i = 0; i < m_players.Length; i++)
                {
                    m_players[i].GetComponent<GCSharp.MaterialChanger>().SetBHMat(m_newMat);
                }
            }
            else
            {
                selectedColour += 1;

                m_newMat = ColourList[selectedColour];
                colourWheel.transform.Rotate(0, 30, 0);
                for (int i = 0; i < m_players.Length; i++)
                {
                    m_players[i].GetComponent<GCSharp.MaterialChanger>().SetBHMat(m_newMat);
                }
            }
            

        }
        if (Input.GetKeyUp(KeyCode.LeftArrow) && selectedColour >=0)
        {
            if (selectedColour == 0)
            {
                selectedColour = 11;
                
                m_newMat = ColourList[selectedColour];
                colourWheel.transform.Rotate(0, -30, 0);
                for (int i = 0; i < m_players.Length; i++)
                {
                    m_players[i].GetComponent<GCSharp.MaterialChanger>().SetBHMat(m_newMat);
                }
            }
            else
            {
                selectedColour -= 1;
                m_newMat = ColourList[selectedColour];
                colourWheel.transform.Rotate(0, -30, 0);
                for (int i = 0; i < m_players.Length; i++)
                {
                    m_players[i].GetComponent<GCSharp.MaterialChanger>().SetBHMat(m_newMat);
                }
            }
            
        }
    }


    private void CycleColour()
    {
        
    }


  
}