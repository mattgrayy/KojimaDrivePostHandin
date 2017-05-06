using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Used to get access to scene objects from GameMode scripts.
/// An object containing one of these is automatically created in each GameMode scene when the GameMode class is created.
/// </summary>
public class GameModeReferences : MonoBehaviour
{

    [System.Serializable]
    private struct ObjectReferenceByName
    {
        public string name;
        public GameObject obj;
    }

    [System.Serializable]
    private struct ComponentReferenceByName
    {
        public string name;
        public Component obj;
    }

    public Dictionary<string, GameObject> gameObjects;
    public Dictionary<string, Component> components;

    [SerializeField, HideInInspector]
    private List<ObjectReferenceByName> m_gameObjects = new List<ObjectReferenceByName>();

    [SerializeField, HideInInspector]
    private List<ComponentReferenceByName> m_components = new List<ComponentReferenceByName>();

    [SerializeField, HideInInspector]
    //Holds a reference to the GameMode that owns this object
    private GameModeBase m_gameMode;

    public GameModeBase gameMode
    {
        get
        {
            return m_gameMode;
        }
    }


    void Awake()
    {
        BuildDictionary();
    }

    private void BuildDictionary()
    {
        foreach (ObjectReferenceByName objRefByName in m_gameObjects)
        {
            gameObjects.Add(objRefByName.name, objRefByName.obj);
        }
        foreach (ComponentReferenceByName compRefByName in m_components)
        {
            components.Add(compRefByName.name, compRefByName.obj);
        }
    }
    
}
