using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "MapSet", menuName = "Data/MapSet", order = 8)]
public class MapSet : ScriptableObject
{
    [SerializeField] private List<ProjectScene> _scenes = new List<ProjectScene>();

    public IReadOnlyCollection<ProjectScene> Scenes => _scenes.AsReadOnly();
}

public enum SceneType { None = -1, Lobby, Game }

[System.Serializable]
public class ProjectScene
{
    [SerializeField] private SceneType _type = SceneType.None;

    [Scene]
    [SerializeField] private string _scene;

    public SceneType Type => _type;
    public string Name => _scene;
}
