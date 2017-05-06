using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//using System;

namespace GCSharp
{
    public class TreeSpawning : MonoBehaviour
    {
        // public Transform[] children;
        public GameObject m_greenTree;
        public GameObject m_pinkTree;
        public GameObject m_Tree1;
        public GameObject m_Tree2;
        public GameObject m_Tree3;
        public GameObject m_Tree4;
        public GameObject m_Tree5;
        public GameObject m_rabbit;
        public GameObject m_chicken;


        public GameObject m_sign;
        public GameObject m_sign2;
        public GameObject m_lamp;
        public GameObject m_rock;
        public GameObject m_rock2;
        public GameObject m_rock3;

        public Vector3 m_rockShift;
        public Vector3 m_treeShift;
        public Vector3 m_lampShift;
        public Vector3 m_signShift;

        private int m_treeDensity;
        public float m_distanceBetweenTrees;

        private string[] m_splitString;
        private char m_treeType;
        private float m_radius;
        private bool m_treeSpawn = false;


        // public List<Vector3> m_trees;

        void Awake()
        {
            foreach (Transform child in transform)
            {

                m_radius = 0;

                m_splitString = child.gameObject.name.Split(' ');
                foreach (string T in m_splitString)
                {
                    //Debug.Log(T);
                    switch (T)
                    {
                        case "G":
                        case "g":
                            m_treeType = 'G';
                            m_treeSpawn = true;
                            break;
                        case "P":
                        case "p":
                            m_treeType = 'P';
                            m_treeSpawn = true;
                            break;
                        case "M":
                        case "m":
                            m_treeType = 'M';
                            m_treeSpawn = true;
                            break;

                        case "Sign":
                            if (Random.Range(0f, 1f) >= 0.5f)
                            {
                                Instantiate(m_sign, child.position += m_signShift, child.rotation);
                            }
                            else
                            {
                                Instantiate(m_sign2, child.position += m_signShift, child.rotation);
                            }
                            break;
                        case "rabbit":
                            m_treeType = 'B';
                            m_treeSpawn = true;
                            break;

                        case "chicken":
                            m_treeType = 'C';
                            m_treeSpawn = true;
                            break;

                        case "Lamp":
                            Instantiate(m_lamp, child.position += m_lampShift, child.rotation);
                            break;
                        case "Rock":
                            m_treeType = 'R';
                            m_treeSpawn = true;
                            //Instantiate(m_rock, child.position += m_rockShift, child.rotation);
                            break;
                        case "r1":
                            m_radius = 10f;
                            break;
                        case "r2":
                            m_radius = 15f;
                            break;
                        case "r3":
                            m_radius = 20f;
                            break;
                        case "r4":
                            m_radius = 25f;
                            break;
                        case "r5":
                            m_radius = 30f;
                            break;
                        case "r6":
                            m_radius = 35f;
                            break;
                        case "r7":
                            m_radius = 40f;
                            break;
                        case "r8":
                            m_radius = 45f;
                            break;
                        case "r9":
                            m_radius = 50f;
                            break;
                        case "r10":
                            m_radius = 55f;
                            break;
                        case "r100":
                            m_radius = 100f;
                            break;
                        case "r200":
                            m_radius = 200f;
                            break;
                        case "r300":
                            m_radius = 300f;
                            break;
                        case "r400":
                            m_radius = 400f;
                            break;
                        case "r500":
                            m_radius = 500f;
                            break;
                        default:
                            m_treeDensity = int.Parse(T);
                            break;

                    }
                }
                if (m_treeSpawn)
                {
                    treeSpawn(child.gameObject.transform.position,child);
                }

            }
        }
        private void treeSpawn(Vector3 center, Transform m_child)
        {
            RaycastHit hit;
            //Vector2 newPos;
            for (int i = 0; i < m_treeDensity; i++)
            {
                Vector3 t_pos = RandomCircle(center, m_radius);
                do
                {
                    t_pos = RandomCircle(center, m_radius);
                    if (Physics.Raycast(t_pos, Vector3.down, out hit))
                    {
                        t_pos = hit.point;
                    }

                } while (checkForAnotherTree(t_pos) == false);

                // newPos = new Vector3(t_pos.x, t_pos.y - 6f, t_pos.z);



                switch (m_treeType)
                {

                    case 'G':
                        //Debug.Log("Green");


                        Instantiate(m_greenTree, t_pos += m_treeShift, transform.rotation,m_child);

                        break;
                    case 'P':
                       // Debug.Log("Pink");

                        Instantiate(m_pinkTree, t_pos += m_treeShift, transform.rotation,m_child);

                        break;

                    case 'M':
                       // Debug.Log("Mix");

                        GameObject t_tree = pickTree();
                        Instantiate(t_tree, t_pos += m_treeShift, transform.rotation,m_child);
                        // m_trees.Add(t_pos);
                        break;
                    case 'R':
                        GameObject t_rock = pickRock();
                        Instantiate(t_rock, t_pos += m_rockShift, transform.rotation, m_child);

                        break;
                    case 'B':
                        
                        Instantiate(m_rabbit, t_pos += m_rockShift, transform.rotation, m_child);

                        break;
                    case 'C':
                        Instantiate(m_chicken, t_pos += m_rockShift, transform.rotation, m_child);

                        break;

                }
            }
            m_treeSpawn = false;
        }

        GameObject pickTree()
        {
            int t_i = Random.Range(1, 8);
            switch (t_i)
            {
                case 1:
                    return m_pinkTree;
                    break;
                case 2:
                    return m_greenTree;
                    break;
                case 3:
                    return m_Tree1;
                    break;
                case 4:
                    return m_Tree2;
                    break;
                case 5:
                    return m_Tree3;
                    break;
                case 6:
                    return m_Tree4;
                    break;
                case 7:
                    return m_Tree5;
                    break;
            }
            //to make function happy
            return m_pinkTree;

        }

        GameObject pickRock()
        {
            int t_i = Random.Range(1, 4);
            switch (t_i)
            {
                case 1:
                    return m_rock;
                    break;
                case 2:
                    return m_rock2;
                    break;
                case 3:
                    return m_rock3;
                    break;
                
            }
            //to make function happy
            return m_rock;

        }


        Vector3 RandomCircle(Vector3 center, float radius)
        {
            center.x += Random.Range(-radius, radius);
            center.z += Random.Range(-radius, radius);
            return center;

            //Vector3 pos = Random.insideUnitCircle * radius;
            //return center += pos;


            //float ang = Random.Range(1, 361);
            //Vector3 pos;
            //pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
            //pos.y = center.y;
            //pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);

            //return pos;
        }



        private bool checkForAnotherTree(Vector3 m_posToCheck)
        {
            Collider[] hitColliders = Physics.OverlapSphere(m_posToCheck, m_distanceBetweenTrees);
            if (hitColliders == null)
            {
                return true;
            }
            else
            {
                foreach (Collider C in hitColliders)
                {
                    if (C.gameObject.tag == "Tree")
                    {
                        return false;
                    }
                }
            }

            return true;
        }

       
    }
}
