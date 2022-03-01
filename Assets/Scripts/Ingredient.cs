using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;
using Object = UnityEngine.Object;

public class IngredientObjectEvents : MonoBehaviour, IPointerClickHandler
{
    public Ingredient logic;
    public bool isBeingHeld;
    public Camera cam;

    public void Start()
    {
        isBeingHeld = true;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Use this to tell when the user right-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            isBeingHeld = !isBeingHeld;
        }
    }

    public void FixedUpdate()
    {
        
        if (isBeingHeld)
        {
            Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                transform.position = new Vector3(hit.point.x,
                    10,
                    hit.point.z);
            }
        }
    }
}



public enum IngredientAttr
{
    CUP,
    PLATE,
    TOOL,
    
    WHOLE,
    CUT,
    COLD,
    HOT,
    
    FRUIT,
    ORANGE,
    LEMON,
    APPLE,
    
    OIL,
    DRINK
}

public class Ingredient
{
    public Camera Cam { get; set; }
    public List<GameObject> StateObjects { get; set; }
    public List<IngredientAttr> Attributes { get; set; }
    public GameObject GameObject { get; set; }
    public int TimeToProcess { get; set; }
    public bool IsUtensil { get; set; }
    public int CurrIngState { get; set; }
    
    public Ingredient(bool isTemplate, bool isUtensil, 
        Camera cam, Vector3 spawnPos, 
        GameObject table, List<GameObject> stateObjects, 
        List<IngredientAttr> attributes, int timeToProcess)
    {
        IsUtensil = isUtensil;
        
        CurrIngState = 0;
        
        StateObjects = stateObjects;
        Cam = cam;
        
        Attributes = attributes;
        TimeToProcess = timeToProcess;
        
        //only instantiate if it is not a template
        if (!isTemplate)
        {
            Debug.Log(spawnPos);
            GameObject = Object.Instantiate(StateObjects[0], spawnPos, Quaternion.identity);
            var events = GameObject.AddComponent<IngredientObjectEvents>();
            events.cam = Cam;
            events.logic = this;
        }
        else
        {
            GameObject = StateObjects[0];
        }

    }
        
    public IEnumerator Process(int machineDelay, List<IngredientAttr> inputtedAttr, List<IngredientAttr> outputtedAttr)
    {
        yield return new WaitForSeconds(TimeToProcess + machineDelay);
        Attributes = Attributes.Except(inputtedAttr).ToList();
        Attributes.AddRange(outputtedAttr);
        
        Object.Destroy(GameObject);
        GameObject = Object.Instantiate(StateObjects[++CurrIngState], 
            GameObject.transform.position, 
            Quaternion.identity);
        var events = GameObject.AddComponent<IngredientObjectEvents>();
        events.cam = Cam;
        events.logic = this;
    }

}