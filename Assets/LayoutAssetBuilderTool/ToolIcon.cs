using Codice.Client.Common;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LayoutAssetBuilderTool
{
    public class ToolIcon : MonoBehaviour, IPointerDownHandler
    {
        public GridCreationTool.Tool tool;
            
        private Image _marking;
        private Toolbar _toolbar;
        
        public void Initialize(Toolbar toolbar)
        {
            _toolbar = toolbar;
            _marking = transform.parent.GetComponent<Image>();
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
