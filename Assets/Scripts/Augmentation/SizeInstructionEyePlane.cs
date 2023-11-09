using UnityEngine;
using TMPro;
using System;

public class SizeInstructionEyePlane : MonoBehaviour
{
    public TextMeshPro instruction;
    public GameObject InstructionEyeHitPlane;   

   // public TMP_Text InstructionText;
    private Vector2 InstructionTextSize = new Vector2(0, 0);

    private Vector3 OrigSize = new Vector3(1, 1, 1);
    private bool DoOnce = false;

    float y,z = 0.0f;

    void Start()
    {       
       OrigSize = InstructionEyeHitPlane.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        y = instruction.transform.position.y - 0.06f;
        z = instruction.transform.position.z - 0.08999825f;       

        try
        {

            InstructionTextSize = instruction.GetPreferredValues(instruction.text); //InstructionText.GetPreferredValues(instruction.text);
            

            if (InstructionTextSize.x > 40)
            {
                InstructionTextSize.x = 40;
            }
            else
            {
                InstructionTextSize.x += 0.02f;
            }

            if (InstructionTextSize.y > 2)
            {
                InstructionTextSize.y += 2;
            }

            if (InstructionTextSize.y > 4.3)
            {               
                InstructionTextSize.x += 0.1f;
            }
        }
        catch (Exception e)
        {
            Debug.Log("1: " + e);
        }

        try { 
            if (Methodology.TrainingInstructionNumber == 5)
            {
                if (DoOnce == false)
                {
                    InstructionEyeHitPlane.transform.localScale = new Vector3(InstructionEyeHitPlane.transform.localScale.x, InstructionEyeHitPlane.transform.localScale.y + 4, InstructionEyeHitPlane.transform.localScale.z);
                    DoOnce = true;
                }
            }
            else
            {
                InstructionEyeHitPlane.transform.localScale = OrigSize;
                DoOnce = false;
            }           
        }
        catch (Exception e)
        {
            Debug.Log("2: " + e);
        }
    }
}
