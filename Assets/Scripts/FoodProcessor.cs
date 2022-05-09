using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;

public class FoodProcessorObjectEvents : MonoBehaviour, IPointerClickHandler
{
    public FoodProcessor logic;
    private bool _isOn;
    private Vector3 baseScale;

    public List<IngredientAttr> _addedUtensilAttrs;
    public bool IngredientInProcess;
    
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
                if (logic.AddedUtencils.Contains(otherIng))
                {
                    return;
                }
                
                logic.ADDUtensil(otherIng);
                Destroy(otherIng.GameObject.GetComponent<IngredientObjectEvents>());
                Vector3 pos = gameObject.transform.position;
                otherIng.GameObject.transform.position = new Vector3(pos.x,0,pos.z)
                                                         + logic.AddedUtencils.Count*(Vector3.up*3.0f)
                                                         + Vector3.back*20.0f 
                                                         + Vector3.left*10.0f;
            }
            else
            {
                logic.IngredientInProcess = otherIng;
                IngredientInProcess = true;
                otherIng.GameObject.transform.position = gameObject.transform.position;
            }
        }
        
    }
    private void OnTriggerExit(Collider other)
    { 
        var ingEvents = other.GetComponent<IngredientObjectEvents>();
        if (ingEvents != null)
        {
            Ingredient otherIng = ingEvents.logic;
            if (!otherIng.Attributes.Contains(IngredientAttr.UTENSIL))
//            {
//                logic.TakeUtensilOff(logic.RequiredUtensilAttrs);
//            }
//            else
            {
                _isOn = false;
                StartCoroutine(logic.TurnOff());
                transform.localScale = baseScale;
                
                logic.IngredientInProcess = null;
                IngredientInProcess = false;
                if (isIngProcessed)
                {
                    logic.TakeUtensilOff(logic.RequiredUtensilAttrs);
                }

            }
        }
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
    
    public List<Ingredient> AddedUtencils{ get; set; }

    public List<IngredientAttr> RequiredUtensilAttrs { get; set; }

    public List<IngredientAttr> AddedUtensilAttrs { get; set; }

    public ProcessUnitState IsOn { get; set; }

    public int ProcessingDelay { get; set; }

    private FoodProcessorObjectEvents _foodProcEvents;

    public List<FoodProcessor> LinkedProcessors { get; set; }
    
    public int MaxNumUtensils { get; set; }
    
    public FoodProcessor(
        GameObject gameObject, 
        List<IngredientAttr> acceptedAttrs, 
        List<IngredientAttr> inputAttrs, 
        List<IngredientAttr> outputAttrs, 
        List<IngredientAttr> requiredUtensilAttrs,
        int processingDelay,
        List<FoodProcessor> linkedProcessors,
        int maxNumUtensils)
    {
        GameObject = gameObject;
        _foodProcEvents = GameObject.AddComponent<FoodProcessorObjectEvents>();
        _foodProcEvents.logic = this;
            
        ProcessingDelay = processingDelay;
        AcceptedAttrs = acceptedAttrs;
        InputAttrs = inputAttrs;
        OutputAttrs = outputAttrs;
            
        IngredientInProcess = null;
        
        AddedUtencils = new List<Ingredient>();
        
        AddedUtensilAttrs = new List<IngredientAttr>();
        RequiredUtensilAttrs = requiredUtensilAttrs;

        LinkedProcessors = linkedProcessors;

        MaxNumUtensils = maxNumUtensils;
    }
    
    
    public void ADDUtensil(Ingredient utensilToAdd)
    {
        List<IngredientAttr> utensilToAddCpy = new List<IngredientAttr>(utensilToAdd.Attributes);
        utensilToAddCpy.Remove(IngredientAttr.UTENSIL);
        if (AddedUtencils.Count >= MaxNumUtensils || 
            !ValidUtensil(utensilToAddCpy))
        {
            Object.Destroy(utensilToAdd.GameObject);
            return;
        }
        AddedUtensilAttrs.AddRange(utensilToAddCpy);
        AddedUtencils.Add(utensilToAdd);
        var audioSources = GameObject.GetComponents<AudioSource>();
        if (audioSources.Length > 1)
        {
            AudioSource sound = audioSources[1];
            sound.pitch = Random.Range(0.8f, 1.2f);
            sound.Play();
        }
    }

    public bool ValidUtensil(List<IngredientAttr> utensilAttrsToTest)
    {
        bool isValid = false;
        if (RequiredUtensilAttrs.Count == utensilAttrsToTest.Count)
        {
            foreach (var utensil in RequiredUtensilAttrs)
            {
                if (!utensilAttrsToTest.Contains(utensil))
                {
                    isValid = false;
                }
            }
            isValid = true;
        }

//        foreach (var foodProc in GameObject.GetComponents<FoodProcessorObjectEvents>())
//        {
//            if (foodProc == this._foodProcEvents)
//            {
//                continue;
//            }
//            isValid = isValid || foodProc.logic.ValidUtensil(utensilAttrsToTest);
//        }

        return isValid;
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

        if (IngredientInProcess != null && ValidUtensil(AddedUtensilAttrs))
        {
            if (IsCurrIngrAccepted())
            {
                yield return (GameObject.GetComponent<MonoBehaviour>()
                    .StartCoroutine(
                        IngredientInProcess.Process(ProcessingDelay,
                            InputAttrs, OutputAttrs)
                    ));
                
                if (AddedUtencils.Count > 0)
                {
                    Object.Destroy(AddedUtencils[AddedUtencils.Count - 1].GameObject);
                    AddedUtencils.RemoveAt(AddedUtencils.Count - 1);
                }
                
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