using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IngredientStuff
{
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
    
    public enum IngredientState
    {
        UNPROCESSED = 0,
        IN_PROCESS = 1,
        PROCESSED = 2
    }
    
    public class Ingredient
    {
        private List<GameObject> _statePrefabs;
        private List<IngredientAttr> _attributes;
        private string _name;
        private IngredientState _currState;
        private int _timeToProcess;

        public Ingredient(List<GameObject> statePrefabs, List<IngredientAttr> attributes, string name, int timeToProcess)
        {
            this._statePrefabs = statePrefabs;
            this._attributes = attributes;
            this._name = name;
            this._currState = IngredientState.UNPROCESSED;
            this._timeToProcess = timeToProcess;
        }

        public GameObject getPrefab()
        {
            return _statePrefabs[(int) _currState];
        }
        
        public IEnumerator Process(int machineDelay,List<IngredientAttr> inputtedAttr, List<IngredientAttr> outputtedAttr)
        {
            this._currState = IngredientState.IN_PROCESS;
            yield return new WaitForSeconds(_timeToProcess + machineDelay);
            this._currState = IngredientState.PROCESSED;
        }

        public List<IngredientAttr> GETAttributes()
        {
            return _attributes;
        }
    }
}