using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Valve.VR;

public class HandController : MonoBehaviour
{
    //ActionBasedController controller;
    public Hand hand;

    private SteamVR_ActionSet[] actionSets;

    // Start is called before the first frame update
    void Start()
    {
        //controller = GetComponent<ActionBasedController>();
        actionSets = SteamVR_Input.actionSets;
    }

    // Update is called once per frame
    void Update()
    {
        var sources = SteamVR_Input_Source.GetAllSources();
        foreach (var source in sources)
        {
            if (source == SteamVR_Input_Sources.Any) continue;
            foreach (var actionSet in actionSets)
            {
                foreach (var action in actionSet.allActions) //ボタン
                {
                    if (action is SteamVR_Action_Single) //Axis
                    {
                        var actionSingle = (SteamVR_Action_Single)action;
                        if (actionSingle.GetChanged(source))
                        {
                            var name = actionSingle.GetShortName();
                            var axis = actionSingle.GetAxis(source);
                            if (name == "Squeeze") // トリガー
                            {
                                hand.SetGrip(axis);
                                hand.SetTrigger(axis);
                                Debug.Log("pressed trigger");
                            }
                        }
                    }
                }
            }

            //hand.SetGrip(controller.selectAction.action.ReadValue<float>());
            //hand.SetTrigger(controller.activateAction.action.ReadValue<float>());
        }
    }
}
