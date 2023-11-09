using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using TMPro;
using Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging;
using System.Threading;
using System.Collections;
using System.Threading.Tasks;

public class Methodology : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro phase;

    [SerializeField]
    private TextMeshPro instruction;
    private bool InstructionSet = false;

    private string ExpectedRotation = String.Empty;
    public static string ActualRotation = String.Empty;
    public static string PhaseControl = String.Empty;
    public static bool UDPListen = true;

    [SerializeField]
    private ManipulateAugmentations _manipulateAugmentations;

    [SerializeField]
    private AudioClass _audioClass = null;

    [SerializeField]
    private Automate _automate;

    [SerializeField]
    private TextMeshPro VoiceCue;

    [SerializeField]
    private GameObject BaselineEyetrackingPlane;

    [SerializeField]
    private GameObject EyetrackingPlane;

    public static bool BaselinePhase = false;

    public static bool PracticePhase = false;
    public static int PracticeInstructionNumber = 1;

    public static bool TrainingPhase = false;
    public static int TrainingInstructionNumber = 1;
    public static bool FadeOnce = false;

    public static List<string> WrongMoves = new List<string>();
    public static bool WrongMove = false;

    public static bool CubeIsInSolvedState = true;

  //  private int WaitPhaseSeconds = 30;
    public static bool RecallPhase = false;

  //  bool ConfirmedOnce = false;


    //**************** Methodology Metrics ************************
    public static int WrongMoveCount = 0;

    //public static int NumberOfCorrections = 0;
    public static float NumberofTrainingCycles = 1.0f;
    //public static int CorrectionTypeACount, CorrectionTypeBCount = 0;
    //public static int VoiceCount, TrackingCount = 0;

    //public static string LatestInstruction = "";

    public static TimeSpan t;
    public static string InstructionDeliverAndTimeStamp = "";

    [SerializeField]
    private UserInputRecorder _userInputRecorder;
      
    //*************************************************************
        
    private void Start()
    {
        _audioClass.PlayClick();
        // StartCoroutine(FiveMinuteBaseline());
        _manipulateAugmentations.HideAllAugmentations();
        VoiceCue.text = "Basline recording is about to begin.";
    }

    public void Update()
    {

        if (!InstructionSet)
            SetInstructionTextAndExpectedRotation();

        if (!PhaseControl.Equals(string.Empty))
        {
            Debug.Log("PhaseControl: " +PhaseControl);
            if (PhaseControl.Equals("b_phase"))
            {
                //dont hit b twice, but just incase.
                if (!BaselinePhase)
                {
                    Debug.Log("Starting baseline phase");
                    VoiceCue.text = " ";

                    EyetrackingPlane.SetActive(false);
                    BaselineEyetrackingPlane.SetActive(true);

                    BaselinePhase = true;

                    BasicInputLogger.InstructionAndTSBuffer = new StringBuilder();
                    _userInputRecorder.StartLogging();

                    //_audioClass.PlayPhaseChange();

                    AppPerformanceProfiling APP = FindObjectOfType<AppPerformanceProfiling>();
                    APP = new AppPerformanceProfiling();
                    APP.CreateNewLogFile();
                    StartCoroutine(APP.FPS()); 
                }
            }
            else if (PhaseControl.Equals("p_phase"))
            {
                PracticePhase = true;

                BaselineEyetrackingPlane.SetActive(false);
                EyetrackingPlane.SetActive(true);

                _manipulateAugmentations.ShowAllAugmentations();
            }
            else if (PhaseControl.Equals("w_phase"))
            {
                TrainingPhase = false;           
                                
                _manipulateAugmentations.HideAllAugmentations();
                _manipulateAugmentations.VoiceCue.SetActive(false);

                EyetrackingPlane.SetActive(false);
                BaselineEyetrackingPlane.SetActive(true);

                LogMetrics();
            }
            else if (PhaseControl.Equals("r_phase"))
            {
                RecallPhase = true;
                LogMetrics();

                VoiceCue.text = "Perform the Cube procedure." + Environment.NewLine + "When finished let the PI know.";
                //_audioClass.PlayPhaseChange(); 
                
                phase.text = "Recall phase";
                _manipulateAugmentations.ShowPhase();
                _audioClass.PlayClick();
            }
            else if (PhaseControl.Equals("stop_reset"))
            {
                Debug.Log("UDP deaf");
                UDPListen = false;
               // Spawn s = GameObject.Find("FreshCube").GetComponent<Spawn>();
               // s.SpawnMe();
               // _manipulateAugmentations.InitialiseCubeModelPosition();
            }
            else if (PhaseControl.Equals("restart"))
            {
                /* if (PracticePhase)
                 {
                     PracticeInstructionNumber = 1;
                 }
                 if (TrainingPhase)
                 {
                     TrainingInstructionNumber = 1;
                 }*/

                Debug.Log("UDP undeaf");
                UDPListen = true;
                
                //SetInstructionTextAndExpectedRotation();
            }
            else if (PhaseControl.Equals("finish"))
            {                
                RecallPhase = false;
                Terminate();
            }
            else
            {
                PhaseControl = string.Empty;
            }
            PhaseControl = string.Empty;
        }

        if (!ActualRotation.Equals("")) //We recieved a rotation via UDP.
        {
            FadeOnce = false;

            //Really for recording all wrong moves. Otherwise there is no record of errors made.          
            LogMetrics();

            if (!RecallPhase)
            {
                if (!ActualRotation.Equals(ExpectedRotation))
                {
                    //  if (!RecallPhase)
                    //  {
                    _audioClass.PlayWrong();
                    WrongMoveCount++;
                    WrongMoves.Add(ActualRotation);

                    Automate.UndoInstruction(Automate.UndoRotation);    //undo the instruction animation even if it was corrective,                   
                    Automate.AnimateWrongMove(ActualRotation);          //and do the wrong move animation.
                    WrongMove = true;
                    Automate.WrongCount = 2;

                    //Automate.DontUpdateCubeModelAfterMistake = true;
                    //  }
                }
                else
                {                   

                    if (WrongMoves.Count != 0)
                    {
                        // _audioClass.PlayCorrect();
                        WrongMoves.RemoveAt(WrongMoves.Count - 1); //User did the expected correcitve instruction.                 
                    }
                    else
                    {
                        if (PracticePhase)
                        {
                            //_audioClass.PlayCorrect();
                            if (PracticeInstructionNumber < 12)
                            {
                                PracticeInstructionNumber++;
                            }
                            else
                            {
                                PracticePhase = false;
                            }
                        }

                        if (TrainingPhase)
                        {
                            // _audioClass.PlayCorrect();
                            if (TrainingInstructionNumber < 14)
                            {
                                TrainingInstructionNumber++;
                            }
                            else
                            {
                                NumberofTrainingCycles += 0.5f;
                                _audioClass.PlayClick(); // PlayPhaseChange(); //PlayCorrect();
                                TrainingInstructionNumber = 1;
                                phase.text = "Training cycle " + NumberofTrainingCycles.ToString("F1");
                            }
                        }
                    }
                }

                Automate.RotToUpdateCubeModelWith = Automate.FaceToRotate;
                Automate.FaceToRotate = String.Empty;

                ActualRotation = String.Empty;
                Automate.UndoRotation = String.Empty;

                Automate.UpdateCubeModelAfterARotation = true;
                Automate.LooAtTheCurrentCubeFace = true;

                InstructionSet = false; //To get the next instruction, even if its corrective.
                                              
                

                //cannot set before previous if(TrainingPhase) statement.
                if (PracticeInstructionNumber >= 12 && !PracticePhase && !TrainingPhase)
                {
                    //if (!RecallPhase)
                    //{

                    PracticePhase = false;
                    TrainingPhase = true;
                    _audioClass.PlayStartTrainingPhase();
                    phase.text = "Training cycle " + NumberofTrainingCycles;                    
                    _manipulateAugmentations.VoiceCue.SetActive(true);
                  //  StartCoroutine(_manipulateAugmentations.FadePhaseInAndOut());

                    //}

                    /*if (PracticeInstructionNumber != 1)
                    {
                      //  _audioClass.PlayCorrect();

                    }*/

                }


                Automate.DebugDoOnce = false;                
                
                PivotRotation.speed = 2000; //allows the user to rotate quickly, as model snaps to end of animation.              

                CubeIsInSolvedState = false;
            }
            else
            {
                ActualRotation = String.Empty;
            }
        }
    }

    private void SetInstructionTextAndExpectedRotation()
    {
        if (!FadeOnce)
        {
            StartCoroutine(_manipulateAugmentations.FadeInstructionsInAndOut()); //To differentiate the instructions to the user.
            FadeOnce = true;
        }
        
        Automate.UndoWrongMove = false;

        if (WrongMoves.Count != 0)
        {
            //Reverse the last wrong move.
            switch (WrongMoves.Last())
            {
                case "U":
                    ExpectedRotation = "U'";
                    instruction.text = "Corrective instruction: Rotate the Green face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "U'";
                    Automate.UndoRotation = "U";                    
                    LogMetrics();
                    break;
                case "U'":
                    ExpectedRotation = "U";
                    instruction.text = "Corrective instruction: Rotate the Green face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "U";
                    Automate.UndoRotation = "U'";                    
                    LogMetrics();
                    break;

                case "D":
                    ExpectedRotation = "D'";
                    instruction.text = "Corrective instruction: Rotate the Blue face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "D'";
                    Automate.UndoRotation = "D";                    
                    LogMetrics();
                    break;
                case "D'":
                    ExpectedRotation = "D";
                    instruction.text = "Corrective instruction: Rotate the Blue face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "D";
                    Automate.UndoRotation = "D'";                    
                    LogMetrics();
                    break;


                case "L":
                    ExpectedRotation = "L'";
                    instruction.text = "Corrective instruction: Rotate the Orange face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "L'";
                    Automate.UndoRotation = "L";
                    LogMetrics();
                    break;
                case "L'":
                    ExpectedRotation = "L";
                    instruction.text = "Corrective instruction: Rotate the Orange face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "L";
                    Automate.UndoRotation = "L'";                    
                    LogMetrics();
                    break;

                case "R":
                    ExpectedRotation = "R'";
                    instruction.text = "Corrective instruction: Rotate the Red face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "R'";
                    Automate.UndoRotation = "R";                    
                    LogMetrics();
                    break;
                case "R'":
                    ExpectedRotation = "R";
                    instruction.text = "Corrective instruction: Rotate the Red face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "R";
                    Automate.UndoRotation = "R'";                    
                    LogMetrics();
                    break;

                case "F":
                    ExpectedRotation = "F'";
                    instruction.text = "Corrective instruction: Rotate the Yellow face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "F'";
                    Automate.UndoRotation = "F";
                    LogMetrics();
                    break;
                case "F'":
                    ExpectedRotation = "F";
                    instruction.text = "Corrective instruction: Rotate the Yellow face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "F";
                    Automate.UndoRotation = "F'";               
                    LogMetrics();
                    break;


                case "B":
                    ExpectedRotation = "B'";
                    instruction.text = "Corrective instruction: Rotate the White face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "B'";
                    Automate.UndoRotation = "B";                    
                    LogMetrics();
                    break;
                case "B'":
                    ExpectedRotation = "B";
                    instruction.text = "Corrective instruction: Rotate the White face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "B";
                    Automate.UndoRotation = "B'";                   
                    LogMetrics();
                    break;

                default:
                    instruction.text = "Error setting instruction text to reverse the last wrong move";
                    break;
            }
            InstructionSet = true;            
        }
        else if (PracticePhase)
        {
            switch (PracticeInstructionNumber)
            {
                case 1:
                    ExpectedRotation = "R";
                    instruction.text = "Instruction " + PracticeInstructionNumber + ": Rotate the Red face 90<sup>o</sup> clockwise";

                    Automate.FaceToRotate = "R";
                    Automate.UndoRotation = "R'"; //in the event of a user mistake.
                                                  //     PivotRotation.speed = 60;

                    LogMetrics();
                    break;
                case 2:
                    ExpectedRotation = "R'";
                    instruction.text = "Instruction " + PracticeInstructionNumber + ": Rotate the Red face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "R'";
                    Automate.UndoRotation = "R";
                    //      PivotRotation.speed = 60;
                    LogMetrics();
                    break;

                case 3:
                    ExpectedRotation = "B";
                    instruction.text = "Instruction " + PracticeInstructionNumber + ": Rotate the White face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "B";
                    Automate.UndoRotation = "B'";
                    //      PivotRotation.speed = 60;
                    LogMetrics();
                    break;
                case 4:
                    ExpectedRotation = "B'";
                    instruction.text = "Instruction " + PracticeInstructionNumber + ": Rotate the White face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "B'";
                    Automate.UndoRotation = "B";
                    //      PivotRotation.speed = 60;
                    LogMetrics();
                    break;

                case 5:
                    ExpectedRotation = "L";
                    instruction.text = "Instruction " + PracticeInstructionNumber + ": Rotate the Orange face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "L";
                    Automate.UndoRotation = "L'";
                    //       PivotRotation.speed = 60;
                    LogMetrics();
                    break;
                case 6:
                    ExpectedRotation = "L'";
                    instruction.text = "Instruction " + PracticeInstructionNumber + ": Rotate the Orange face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "L'";
                    Automate.UndoRotation = "L";
                    //       PivotRotation.speed = 60;
                    LogMetrics();
                    break;

                case 7:
                    ExpectedRotation = "F";
                    instruction.text = "Instruction " + PracticeInstructionNumber + ": Rotate the Yellow face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "F";
                    Automate.UndoRotation = "F'";
                    //        PivotRotation.speed = 60;
                    LogMetrics();
                    break;
                case 8:
                    ExpectedRotation = "F'";
                    instruction.text = "Instruction " + PracticeInstructionNumber + ": Rotate the Yellow face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "F'";
                    Automate.UndoRotation = "F";
                    //      PivotRotation.speed = 60;
                    LogMetrics();
                    break;

                case 9:
                    ExpectedRotation = "U";
                    instruction.text = "Instruction " + PracticeInstructionNumber + ": Rotate the Green face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "U";
                    Automate.UndoRotation = "U'";
                    //      PivotRotation.speed = 60;
                    LogMetrics();
                    break;
                case 10:
                    ExpectedRotation = "U'";
                    instruction.text = "Instruction " + PracticeInstructionNumber + ": Rotate the Green face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "U'";
                    Automate.UndoRotation = "U";
                    //      PivotRotation.speed = 60;
                    LogMetrics();
                    break;

                case 11:
                    ExpectedRotation = "D";
                    instruction.text = "Instruction " + PracticeInstructionNumber + ": Rotate the Blue face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "D";
                    Automate.UndoRotation = "D'";
                    //      PivotRotation.speed = 60;
                    LogMetrics();
                    break;
                case 12:
                    ExpectedRotation = "D'";
                    instruction.text = "Instruction " + PracticeInstructionNumber + ": Rotate the Blue face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "D'";
                    Automate.UndoRotation = "D";
                    //      PivotRotation.speed = 60;
                    LogMetrics();
                    break;

                default:
                    instruction.text = "No instruction text available for the current invalid practice instruction number " + PracticeInstructionNumber;
                    break;
            }
            InstructionSet = true;
            PivotRotation.speed = 30; //50;
        }
        else if (TrainingPhase)
        {
            switch (TrainingInstructionNumber)
            {
                case 1:
                    ExpectedRotation = "L";
                    instruction.text = "Instruction " + TrainingInstructionNumber + ": Rotate the Orange face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "L";
                    Automate.UndoRotation = "L'";
                    //     PivotRotation.speed = 60;
                    CubeIsInSolvedState = true;
                    if (NumberofTrainingCycles > 1)
                    {
                        VoiceCue.text = "Let the PI know if you would like to end training now.";
                    }
                    LogMetrics();
                    break;
                case 2:
                    ExpectedRotation = "L";
                    instruction.text = "Instruction " + TrainingInstructionNumber + ": Rotate the Orange face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "L";
                    Automate.UndoRotation = "L'";
                    //     PivotRotation.speed = 60;
                    CubeIsInSolvedState = false;
                    VoiceCue.text = " ";
                    LogMetrics();
                    break;
                case 3:
                    ExpectedRotation = "F";
                    instruction.text = "Instruction " + TrainingInstructionNumber + ": Rotate the Yellow face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "F";
                    Automate.UndoRotation = "F'";
                    //    PivotRotation.speed = 60;
                    VoiceCue.text = " ";
                    LogMetrics();
                    break;
                case 4:
                    ExpectedRotation = "F";
                    instruction.text = "Instruction " + TrainingInstructionNumber + ": Rotate the Yellow face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "F";
                    Automate.UndoRotation = "F'";
                    //      PivotRotation.speed = 60;
                    VoiceCue.text = " ";
                    LogMetrics();
                    break;
                case 5:
                    ExpectedRotation = "B";
                    instruction.text = "Instruction " + TrainingInstructionNumber + ": Rotate the White face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "B";
                    Automate.UndoRotation = "B'";
                    //      PivotRotation.speed = 60;
                    VoiceCue.text = " ";
                    LogMetrics();
                    break;
                case 6:
                    ExpectedRotation = "B";
                    instruction.text = "Instruction " + TrainingInstructionNumber + ": Rotate the White face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "B";
                    Automate.UndoRotation = "B'";
                    //      PivotRotation.speed = 60;
                    VoiceCue.text = " ";
                    LogMetrics();
                    break;
                case 7:
                    ExpectedRotation = "L";
                    instruction.text = "Instruction " + TrainingInstructionNumber + ": Rotate the Orange face 90<sup>o</sup> clockwise";
                    Automate.FaceToRotate = "L";
                    Automate.UndoRotation = "L'";
                    //      PivotRotation.speed = 60;
                    VoiceCue.text = " ";
                    LogMetrics();
                    break;
                case 8:
                    ExpectedRotation = "L'";
                    instruction.text = "Instruction " + TrainingInstructionNumber + ": Rotate the Orange face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "L'";
                    Automate.UndoRotation = "L";
                    //     PivotRotation.speed = 60;
                    NumberofTrainingCycles += 0.5f;
                    phase.text = "Training cycle " + NumberofTrainingCycles.ToString("F1");
                    _audioClass.PlayHalfWay(); //PlayCorrect();
                   // StartCoroutine(_manipulateAugmentations.FadePhaseInAndOut());
                    VoiceCue.text = " ";
                    LogMetrics();
                    break;
                case 9:
                    ExpectedRotation = "B'";
                    instruction.text = "Instruction " + TrainingInstructionNumber + ": Rotate the White face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "B'";
                    Automate.UndoRotation = "B";
                    //     PivotRotation.speed = 60;
                    VoiceCue.text = " ";
                    LogMetrics();
                    break;
                case 10:
                    ExpectedRotation = "B'";
                    instruction.text = "Instruction " + TrainingInstructionNumber + ": Rotate the White face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "B'";
                    Automate.UndoRotation = "F'";
                    //    PivotRotation.speed = 60;
                    VoiceCue.text = " ";
                    LogMetrics();
                    break;
                case 11:
                    ExpectedRotation = "F'";
                    instruction.text = "Instruction " + TrainingInstructionNumber + ": Rotate the Yellow face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "F'";
                    Automate.UndoRotation = "F";
                    //     PivotRotation.speed = 60;
                    VoiceCue.text = " ";
                    LogMetrics();
                    break;
                case 12:
                    ExpectedRotation = "F'";
                    instruction.text = "Instruction " + TrainingInstructionNumber + ": Rotate the Yellow face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "F'";
                    Automate.UndoRotation = "F";
                    //     PivotRotation.speed = 60;
                    VoiceCue.text = " ";
                    LogMetrics();
                    break;
                case 13:
                    ExpectedRotation = "L'";
                    instruction.text = "Instruction " + TrainingInstructionNumber + ": Rotate the Orange face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "L'";
                    Automate.UndoRotation = "L";
                    //      PivotRotation.speed = 60;
                    VoiceCue.text = " ";
                    LogMetrics();
                    break;
                case 14:
                    ExpectedRotation = "L'";
                    instruction.text = "Instruction " + TrainingInstructionNumber + ": Rotate the Orange face 90<sup>o</sup> anticlockwise";
                    Automate.FaceToRotate = "L'";
                    Automate.UndoRotation = "L";
                    //      PivotRotation.speed = 60;
                    VoiceCue.text = " ";
                    LogMetrics();
                    break;

                default:
                    instruction.text = "No instruction text for invalid training instruction" + TrainingInstructionNumber;
                    break;
            }
            InstructionSet = true;
            PivotRotation.speed = 30; //50;
        }
        else {
            instruction.text = "Practice phase is about to begin.";
            InstructionSet = false;
        }
    }       
    
    void LogMetrics()
    {
        InstructionDeliverAndTimeStamp = "";
        t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
        long timestamp = (long)t.TotalMilliseconds;
        float trainingcycles = NumberofTrainingCycles;

        if (trainingcycles.ToString().Contains(".5"))
        {
            trainingcycles -= 1.5f;
        }
        else
        {
            trainingcycles -= 1;
        }

        InstructionDeliverAndTimeStamp =
            +timestamp + ","            
            + PracticePhase + ","
            + TrainingPhase + ","
            + RecallPhase + ","
            + PracticeInstructionNumber + ","
            + TrainingInstructionNumber + ","
            + trainingcycles + ","
            + ActualRotation + ","
            + "\n";

        BasicInputLogger.InstructionAndTSBuffer.AppendLine(Methodology.InstructionDeliverAndTimeStamp);
    }
    
    public void Terminate()
    {
        _manipulateAugmentations.HideAllAugmentations();
        VoiceCue.text = "Thank you for your time.";
        _userInputRecorder.StopLoggingAndSave();
        AppPerformanceProfiling.WrtitePofilingDataToFile();
        _audioClass.PlayClick(); //PlaySuccess();     
    }
}
