using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kojima
{
    public class DropPointSpawn : MonoBehaviour
    {
        public GameObject dropPoint;

        public List<GameObject> dropPointList = new List<GameObject>();
        private RaceScript rs;

        public void BuildObject()
        {
            GameObject spawnedDropPoint = Instantiate(dropPoint, this.transform.position, Quaternion.Euler(0, 0, 0)) as GameObject;
            spawnedDropPoint.transform.parent = this.transform;
            dropPointList.Add(spawnedDropPoint);
        }

        public void PrintList()
        {
            for (int i = 0; i < dropPointList.Count; i++)
            {
                Debug.Log("In List : " + dropPointList[i] + "values");
            }
        }

        public void NameObjects()
        {
            for (int i = 0; i < dropPointList.Count; i++)
            {
                if (i < 10)
                {
                    dropPointList[i].name = dropPointList[i].name + 0 + i;
                }
                else
                {
                    dropPointList[i].name = dropPointList[i].name + i;
                }
            }
        }

        public void ResetNames()
        {
            for (int i = 0; i < dropPointList.Count; i++)
            {
                dropPointList[i].name = "dropPoint";
            }
        }

        public void StandardiseSize()
        {
            for (int i = 0; i < dropPointList.Count; i++)
            {
                if (i != 0)
                {
                    dropPointList[i].transform.localScale = dropPointList[0].transform.localScale;
                }
            }
        }

        public List<GameObject> GetList()
        {
            return dropPointList;
        }
    }
}