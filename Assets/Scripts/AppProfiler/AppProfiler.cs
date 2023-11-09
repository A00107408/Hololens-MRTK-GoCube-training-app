/*
 * ARubik. Project submitted to AIT in 2022 in partial fulfillment
 * of the requirements for the degree of PhD. 
 * A.I.T student: A00107408.  
 * Student Name: Eoghan Hynes.
 */

using System;
using System.Text;
using System.IO;

using UnityEngine;

using Microsoft.MixedReality.Toolkit.Diagnostics;
using System.Collections;

#if WINDOWS_UWP
using Windows.Storage;
#endif
public class AppPerformanceProfiling : MonoBehaviour
{   

    //Test framework members.
    public static bool testing = false;
    public static int InstInd = 0;
    public static bool startInstructions = false;

    /// <summary>
    /// App profiling members
    /// </summary>
    private string MyCPUUsage = "";    
    private TimeSpan t;
    public static long timestamp = 0;
    string cpuFPS, page, TSandStats = "";
    private static StringBuilder buffer = null;

    public static Vector3 position;

    private float _mFrequency = 1.0f;

#if WINDOWS_UWP
    private static StorageFile logFile;
    private StorageFolder logRootFolder = KnownFolders.MusicLibrary;
    private StorageFolder sessionFolder;
#endif

    public string UserName = "Participant";
    public string LogDirectory => Path.Combine("ProfilerStats", UserName);

    float avgFrameRate = 0;

    private string Filename
    {
        get
        {
            return FilenameWithTimestamp;
        }
    }

    protected string FilenameWithTimestamp
    {
        get { return (FormattedTimeStamp + ".csv"); }
    }

    private string FormattedTimeStamp
    {
        get
        {
            string year = (DateTime.Now.Year - 2000).ToString();
            string month = AddLeadingZeroToSingleDigitIntegers(DateTime.Now.Month);
            string day = AddLeadingZeroToSingleDigitIntegers(DateTime.Now.Day);
            string hour = AddLeadingZeroToSingleDigitIntegers(DateTime.Now.Hour);
            string minute = AddLeadingZeroToSingleDigitIntegers(DateTime.Now.Minute);
            string sec = AddLeadingZeroToSingleDigitIntegers(DateTime.Now.Second);

            return string.Format("{0}{1}{2}-{3}{4}{5}",
                year,
                month,
                day,
                hour,
                minute,
                sec);
        }
    }

    private string AddLeadingZeroToSingleDigitIntegers(int val)
    {
        return (val < 10) ? ("0" + val) : ("" + val);
    }

    private void Awake()
    {                  
      //  buffer = new StringBuilder();
      //  buffer.Length = 0;

        //CreateNewLogFile();
        //Debug.Log("Calling TS&Stats.");
        //StartCoroutine(FPS());
    }
      
    public AppPerformanceProfiling()
    {
        buffer = new StringBuilder();
        buffer.Length = 0;
    }

    public IEnumerator FPS()
    {        
        while (true)
        {
            // Capture frame-per-second
            int lastFrameCount = Time.frameCount;
            float lastTime = Time.realtimeSinceStartup;
            yield return new WaitForSeconds(_mFrequency);
            float timeSpan = Time.realtimeSinceStartup - lastTime;
            int frameCount = Time.frameCount - lastFrameCount;

            t = (DateTime.UtcNow - new DateTime(1970, 1, 1));
            timestamp = (long)t.TotalMilliseconds;

            MyCPUUsage = MixedRealityToolkitVisualProfiler.cpuFrameRateText.text;

            //Extract stats from strings
            GetPage(MyCPUUsage);

            //Concatinate stats into one string for .csv file write.
            TSandStats = timestamp.ToString() + "," + string.Format("{0}", Mathf.RoundToInt(frameCount / timeSpan)) + "," + page + "," + MixedRealityToolkitVisualProfiler.AppMemoryUsage.ToString(); //memory;

            //Append to app profiling buffer.
            buffer.AppendLine(TSandStats);

            //Debug.Log("TS&Stats: " +TSandStats);
        }    
    }      

    private void GetPage(string cpuUsageString)
    {
        string[] _1 = cpuUsageString.Split(' ');
        cpuFPS = _1[1];
        string pagestring = _1[3];
        string[] _2 = pagestring.Split('(');
        page = _2[1];
    }
        

#if WINDOWS_UWP
    public async void CreateNewLogFile()
    {
        try
        {
            if (logRootFolder != null)
            {
                string fullPath = Path.Combine(logRootFolder.Path, LogDirectory);

                if (!Directory.Exists(fullPath))
                {                    
                    sessionFolder = await logRootFolder.CreateFolderAsync(LogDirectory, CreationCollisionOption.GenerateUniqueName);

                    sessionFolder = await logRootFolder.GetFolderAsync(LogDirectory);
                    logFile = await sessionFolder.CreateFileAsync(Filename, CreationCollisionOption.ReplaceExisting);
                }
            }
        }
        catch (Exception ex)
        {
            sessionFolder = await logRootFolder.CreateFolderAsync(LogDirectory, CreationCollisionOption.GenerateUniqueName);
            Debug.Log("Exception: " + ex);
        }

        WriteHeader();
    }

    private async void WriteHeader()
    {
        try
        {
            await FileIO.AppendTextAsync(logFile, "Unix_TS,CPU_FPS,Page_Dur,Mem_Use_Bytes\n");
        }
        catch (Exception ex)
        {
            Debug.Log("File write exception " + ex);
        }
    }

    private static async void WriteStats()
    {
        do {
            try
            {
                await FileIO.AppendTextAsync(logFile, buffer.ToString());
                Debug.Log("Wrote profiler data to file.");
                break;
            }
            catch (Exception ex)
            {
                Debug.Log("File write exception " + ex);
            }
        }while(true);

        buffer = new StringBuilder();
        buffer.Length = 0;
    }
#else
    #region for UNITY_Editor   
    
    public void CreateNewLogFile(){}
    private void WriteHeder(){}
    private static void WriteStats(){}

    #endregion
#endif

    public static void WrtitePofilingDataToFile()
    {
        WriteStats();      
    }
}
