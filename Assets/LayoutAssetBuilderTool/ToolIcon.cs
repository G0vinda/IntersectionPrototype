using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LayoutAssetBuilderTool
{
    public class ToolIcon : MonoBehaviour, IPointerDownHandler
    {
        private Image _marking;
        private Toolbar _toolbar;
        
        public void Initialize(Toolbar toolbar)
        {
            _toolbar = toolbar;
            _marking = GetComponentInParent<Image>();
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            _toolbar.IconClicked(this);
        }

        public void SetMarking(bool mark)
        {
            _marking.gameObject.SetActive(true);
        }
    }
}
