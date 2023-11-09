using UnityEngine;
using TMPro;

public class Instruction : MonoBehaviour
{
    public TextMeshPro _Instruction;

    public void ShowInstruction(string instruction)
    {
        _Instruction.text = instruction;
    }
}
