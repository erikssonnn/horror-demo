using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="new Room", menuName = "Custom/Room")]
public class Room : ScriptableObject {
    public new string name;
    public GameObject prefab;
    public Vector3 rot;
    public Vector2Int size;
}
