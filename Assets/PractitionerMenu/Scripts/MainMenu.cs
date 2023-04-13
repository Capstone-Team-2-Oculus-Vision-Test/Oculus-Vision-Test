using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using static Utility.UtilityScripts;
using System;
using Utility;
using static PatientTest.Scripts.DataTransfer;

namespace PractitionerMenu.Scripts
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private VisualTreeAsset newTestTemplate;

        [SerializeField] private VisualTreeAsset startMenuTemplate;
        [SerializeField] private VisualTreeAsset previousTestsTemplate;
        [SerializeField] private VisualTreeAsset resultsTemplate;

        private static bool _showResults;
        private UIDocument _doc;
        private VisualElement _background;
        private VisualElement _startMenuView;
        private VisualElement _newTestView;
        private VisualElement _previousResultsView;
        private VisualElement _resultView;

        public Camera resultCamera;
        public TextMeshPro textPrefab;


        private void OnEnable()
        {
            _doc = GetComponent<UIDocument>();
            _background = _doc.rootVisualElement.Q<VisualElement>("Background");
            
            if (_showResults) 
            {
                _resultView = resultsTemplate.CloneTree();
                _background.Add(_resultView);
                _resultView.StretchToParentSize();
                var resultsBackButton = _resultView.Q<Button>("BackButton");
                var savePdfButton = _resultView.Q<Button>("SavePDFButton");
                var saveResultsButton = _resultView.Q<Button>("SaveTestButton");
                saveResultsButton.clicked += SaveResultsToDB;
                resultsBackButton.clicked += GoToMainMenu;
                savePdfButton.clicked += SavePdf;
                SetResultValues();
                
            }
            if (startMenuTemplate != null)
            {
                _startMenuView = startMenuTemplate.CloneTree();
                if (!_showResults)
                {
                    _background.Add(_startMenuView);
                }
                var newTestButton = _startMenuView.Q<Button>("NewTestButton");
                newTestButton.clicked += NewTestButtonClicked;
                var prevResultsButton = _startMenuView.Q<Button>("PreviousResultsButton");
                prevResultsButton.clicked += PrevResultsButtonClicked;
                var exitButton = _startMenuView.Q<Button>("ExitButton");
                exitButton.clicked += Application.Quit;
            }

            if (newTestTemplate != null)
            {
                _newTestView = newTestTemplate.CloneTree();
                var newTestBackButton = _newTestView.Q<Button>("BackButton");
                newTestBackButton.clicked += GoToMainMenu;
                var startButton = _newTestView.Q<Button>("StartButton");
                startButton.clicked += NewTestStartButton;
            }

            if (previousTestsTemplate != null)
            {
                _previousResultsView = previousTestsTemplate.CloneTree();
                var prevTestsBackButton = _previousResultsView.Q<Button>("BackButton");
                prevTestsBackButton.clicked += GoToMainMenu;
            }
        }

        private void SaveResultsToDB()
        {
            var dbConnection = new Sqlite();
            dbConnection.InsertTestResults(resultsDTO);
        }

        private void SavePdf()
        {
            throw new NotImplementedException();
        }

        private void SetResultValues()
        {
            foreach (var point in resultsDTO.Data)
            {
                var position = new Vector3(point.x, point.y, point.z);
                var text = Instantiate(textPrefab, resultCamera.transform);
                text.transform.localPosition = position;
                text.text = point.w.ToString(CultureInfo.InvariantCulture);
            }
            _resultView.Q<TemplateContainer>("Name").Q<Label>("Text").text = patientDTO.FirstName + " " + patientDTO.LastName;
            _resultView.Q<TemplateContainer>("Age").Q<Label>("Text").text = patientDTO.Age;
            _resultView.Q<TemplateContainer>("Sex").Q<Label>("Text").text = patientDTO.Sex;
            _resultView.Q<TemplateContainer>("Id").Q<Label>("Text").text = patientDTO.ID;
            var testField = _resultView.Q<TemplateContainer>("Test").Q<Label>("Text");
            testField.text = resultsDTO.Test switch
            {
                TestEnum.ThirtyDashTwo => "30-2",
                TestEnum.TwentyDashTwo => "20-2",
                _ => testField.text
            };
            _resultView.Q<TemplateContainer>("Eye").Q<Label>("Text").text = resultsDTO.EyeTested.ToString();
        }

        private void NewTestButtonClicked()
        {
            _background.Clear();
            _background.Add(_newTestView);
            _newTestView.StretchToParentSize();
        }

        private void PrevResultsButtonClicked()
        {
            _background.Clear();
            _background.Add(_previousResultsView);
            _previousResultsView.StretchToParentSize();

        }

        private void GoToMainMenu()
        {
            _showResults = false;
            _background.Clear();
            _background.Add(_startMenuView);
            _startMenuView.StretchToParentSize();
        }

        private void NewTestStartButton()
        {
            StorePatientInfo();

            var dropdownEye = _newTestView.Q<DropdownField>("SelectEye");
            resultsDTO.EyeTested = dropdownEye.value switch
            {
                "Left" => EyeEnum.Left,
                "Right" => EyeEnum.Right,
                _ => resultsDTO.EyeTested

            };

            var dropdownTestType = _newTestView.Q<DropdownField>("SelectTestType");
            resultsDTO.Test = dropdownTestType.value switch
            {
                "20-2" => TestEnum.TwentyDashTwo,
                "30-2" => TestEnum.ThirtyDashTwo,
                _ => resultsDTO.Test
            };
            // var firstName = _newTestView.Q<TextField>("FirstName");
            // var lastName = _newTestView.Q<TextField>("LastName");
            // var age = _newTestView.Q<TextField>("Age");
            // var sex = _newTestView.Q<TextField>("Sex");
            // var id = _newTestView.Q<TextField>("ID");
            // SelectedPatient = new PatientInfo
            // {
            //     FirstName = firstName.value,
            //     LastName = lastName.value,
            //     Age = Int32.Parse(age.value),
            //     Sex = sex.value,
            //     Id = Int32.Parse(id.value)
            // };
            SceneManager.LoadScene("PatientTestScene");
        }

        public static void SetShowResults(bool show)
        {
            _showResults = show;
        }
        private void StorePatientInfo()
        {
            // Store patient info into static current patient
            patientDTO.FirstName = _newTestView.Q<TextField>("FirstName").value; 
            patientDTO.LastName = _newTestView.Q<TextField>("LastName").value;
            patientDTO.Age = _newTestView.Q<TextField>("Age").value;
            patientDTO.Sex = _newTestView.Q<TextField>("Sex").value;
            patientDTO.ID = _newTestView.Q<TextField>("ID").value;
            // Store patient info into database
            var database = new Sqlite();
            database.InsertPatient(patientDTO.ID, patientDTO.FirstName,patientDTO.LastName,patientDTO.Age,patientDTO.Sex);
        }
    }
}
