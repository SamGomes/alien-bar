using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

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

    public class Ingredient
    {
        private List<GameObject> _stateObjects;
        private GameObject _gameObject;
        private List<IngredientAttr> _attributes;
        private string _name;
        private int _timeToProcess;

        public Ingredient(GameObject table, List<GameObject> stateObjects, List<IngredientAttr> attributes, 
            string name, int timeToProcess)
        {
            _stateObjects = stateObjects;
            _gameObject = Object.Instantiate(_stateObjects[0]);
            _gameObject.transform.position = table.transform.position;
            
            _attributes = attributes;
            _name = name;
            _timeToProcess = timeToProcess;
        }

        public GameObject getPrefab()
        {
            return _stateObjects[0];
        }
        
        public IEnumerator Process(int machineDelay, List<IngredientAttr> inputtedAttr, List<IngredientAttr> outputtedAttr)
        {
            yield return new WaitForSeconds(_timeToProcess + machineDelay);
            _attributes = _attributes.Except(inputtedAttr).ToList();
            _attributes.AddRange(outputtedAttr);
        }

        public List<IngredientAttr> GETAttributes()
        {
            return _attributes;
        }
    }
}