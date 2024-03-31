using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
public class RoomNodeSO:ScriptableObject
{
    public string id;
    public List<string> parentRoomNodeIDList = new List<string>();
    public List<string> childRoomNodeIDList = new List<string>();
    public RoomNodeGraphSO roomNodeGraph;
    public RoomNodeTypeSO roomNodeType;
    public RoomNodeTypeListSO roomNodeTypeList;
#if UNITY_EDITOR
    public Rect rect;
    public bool isLeftClickDragging=false;
    public bool isSelected = false;
    public void Initialise(Rect rect,RoomNodeGraphSO nodeGraph,RoomNodeTypeSO roomNodeType)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "RoomNode";
        this.roomNodeGraph = nodeGraph;
        this.roomNodeType = roomNodeType;
        roomNodeTypeList = GameResources.Instance.roomNodeTypeList;
    }
    public void Draw(GUIStyle nodeStyle)
    {
        GUILayout.BeginArea(rect, nodeStyle);
        EditorGUI.BeginChangeCheck();
        if(parentRoomNodeIDList.Count>0||roomNodeType.isEntrance)
        {
            EditorGUILayout.LabelField(roomNodeType.roomNodeTypeName);
        }
        else
        {
            int selected = roomNodeTypeList.list.FindIndex(x => x == roomNodeType);
            int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());
            roomNodeType = roomNodeTypeList.list[selection];
            //如果房间类型改变后使得孩子房间的连接变得不可用
            if(roomNodeTypeList.list[selected].isCorridor&&!roomNodeTypeList.list[selection].isCorridor
                || !roomNodeTypeList.list[selected].isCorridor&&roomNodeTypeList.list[selection].isCorridor
                ||!roomNodeTypeList.list[selected].isBossRoom&&roomNodeTypeList.list[selection].isBossRoom)
            {
                if (childRoomNodeIDList.Count > 0)
                {
                    for (int i = childRoomNodeIDList.Count - 1; i >= 0; i--)
                    {
                        RoomNodeSO childRoomNode = roomNodeGraph.GetRoomNode(childRoomNodeIDList[i]);
                        if (childRoomNode != null)
                        {
                            RemoveChildNodeFromList(childRoomNode.id);
                            childRoomNode.RemoveParentNodeFromList(id);
                        }
                    }
                }
            }
        }
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(this);
        GUILayout.EndArea();
    }
    public string[] GetRoomNodeTypesToDisplay()
    {
        string[] roomArray = new string[roomNodeTypeList.list.Count];
        for (int i = 0; i < roomNodeTypeList.list.Count; i++)
        {
            if(roomNodeTypeList.list[i].displayInNodeGraphEditor)
            {
                roomArray[i] = roomNodeTypeList.list[i].roomNodeTypeName;
            }
        }
        return roomArray;
    }
    public void ProcessEvents(Event currentEvent)
    {
        switch(currentEvent.type)
        {
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            default:
                break;
        }
    }
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        if(currentEvent.button==0)
        {
            ProcessLeftClickDownEvent();
        }
        else if(currentEvent.button==1)
        {
            ProcessRightClickDownEvent(currentEvent);
        }
    }
    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;
        isSelected = !isSelected;
    }
    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        roomNodeGraph.SetRoomNodeToDrawLine(this, currentEvent.mousePosition);
    }
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        if (currentEvent.button == 0)
        {
            ProcessLeftClickUpEvent();
        }
    }
    private void ProcessLeftClickUpEvent()
    {
        if(isLeftClickDragging)
        {
            isLeftClickDragging = false;
        }
    }
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        if(currentEvent.button==0)
        {
            ProcessLeftClickDragEvent(currentEvent);
        }
    }
    private void ProcessLeftClickDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;
        DragNode(currentEvent.delta);
        GUI.changed = true;
    }
    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }
    public bool AddChildNodeToList(string childID)
    {
        if(IsChildRoomValid(childID))
        {
            childRoomNodeIDList.Add(childID);
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool IsChildRoomValid(string childID)
    {
        bool isConnectedBossNodeAlready = false;
        foreach(RoomNodeSO roomNode in roomNodeGraph.roomNodeList)
        {
            if(roomNode.roomNodeType.isBossRoom&&roomNode.parentRoomNodeIDList.Count>0)
            {
                isConnectedBossNodeAlready = true;
            }
        }
        if(roomNodeGraph.GetRoomNode(childID).roomNodeType.isBossRoom&&isConnectedBossNodeAlready)
        {
            return false;
        }
        if (roomNodeGraph.GetRoomNode(childID).roomNodeType.isNone)
        {
            return false;
        }
        if(parentRoomNodeIDList.Contains(childID))
        {
            return false;
        }
        if(childRoomNodeIDList.Contains(childID))
        {
            return false;
        }
        if(id==childID)
        {
            return false;
        }
        if(roomNodeGraph.GetRoomNode(childID).parentRoomNodeIDList.Count>0)
        {
            return false;
        }
        //走廊不能跟走廊相连
        if(roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor&&roomNodeType.isCorridor)
        {
            return false;
        }
        //房间只能跟走廊相连
        if(!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor && !roomNodeType.isCorridor)
        {
            return false;
        }
        if(roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor&&childRoomNodeIDList.Count>=Settings.maxChildCorridors)
        {
            return false;
        }
        if(roomNodeGraph.GetRoomNode(childID).roomNodeType.isEntrance)
        {
            return false;
        }
        //保证走廊房间只跟一个房间相连
        if(!roomNodeGraph.GetRoomNode(childID).roomNodeType.isCorridor&&childRoomNodeIDList.Count>0)
        {
            return false;
        }
        return true;
    }
    public bool AddParentNodeToList(string parentID)
    {
        parentRoomNodeIDList.Add(parentID);
        return true;
    }
    public bool RemoveChildNodeFromList(string childID)
    {
        if(childRoomNodeIDList.Contains(childID))
        {
            childRoomNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }
    public bool RemoveParentNodeFromList(string childID)
    {
        if (parentRoomNodeIDList.Contains(childID))
        {
            parentRoomNodeIDList.Remove(childID);
            return true;
        }
        return false;
    }
#endif
}
