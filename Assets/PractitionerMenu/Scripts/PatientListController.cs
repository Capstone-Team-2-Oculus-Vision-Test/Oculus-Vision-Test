using System.Collections.Generic;
using PatientTest.Scripts;
using UnityEngine.UIElements;
using Utility;

namespace PractitionerMenu.Scripts
{
    public class PatientListController
    {
        private VisualTreeAsset _listEntryTemplate;
        private ListView _patientList;
        private TextField _firstName;
        private TextField _lastName;
        private TextField _age;
        private TextField _sex;
        private TextField _patientID;
        private TestResultsDTO _selectedTest;
        private List<PatientDTO> _allPatients;

        public void InitializePatientList(VisualElement root)
        {
            EnumerateAllPatients();
            _firstName = root.Q<TextField>("FirstName");
            _lastName = root.Q<TextField>("LastName");
            _age = root.Q<TextField>("Age");
            _sex = root.Q<TextField>("Sex");
            _patientID = root.Q<TextField>("ID");
            
            _patientList = root.Q<ListView>("patientList");
            FillPatientList();

            _patientList.onSelectionChange += OnPatientSelected;
        }


        private void EnumerateAllPatients()
        {
            _allPatients = new List<PatientDTO>();
            var db = new Sqlite();
            _allPatients.AddRange(Sqlite.GetAllPatients());
        }

        private void FillPatientList()
        {
            // override make item function
            _patientList.makeItem = () => new Label();
            // bind function for specific entry
            _patientList.bindItem = (item, index) =>
            {
                ((Label)item).text = _allPatients[index].FirstName + " " + _allPatients[index].LastName;
            };

            _patientList.itemsSource = _allPatients;
        }
        
        
        
        private void OnPatientSelected(IEnumerable<object> obj)
        {
            var selectedPatient = (PatientDTO)_patientList.selectedItem;
            // set text fields to patientDTO
            // if patientDTO is null, set text fields to empty
            _patientID.value = selectedPatient?.ID ?? "";
            _firstName.value = selectedPatient?.FirstName ?? "";
            _lastName.value = selectedPatient?.LastName ?? "";
            _age.value = selectedPatient?.Age ?? "";
            _sex.value = selectedPatient?.Sex ?? "";


        }

    }
}