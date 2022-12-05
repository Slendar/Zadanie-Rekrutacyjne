using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WorkerNavMesh : MonoBehaviour
{
    //List of Worker states
    private enum State
    {
        Idle,
        MovingTo,
        GettingResource,

    }

    //Resources
    public GameResourceSO resourceSO_Wood;
    public GameResourceSO resourceSO_Chair;
    public GameResourcesList resourcesList;

    //Buildings
    private GameObject building;
    private GameResourcesList lastBuilding;

    //Worker
    private NavMeshAgent navMeshAgent;
    private State state;
    private GameObject box;

    [SerializeField] private float speedWithoutBox = 2f; //Speed of worker without carrying resources
    [SerializeField] private float speedWithBox = 1f; //Speed of worker when carrying resources
    [SerializeField] private int carryCapacity = 2; //How many same items can Worker carry
    private int manageAmount = 1; //How many same resources can Worker take and give to building (potential feature)
    private int carrying = 0; //How many same items Worker currently carrying
    [SerializeField] private float timeToManageResource = 1f; //Time needed to take or give resource back
    private float timeProgress = 0f;

    //Animations
    private Animator animator;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        state = State.Idle;
        box = gameObject.transform.GetChild(3).gameObject;
        animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        
        if(building == null)
        {
            animator.SetBool("IsIdle", true);
            state = State.Idle;
        }
        else
        {
            lastBuilding = building.GetComponentInParent<GameResourcesList>();
        }
        
            switch (state)
            {
                case State.Idle:
                    if (animator.GetBool("IsIdle"))
                    {
                        animator.SetBool("IsIdle", true);
                        if (carrying > 0)
                        {
                            ShowBox(true);
                            building = GameManager.GetStoragePosition_Static();
                            if (building != null)
                            {
                                state = State.MovingTo;
                            }
                        }
                        else
                        {
                            ShowBox(false);
                            building = GameManager.GetCarpenterTakeawayPosition_Static();
                            if(building != null)
                            {
                                state = State.MovingTo;
                            }
                        }
                        
                    }
                    break;
                case State.MovingTo:
                        if(carrying == 0)
                        {
                            MoveTo(building.transform.position, State.GettingResource);
                        }
                        else
                        {
                            MoveTo(building.transform.position, State.Idle);
                        }     
                    break;
                case State.GettingResource:
                    if (animator.GetBool("IsIdle"))
                    {   //Check if Worker can take more resources and if building have these resources
                        if (carrying < carryCapacity && (carrying + manageAmount) <= carryCapacity && ((building.tag == "Woodcutter" && CanTakeResource(building, resourceSO_Wood)) || (building.tag == "Carpenter" && CanTakeResource(building, resourceSO_Chair)))) //getting resources from building
                        {   //Take resources over time
                            timeProgress += Time.deltaTime;
                            if (timeProgress > timeToManageResource && building.tag == "Woodcutter" && CanTakeResource(building, resourceSO_Wood))
                            {
                                building.GetComponentInParent<ExtractionBuilding>().Remove(resourceSO_Wood, manageAmount);
                                Take(resourceSO_Wood, manageAmount);
                            }
                            else if (timeProgress > timeToManageResource && building.tag == "Carpenter" && CanTakeResource(building, resourceSO_Chair))
                            {
                                building.GetComponentInParent<ProductionBuilding>().Remove(resourceSO_Chair, manageAmount);
                                Take(resourceSO_Chair, manageAmount);
                            }
                            //Base for taking resources from warehouse
                            /*else if (timeProgress > timeToManageResource && building.tag == "Warehouse")
                            {
                                building.GetComponentInParent<StorageBuilding>().Remove( , manageAmount);
                                Take( , manageAmount);
                            }*/
                        }
                        else 
                        {//if cant take anymore check if Worker have wood or chairs and set new destination
                            if (resourcesList.HowManyResources(resourceSO_Wood) > 0)
                            {
                                lastBuilding.SetOcuppied(false);
                                building = GameManager.GetCarpenterDeliveryPosition_Static();
                                state = State.MovingTo;
                            }
                            else if (resourcesList.HowManyResources(resourceSO_Chair) > 0)
                            {
                                lastBuilding.SetOutOcuppied(false);
                                building = GameManager.GetStoragePosition_Static();
                                state = State.MovingTo;
                            }  
                        }   
                    }
                    break;              
            }
    }

    //Moves Worker to destination and after arrival puts him into next state
    private void MoveTo(Vector3 destination, State nextstate)
    {
        navMeshAgent.destination = destination;

        if(carrying == 0)
        {
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsMoving", true);
            ShowBox(false);
            navMeshAgent.speed = speedWithoutBox;
        }
        else
        {
            animator.SetBool("IsIdle", false);
            animator.SetBool("IsCarrying", true);
            ShowBox(true);
            navMeshAgent.speed = speedWithBox;
        }

        if (Arrived())
        {

            animator.SetBool("IsIdle", true);
            animator.SetBool("IsMoving", false);
            animator.SetBool("IsCarrying", false);

            timeProgress += Time.deltaTime;
            if (timeProgress > timeToManageResource && resourcesList.HowManyResources(resourceSO_Wood) > 0)
            {
                if(building.tag == "Carpenter")
                {
                    building.GetComponentInParent<ProductionBuilding>().Add(resourceSO_Wood, manageAmount);
                    Give(resourceSO_Wood, manageAmount);
                }
                else
                {
                    building.GetComponentInParent<StorageBuilding>().Add(resourceSO_Wood, manageAmount);
                    Give(resourceSO_Wood, manageAmount);
                }
                
                
            }
            else if (timeProgress > timeToManageResource && resourcesList.HowManyResources(resourceSO_Chair) > 0)
            {
                building.GetComponentInParent<StorageBuilding>().Add(resourceSO_Chair, manageAmount);
                Give(resourceSO_Chair, manageAmount);
            }
            
            if(carrying == 0)
            {
                if(nextstate != State.GettingResource)
                {
                    lastBuilding.SetOcuppied(false);
                }
                ShowBox(false);
                state = nextstate;
            }            
        }
    }

    //Show or Hide box
    private void ShowBox(bool maybe)
    {
        box.SetActive(maybe);
    }

    //Adding resources to Worker and removing from destination
    private void Take(GameResourceSO resourceSO, int amount)
    {
        carrying += amount;
        resourcesList.Add(resourceSO, amount);
        ShowBox(true);
        timeProgress = 0f;
    }

    //Removing resources from worker and giving to destination
    private void Give(GameResourceSO resourceSO, int amount)
    {
        carrying -= amount;
        resourcesList.Remove(resourceSO, amount);
        timeProgress = 0f;
    }

    //Check if building have enough resources to give to Worker
    private bool CanTakeResource(GameObject building, GameResourceSO resourceSO)
    {
        if(building.GetComponentInParent<GameResourcesList>().HowManyResources(resourceSO) >= manageAmount)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    //Check if Worker arrived for destination in NavMesh
    private bool Arrived()
    {
        if (!navMeshAgent.pathPending)
        {
            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                if (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true; 
                }
            }
        }
        return false;
    }
}
