using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using static Utility.UtilityScripts;

namespace PractitionerMenu.Scripts
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private VisualTreeAsset newTestTemplate;

        [SerializeField] private VisualTreeAsset startMenuTemplate;
        [SerializeField] private VisualTreeAsset previousTestsTemplate;

        private UIDocument _doc;
        private VisualElement _background;
        private VisualElement _startMenuView;
        private VisualElement _newTestView;
        private VisualElement _previousResultsView;
        

        
        private void OnEnable()
        {
            _doc = GetComponent<UIDocument>();
            _background = _doc.rootVisualElement.Q<VisualElement>("Background");
            if (startMenuTemplate != null)
            {
                _startMenuView = startMenuTemplate.CloneTree();
                _background.Add(_startMenuView);
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
                var backButton = _newTestView.Q<Button>("BackButton");
                backButton.clicked += NewTestBackButton;
                var startButton = _newTestView.Q<Button>("StartButton");
                startButton.clicked += NewTestStartButton;
            }

            if (previousTestsTemplate != null)
            {
                _previousResultsView = previousTestsTemplate.CloneTree();
                var backButton = _previousResultsView.Q<Button>("BackButton");
                backButton.clicked += PrevTestsBackButtonClicked;
            }
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

        private void PrevTestsBackButtonClicked()
        {
            _background.Clear();
            _background.Add(_startMenuView);
        }

        private void NewTestStartButton()
        {
            var dropdownEye = _newTestView.Q<DropdownField>("SelectEye");
            switch (dropdownEye.value)
            {
                case "Left":
                    PlayerPrefs.SetInt("Eye", (int)EyeEnum.Left);
                    break;
                case "Right":
                    PlayerPrefs.SetInt("Eye", (int)EyeEnum.Right);
                    break;
            }
            
            var dropdownTestType = _newTestView.Q<DropdownField>("SelectTestType");
            switch (dropdownTestType.value)
            {
                case "20-2":
                    PlayerPrefs.SetInt("TestType", (int)TestEnum.TwentyDashTwo);
                    break;
                case "30-2":
                    PlayerPrefs.SetInt("TestType", (int)TestEnum.ThirtyDashTwo);
                    break;
            }
            SceneManager.LoadScene("PatientTestScene");
        }

        private void NewTestBackButton()
        {
            _background.Clear();
            _background.Add(_startMenuView);
        }

        
    }
}
