using System;
using System.Collections.Generic;

namespace Inria.Avatar.AvatarReady
{
    public partial class CharacterCreatorFacialAnimationFeature
    {
        public void SetActionUnit(ActionUnits actionUnit, float weight)
        {
            List<string> blendshapes = actionUnitToBlendshapes[actionUnit];

            if (blendshapes != null)
            {
                foreach (string blendshape in blendshapes)
                {
                    SetBlendshape(blendshape, weight);
                }
            }
        }

        public void SetEmotion(Emotions emotion, float weight)
        {
            List<Tuple<ActionUnits, float>> actionUnits = emotionToActionUnits[emotion];

            if (actionUnits != null)
            {
                foreach (Tuple<ActionUnits, float> actionUnit in actionUnits)
                {
                    SetActionUnit(actionUnit.Item1, actionUnit.Item2 * weight);
                }
            }
        }

        public void SetViseme(Visemes viseme, float weight)
        {
            List<string> blendshapes = visemeToBlendshapes[viseme];

            if (blendshapes != null)
            {
                foreach (string blendshape in blendshapes)
                {
                    SetBlendshape(blendshape, weight);
                }
            }
        }

        public void SetViseme(float[] visemesWeights)
        {
            if (visemesWeights.Length != Enum.GetNames(typeof(Visemes)).Length)
                throw new InvalidOperationException();

            for (int visemeIndex = 0; visemeIndex < visemesWeights.Length; visemeIndex++)
            {
                Visemes viseme = (Visemes)visemeIndex;

                SetViseme(viseme, visemesWeights[visemeIndex] * 100f);
            }
        }
    }
}
