using UnityEngine;

public class GamepadAxisDebugger : MonoBehaviour
{
    void Update()
    {
        for (int i = 1; i <= 10; i++) // teste les 10 premiers axes
        {
            float axis = Input.GetAxis("JoystickAxis" + i);
            if (Mathf.Abs(axis) > 0.1f)
            {
                Debug.Log("JoystickAxis" + i + " = " + axis);
            }
        }
    }
}
