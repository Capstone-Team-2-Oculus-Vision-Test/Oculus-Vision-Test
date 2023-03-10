using UnityEngine;
using UnityEngine.UIElements;

namespace PatientTest.Scripts
{
public class PractitionerUI : MonoBehaviour
{
    public GridLogic gridLogic;
    private ProgressBar _progressBar;
    private int _progressMax = 0;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        var buttonPause = root.Q<Button>("Pause");
        var buttonRestart = root.Q<Button>("Restart");
        var buttonCancel = root.Q<Button>("Cancel");
        
        
        buttonCancel.clicked += () => gridLogic.StopTest();
        buttonPause.clicked += () =>
        {
            gridLogic.PauseTest();
            buttonPause.text = buttonPause.text == "Resume" ? "Pause" : "Resume";
        };
        buttonRestart.clicked += () =>
        {
            gridLogic.ResetTest();
        };
    }

    private void Start()
    {
        _progressBar = GetComponent<UIDocument>().rootVisualElement.Q<ProgressBar>("TestProgressBar");
    }

    private void Update()
    {
        if (_progressMax == 0)
        {
            _progressMax = gridLogic.points.Count;
        }
        
        _progressBar.SetValueWithoutNotify(gridLogic.eyeResults.Count);
        _progressBar.title = $"{_progressBar.value} / {_progressMax} points tested.";
    }

}

}
    
