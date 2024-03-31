using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="RoomNodeGraph",menuName ="Scriptable Objects/Dungeon/Room Node Graph")]

public class RoomNodeGraphSO : ScriptableObject
{
    public RoomNodeTypeListSO roomNodeTypeList;
    public List<RoomNodeSO> roomNodeList = new List<RoomNodeSO>();
    public Dictionary<string, RoomNodeSO> roomNodeDictionary = new Dictionary<string, RoomNodeSO>();
    private void Awake()
    {
        LoadRoomNodeDictionary();
    }
    private void LoadRoomNodeDictionary()
    {
        roomNodeDictionary.Clear();
        foreach (RoomNodeSO roomNode in roomNodeList)
        {
            roomNodeDictionary[roomNode.id] = roomNode;
        }
    }
    public RoomNodeSO GetRoomNode(string roomNodeID)
    {
        if(roomNodeDictionary.TryGetValue(roomNodeID,out RoomNodeSO roomNodeSO))
        {
            return roomNodeSO;
        }
        return null;
    }
    public RoomNodeSO GetRoomNode(RoomNodeTypeSO roomNodeType)
    {
        foreach (RoomNodeSO node in roomNodeList)
        {
            if(node.roomNodeType==roomNodeType)
            {
                return node;
            }
        }
        return null;
    }
    public IEnumerable<RoomNodeSO> GetChildRoomNode(RoomNodeSO parentRoomNode)
    {
        foreach (string childRoomNode in parentRoomNode.childRoomNodeIDList)
        {
            yield return GetRoomNode(childRoomNode);
        }
    }
#if UNITY_EDITOR
    public RoomNodeSO roomNodeToDrawLineFrom = null;
    public Vector2 linePosition;
    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }
    public void SetRoomNodeToDrawLine(RoomNodeSO roomNode,Vector2 position)
    {
        roomNodeToDrawLineFrom = roomNode;
        linePosition = position;
    }
#endif
}
