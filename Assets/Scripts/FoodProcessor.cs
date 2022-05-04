using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

public class FoodProcessorObjectEvents : MonoBehaviour, IPointerClickHandler
{
    public FoodProcessor logic;
    private bool _isOn;
    private Vector3 baseScale;

    public List<IngredientAttr> _addedUtensilAttrs;
    
    public bool isIngProcessed;
    
    public void Start()
    {
        baseScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Use this to tell when the user left-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            _isOn = !_isOn;
            
            if (_isOn)
            {
                transform.localScale = baseScale * 1.2f;
                StartCoroutine(logic.TurnOn());
            }else{
                transform.localScale = baseScale;
                StartCoroutine(logic.TurnOff());
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        transform.localScale = 1.1f * baseScale;
        var ingEvents = other.GetComponent<IngredientObjectEvents>();
        if (ingEvents != null)
        {
            ingEvents.isBeingHeld = false;
            Ingredient otherIng = ingEvents.logic;
            if (otherIng.Attributes.Contains(IngredientAttr.UTENSIL))
            {
                logic.ADDUtensil(otherIng);
            }
            else
            {
                logic.IngredientInProcess = otherIng;
            }
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        logic.IngredientInProcess = null;
        if (isIngProcessed)
        {
            logic.TakeUtensilOff(logic.RequiredUtensilAttrs);
        }
        _isOn = false;
        StartCoroutine(logic.TurnOff());
        transform.localScale = baseScale;
    }


    public void Update()
    {
        _addedUtensilAttrs = logic.AddedUtensilAttrs;
    }
}
    
public enum ProcessUnitState{
    OFF = 0,
    ON = 1
}
public class FoodProcessor
{
    public GameObject GameObject { get; set; }

    public List<IngredientAttr> AcceptedAttrs { get; set; }

    public List<IngredientAttr> InputAttrs { get; set; }

    public List<IngredientAttr> OutputAttrs { get; set; }

    public Ingredient IngredientInProcess { get; set; }

    public List<IngredientAttr> RequiredUtensilAttrs { get; set; }

    public List<IngredientAttr> AddedUtensilAttrs { get; set; }

    public ProcessUnitState IsOn { get; set; }

    public int ProcessingDelay { get; set; }

    private FoodProcessorObjectEvents _foodProcEvents;

    public List<FoodProcessor> LinkedProcessors { get; set; }
    
    public FoodProcessor(
        GameObject gameObject, 
        List<IngredientAttr> acceptedAttrs, 
        List<IngredientAttr> inputAttrs, 
        List<IngredientAttr> outputAttrs, 
        List<IngredientAttr> requiredUtensilAttrs,
        int processingDelay,
        List<FoodProcessor> linkedProcessors)
    {
        GameObject = gameObject;
        _foodProcEvents = GameObject.AddComponent<FoodProcessorObjectEvents>();
        _foodProcEvents.logic = this;
            
        ProcessingDelay = processingDelay;
        AcceptedAttrs = acceptedAttrs;
        InputAttrs = inputAttrs;
        OutputAttrs = outputAttrs;
            
        IngredientInProcess = null;
        
        AddedUtensilAttrs = new List<IngredientAttr>();
        RequiredUtensilAttrs = requiredUtensilAttrs;

        LinkedProcessors = linkedProcessors;
    }
    
    
    public void ADDUtensil(Ingredient utensilToAdd)
    {
//        List<IngredientAttr> utensilAttrsToAdd = 
//            utensilToAdd.Attributes.Except(AddedUtensilAttrs).ToList();
        List<IngredientAttr> utensilToAddCpy = new List<IngredientAttr>(utensilToAdd.Attributes);
        utensilToAddCpy.Remove(IngredientAttr.UTENSIL);
        AddedUtensilAttrs.AddRange(utensilToAddCpy);
        Object.Destroy(utensilToAdd.GameObject);
    }

    public bool HasUtensils()
    {
        if (RequiredUtensilAttrs.Count > 0)
        {
            foreach (var utensil in RequiredUtensilAttrs)
            {
                if (!AddedUtensilAttrs.Contains(utensil))
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool IsCurrIngrAccepted()
    {
        foreach (var attr in AcceptedAttrs)
        {
            if (!IngredientInProcess.Attributes.Contains(attr))
            {
                return false;
            }
        }
        return true;
    }
    
    public IEnumerator TurnOn()
    {
        IsOn = ProcessUnitState.ON;

        if (IngredientInProcess != null && HasUtensils())
        {
            if (IsCurrIngrAccepted())
            {
                yield return (GameObject.GetComponent<MonoBehaviour>()
                    .StartCoroutine(
                        IngredientInProcess.Process(ProcessingDelay,
                            InputAttrs, OutputAttrs)
                    ));

                _foodProcEvents.GetComponent<AudioSource>().Play();
                _foodProcEvents.isIngProcessed = true;
            }
            yield return null;
        }
        yield return null;
    }

    public IEnumerator TurnOff()
    {
        IsOn = ProcessUnitState.OFF;
        _foodProcEvents.isIngProcessed = false;
        yield return null;
    }

    public void ClearAddedUtensilAttributes()
    {
        AddedUtensilAttrs.Clear();
    }

    public void TakeUtensilOff(List<IngredientAttr> ingredsToTake)
    {
        foreach (var utensilAttr in ingredsToTake)
        {
            AddedUtensilAttrs.Remove(utensilAttr);
        }

        foreach (FoodProcessor processor in LinkedProcessors)
        {
            processor.TakeUtensilOff(ingredsToTake);
        }
    }
}