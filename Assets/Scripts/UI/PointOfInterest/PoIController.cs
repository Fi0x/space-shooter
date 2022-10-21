using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PoIController : MonoBehaviour
{
    public GameObject PoIElement;

    private Dictionary<PointOfInterest, PoIUI> elements = new Dictionary<PointOfInterest, PoIUI>();

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelLoaded;
    }

    private void Awake()
    {
        elements = new Dictionary<PointOfInterest, PoIUI>();
        PointOfInterest.OnPoIAdded += AddPoI;
        PointOfInterest.OnPoIRemoved += RemovePoI;
    }

    private void AddPoI(PointOfInterest poi)
    {
        if (!elements.ContainsKey(poi))
        {
            var newElement = Instantiate(PoIElement, transform);
            var poiUI = newElement.GetComponent<PoIUI>();
            elements.Add(poi, newElement.GetComponent<PoIUI>());
            poiUI.SetPoi(poi);
        }
    }
    
    private void RemovePoI(PointOfInterest poi)
    {
        if (elements.ContainsKey(poi))
        {
            Destroy(elements[poi].gameObject);
            elements.Remove(poi);
        }
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelLoaded;
        PointOfInterest.OnPoIAdded -= AddPoI;
        PointOfInterest.OnPoIRemoved -= RemovePoI;
        foreach (var element in elements.Keys)
        {
            Destroy(elements[element].gameObject);
        }
        elements.Clear();
    }

    private void OnLevelLoaded(Scene scene, LoadSceneMode mode)
    {
        foreach (var element in elements)
        {
            if (element.Key == null) elements.Remove(element.Key);
        }
    }
}
