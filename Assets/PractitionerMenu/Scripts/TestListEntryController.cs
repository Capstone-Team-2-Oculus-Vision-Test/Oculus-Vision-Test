using PatientTest.Scripts;
using UnityEngine.UIElements;

namespace PractitionerMenu.Scripts
{
    public class TestListEntryController
    {
        private Label NameIDLabel;
        private Label DateLabel;

        public void SetVisualElement(VisualElement visualElement)
        {
            NameIDLabel = visualElement.Q<Label>("name-id");
            DateLabel = visualElement.Q<Label>("date");
        }

        public void SetTestData(TestListData testData)
        {
            NameIDLabel.text = testData.Name + " " + testData.ID;
            DateLabel.text = testData.Date;
        }
    }
}