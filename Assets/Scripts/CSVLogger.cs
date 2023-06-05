using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CSVLogger: MonoBehaviour
{
    [SerializeField] private ExperimentManager exp;
    public ExperimentManager Exp => exp;

    // Record controller motion amount
    public Vector2 distanceTravelled = Vector2.zero;   
    public Transform leftHandTransform;
    public Transform rightHandTransform;
    private Vector3 lLastPosition;
    private Vector3 rLastPosition;
    public bool isMeasuring = false;

    // To save the results
    public string m_sessionPath;
    private StringBuilder m_csvData;
    private string m_filePath;
    private string m_sessionId;
    private const string SessionFolderRoot = "CSVLogger";

    //public GameObject prompt;
    public static CSVLogger instance = null;

    protected void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != this)
        {
            Destroy(gameObject);
        }

        if (Exp == null)
        {
            Debug.LogError("Please fill the parameters for the Punch Logger component.");
        }
    }

    // Record the quantity of motion of the controllers
    void FixedUpdate()
    {
        if (isMeasuring)
        {
            // Left hand
            var positionDif = (leftHandTransform.position - lLastPosition).magnitude;
            if (positionDif > 0.001)
            {
                distanceTravelled.x += positionDif;
            }
            lLastPosition = leftHandTransform.position;

            // Right hand
            positionDif = (rightHandTransform.position - rLastPosition).magnitude;
            if (positionDif > 0.001)
            {
                distanceTravelled.y += positionDif;
            }
            rLastPosition = rightHandTransform.position;
        }
    }

    async void Start()
    {
        await MakeNewSession();
    }

    async Task MakeNewSession()
    {
        m_sessionId = DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_id_" + Exp.userID.ToString() + "_cond_" + Exp.condition;
        string rootPath = "";

        //rootPath = Path.Combine(Application.dataPath, "Questionnaires/Data/Answers");
        ////if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);
        rootPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), SessionFolderRoot);
        if (!Directory.Exists(rootPath)) Directory.CreateDirectory(rootPath);

        m_sessionPath = Path.Combine(rootPath, m_sessionId);
        Directory.CreateDirectory(m_sessionPath);
        Debug.Log("Logging data to " + m_sessionPath);
    }

    public void StartNewCSV(string header, string type)
    {
        string fileName = "id_" + Exp.userID.ToString() + "_" + Exp.condition + "_" + type + "_result.csv";
        Debug.Log("Creating file: " + fileName);

        m_filePath = Path.Combine(m_sessionPath, fileName);
        if (m_csvData != null)
        {
            EndCSV();
        }
        m_csvData = new StringBuilder();
        m_csvData.AppendLine(header);
    }

    public void EndCSV()
    {
        if (m_csvData == null)
        {
            return;
        }
        using (var csvWriter = new StreamWriter(m_filePath, true))
        {
            csvWriter.Write(m_csvData.ToString());
        }
        m_csvData = null;
    }

    /// <summary>
    /// Write the result of the Experiment in a CSV file format.
    /// </summary>
    public void WriteSpeedResultsCSV()
    {
        //Create file and add header to text
        StartNewCSV("Force", "SPEED");

        //Add lines to the results
        for (int i = 0; i < Exp.objectManagerList.Count; i++)
        {
            for (int j = 0; j < Exp.objectManagerList[i].hitList.Count; j++)
            {
                string line = Exp.objectManagerList[i].hitList[j].ToString();
                m_csvData.AppendLine(line);
            }
        }

        //Save the data and close the file
        EndCSV();
    }

    public void WriteDistanceResultsCSV()
    {
        //Create file and add header to text
        StartNewCSV("LeftHand,RightHand", "DISTANCE");

        //Add lines to the results
        string line = distanceTravelled.x.ToString() + "," + distanceTravelled.y.ToString();
        m_csvData.AppendLine(line);

        //Save the data and close the file
        EndCSV();
    }

    public void WriteSAMResultsCSV(string data)
    {
        //Create file and add header to text
        StartNewCSV("Valence,Arousal,Dominance", "SAM");

        //Add lines to the results
        m_csvData.AppendLine(data);

        //Save the data and close the file
        EndCSV();
    }

    public void WriteQuestionnaire(string header, string type, string[][] data)
    {
        //Create file and add header to text
        StartNewCSV(header, type);

        for (int i = 0; i < data.GetLength(0); i++)
            m_csvData.AppendLine(string.Join(",", data[i]));

        //Save the data and close the file
        EndCSV();
    }
}
