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

        private int _currentEditIndex;
        private List<CityLayout.LayoutBlockData> _layouts;

        #region OnEnable/OnDisable

        private void OnEnable()
        {
            LayoutUIField.EditButtonPressed += OpenLayoutForEdit;
            LayoutUIField.DeleteButtonPressed += OpenDeleteConfirm;
        }

        private void OnDisable()
        {
            LayoutUIField.EditButtonPressed -= OpenLayoutForEdit;
            LayoutUIField.DeleteButtonPressed += OpenDeleteConfirm;
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

            layoutFieldContainer.SetupLayouts(_layouts);
        }

        public void OpenNewLayout(string name)
        {
            var newLayout = new CityLayout.LayoutBlockData(name);
            _currentEditIndex = newLayout.id;

            EnterLayoutBuilderTool(newLayout);
        }

        private void OpenLayoutForEdit(int index)
        {
            _currentEditIndex = index;
            var layoutToEdit = _layouts.Single(layout => layout.id == index);
            
            EnterLayoutBuilderTool(layoutToEdit);          
        }

        public void OpenDeleteConfirm(int index)
        {
            var layoutToDelete = _layouts.Single(layout => layout.id == index);
            deleteLayoutConfirmDialog.Open(layoutToDelete.name, index);
        }

        public void DeleteLayout(int index)
        {
            var layoutToDelete = _layouts.Single(layout => layout.id == index);
            _layouts.Remove(layoutToDelete);
            layoutFieldContainer.SetupLayouts(_layouts);

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
            layoutFieldContainer.SetupLayouts(_layouts);
        }
    }
}

#endif
