using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using static Utility.UtilityScripts;
using System;
using PatientTest.Scripts;
using Unity.VisualScripting;
using Utility;
using static PatientTest.Scripts.DataTransfer;

namespace PractitionerMenu.Scripts
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Settings")] [SerializeField] private VisualTreeAsset newTestTemplate;
        [SerializeField] private VisualTreeAsset testListEntryTemplate;
        [SerializeField] private VisualTreeAsset startMenuTemplate;
        [SerializeField] private VisualTreeAsset previousTestsTemplate;
        [SerializeField] private VisualTreeAsset resultsTemplate;
        [SerializeField] private VisualTreeAsset settingsTemplate;

        private static bool _showResults;
        private UIDocument _doc;
        private VisualElement _background;
        private VisualElement _startMenuView;
        private VisualElement _resultView;
        private GameObject _pointsParent;
        private static TestListData _selectedTest;

        public Camera resultCamera;
        public TextMeshPro textPrefab;


        private void OnEnable()
        {
            // Get the root of the UI document
            _doc = GetComponent<UIDocument>();
            _background = _doc.rootVisualElement.Q<VisualElement>("Background");

            // Show results if we are coming from the test
            if (_showResults)
            {
                DisplayResults();
            }

            // Otherwise, show the start menu
            else
            {
                DisplayStartMenu();
            }
        }

        private void DisplayStartMenu()
        {
            // Store start menu layout
            _startMenuView ??= startMenuTemplate.CloneTree();

            // Reset background and add start menu
            _background.Clear();
            _background.Add(_startMenuView);
            _startMenuView.StretchToParentSize();

            // Get buttons and add listeners
            var newTestButton = _startMenuView.Q<Button>("NewTestButton");
            newTestButton.clicked += NewTestButtonClicked;
            var prevResultsButton = _startMenuView.Q<Button>("PreviousResultsButton");
            prevResultsButton.clicked += PrevResultsButtonClicked;
            var settingsButton = _startMenuView.Q<Button>("SettingsButton");
            settingsButton.clicked += SettingsButtonClicked;
            var exitButton = _startMenuView.Q<Button>("ExitButton");
            exitButton.clicked += Application.Quit;
        }

        private void SettingsButtonClicked()
        {
            // Clear background and add settings layout
            _background.Clear();
            TemplateContainer root = settingsTemplate.CloneTree();
            
            // Get buttons and add listeners
            var backButton = root.Q<Button>("Back");
            backButton.clicked += GoToMainMenu;
            var saveButton = root.Q<Button>("SaveSettings");
            
            saveButton.clicked += () =>
            {
                SaveSettings(root);
            };
            SetSettingsValues(root);
            _background.Add(root);
            root.StretchToParentSize();
        }

        private void SetSettingsValues(TemplateContainer root)
        {
            root.Q<TextField>("MinInputDelay").value = $"{PlayerPrefs.GetFloat("MinInputDelay", 1f)}";
            root.Q<TextField>("MaxInputDelay").value = $"{PlayerPrefs.GetFloat("MaxInputDelay", 2.3f)}";
            root.Q<TextField>("MaxBrightness").value = $"{PlayerPrefs.GetFloat("MaxBrightness", 100f)}";
            root.Q<TextField>("StimulusTiming").value = $"{PlayerPrefs.GetFloat("StimulusTiming", 0.2f)}";
        }

        private void SaveSettings(TemplateContainer root)
        {
            PlayerPrefs.SetFloat("MinInputDelay", float.Parse(root.Q<TextField>("MinInputDelay").value));
            PlayerPrefs.SetFloat("MaxInputDelay", float.Parse(root.Q<TextField>("MaxInputDelay").value));
            PlayerPrefs.SetFloat("MaxBrightness", float.Parse(root.Q<TextField>("MaxBrightness").value));
            PlayerPrefs.SetFloat("StimulusTiming", float.Parse(root.Q<TextField>("StimulusTiming").value));
        }

        private void DisplayResults(bool save = true)
        {
            // Store results layout
            _resultView ??= resultsTemplate.CloneTree();

            // Reset background and add results
            _background.Clear();
            _background.Add(_resultView);
            _resultView.StretchToParentSize();

            // Get buttons and add listeners
            var resultsBackButton = _resultView.Q<Button>("BackButton");
            var savePdfButton = _resultView.Q<Button>("SavePDFButton");
            var saveResultsButton = _resultView.Q<Button>("SaveTestButton");
            if (!save) saveResultsButton.visible = false;
            saveResultsButton.clicked += () =>
            {
                Sqlite.InsertTestResults();
                saveResultsButton.text = "Results Saved";
                saveResultsButton.SetEnabled(false);
            };
            resultsBackButton.clicked += GoToMainMenu;
            savePdfButton.clicked += () =>
            {
                PdfGenerate();
                savePdfButton.text = "PDF Saved";
                savePdfButton.SetEnabled(false);
            };

            // Assign values to ui elements
            SetResultValues();
        }

        private void SetResultValues()
        {
            // Create a new parent for the points to remove them from the scene easier
            _pointsParent = new GameObject
            {
                transform =
                {
                    parent = resultCamera.transform,
                    localPosition = Vector3.zero
                }
            };

            // Instantiate points in the scene and assign db values
            foreach (Vector4 point in resultsDTO.Data)
            {
                var position = new Vector3(point.x / resultsDTO.ScalingFactor, point.y / resultsDTO.ScalingFactor,
                    point.z);
                TextMeshPro text = Instantiate(textPrefab, _pointsParent.transform);
                text.transform.localPosition = position;
                text.text = $"{point.w}";
            }

            // Assign values to ui elements
            _resultView.Q<TemplateContainer>("Name").Q<Label>("Text").text =
                patientDTO.FirstName + " " + patientDTO.LastName;
            _resultView.Q<TemplateContainer>("Age").Q<Label>("Text").text = patientDTO.Age;
            _resultView.Q<TemplateContainer>("Sex").Q<Label>("Text").text = patientDTO.Sex;
            _resultView.Q<TemplateContainer>("Id").Q<Label>("Text").text = patientDTO.ID;
            var testField = _resultView.Q<TemplateContainer>("Test").Q<Label>("Text");
            testField.text = resultsDTO.Test switch
            {
                TestEnum.ThirtyDashTwo => "30-2",
                TestEnum.TwentyDashTwo => "20-2",
                var _ => testField.text
            };
            _resultView.Q<TemplateContainer>("Eye").Q<Label>("Text").text = resultsDTO.EyeTested.ToString();
        }

        private void NewTestButtonClicked()
        {
            // Clear background and get new test layout
            _background.Clear();
            TemplateContainer root = newTestTemplate.CloneTree();

            // Get buttons and add listeners
            var backButton = root.Q<Button>("BackButton");
            backButton.clicked += GoToMainMenu;
            var startButton = root.Q<Button>("StartButton");
            startButton.clicked += () => NewTestStartButton(root);
            var saveButton = root.Q<Button>("SavePatient");
            saveButton.clicked += () =>
            {
                StorePatientInfo(root);
                var patientListController = new PatientListController();
                patientListController.InitializePatientList(root);
            };

            // create patient list
            var patientListController = new PatientListController();
            patientListController.InitializePatientList(root);

            // Add new test layout to background
            _background.Add(root);
            root.StretchToParentSize();
        }

        private void PrevResultsButtonClicked()
        {
            // Clear background and get previous tests layout
            _background.Clear();
            TemplateContainer root = previousTestsTemplate.CloneTree();

            // Get buttons and add listeners
            var backButton = root.Q<Button>("BackButton");
            var loadButton = root.Q<Button>("LoadResults");
            backButton.clicked += GoToMainMenu;
            loadButton.clicked += () =>
            {
                if (_selectedTest == null) return;
                resultsDTO = _selectedTest.Results;
                patientDTO = Sqlite.GetPatient(_selectedTest.ID);
                DisplayResults(false);
            };

            // create test list
            var testListController = new TestListController();
            testListController.InitializeTestList(root, testListEntryTemplate);

            // Add previous tests layout to background
            _background.Add(root);
            root.StretchToParentSize();
        }

        private void GoToMainMenu()
        {
            // Clear background and add start menu
            Destroy(_pointsParent);
            _showResults = false;
            _background.Clear();
            DisplayStartMenu();
            
            // reset patient and test info
            patientDTO = new PatientDTO();
            resultsDTO = new TestResultsDTO();
        }

        private static void NewTestStartButton(VisualElement root)
        {
            // Store patient info into static current patient
            patientDTO.FirstName = root.Q<TextField>("FirstName").value;
            patientDTO.LastName = root.Q<TextField>("LastName").value;
            patientDTO.Age = root.Q<TextField>("Age").value;
            patientDTO.Sex = root.Q<TextField>("Sex").value;
            patientDTO.ID = root.Q<TextField>("ID").value;

            if (patientDTO.MissingData()) return;
            // Store test info into static current test
            var dropdownEye = root.Q<DropdownField>("SelectEye");
            resultsDTO.EyeTested = dropdownEye.value switch
            {
                "Left" => EyeEnum.Left,
                "Right" => EyeEnum.Right,
                var _ => resultsDTO.EyeTested
            };

            var dropdownTestType = root.Q<DropdownField>("SelectTestType");
            resultsDTO.Test = dropdownTestType.value switch
            {
                "20-2" => TestEnum.TwentyDashTwo,
                "30-2" => TestEnum.ThirtyDashTwo,
                var _ => resultsDTO.Test
            };

            // load test scene
            SceneManager.LoadScene("PatientTestScene");
        }

        public static void SetShowResults(bool show)
        {
            _showResults = show;
        }

        private static void StorePatientInfo(VisualElement root)
        {
            // Get patient info from UI
            var patient = new PatientDTO
            {
                ID = root.Q<TextField>("ID").value,
                FirstName = root.Q<TextField>("FirstName").value,
                LastName = root.Q<TextField>("LastName").value,
                Age = root.Q<TextField>("Age").value,
                Sex = root.Q<TextField>("Sex").value
            };

            // if any patient fields empty, return
            if (patient.MissingData()) return;

            // Store patient info into database
            Sqlite.InsertPatient(patient.ID, patient.FirstName, patient.LastName, patient.Age, patient.Sex);
        }

        public static void SetSelectedTest(TestListData test)
        {
            _selectedTest = test;
        }

        public static void ClearSelectedTest()
        {
            _selectedTest = null;
        }
    }
}