using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if UNITY_EDITOR

namespace LayoutAssetBuilderTool
{
    public class ToolIcon : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Image _marking;
        
        public GridCreationTool.Tool tool;
            
        private Toolbar _toolbar;
        
        public void Initialize(Toolbar toolbar)
        {
            _toolbar = toolbar;
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _toolbar.IconClicked(this);
        }

        public void SetMarking(bool mark)
        {
            _marking.enabled = mark;
        }
    }
}

#endif