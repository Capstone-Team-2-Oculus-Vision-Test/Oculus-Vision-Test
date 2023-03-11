using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
namespace PractitionerMenu.Scripts
{
    public class MainMenu : MonoBehaviour
    {

        private enum EyeEnum
        {
            Left,
            Right
        }

        private enum TestEnum
        {
            ThirtyDashTwo,
            TwentyDashTwo
        }

        private void OnEnable()
        {
            VisualElement document = GetComponent<UIDocument>().rootVisualElement;

            var buttonExit = document.Q<Button>("ExitButton");
            var buttonStart = document.Q<Button>("SubmitButton");


            buttonExit.clicked += () => Application.Quit();
            buttonStart.clicked += () =>
            {
                var dropdownEye = document.Q<DropdownField>("SelectEye");
                switch (dropdownEye.value)
                {
                    case "Left":
                        PlayerPrefs.SetInt("Eye", (int)EyeEnum.Left);
                        break;
                    case "Right":
                        PlayerPrefs.SetInt("Eye", (int)EyeEnum.Right);
                        break;
                }

                var dropdownTestType = document.Q<DropdownField>("SelectTestType");
                switch (dropdownTestType.value)
                {
                    case "20-2":
                        PlayerPrefs.SetInt("Test Type", (int)TestEnum.TwentyDashTwo);
                        break;
                    case "30-2":
                        PlayerPrefs.SetInt("Test Type", (int)TestEnum.ThirtyDashTwo);
                        break;
                }
                SceneManager.LoadScene("PatientTestScene");
            };
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
