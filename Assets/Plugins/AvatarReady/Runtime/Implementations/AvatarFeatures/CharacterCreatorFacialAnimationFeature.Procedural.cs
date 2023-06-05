using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    public partial class CharacterCreatorFacialAnimationFeature
    {
        public bool EyeBlink = true;
        public bool EyeMicroMovement = true;
        public bool BreathingMovement = true;
        public bool FacialMicroExpressions = true;
        public MicroExpressionTypes MicroExpressionType = MicroExpressionTypes.Continuous;

        public enum MicroExpressionTypes
        {
            Continuous,
            Pulse,
        }

        private Animator animator;
        private float nextBlink;
        private Transform leftEye;
        private Transform rightEye;
        private Vector3 leftInitialRotation;
        private Vector3 rightInitialRotation;
        private float nextEyeMovement;
        private float nextMicroExpressionPulse;
        private bool launchedContinuousCoroutine = false;

        private void AwakeProcedural()
        {
            animator = GetComponent<Animator>();

            // for eyes micro movements
            leftEye = animator.GetBoneTransform(HumanBodyBones.LeftEye);
            rightEye = animator.GetBoneTransform(HumanBodyBones.RightEye);
        }

        private void StartProcedural()
        {
            // eyes initial rotations
            leftInitialRotation = leftEye.localEulerAngles;
            rightInitialRotation = rightEye.localEulerAngles;
        }

        private void UpdateProcedural()
        {
            // Eye blink
            if (EyeBlink)
            {
                if (Time.time > nextBlink)
                {
                    // Blink eyes every 2-5 seconds
                    StartCoroutine(BlinkCoroutine());
                    nextBlink = Time.time + UnityEngine.Random.Range(2f, 5f);
                }
            }
            else
            {
                SetActionUnit(ActionUnits.Blink, 0);
            }

            // Eye micro movement
            if (EyeMicroMovement)
            {
                // Just apply a very small rotation on the eyes
                if (Time.time > nextEyeMovement)
                {
                    // x is up/down, z is left/right
                    float xOffset = UnityEngine.Random.Range(0f, 0.5f);
                    float zOffset = UnityEngine.Random.Range(0f, 1.5f);

                    if (UnityEngine.Random.value > 0.5f)
                        xOffset = -xOffset;

                    if (UnityEngine.Random.value > 0.5f)
                        zOffset = -zOffset;

                    leftEye.localEulerAngles = new Vector3(leftInitialRotation.x + xOffset, leftInitialRotation.y, leftInitialRotation.z + zOffset);
                    rightEye.localEulerAngles = new Vector3(rightInitialRotation.x + xOffset, rightInitialRotation.y, rightInitialRotation.z + zOffset);

                    nextEyeMovement = Time.time + UnityEngine.Random.Range(0.1f, 0.8f);
                }
            }
            else
            {
                leftEye.localEulerAngles = leftInitialRotation;
                rightEye.localEulerAngles = rightInitialRotation;
            }

            // Breathing
            // For now, only slight nostrils movement
            if (BreathingMovement)
            {
                // increase for 1.96 seconds then decrease for 1.96 seconds, from 0f to 1f and then from 1f to 0f
                float breathing = Mathf.Abs(Mathf.Sin(Time.time * 0.8f));

                float nostrilsWeight = breathing * 30f; // max blenshape weight

                SetBlendshape("Nose_Nostrils_Flare", nostrilsWeight);
            }

            // Random facial micro-expression
            if (FacialMicroExpressions)
            {
                if (MicroExpressionType == MicroExpressionTypes.Continuous)
                {
                    if (!launchedContinuousCoroutine)
                    {
                        launchedContinuousCoroutine = true;

                        foreach (var microExpression in microExpressionBlendshapes)
                        {
                            StartCoroutine(MicroExpressionContinuousCoroutine(microExpression));
                        }
                    }
                }
                else if (MicroExpressionType == MicroExpressionTypes.Pulse)
                {
                    if (Time.time > nextMicroExpressionPulse)
                    {
                        StartCoroutine(MicroExpressionPulseCoroutine());
                        nextMicroExpressionPulse = Time.time + UnityEngine.Random.Range(0.2f, 2f);
                    }
                }


            }
        }

        private IEnumerator BlinkCoroutine()
        {
            const float timeToBlink = 0.07f;
            for (float time = 0f; time <= timeToBlink; time += Time.deltaTime)
            {
                SetActionUnit(ActionUnits.Blink, Mathf.InverseLerp(0f, timeToBlink, time) * 100f);
                yield return null;
            }

            SetActionUnit(ActionUnits.Blink, 100);
            yield return null;

            for (float time = 0f; time <= timeToBlink; time += Time.deltaTime)
            {
                SetActionUnit(ActionUnits.Blink, Mathf.InverseLerp(timeToBlink, 0f, time) * 100f);
                yield return null;
            }

            SetActionUnit(ActionUnits.Blink, 0);
        }

        private IEnumerator MicroExpressionContinuousCoroutine(Tuple<float, List<string>> microExpression)
        {
            float nextMovement = 0;
            float targetWeight = 0;
            float currentWeight = 0;
            while (FacialMicroExpressions && MicroExpressionType == MicroExpressionTypes.Continuous)
            {
                if (Time.time > nextMovement)
                {
                    targetWeight = UnityEngine.Random.Range(0f, microExpression.Item1);
                    nextMovement = Time.time + UnityEngine.Random.Range(2f, 6f);
                }

                currentWeight = Mathf.MoveTowards(currentWeight, targetWeight, Time.deltaTime * 10f);

                foreach (string blendshape in microExpression.Item2)
                    SetBlendshape(blendshape, currentWeight);

                yield return null;
            }
        }

        private IEnumerator MicroExpressionPulseCoroutine()
        {
            int expr = UnityEngine.Random.Range(0, microExpressionBlendshapes.Count);

            List<string> blendshapes = microExpressionBlendshapes[expr].Item2;
            float targetWeight = UnityEngine.Random.Range(microExpressionBlendshapes[expr].Item1 / 4.0f, microExpressionBlendshapes[expr].Item1);
            float timeToMove = UnityEngine.Random.Range(0.2f, 0.6f);

            for (float time = 0f; time <= timeToMove; time += Time.deltaTime)
            {
                foreach (string blendshape in blendshapes)
                    SetBlendshape(blendshape, Mathf.InverseLerp(0f, timeToMove, time) * targetWeight);
                yield return null;
            }

            foreach (string blendshape in blendshapes)
                SetBlendshape(blendshape, targetWeight);
            yield return null;

            for (float time = 0f; time <= timeToMove; time += Time.deltaTime)
            {
                foreach (string blendshape in blendshapes)
                    SetBlendshape(blendshape, Mathf.InverseLerp(timeToMove, 0f, time) * targetWeight);
                yield return null;
            }

            foreach (string blendshape in blendshapes)
                SetBlendshape(blendshape, 0f);
        }
    }
}
