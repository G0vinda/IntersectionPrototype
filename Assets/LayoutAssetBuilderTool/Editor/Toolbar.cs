using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

namespace LayoutAssetBuilderTool
{
    public class Toolbar : MonoBehaviour
    {
        [SerializeField] private GridCreationTool gridCreationTool;
        [Header("Icon References")]
        [SerializeField] private ToolIcon npcIcon;
        [SerializeField] private ToolIcon waterIcon;
        [SerializeField] private ToolIcon parkIcon; 

        private void Start()
        {
            npcIcon.Initialize(this);
            waterIcon.Initialize(this);
            parkIcon.Initialize(this);
        }

        public void IconClicked(ToolIcon clickedIcon)
        {
            UnmarkAllIcons();
            if (gridCreationTool.CurrentTool == clickedIcon.tool)
            {
                gridCreationTool.CurrentTool = GridCreationTool.Tool.None;
            }
            else
            {
                gridCreationTool.CurrentTool = clickedIcon.tool;
                clickedIcon.SetMarking(true);
            }
        }

        private void UnmarkAllIcons()
        {
            npcIcon.SetMarking(false);
            waterIcon.SetMarking(false);
            parkIcon.SetMarking(false);
        }
    }
}

#endif