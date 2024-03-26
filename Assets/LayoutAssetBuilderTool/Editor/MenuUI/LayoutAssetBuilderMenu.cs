using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;
using System.Linq;

#if UNITY_EDITOR

namespace LayoutAssetBuilderTool
{
    public class LayoutAssetBuilderMenu : MonoBehaviour
    {
        [SerializeField] private TextAsset layoutBlockDataFile;
        [SerializeField] private LayoutFieldContainer layoutFieldContainer;
        [SerializeField] private GameObject menuBackground;
        [SerializeField] private GameObject editBackground;
        [SerializeField] private GridCreationTool gridCreationTool;
        [SerializeField] private Toolbar toolbar;
        [SerializeField] private DeleteLayoutConfirmDialog deleteLayoutConfirmDialog;
        [SerializeField] private GameObject dragIndicatorPrefab;

        private List<CityLayout.LayoutBlockData> _layouts;
        private Transform[] _layoutTransforms;
        private bool _isDragging;

        #region OnEnable/OnDisable

        private void OnEnable()
        {
            LayoutUIField.EditButtonPressed += OpenLayoutForEdit;
            LayoutUIField.DeleteButtonPressed += OpenDeleteConfirm;
            LayoutUIField.CopyButtonPressed += CopyLayout;
            LayoutUIField.DragStarted += OnLayoutUIElementDragStarted;
            LayoutUIField.DragEnded += OnLayoutUIElementDragEnded;
            LayoutUIField.NameChanged += ChangeLayoutName;
            LayoutUIField.DifficultyChanged += ChangeLayoutDifficulty;
        }

        private void OnDisable()
        {
            LayoutUIField.EditButtonPressed -= OpenLayoutForEdit;
            LayoutUIField.DeleteButtonPressed -= OpenDeleteConfirm;
            LayoutUIField.CopyButtonPressed -= CopyLayout;
            LayoutUIField.DragStarted -= OnLayoutUIElementDragStarted;
            LayoutUIField.DragEnded -= OnLayoutUIElementDragEnded;
            LayoutUIField.NameChanged -= ChangeLayoutName;
            LayoutUIField.DifficultyChanged -= ChangeLayoutDifficulty;
        }

        #endregion

        private void Start()
        {
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(layoutBlockDataFile));
            var layoutsString = layoutBlockDataFile.ToString();
            _layouts = JsonConvert.DeserializeObject<List<CityLayout.LayoutBlockData>>(layoutsString);
            if (_layouts == null)
            {
                _layouts = new List<CityLayout.LayoutBlockData>();
            }

            _layoutTransforms = layoutFieldContainer.SetupLayouts(_layouts);
        }

        public void OpenNewLayout(string name, int difficulty)
        {
            var newLayout = new CityLayout.LayoutBlockData(name, difficulty);
            EnterLayoutBuilderTool(newLayout);
        }

        private void OpenLayoutForEdit(int index)
        {
            var layoutToEdit = _layouts.Single(layout => layout.id == index);
            EnterLayoutBuilderTool(layoutToEdit);          
        }

        public void OpenDeleteConfirm(int index)
        {
            var layoutToDelete = _layouts.Single(layout => layout.id == index);
            deleteLayoutConfirmDialog.Open(layoutToDelete.name, index);
        }

        private void OnLayoutUIElementDragStarted(int layoutIndex)
        {
            StartCoroutine(LayoutUIElementDragging(layoutIndex));
        }

        private void OnLayoutUIElementDragEnded()
        {
            _isDragging = false;
        }

        private IEnumerator LayoutUIElementDragging(int layoutIndex)
        {
            _isDragging = true;
            var mouseYDiffNeeded = 5f;
            
            var startMouseY = Input.mousePosition.y;
            var mouseYDiff = 0f;
            do
            {
                mouseYDiff = Mathf.Abs(startMouseY - Input.mousePosition.y);
                if(!_isDragging)
                    yield break;

                yield return null;
            } while(mouseYDiff < mouseYDiffNeeded);

            var dragIndicator = Instantiate(dragIndicatorPrefab, layoutFieldContainer.transform);
            int dragIndex = 0;

            while (_isDragging)
            {
                var mousePositionY = Input.mousePosition.y;
                dragIndex = 0;
                for (int i = _layouts.Count - 1; i >= 0; i--)               
                {
                    if(_layoutTransforms[i].position.y > mousePositionY)
                    {
                        dragIndex = i + 1;
                        break;
                    }
                }
                dragIndicator.transform.SetSiblingIndex(dragIndex);

                yield return null;
            }

            Destroy(dragIndicator);
            var layoutListIndex = _layouts.FindIndex(layout => layout.id == layoutIndex);
            dragIndex = dragIndex > layoutListIndex ? dragIndex - 1 : dragIndex;
            var draggedLayout = _layouts[layoutListIndex];
            _layouts.RemoveAt(layoutListIndex);
            _layouts.Insert(dragIndex, draggedLayout);

            SaveLayouts();
            _layoutTransforms = layoutFieldContainer.SetupLayouts(_layouts);
        }

        public void CopyLayout(int index)
        {
            var listId = _layouts.FindIndex(layout => layout.id == index);
            var layoutToCopy = _layouts[listId];
            
            var copyName = "";
            var copyNamePostfix = 2;
            var copyNameUnique = false;
            do
            {
                copyName = $"{layoutToCopy.name} {copyNamePostfix}";
                if(!_layouts.Any(layout => layout.name == copyName))
                {
                    copyNameUnique = true;
                }
                else
                {
                    copyNamePostfix++;
                }
            } while (!copyNameUnique);

            var layoutCopy = CityLayout.LayoutBlockData.CopyData(copyName, layoutToCopy);
            _layouts.Insert(listId + 1, layoutCopy);

            SaveLayouts();
            _layoutTransforms = layoutFieldContainer.SetupLayouts(_layouts);
        }

        public void DeleteLayout(int index)
        {
            var layoutToDelete = _layouts.Single(layout => layout.id == index);
            _layouts.Remove(layoutToDelete);

            SaveLayouts();
            _layoutTransforms = layoutFieldContainer.SetupLayouts(_layouts);
        }

        private void ChangeLayoutName(int index, string newName)
        {
            var layoutToChange = _layouts.Single(layout => layout.id == index);
            layoutToChange.name = newName;

            SaveLayouts();
        }

        private void ChangeLayoutDifficulty(int index, int newDifficulty)
        {
            var layoutToChange = _layouts.Single(layout => layout.id == index);
            layoutToChange.difficulty = newDifficulty;

            SaveLayouts();
            _layoutTransforms = layoutFieldContainer.SetupLayouts(_layouts);
        }

        private void SaveLayouts()
        {
            var writeString = JsonConvert.SerializeObject(_layouts);
            File.WriteAllText(AssetDatabase.GetAssetPath(layoutBlockDataFile), writeString);
        }

        private void EnterLayoutBuilderTool(CityLayout.LayoutBlockData layoutBlockData)
        {
            menuBackground.SetActive(false);
            editBackground.SetActive(true);
            gridCreationTool.saveButtonClicked += SaveLayout;
            gridCreationTool.cancelButtonClicked += LeaveLayoutBuilderTool;
            gridCreationTool.BuildGrid(layoutBlockData);
            toolbar.UnmarkAllIcons();
        }

        private void LeaveLayoutBuilderTool()
        {
            gridCreationTool.saveButtonClicked -= SaveLayout;
            gridCreationTool.cancelButtonClicked -= LeaveLayoutBuilderTool;
        }

        private void SaveLayout(CityLayout.LayoutBlockData layoutData)
        {
            gridCreationTool.saveButtonClicked -= SaveLayout;
            var editListIndex = _layouts.FindIndex(layout => layout.id == layoutData.id);
            if (editListIndex >= 0)
            {
                _layouts.RemoveAt(editListIndex);
                _layouts.Insert(editListIndex, layoutData);
            }
            else
            {
                _layouts.Add(layoutData);
            }   

            var writeString = JsonConvert.SerializeObject(_layouts);
            File.WriteAllText(AssetDatabase.GetAssetPath(layoutBlockDataFile), writeString);

            menuBackground.SetActive(true);
            editBackground.SetActive(false);
            _layoutTransforms = layoutFieldContainer.SetupLayouts(_layouts);
        }
    }
}

#endif
