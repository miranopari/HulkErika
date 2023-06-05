using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.IO;

public class SubmitButton : MonoBehaviour
{
    //連携するGameObject
    public ToggleGroup valence_toggleGroup;
    public ToggleGroup arousal_toggleGroup;
    public ToggleGroup dominance_toggleGroup;

    GameObject CanvasController;

    StreamWriter sw;

    // Use this for initialization
    void Start()
    {
        //CanvasController = GameObject.Find("CanvasController");
        //sw = CanvasController.GetComponent<CanvasController>().sw;
    }

    public void onClick()
    {
        //Get the label in activated toggles
        string selectedLabel_v = valence_toggleGroup.ActiveToggles()
            .First().GetComponentsInChildren<Text>()
            .First(t => t.name == "Label").text;

        string selectedLabel_a = arousal_toggleGroup.ActiveToggles()
            .First().GetComponentsInChildren<Text>()
            .First(t => t.name == "Label").text;


        string selectedLabel_d = dominance_toggleGroup.ActiveToggles()
            .First().GetComponentsInChildren<Text>()
            .First(t => t.name == "Label").text;

        if (selectedLabel_v != "null" && selectedLabel_a != "null" && selectedLabel_d != "null")
        {
            //sw = CanvasController.GetComponent<CanvasController>().sw;
            string[] ans_array = new string[] { selectedLabel_v, selectedLabel_a, selectedLabel_d }; 
            CSVLogger.instance.WriteSAMResultsCSV(string.Join(",", ans_array));

            this.gameObject.transform.parent.gameObject.SetActive(false);
        }
    }
}