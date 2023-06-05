using UnityEngine;
using System.IO;
using System;

public class CanvasController : MonoBehaviour
{
    public StreamWriter sw;
    public ExperimentManager Exp;

    // Start is called before the first frame update
    void Start()
    {
        //var sampleData = "SampleText";
        //CreateCSV(sampleData, "SAM");
    }

    private void CreateCSV(string data, string type)
    {
        FileInfo fi;
        DateTime now = DateTime.Now;

        string fileName = "id_" + Exp.userID.ToString() + "_" + type + "_result.csv";
        //fileName = fileName + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + now.Month.ToString() + "_" + now.Day.ToString() + "__" + now.Hour.ToString() + "_" + now.Minute.ToString() + "_" + now.Second.ToString();
        fi = new FileInfo(CSVLogger.instance.m_sessionPath + fileName);
        Debug.Log("Creating file: " + fileName);
        Debug.Log("Full file path: " + fi);
        sw = fi.AppendText();
        string[] string_array = new string[] { "valence_score", "arousal_score", "dominance_score" };
        sw.WriteLine(string.Join(",", string_array));
    }

    private void OnApplicationQuit()
    {
        sw.Flush();
        sw.Close();
        Debug.Log("Save Completed");
    }
}
