using UnityEngine;
using Valve.VR;

public class AnimateHands : MonoBehaviour
{
    public GameObject avatar;

    //    private SteamVR_ActionSet[] actionSets;
    private Animator animator;

    private void Start()
    {
        animator = avatar.GetComponent<Animator>();
        SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(LeftTriggerPressed, SteamVR_Input_Sources.LeftHand);
        SteamVR_Actions.default_GrabPinch.AddOnStateUpListener(LeftTriggerUnpressed, SteamVR_Input_Sources.LeftHand);
        SteamVR_Actions.default_GrabPinch.AddOnStateDownListener(RightTriggerPressed, SteamVR_Input_Sources.RightHand);
        SteamVR_Actions.default_GrabPinch.AddOnStateUpListener(RightTriggerUnpressed, SteamVR_Input_Sources.RightHand);
    }

    private void LeftTriggerPressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        animator.SetBool("isClosedL", true);
    }

    private void LeftTriggerUnpressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        animator.SetBool("isClosedL", false);
    }

    private void RightTriggerPressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        animator.SetBool("isClosedR", true);
    }

    private void RightTriggerUnpressed(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
    {
        animator.SetBool("isClosedR", false);
    }
}

//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using Valve.VR;

//public class ControllerInputManager : MonoBehaviour
//{
//    public GameObject avatar;

//    private SteamVR_ActionSet[] actionSets;
//    private Animator animator;

//    // Use this for initialization
//    void Start()
//    {
//        actionSets = SteamVR_Input.actionSets;
//        //if (actionSets == null) { actionSets = SteamVR_Input_References.instance.actionSetObjects; }
//        animator = avatar.GetComponent<Animator>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        var sources = SteamVR_Input_Source.GetAllSources();
//        foreach (var source in sources)
//        {
//            if (source == SteamVR_Input_Sources.Any) continue;
//            foreach (var actionSet in actionSets)
//            {
//                foreach (var action in actionSet.allActions) //ボタン
//                {
//                    if (action is SteamVR_Action_Boolean)
//                    {
//                        var actionBoolean = (SteamVR_Action_Boolean)action;
//                        if (actionBoolean.GetStateDown(source))
//                        {
//                            var name = actionBoolean.GetShortName();
//                            if (name == "InteractUI") // トリガー半引き
//                            {
//                                //Debug.Log("pressed Trigger half pull");
//                            }
//                            else if (name == "GrabPinch") // トリガー全引き
//                            {
//                                //Debug.Log("pressed  Full trigger pull");
//                            }
//                            else if (name == "Teleport") // タッチパッド押し
//                            {
//                                //Debug.Log("pressed touchpad push");
//                            }
//                            else if (name == "GrabGrip") // グリップボタン押し
//                            {
//                                //Debug.Log("pressed Grip button press");
//                            }
//                        }
//                    }
//                    else if (action is SteamVR_Action_Single) //Axis
//                    {
//                        var actionSingle = (SteamVR_Action_Single)action;
//                        if (actionSingle.GetChanged(source))
//                        {
//                            var name = actionSingle.GetShortName();
//                            var axis = actionSingle.GetAxis(source);
//                            if (name == "Squeeze") // トリガー
//                            {
//                                Debug.Log(axis);
//                                //Debug.Log("pressed trigger");
//                                animator.SetBool("isClosed", true);
//                                if (axis == 0)
//                                {
//                                    animator.SetBool("isClosed", false);
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }
//    }
//}