using System.Collections.Generic;
using PatientTest.Scripts;
using UnityEngine.UIElements;
using Utility;

namespace PractitionerMenu.Scripts
{
    public class TestListController
    {
        private VisualTreeAsset _listEntryTemplate;
        private ListView _testList;
        private TestResultsDTO _selectedTest;
        private List<TestListData> _allTests;

        public void InitializeTestList(VisualElement root, VisualTreeAsset listElementTemplate)
        {
            EnumerateAllTests();

            _listEntryTemplate = listElementTemplate;

            _testList = root.Q<ListView>("testList");
            
            FillTestList();

            _testList.onSelectionChange += OnTestSelected;
        }

        

        void EnumerateAllTests()
        {
            _allTests = new List<TestListData>();
            var db = new Sqlite();
            _allTests.AddRange(Sqlite.GetAllTests());
        }

        private void FillTestList()
        {
            // override make item function
            _testList.makeItem = () =>
            {
                var newListEntry = _listEntryTemplate.Instantiate();
                // controller for data
                var newListEntryLogic = new TestListEntryController();
                // assign script to visual element
                newListEntry.userData = newListEntryLogic;
                // initialize controller script
                newListEntryLogic.SetVisualElement(newListEntry);
                return newListEntry;
            };
            // bind function for specific entry
            _testList.bindItem = (item, index) =>
            {
                ((TestListEntryController)item.userData).SetTestData(_allTests[index]);
            };

            _testList.fixedItemHeight = 80;

            _testList.itemsSource = _allTests;
        }
        
        private void OnTestSelected(IEnumerable<object> obj)
        {
            if (_testList.selectedItem is not TestListData selectedTest)
            {
                MainMenu.ClearSelectedTest();
                return;
            }
            MainMenu.SetSelectedTest(selectedTest);
        }

    }
}