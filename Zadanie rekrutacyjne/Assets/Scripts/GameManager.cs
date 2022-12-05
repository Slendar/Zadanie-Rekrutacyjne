using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    private GameObject tmpbuilding;

    private List<GameObject> woodcuttersList;
    private List<GameObject> carpentersList;
    private List<GameObject> storagesList;

    private void Awake()
    {
        instance = this;
        woodcuttersList = new List<GameObject>();
        carpentersList = new List<GameObject>();
        storagesList = new List<GameObject>();

    }

    //Add building to its list
    private void AddToList(GameObject building)
    {
        if(building.name == "Woodcutter(Clone)")
        {
            woodcuttersList.Add((GameObject)building);
        }
        else if (building.name == "Carpenter(Clone)")
        {
            carpentersList.Add((GameObject)building);
        }
        else if (building.name == "Warehouse(Clone)")
        {
            storagesList.Add((GameObject)building);
        }
    }
    public static void AddToList_Static(GameObject building)
    {
        instance.AddToList(building);
    }

    //Returns randomly one woodcutter with resources inside
    private GameObject GetResourcePosition()
    {
        if(woodcuttersList.Count != 0)
        {
            List<GameObject> tmpBuildingsList = new List<GameObject>(woodcuttersList);

            for(int i = 0; i < tmpBuildingsList.Count; i++)
            {
                if(tmpBuildingsList[i].GetComponent<GameResourcesList>().HowManyResources(tmpBuildingsList[i].GetComponent<ExtractionBuilding>().resourceSO) < 1 || tmpBuildingsList[i].GetComponent<GameResourcesList>().CheckIfOcuppied())
                {
                    tmpBuildingsList.RemoveAt(i);
                    i--;
                }
            }
            if(tmpBuildingsList.Count > 0)
            {
                tmpbuilding = tmpBuildingsList[Random.Range(0, tmpBuildingsList.Count)];
                tmpbuilding.GetComponent<GameResourcesList>().SetOcuppied(true);
                return tmpbuilding.transform.GetChild(2).gameObject;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
    public static GameObject GetResourcePosition_Static()
    {
        return instance.GetResourcePosition();
    }

    //Returns randomly one carpenter with odd number of wood inside
    //if not returns random one carpenter
    private GameObject GetCarpenterDeliveryPosition()
    {
        if(carpentersList.Count != 0)
        {
            List<GameObject> tmpBuildingsList = new List<GameObject>(carpentersList);
            List<GameObject> tmpCarpentersWithWoodList = new List<GameObject>();

            for (int i = 0; i < tmpBuildingsList.Count; i++)
            {
                if (tmpBuildingsList[i].GetComponent<GameResourcesList>().HowManyResources(tmpBuildingsList[i].GetComponent<ProductionBuilding>().inputResourceSO)%2 == 1 && !tmpBuildingsList[i].GetComponent<GameResourcesList>().CheckIfOcuppied())
                {
                    tmpCarpentersWithWoodList.Add(tmpBuildingsList[i]);
                }
                else if(tmpBuildingsList[i].GetComponent<GameResourcesList>().CheckIfOcuppied())
                {
                    tmpBuildingsList.RemoveAt(i);
                    i--;
                }
            }
            if (tmpCarpentersWithWoodList.Count > 0)
            {
                tmpbuilding = tmpCarpentersWithWoodList[Random.Range(0, tmpCarpentersWithWoodList.Count)];
                tmpbuilding.GetComponent<GameResourcesList>().SetOcuppied(true);
                return tmpbuilding.transform.GetChild(3).gameObject;
            }
            else
            {
                if(tmpBuildingsList.Count > 0)
                {
                    tmpbuilding = tmpBuildingsList[Random.Range(0, tmpBuildingsList.Count)];
                    tmpbuilding.GetComponent<GameResourcesList>().SetOcuppied(true);
                    return tmpbuilding.transform.GetChild(3).gameObject;
                }
                else
                {
                    return GetStoragePosition();
                }
            }
        }
        else
        {
            return GetStoragePosition();
        }
        
    }
    public static GameObject GetCarpenterDeliveryPosition_Static()
    {
        return instance.GetCarpenterDeliveryPosition();
    }    

    //Returns randomly one carpenter with chairs
    //if not returns randomly one woodcutter with resource inside
    private GameObject GetCarpenterTakeawayPosition()
    {
        if(carpentersList.Count != 0)
        {
            List<GameObject> tmpBuildingsList = new List<GameObject>(carpentersList);
            List<GameObject> tmpCarpentersWithChairsList = new List<GameObject>();

            for (int i = 0; i < tmpBuildingsList.Count; i++)
            {
                if (tmpBuildingsList[i].GetComponent<GameResourcesList>().HowManyResources(tmpBuildingsList[i].GetComponent<ProductionBuilding>().outputResourceSO) >= 1 && !tmpBuildingsList[i].GetComponent<GameResourcesList>().CheckIfOutOcuppied())
                {
                    tmpCarpentersWithChairsList.Add(tmpBuildingsList[i]);
                }
            }
            if (tmpCarpentersWithChairsList.Count > 0)
            {
                tmpbuilding = tmpCarpentersWithChairsList[Random.Range(0, tmpCarpentersWithChairsList.Count)];
                tmpbuilding.GetComponent<GameResourcesList>().SetOutOcuppied(true);
                return tmpbuilding.transform.GetChild(2).gameObject;
            }
            else
            {
                return GetResourcePosition();
            }
        }
        else
        {
            return GetResourcePosition();
        }
    }
    public static GameObject GetCarpenterTakeawayPosition_Static()
    {
        return instance.GetCarpenterTakeawayPosition();
    }

    private GameObject GetStoragePosition()
    {
        List<GameObject> tmpBuildingsList = new List<GameObject>(storagesList);
        for (int i = 0; i < tmpBuildingsList.Count; i++)
        {
            if (tmpBuildingsList[i].GetComponent<GameResourcesList>().CheckIfOcuppied())
            {
                tmpBuildingsList.RemoveAt(i);
                i--;
            }
        }
        if (tmpBuildingsList.Count != 0)
        {
            tmpbuilding = tmpBuildingsList[Random.Range(0, tmpBuildingsList.Count)];
            tmpbuilding.GetComponent<GameResourcesList>().SetOcuppied(true);
            return tmpbuilding.transform.GetChild(2).gameObject;
        }
        else
        {
            return null;
        }      
    }
    public static GameObject GetStoragePosition_Static()
    {
        return instance.GetStoragePosition();
    }
}
