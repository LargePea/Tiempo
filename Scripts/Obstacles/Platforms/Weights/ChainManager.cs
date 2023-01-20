using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Sirenix.OdinInspector;

public class ChainManager : MonoBehaviour
{
    [SerializeField] private Transform _maxHeight;
    [SerializeField] private List<GameObject> _links;

    [Header("Chain Auto Build")]
    [SerializeField] private float _scale = 1f;
    [SerializeField] private Transform _lowerBound;
    [SerializeField] private Transform _upperBound;
    [SerializeField] private GameObject _chainPrefab;
    [SerializeField] private Transform _parentObject;
    private float ChainDistance => 0.4f * _scale;

    private GameObject _currentLink;
    private int _currentIndex;
    private float _maxYLevel;

    private void Awake()
    {
        if (_maxHeight == null) return;

        if(_links.Count > 0)
        {
            _currentLink = _links[0];
            _maxYLevel = _maxHeight.position.y;
        }
        else gameObject.SetActive(false);
    }

    private void Update()
    {
        //dont calculate hiding if there is not limit
        if (_maxHeight == null) return;

        //reactivate current link if deactivated
        if (!_currentLink.activeInHierarchy && _currentLink.transform.position.y < _maxYLevel)
        {
            _currentLink.SetActive(true);
            return;
        }

        //Check if link above is below threshold
        if (_currentIndex - 1 >= 0 && _links[_currentIndex - 1].transform.position.y < _maxYLevel)
        {
            _currentLink = _links[_currentIndex - 1];
            _currentIndex--;
            _currentLink.SetActive(true);
            return;
        }

        //check currentLink is below threshold
        if (_currentLink.transform.position.y > _maxYLevel)
        {
            _currentLink.SetActive(false);
            _currentIndex = Mathf.Clamp(_currentIndex + 1, 0, _links.Count - 1);
            _currentLink = _links[_currentIndex];
        }
    }

    [HorizontalGroup("Split", 0.5f)]
    [Button("Recalculate Chain")]
    private void RecalculateChain()
    {
        float displacement = (float) Math.Round(_upperBound.transform.position.y - _lowerBound.transform.position.y, 2);
        int chainCount = (int) (displacement / ChainDistance) + 1;

        if(_links.Count > 0 && _links[0].transform.localScale.x != _scale)
        {
            for(int i = 0; i < _links.Count; i++)
            {
                _links[i].transform.localScale = new Vector3(_scale, _scale, _scale);
                _links[i].transform.localPosition = new Vector3(0, _lowerBound.localPosition.y + ((_links.Count - (i + 1)) * ChainDistance), 0);
            }
        }

        if(chainCount > _links.Count)
        {
            //add more
            int currentChain = _links.Count;
            while(currentChain < chainCount)
            {
                //spawn in new chain
                _links.Insert(0, Instantiate(_chainPrefab, _parentObject, false));
                _links[0].transform.localPosition = new Vector3(0, _lowerBound.localPosition.y + (currentChain * ChainDistance), 0);
                _links[0].transform.localRotation = Quaternion.Euler(0, currentChain % 2 == 0 ? 90 : 0, 0);
                _links[0].transform.localScale = new Vector3(_scale, _scale, _scale);
                currentChain++;
            }
        }

        else if(chainCount < _links.Count)
        {
            //remove top ones
            while(_links.Count > chainCount)
            {
                DestroyImmediate(_links[0]);
                _links.RemoveAt(0);
            }
        }
    }

    [HorizontalGroup("Split/right")]
    [Button("Clear Chains")]
    private void ClearChains()
    {
        foreach(var link in _links)
        {
            if(link != null) DestroyImmediate(link);
        }

        _links.Clear();
    }
}
