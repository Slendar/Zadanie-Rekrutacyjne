using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameResourcesList : MonoBehaviour
{
    [HideInInspector]
    public List<GameResource> resources;
    public List<GameResourceSO> resourceSOs;

    List<GameResourceView> resourceViews;
    [SerializeField]
    GameResourceView resourceViewPrefab;

    [SerializeField]
    Transform resourceViewsParent;

    /*[HideInInspector]*/ public bool ocuppied = false;
    /*[HideInInspector]*/ public bool outOcuppied = false;

    // Start is called before the first frame update
    void Start()
    {
        resources = new List<GameResource>();
        resourceViews = new List<GameResourceView>();

        foreach (var resourceSO in resourceSOs)
        {
            CreateResource(resourceSO);
        }
    }

    public void SetOcuppied(bool maybe)
    {
        ocuppied = maybe;
    }
    public void SetOutOcuppied(bool maybe)
    {
        outOcuppied = maybe;
    }
    public bool CheckIfOutOcuppied()
    {
        return outOcuppied;
    }
    public bool CheckIfOcuppied()
    {
        return ocuppied;
    }


    public void Add(GameResourceSO resourceSO, int amount)
    {
        var resource = resources.Find((x) => x.resourceSO == resourceSO);

        if (resource == null)
        {
            CreateResource(resourceSO);
        }

        var resourceView = resourceViews.Find((x) => x.resourceSO == resourceSO);

        resource.amount += amount;
        resourceView.UpdateAmount(resource.amount);
    }

    public void Remove(GameResourceSO resourceSO, int amount)
    {
        var resource = resources.Find((x) => x.resourceSO == resourceSO);

        var resourceView = resourceViews.Find((x) => x.resourceSO == resourceSO);

        if (resource.amount >= amount)
        {
            resource.amount -= amount;
            resourceView.UpdateAmount(resource.amount);
        }
    }

    public bool TryUse(GameResourceSO resourceSO, int amount)
    {
        var resource = resources.Find((x) => x.resourceSO == resourceSO);

        if (resource == null)
        {
            CreateResource(resourceSO);
        }

        var resourceView = resourceViews.Find((x) => x.resourceSO == resourceSO);

        if (amount > resource.amount)
        {
            return false;
        }

        resource.amount -= amount;
        resourceView.UpdateAmount(resource.amount);

        return true;
    }

    public int HowManyResources(GameResourceSO resourceSO)
    {
        var resource = resources.Find((x) => x.resourceSO == resourceSO);

        if (resource == null)
        {
            return 0;
        }

        var resourceView = resourceViews.Find((x) => x.resourceSO == resourceSO);

        return resource.amount;
    }

    private void CreateResource(GameResourceSO resourceSO)
    {
        var resource = new GameResource(resourceSO);
        resources.Add(resource);

        GameResourceView resourceView = Instantiate<GameResourceView>(resourceViewPrefab, resourceViewsParent);
        resourceView.resourceSO = resourceSO;
        resourceView.UpdateResourceName(resourceSO.resourceName);
        resourceViews.Add(resourceView);
    }
}
