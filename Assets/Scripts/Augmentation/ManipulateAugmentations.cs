using System.Collections;
using System.Threading;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

//using Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking.Logging;

public class ManipulateAugmentations : MonoBehaviour 
{

    public GameObject CubeModel;
    public GameObject Instruction;
    public GameObject Phase;
    public GameObject Arrow1, Arrow2, Arrow3, Arrow4;
    public GameObject Arrow5, Arrow6, Arrow7, Arrow8;

    [SerializeField]
    public GameObject VoiceCue;


    //For fadeing instructions in and out to distinguish each instruction.
    public bool Fadeing = false;

    [SerializeField]
    private TextMeshPro _instruction;

    [SerializeField]
    private TextMeshPro _phase;

    [SerializeField]
    private List<GameObject> reds = new List<GameObject>();

    [SerializeField]
    private List<GameObject> whites = new List<GameObject>();

    [SerializeField]
    private List<GameObject> oranges = new List<GameObject>();

    [SerializeField]
    private List<GameObject> yellows = new List<GameObject>();

    [SerializeField]
    private List<GameObject> greens = new List<GameObject>();

    [SerializeField]
    private List<GameObject> blues = new List<GameObject>();

    public void InitialiseCubeModelPosition()
    {
        CubeModel.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        CubeModel.transform.localPosition = new Vector3(AppPerformanceProfiling.position.x + 0.25f, AppPerformanceProfiling.position.y - 0.23f, AppPerformanceProfiling.position.z + 0.8f);
        CubeModel.transform.Rotate(-15, 45, -15, Space.Self);
    }

    /*public void DisplayCubeModelAndInstruction()
    {
        CubeModel.SetActive(true);        
        Instruction.SetActive(true);
    }*/
    
    public void HideAllAugmentations() //for during recall phase.
    {
        CubeModel.SetActive(false);      
        Instruction.SetActive(false);  
        Phase.SetActive(false);
        //VoiceCue.SetActive(false);
        Arrow1.SetActive(false);
        Arrow2.SetActive(false);
        Arrow3.SetActive(false);
        Arrow4.SetActive(false);
        Arrow5.SetActive(false);
        Arrow6.SetActive(false);
        Arrow7.SetActive(false);
        Arrow8.SetActive(false);
    }

    public void ShowAllAugmentations() 
    {
        CubeModel.SetActive(true);
        Instruction.SetActive(true);
        Phase.SetActive(true);
        VoiceCue.SetActive(true);
        Arrow1.SetActive(true);
        Arrow2.SetActive(true);
        Arrow3.SetActive(true);
        Arrow4.SetActive(true);
        Arrow5.SetActive(true);
        Arrow6.SetActive(true);
        Arrow7.SetActive(true);
        Arrow8.SetActive(true);
    }

    public void ShowPhase()
    {
        Phase.SetActive(true);
        VoiceCue.SetActive(true);
    }

    public void HideClockWiseArrows()
    {
        Arrow1.SetActive(false);
        Arrow2.SetActive(false);
        Arrow3.SetActive(false);
        Arrow4.SetActive(false);
    }

    public void HideAntiClockWiseArrows()
    {
        Arrow5.SetActive(false);
        Arrow6.SetActive(false);
        Arrow7.SetActive(false);
        Arrow8.SetActive(false);
    }

    public void LookAtFace(string face)
    {        
        CubeModel.transform.rotation = Quaternion.identity;

        if (!CubeState.autoRotating)
        {
            switch (face)
            {
                case "U":
                    CubeModel.transform.Rotate(-15, -45, 115, Space.Self);
                    //CubeState.autoRotating = false;
                    break;
                case "U'":
                    CubeModel.transform.Rotate(-15, -45, 115, Space.Self);
                    //CubeState.autoRotating = false;
                    break;

                case "D":
                    CubeModel.transform.Rotate(-15, -45, -75, Space.Self);
                    //CubeState.autoRotating = false;
                    break;
                case "D'":
                    CubeModel.transform.Rotate(-15, -45, -75, Space.Self);
                    //CubeState.autoRotating = false;
                    break;

                case "L":
                    CubeModel.transform.Rotate(15, -135, 15, Space.Self);
                    //CubeState.autoRotating = false;
                    break;
                case "L'":
                    CubeModel.transform.Rotate(15, -135, 15, Space.Self);
                    //CubeState.autoRotating = false;
                    break;

                case "R":
                    CubeModel.transform.Rotate(-15, 45, -15, Space.Self);
                    //CubeState.autoRotating = false;
                    break;
                case "R'":
                    CubeModel.transform.Rotate(-15, 45, -15, Space.Self);
                    //CubeState.autoRotating = false;
                    break;

                case "F":
                    CubeModel.transform.Rotate(-15, -45, 15, Space.Self);
                    //CubeState.autoRotating = false;
                    break;
                case "F'":
                    CubeModel.transform.Rotate(-15, -45, 15, Space.Self);
                    //CubeState.autoRotating = false;
                    break;

                case "B":
                    CubeModel.transform.Rotate(15, 135, -15, Space.Self);
                    //CubeState.autoRotating = false;
                    break;
                case "B'":
                    CubeModel.transform.Rotate(15, 135, -15, Space.Self);
                    //CubeState.autoRotating = false;
                    break;

                default:
                    break;
            }

        }
    }

    public IEnumerator FadePhaseInAndOut()
    {
        float fadespeed = 2;

        while (_instruction.color.a > 0) //(currentTime < duration)
        {
            float fadeAmount = _instruction.color.a - (fadespeed * Time.deltaTime);

            _phase.color = new Color(_instruction.color.r, _instruction.color.g, _instruction.color.b, fadeAmount);
            yield return null;
        }

        while (_instruction.color.a < 1)//(currentTime < duration)
        {
            float fadeAmount = _instruction.color.a + (fadespeed * Time.deltaTime);

            _phase.color = new Color(_instruction.color.r, _instruction.color.g, _instruction.color.b, fadeAmount);

            yield return null;
        }
        yield return null;
    }

    public IEnumerator FadeInstructionsInAndOut()
    {
        float fadespeed = 2;


        //Fadeing = true;

        //Fade out instructions
        while (_instruction.color.a > 0) //(currentTime < duration)
        {
            float fadeAmount = _instruction.color.a - (fadespeed * Time.deltaTime);

          //  Color ArrowsColor = Arrow1.GetComponent<Renderer>().material.color;
            
           /* Color RedColor = reds.First().GetComponent<Renderer>().material.color;
            Color WhiteColor = whites.First().GetComponent<Renderer>().material.color;
            Color OrangeColor = oranges.First().GetComponent<Renderer>().material.color;
            Color YellowColor = yellows.First().GetComponent<Renderer>().material.color;
            Color GreenColor = greens.First().GetComponent<Renderer>().material.color;
            Color BlueColor = blues.First().GetComponent<Renderer>().material.color;*/

            //   Color BlackColor = black.GetComponent<Renderer>().material.color;

            _instruction.color = new Color(_instruction.color.r, _instruction.color.g, _instruction.color.b, fadeAmount);
         //   _phase.color = new Color(_instruction.color.r, _instruction.color.g, _instruction.color.b, fadeAmount);

           /* ArrowsColor = new Color(ArrowsColor.r, ArrowsColor.g, ArrowsColor.b, fadeAmount);
            Arrow1.GetComponent<Renderer>().material.color = ArrowsColor;
            Arrow2.GetComponent<Renderer>().material.color = ArrowsColor;
            Arrow3.GetComponent<Renderer>().material.color = ArrowsColor;
            Arrow4.GetComponent<Renderer>().material.color = ArrowsColor;
            Arrow5.GetComponent<Renderer>().material.color = ArrowsColor;
            Arrow6.GetComponent<Renderer>().material.color = ArrowsColor;
            Arrow7.GetComponent<Renderer>().material.color = ArrowsColor;
            Arrow8.GetComponent<Renderer>().material.color = ArrowsColor;*/

            //  //  BlackColor = new Color(BlackColor.r, BlackColor.g, BlackColor.b, fadeAmount);
           /* RedColor = new Color(RedColor.r, RedColor.g, RedColor.b, fadeAmount);
            WhiteColor = new Color(WhiteColor.r, WhiteColor.g, WhiteColor.b, fadeAmount);
            OrangeColor = new Color(OrangeColor.r, OrangeColor.g, OrangeColor.b, fadeAmount);
            YellowColor = new Color(YellowColor.r, YellowColor.g, YellowColor.b, fadeAmount);
            GreenColor = new Color(GreenColor.r, GreenColor.g, GreenColor.b, fadeAmount);
            BlueColor = new Color(BlueColor.r, BlueColor.g, BlueColor.b, fadeAmount);*/

            //   black.GetComponent<Renderer>().material.color = BlackColor;

           /* foreach (GameObject RedTile in reds)
            {
                RedTile.GetComponent<Renderer>().material.color = RedColor;
            }
           
            foreach (GameObject WhiteTile in whites)
            {
                WhiteTile.GetComponent<Renderer>().material.color = WhiteColor;
            }

            foreach (GameObject OrangeTile in oranges)
            {
                OrangeTile.GetComponent<Renderer>().material.color = OrangeColor;
            }

            foreach (GameObject YellowTile in yellows)
            {
                YellowTile.GetComponent<Renderer>().material.color = YellowColor;
            }

            foreach (GameObject GreenTile in greens)
            {
                GreenTile.GetComponent<Renderer>().material.color = GreenColor;
            }

            foreach (GameObject BlueTile in blues)
            {
                BlueTile.GetComponent<Renderer>().material.color = BlueColor;
            }*/

            yield return null;
        }

        //Fade in instructions
        while (_instruction.color.a < 1)//(currentTime < duration)
        {
            float fadeAmount = _instruction.color.a + (fadespeed * Time.deltaTime);

           // Color ArrowsColor = Arrow1.GetComponent<Renderer>().material.color;
            
          /*  Color RedColor = reds.First().GetComponent<Renderer>().material.color;
            Color WhiteColor = whites.First().GetComponent<Renderer>().material.color;
            Color OrangeColor = oranges.First().GetComponent<Renderer>().material.color;
            Color YellowColor = yellows.First().GetComponent<Renderer>().material.color;
            Color GreenColor = greens.First().GetComponent<Renderer>().material.color;
            Color BlueColor = blues.First().GetComponent<Renderer>().material.color;*/

            //   Color BlackColor = black.GetComponent<Renderer>().material.color;

            _instruction.color = new Color(_instruction.color.r, _instruction.color.g, _instruction.color.b, fadeAmount);
          //  _phase.color = new Color(_instruction.color.r, _instruction.color.g, _instruction.color.b, fadeAmount);

            /*ArrowsColor = new Color(ArrowsColor.r, ArrowsColor.g, ArrowsColor.b, fadeAmount);
            Arrow1.GetComponent<Renderer>().material.color = ArrowsColor;
            Arrow2.GetComponent<Renderer>().material.color = ArrowsColor;
            Arrow3.GetComponent<Renderer>().material.color = ArrowsColor;
            Arrow4.GetComponent<Renderer>().material.color = ArrowsColor;
            Arrow5.GetComponent<Renderer>().material.color = ArrowsColor;
            Arrow6.GetComponent<Renderer>().material.color = ArrowsColor;
            Arrow7.GetComponent<Renderer>().material.color = ArrowsColor;
            Arrow8.GetComponent<Renderer>().material.color = ArrowsColor;*/

            //   BlackColor = new Color(BlackColor.r, BlackColor.g, BlackColor.b, fadeAmount);
          /*  RedColor = new Color(RedColor.r, RedColor.g, RedColor.b, fadeAmount);
            WhiteColor = new Color(WhiteColor.r, WhiteColor.g, WhiteColor.b, fadeAmount);
            OrangeColor = new Color(OrangeColor.r, OrangeColor.g, OrangeColor.b, fadeAmount);
            YellowColor = new Color(YellowColor.r, YellowColor.g, YellowColor.b, fadeAmount);
            GreenColor = new Color(GreenColor.r, GreenColor.g, GreenColor.b, fadeAmount);
            BlueColor = new Color(BlueColor.r, BlueColor.g, BlueColor.b, fadeAmount);*/

            //   black.GetComponent<Renderer>().material.color = BlackColor;

            /*foreach (GameObject RedTile in reds)
            {
                RedTile.GetComponent<Renderer>().material.color = RedColor;
            }                  

            foreach (GameObject WhiteTile in whites)
            {
                WhiteTile.GetComponent<Renderer>().material.color = WhiteColor;
            }

            foreach(GameObject OrangeTile in oranges)
            {
                OrangeTile.GetComponent<Renderer>().material.color = OrangeColor;
            }

            foreach (GameObject YellowTile in yellows)
            {
                YellowTile.GetComponent<Renderer>().material.color = YellowColor;
            }

            foreach (GameObject GreenTile in greens)
            {
                GreenTile.GetComponent<Renderer>().material.color = GreenColor;
            }

            foreach (GameObject BlueTile in blues)
            {
                BlueTile.GetComponent<Renderer>().material.color = BlueColor;
            }*/

            yield return null;
        }

        yield break;

        // Fadeing = false;
    }
}
