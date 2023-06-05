using System.Collections.Generic;
using UnityEngine;
using VRQuestionnaireToolkit;

/// Manages the experimenter's keyboard inputs to run the experiment.
///     'C' --> calibrates the avatar and makes the virtual mirror appear
///     'V' --> launches the video for the VM stimulation.
///     'T' --> launches the main task. Stones will start appearing on the conveyor belts.
///             Also starts logging punch speed and controller motion data.
///     'Q' --> saves the main task's data and activates the VEQ and cybersickness 
///             questionnaires.
/// Additional commands:
///     'DEL' --> deletes all stones that were instantiated.
///     
/// Provided by VRIKCalibrationController:
///     'D'   --> saves the calibration data of the avatar (not used).
///     'S'   --> Recalibrates avatar scale only (not used).
public class ExperimentManager : MonoBehaviour
{
    // /!\ Update in the inspector the participant's ID for the log files
    public int userID;
    public string condition;

    // Controllers are displayed upon starting to help the user grab them more easily.
    [SerializeField] private GameObject[] controllers;
    // Mirror is hidden until the calibration is performed.
    [SerializeField] private GameObject mirror;
    // Tutorial video hidden by default. Display when ready.
    [SerializeField] private GameObject video;
    // Questionnaires. Hidden by default. Display when ready.
    [SerializeField] private GameObject questionnaireManager;
    [SerializeField] private GameObject samScale;
    [SerializeField] private GameObject vivePointers;

    // List of object managers (one per conveyor belt)
    public List<ObjectManager> objectManagerList = new List<ObjectManager>();

    // Number of objects to spawn per conveyor belt
    public int numberOfTrials = 15;

    // Max number of objects per conveyor before cleaning the oldest ones
    public int maxActiveObjectNumber = 5;

    private void Awake()
    {
        mirror.SetActive(false);
        video.SetActive(false);
        questionnaireManager.SetActive(false);
        samScale.SetActive(false);
        vivePointers.SetActive(false);

        // Initialize the object managers
        for (int i = 0; i < objectManagerList.Count; i++)
        {
            objectManagerList[i].numberOfTrials = numberOfTrials;
            objectManagerList[i].maxActiveObjectNumber = maxActiveObjectNumber;
        }

        // Initialize session data for questionnaire files
        var setup = questionnaireManager.GetComponentInChildren<StudySetup>();
        setup.ParticipantId = userID.ToString();
        setup.Condition = condition;
    }

    void Update()
    {

        // Calibrate the avatar, make it visible, and display the mirror
        if (Input.GetKeyDown(KeyCode.C))
        {
            // Hide the controllers
            controllers[0].SetActive(false);
            controllers[1].SetActive(false);

            // Show the virtual mirror
            mirror.SetActive(true);

            Debug.Log("Calibration done.");
            // Note: calibration is done in VRIKCalibrationController, also listening for
            // 'C' input.
        }

        // Display the video
        if (Input.GetKeyDown(KeyCode.V))
        {
            video.SetActive(true);
        }

        // Run the main task
        if (Input.GetKeyDown(KeyCode.T))
        {
            video.SetActive(false);

            for (int i = 0; i < objectManagerList.Count; i++)
            {
                objectManagerList[i].isSpawning = !objectManagerList[i].isSpawning;
            }

            // Start logging the distance travelled by the controllers
            CSVLogger.instance.isMeasuring = true;
        }

        //  Display the questionnaires
        if (Input.GetKeyDown(KeyCode.Q))
        {
            vivePointers.SetActive(true);

            // Save the main task's data
            CSVLogger.instance.WriteDistanceResultsCSV();
            CSVLogger.instance.WriteSpeedResultsCSV();

            //questionnaireManager.SetActive(true);
            samScale.SetActive(true);
        }

         // Remove all stones from the scene
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            for (int i = 0; i < objectManagerList.Count; i++)
            {
                objectManagerList[i].ClearCubes();
            }
        }
    }
}
