using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(DungeonController))]
public class DungeonControllerEditor : Editor {
    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        DungeonController dc = (DungeonController)target;

        if (GUILayout.Button("GENERATE")) {
            dc.Generate();
        }
        if (GUILayout.Button("CLEAR")) {
            dc.Clear();
        }
        EditorUtility.SetDirty(dc);
    }
}

public class DungeonController : MonoBehaviour {
    [Header("TWEAKABLES: ")]
    [SerializeField] private int roomGoal = 5;
    [SerializeField] private int maxRetries = 5;
    [SerializeField] private int maxSequentCorners = 3;
    [SerializeField] private LayerMask lm = 3;

    [Header("ASSIGNABLES: ")]
    [SerializeField] private Room startRoom = null;
    [SerializeField] private Room endRoom = null;
    [SerializeField] private Room[] rooms = null;

    private List<GameObject> spawnedRooms = new List<GameObject>();
    private List<Room> spawnedRoomObjects = new List<Room>();
    private List<Transform> emptyDirs = new List<Transform>();
    private int cornersInaRow = 0;
    private int retriesInaRow = 0;

    public void ClearLog() {
        var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    public void Clear() {
        if(transform.childCount > 0) {
            for (int i = transform.childCount - 1; i >= 0; i--) {
                Transform child = transform.GetChild(i);
                DestroyImmediate(child.gameObject);
            }
        }

        cornersInaRow = 0;
        retriesInaRow = 0;
        emptyDirs.Clear();
        spawnedRooms.Clear();
        spawnedRoomObjects.Clear();
        ClearLog();
    }

    public void Generate() {
        Clear();
        GameObject initialRoom = Instantiate(startRoom.prefab, Vector3.zero, Quaternion.identity);
        spawnedRooms.Add(initialRoom);
        spawnedRoomObjects.Add(startRoom);
        initialRoom.transform.SetParent(transform, true);

        var dirs = GetDirections(initialRoom);
        for (int i = 0; i < dirs.Count; i++) {
            emptyDirs.Add(dirs[i]);
        }

        if(emptyDirs.Count > 0) {
            GenerateRoom();
        }
    }

    private bool CanPlaceRoom(GameObject newRoom, Room roomObj) {
        var pos = newRoom.transform.GetChild(0).transform.position;
        var size = new Vector3(roomObj.size.x, roomObj.size.x, roomObj.size.x);

        RaycastHit hit;
        if (Physics.BoxCast(pos, size, transform.up, out hit, Quaternion.identity, 0.5f, lm)) {
            if (hit.collider != null) {
                print("Hit: " + hit.transform.name);
                return false;
            }
        }

        return true;
    }

    private void OnDrawGizmos() {
        for (int i = 0; i < spawnedRooms.Count; i++) {
            var pos = spawnedRooms[i].transform.GetChild(0).transform.position;
            var size = new Vector3(spawnedRoomObjects[i].size.x, spawnedRoomObjects[i].size.x, spawnedRoomObjects[i].size.x);
            RaycastHit hit;

            if (Physics.BoxCast(pos, size, transform.up, out hit, Quaternion.identity, 0.5f, lm)) {
                if (hit.collider != null) {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(pos, size);
                }
            } else {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(pos, size);
            }
        }
    }

    private float GetRoomRadius (GameObject room) {
        var col = room.GetComponentInChildren<Collider>();
        return Mathf.Sqrt(Mathf.Pow(col.bounds.size.x, 2) + Mathf.Pow(col.bounds.size.z, 2)) / 2;
    }

    private List<Transform> GetDirections(GameObject parent) {
        List<Transform> directions = new List<Transform>();
        foreach (Transform child in parent.transform) {
            if (child.CompareTag("direction")) {
                directions.Add(child.transform);
            }
        }
        return directions;
    }

    private void GenerateRoom() {
        Room selectedRoom = endRoom;

        if (spawnedRooms.Count < roomGoal) {
            selectedRoom = rooms[Random.Range(0, rooms.Length)];
            if (selectedRoom.name == "CORNER" || selectedRoom.name == "CORNER_2") {
                cornersInaRow++;
            } else {
                cornersInaRow = 0;
            }

            if (cornersInaRow > maxSequentCorners) {
                selectedRoom = rooms[0];
            }
        }

        Transform selectedDir = emptyDirs[Random.Range(0, emptyDirs.Count)];
        GameObject newRoom = Instantiate(selectedRoom.prefab);

        newRoom.transform.position = selectedDir.position;
        newRoom.transform.eulerAngles = selectedDir.eulerAngles + selectedRoom.rot;
        newRoom.transform.SetParent(transform, true);
        newRoom.transform.name = newRoom.transform.name + "_" + spawnedRooms.Count;

        var canPlaceRoom = CanPlaceRoom(newRoom, selectedRoom);

        if (!canPlaceRoom) {
            if (retriesInaRow < maxRetries) {
                retriesInaRow++;
                DestroyImmediate(newRoom);
                GenerateRoom();
                return;
            }
            selectedRoom = endRoom;
            // print("max retries reached, no solution, place endRoom");
        }

        retriesInaRow = 0;
        emptyDirs.Remove(selectedDir);
        spawnedRooms.Add(newRoom);
        spawnedRoomObjects.Add(selectedRoom);

        var dirs = GetDirections(newRoom);
        for (int i = 0; i < dirs.Count; i++) {
            emptyDirs.Add(dirs[i]);
        }

        if (emptyDirs.Count > 0) {
            GenerateRoom();
        } else {
            if (spawnedRooms.Count > 0 && spawnedRooms.Count < roomGoal) {
                Generate();
            }
        }
    }
}
