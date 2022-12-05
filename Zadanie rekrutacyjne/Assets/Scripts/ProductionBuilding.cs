using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : MonoBehaviour
{
    public int inputAmountRequired = 2;
    public GameResourceSO inputResourceSO;
    public GameResourceSO outputResourceSO;

    public float timeToExtract = 5f;

    float timeProgress = 0f;
    public GameResourcesList resourcesList;

    [SerializeField]
    FloatingText floatingTextPrefab;

    private GameObject box;
    private GameObject box1;
    private GameObject box2;

    // Start is called before the first frame update
    void Start()
    {
        timeProgress = 0f;
        box = this.gameObject.transform.GetChild(2).gameObject;
        box1 = this.gameObject.transform.GetChild(3).gameObject;
        box2 = this.gameObject.transform.GetChild(4).gameObject;

        GameManager.AddToList_Static(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        timeProgress += Time.deltaTime;

        if (timeProgress > timeToExtract)
        {
            Product();
            timeProgress = 0f;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Add(inputResourceSO, 1);
        }

        //Manage box appear
        if (resourcesList.HowManyResources(inputResourceSO) >= 1)
        {
            box1.SetActive(true);
            if (resourcesList.HowManyResources(inputResourceSO) >= 2)
            {
                box2.SetActive(true);
            }
            else
            {
                box2.SetActive(false);
            }
        }
        else
        {
            box1.SetActive(false);
            box2.SetActive(false);
        }
    
        if(resourcesList.HowManyResources(outputResourceSO) >= 1)
        {
            box.SetActive(true);
        }
        else
        {
            box.SetActive(false);
        }
        //
    }

    private void Product()
    {
        if (resourcesList.TryUse(inputResourceSO, inputAmountRequired))
        {
            resourcesList.Add(outputResourceSO, 1);

            var floatingText = Instantiate(floatingTextPrefab, transform.position + Vector3.up, Quaternion.identity);
            floatingText.SetText($"{inputResourceSO.resourceName} -{inputAmountRequired}\n{outputResourceSO.resourceName}+1");
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
