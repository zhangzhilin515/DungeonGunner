using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName ="DungeonLevel_",menuName ="Scriptable Objects/Dungeon/Dungeon Level")]

public class DungeonLevelSO : ScriptableObject
{
    public string levelName;
    public List<RoomTemplateSO> roomTemplateList;
    public List<RoomNodeGraphSO> roomNodeGraphList;
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelpUtilities.ValidateCheckEmptyString(this, nameof(levelName), levelName);
        if(HelpUtilities.ValidateCheckEnumerableValues(this,nameof(roomTemplateList),roomTemplateList))
        {
            return;
        }
        if (HelpUtilities.ValidateCheckEnumerableValues(this, nameof(roomNodeGraphList), roomNodeGraphList))
        {
            return;
        }
        bool isEWCorridor = false;
        bool isNSCorridor = false;
        bool isEntrance = false;
        foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
        {
            if(roomTemplateSO==null)
            {
                return;
            }
            if(roomTemplateSO.roomNodeType.isCorridorEW)
            {
                isEWCorridor = true;
            }
            if(roomTemplateSO.roomNodeType.isCorridorNS)
            {
                isNSCorridor = true;
            }
            if(roomTemplateSO.roomNodeType.isEntrance)
            {
                isEntrance = true;
            }
        }
        if(isEWCorridor==false)
        {
            Debug.Log("In " + this.name.ToString() + " No EWCorridor Specified");
        }
        if (isNSCorridor == false)
        {
            Debug.Log("In " + this.name.ToString() + " No NSCorridor Specified");
        }
        if (isEntrance == false)
        {
            Debug.Log("In " + this.name.ToString() + " No Entrance Specified");
        }
        foreach (RoomNodeGraphSO roomNodeGraph in roomNodeGraphList)
        {
            if (roomNodeGraph == null)
                return;

            // Loop through all nodes in node graph
            foreach (RoomNodeSO roomNodeSO in roomNodeGraph.roomNodeList)
            {
                if (roomNodeSO == null)
                    continue;
                // Check that a room template has been specified for each roomNode type

                // Corridors and entrance already checked
                if (roomNodeSO.roomNodeType.isEntrance || roomNodeSO.roomNodeType.isCorridorEW || roomNodeSO.roomNodeType.isCorridorNS || roomNodeSO.roomNodeType.isCorridor || roomNodeSO.roomNodeType.isNone)
                    continue;
                bool isRoomNodeTypeFound = false;
                // Loop through all room templates to check that this node type has been specified
                foreach (RoomTemplateSO roomTemplateSO in roomTemplateList)
                {
                    if (roomTemplateSO == null)
                        continue;
                    if (roomTemplateSO.roomNodeType == roomNodeSO.roomNodeType)
                    {
                        isRoomNodeTypeFound = true;
                        break;
                    }
                }
                if (!isRoomNodeTypeFound)
                    Debug.Log("In " + this.name.ToString() + " : No room template " + roomNodeSO.roomNodeType.name.ToString() + " found for node graph " + roomNodeGraph.name.ToString());
            }
        }
    }
#endif
}
