using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;


public class AgentController : Agent
{
    public GameObject robotArm;
    public GameObject robotHand;
    public Transform cube;


    public override void OnEpisodeBegin()
    {
        if (cube.position.y < 0.778f)
        {
            cube.position = new Vector3(0.485f, 0.778f, -0.17f);
        }
    }


    public override void OnActionReceived(float[] vectorAction)
    {
        RobotController robotController = robotArm.GetComponent<RobotController>();
        PincherController pincherController = robotHand.GetComponent<PincherController>();

        for (int i = 0; i < robotController.joints.Length + 1; i++)
        {
            float inputVal = vectorAction[i];

            if (i == 7)
            {
                pincherController.gripState = GripStateForInput(inputVal);
            }

            else
            {
                if (Mathf.Abs(inputVal) > 0)
                {
                    RotationDirection direction = GetRotationDirection(inputVal);
                    robotController.RotateJoint(i, direction);
                    return;
                }
            }
        }

        robotController.StopAllJointRotations();

        if (cube.position.y < 0.778f)
        {
            EndEpisode();
        }
    }


    public override void Heuristic(float[] vectorAction)
    {
        vectorAction[0] = Input.GetAxis("Base");
        vectorAction[1] = Input.GetAxis("Shoulder");
        vectorAction[2] = Input.GetAxis("Elbow");
        vectorAction[3] = Input.GetAxis("Wrist1");
        vectorAction[4] = Input.GetAxis("Wrist2");
        vectorAction[5] = Input.GetAxis("Wrist3");
        vectorAction[6] = Input.GetAxis("Hand");
        vectorAction[7] = Input.GetAxis("Fingers");
    }


    static GripState GripStateForInput(float inputVal)
    {
        if (inputVal > 0)
        {
            return GripState.Closing;
        }
        else if (inputVal < 0)
        {
            return GripState.Opening;
        }
        else
        {
            return GripState.Fixed;
        }
    }
    
    
    static RotationDirection GetRotationDirection(float inputVal)
    {
        if (inputVal > 0)
        {
            return RotationDirection.Positive;
        }
        else if (inputVal < 0)
        {
            return RotationDirection.Negative;
        }
        else
        {
            return RotationDirection.None;
        }
    }
}
