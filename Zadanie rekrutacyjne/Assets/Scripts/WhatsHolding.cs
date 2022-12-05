using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhatsHolding : MonoBehaviour
{
    public Canvas buildingCanvas;
    private GameObject selection;

    private void Start()
    {
        selection = transform.Find("Selected").gameObject;
    }
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {

            if ((hit.transform.gameObject == this.gameObject) && (Input.GetMouseButtonDown(0)))
            {
                buildingCanvas.gameObject.SetActive(true);
                selection.SetActive(true);
            }else if ((hit.transform.gameObject != this.gameObject) && (Input.GetMouseButtonDown(0)))
            {
                buildingCanvas.gameObject.SetActive(false);
                selection.SetActive(false);
            }
        }
    }
}
