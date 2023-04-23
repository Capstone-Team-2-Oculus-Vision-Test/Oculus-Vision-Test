using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace PatientTest.Scripts
{
public class PractitionerUI : MonoBehaviour
{
    public GridLogic gridLogic;
    private ProgressBar _progressBar;
    private int _progressMax;
    private const bool Debug = true;

    private void OnEnable()
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        var buttonPause = root.Q<Button>("Pause");
        var buttonRestart = root.Q<Button>("Restart");
        var buttonCancel = root.Q<Button>("Cancel");
        var buttonDebug = root.Q<Button>("DebugResults");
        
        buttonCancel.clicked += () =>
        {
            gridLogic.StopTest();
            SceneManager.LoadScene("PractitionerMenu");
        };
        buttonPause.clicked += () =>
        {
            gridLogic.PauseTest();
            if (buttonPause.text == "Begin")
            {
                buttonPause.text = "Pause";
            }
            else
                buttonPause.text = buttonPause.text == "Resume" ? "Pause" : "Resume";
        };
        buttonRestart.clicked += () =>
        {
            buttonPause.text = "Begin";
            gridLogic.ResetTest();
        };
        if (Debug)
        {
            buttonDebug.clicked += () =>
            {
                gridLogic.PauseTest();
                gridLogic.DebugResults();
            };
        }
        else
        {
            buttonDebug.visible = false;
        }
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
            _progressBar.highValue = _progressMax;
        }
        
        _progressBar.SetValueWithoutNotify(gridLogic.eyeResults.Count);
        _progressBar.title = $"{_progressBar.value} / {_progressMax} points tested";
    }

}

}
    
