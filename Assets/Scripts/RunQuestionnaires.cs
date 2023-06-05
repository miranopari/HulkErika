using UnityEngine;
using VRQuestionnaireToolkit;

public class RunQuestionnaires : MonoBehaviour
{
    private GameObject _vrQuestionnaireToolkit;
    private GenerateQuestionnaire _generateQuestionnaire;
    private ExportToCSV _exportToCsvScript;
    private GameObject _exportToCsv;
    private int _currentQuestionnaire;

    void Start()
    {
        _vrQuestionnaireToolkit = GameObject.FindGameObjectWithTag("VRQuestionnaireToolkit");
        _generateQuestionnaire = _vrQuestionnaireToolkit.GetComponentInChildren<GenerateQuestionnaire>();

        _exportToCsv = GameObject.FindGameObjectWithTag("ExportToCSV");
        _exportToCsvScript = _exportToCsv.GetComponent<ExportToCSV>();
        _exportToCsvScript.QuestionnaireFinishedEvent.AddListener(NextQuestionnaire); // e.g, call next questionnaire from list
    }

    void NextQuestionnaire()
    {
        if (_currentQuestionnaire < _generateQuestionnaire.Questionnaires.Count)
        {
            Debug.Log("next questionnaire");
            _generateQuestionnaire.Questionnaires[_currentQuestionnaire].SetActive(false); // disable questionnaire 0
            _generateQuestionnaire.Questionnaires[_currentQuestionnaire+1].SetActive(true); // enable questionnaire 1
            _currentQuestionnaire++;
        }
    }
}
