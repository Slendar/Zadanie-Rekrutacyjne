using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageBuilding : MonoBehaviour
{
    public GameResourceSO resourceSO_Wood;
    public GameResourceSO resourceSO_Chair;
    public GameResourcesList resourcesList;

    [SerializeField]
    FloatingText floatingTextPrefab;

    private GameObject box;

    // Start is called before the first frame update
    void Start()
    {
        box = this.gameObject.transform.GetChild(2).gameObject;

        GameManager.AddToList_Static(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //Manage box appear
        if (resourcesList.HowManyResources(resourceSO_Wood) >= 1 || resourcesList.HowManyResources(resourceSO_Chair) >= 1)
        {
            box.SetActive(true);
        }
        else
        {
            box.SetActive(false);
        }

    }

    public void Add(GameResourceSO resourceSO, int amount)
    {
        resourcesList.Add(resourceSO, amount);

        var floatingText = Instantiate(floatingTextPrefab, transform.position + Vector3.up, Quaternion.identity);
        floatingText.SetText(resourceSO.resourceName + " +1");
    }

    public void Remove(GameResourceSO resourceSO, int amount)
    {
        resourcesList.Remove(resourceSO, amount);

        var floatingText = Instantiate(floatingTextPrefab, transform.position + Vector3.up, Quaternion.identity);
        floatingText.SetText(resourceSO.resourceName + " -1");
    }
}
