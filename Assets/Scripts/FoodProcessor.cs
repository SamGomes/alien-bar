﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
using Vector3 = UnityEngine.Vector3;

public class FoodProcessorObjectEvents : MonoBehaviour, IPointerClickHandler
{
    public FoodProcessor logic;
    private Vector3 baseScale;

    public List<IngredientAttr> _addedUtensilAttrs;

    public void Start()
    {
        baseScale = transform.localScale;
    }

    
    public void Update()
    {
        _addedUtensilAttrs = logic.AddedUtensilAttrs;
        
        if (transform.localScale == baseScale && (logic.IsOn == ProcessUnitState.ON))
        {
            transform.localScale = baseScale * 1.2f;
        }
        else if(transform.localScale != baseScale && (logic.IsOn == ProcessUnitState.OFF))
        {
            transform.localScale = baseScale;
        }

    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        //Use this to tell when the user left-clicks on the Button
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            if (logic.IsOn == ProcessUnitState.ON)
                StartCoroutine(logic.TurnOff());
            else
                StartCoroutine(logic.TurnOn());
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        var ingEvents = other.GetComponent<IngredientObjectEvents>();
        if (ingEvents != null)
        {
            ingEvents.isBeingHeld = false;
            Ingredient otherIng = ingEvents.logic;
            if (otherIng.Attributes.Contains(IngredientAttr.UTENSIL))
            {
                if (logic.AddedUtencils.Contains(otherIng) || !logic.ADDUtensil(otherIng))
                {
                    return;
                }
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
                StartCoroutine(logic.TurnOff());
                logic.IngredientInProcess = null;
            }
        }
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
    
    
    public bool ADDUtensil(Ingredient utensilToAdd)
    {
        List<IngredientAttr> utensilToAddCpy = new List<IngredientAttr>(utensilToAdd.Attributes);
        utensilToAddCpy.Remove(IngredientAttr.UTENSIL);
        if (AddedUtencils.Count >= MaxNumUtensils || 
            !ValidUtensil(utensilToAddCpy))
        {
            return false;
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
        return true;
    }

    public bool ValidUtensil(List<IngredientAttr> utensilAttrsToTest)
    {
        return Enumerable.SequenceEqual<IngredientAttr>(RequiredUtensilAttrs, utensilAttrsToTest);
//        bool isValid = false;
//        if (RequiredUtensilAttrs.Count == utensilAttrsToTest.Count)
//        {
//            isValid = true;
//            foreach (var utensil in RequiredUtensilAttrs)
//            {
//                if (!utensilAttrsToTest.Contains(utensil))
//                {
//                    isValid = false;
//                    break;
//                }
//            }
//        }
//        return isValid;
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

        if (IngredientInProcess != null && 
            RequiredUtensilAttrs.Count == 0 || 
            (RequiredUtensilAttrs.Count > 0 && AddedUtencils.Count > 0))
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
                IsOn = ProcessUnitState.OFF;
                TakeUtensilOff(RequiredUtensilAttrs);
            }
            yield return null;
        }
        yield return null;
    }

    public IEnumerator TurnOff()
    {
        IsOn = ProcessUnitState.OFF;
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