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
                    transform.position.y,
                    hit.point.z);
            }
        }
    }
}



public enum IngredientAttr
{
    WHOLE,
    CUT,
    COLD,
    HOT,
    DRINKABLE,
    FRUIT,
    OIL,
    DRINK
}

public class Ingredient
{
    private List<GameObject> _stateObjects;
    private GameObject _gameObject;
    private List<IngredientAttr> _attributes;
    private string _name;
    private int _timeToProcess;

    
    // public List<GameObject> StateObjects => _stateObjects;
    public List<GameObject> GETStateObjects()
    {
        return _stateObjects;
    }

    public List<IngredientAttr> Attributes
    {
        get => _attributes;
        set => _attributes = value;
    }

    // public string Name
    // {
    //     get => _name;
    //     set => _name = value;
    // }
    public string GETName()
    {
        return _name;
    }


    // public int TimeToProcess
    // {
    //     get => _timeToProcess;
    //     set => _timeToProcess = value;
    // }
    public int GETTimeToProcess()
    {
        return _timeToProcess;
    }

        
    public GameObject GETGameObject()
    {
        return _gameObject;
    }

    public Ingredient(bool isTemplate, Camera cam, Vector3 spawnPos, GameObject table, List<GameObject> stateObjects, List<IngredientAttr> attributes, 
        string name, int timeToProcess)
    {
        _stateObjects = stateObjects;

        //only instantiate if it is not a template
        if (!isTemplate)
        {
            Debug.Log(spawnPos);
            _gameObject = Object.Instantiate(_stateObjects[0], spawnPos, Quaternion.identity);
            _gameObject.AddComponent<IngredientObjectEvents>();
            _gameObject.GetComponent<IngredientObjectEvents>().cam = cam;
            _gameObject.GetComponent<IngredientObjectEvents>().logic = this;
        }
        else
        {
            _gameObject = _stateObjects[0];
        }

        _attributes = attributes;
        _name = name;
        _timeToProcess = timeToProcess;
    }
        
    public IEnumerator Process(int machineDelay, List<IngredientAttr> inputtedAttr, List<IngredientAttr> outputtedAttr)
    {
        yield return new WaitForSeconds(_timeToProcess + machineDelay);
        _attributes = _attributes.Except(inputtedAttr).ToList();
        _attributes.AddRange(outputtedAttr);
        
        Object.Destroy(_gameObject);
        _gameObject = Object.Instantiate(_stateObjects[1], 
            _gameObject.transform.position, 
            Quaternion.identity);;
    }

    public List<IngredientAttr> GETAttributes()
    {
        return _attributes;
    }
}