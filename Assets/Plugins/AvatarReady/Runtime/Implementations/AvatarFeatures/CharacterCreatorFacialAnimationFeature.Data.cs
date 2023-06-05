using System;
using System.Collections.Generic;
using UnityEngine.Assertions;

namespace Inria.Avatar.AvatarReady
{
    public partial class CharacterCreatorFacialAnimationFeature
    {
        // EDITOR ONLY VARIABLES
        #region EDITOR_VARIABLES
        [NonSerialized]
        public float[] EditorEmotionsWeight;

        [NonSerialized]
        public float[] EditorVisemesWeight;

        [NonSerialized]
        public float[] EditorActionUnitsWeight;
        #endregion

        // Emotions
        public enum Emotions
        {
            None,
            Happiness,
            Sadness,
            Surprise,
            Fear,
            Anger,
            Disgust,
            Contempt,
        }

        // Visemes (LipSync)
        public enum Visemes
        {
            sil,
            PP,
            FF,
            TH,
            DD,
            kk,
            CH,
            SS,
            nn,
            RR,
            aa,
            E,
            ih,
            oh,
            ou
        }

        // https://fr.wikipedia.org/wiki/Facial_action_coding_system
        public enum ActionUnits
        {
            NeutralFace = 0,
            InnerBrowRaiser,
            OuterBrowRaiser,

            BrowLowerer = 4,
            UpperLidRaiser,
            CheekRaiser,
            LidTightener,
            LipsTowardEachOther,
            NoseWrinkler,
            UpperLipRaiser,
            NasolabialDeepener,
            LipCornerPuller,
            SharpLipPuller,
            Dimpler,
            LipCornerDepressor,
            LowerLipDepressor,
            ChinRaiser,
            LipPucker,
            TongueShow,
            LipStretcher,
            NeckTightener,
            LipFunneler,
            LipTightener,
            LipPressor,
            LipsPart,
            JawDrop,
            MouthStretch,
            LipSuck,

            LidDroop = 41,
            Slit,
            EyesClosed,
            Squint,
            Blink,
            Wink,

            LipCornerPullerRight = 121,
            DimplerRight = 141,
        }

        /*
         * Intensity of ActionUnit
         * A Trace
         * B Slight
         * C Marked or pronounced
         * D Severe or extreme
         * E Maximum
         */
        private const float FACSIntensityDefault = 1.0f;
        private const float FACSIntensityA = 0.6f;
        private const float FACSIntensityB = 0.7f;
        private const float FACSIntensityC = 0.8f;
        private const float FACSIntensityD = 0.9f;
        private const float FACSIntensityE = 1.0f;

        // Very helpful to understand the ActionUnits
        // https://imotions.com/blog/facial-action-coding-system/
        /*
         * Happiness 	6+12
         * Sadness 	    1+4+15
         * Surprise 	1+2+5B+26
         * Fear 	    1+2+4+5+7+20+26
         * Anger 	    4+5+7+23
         * Disgust 	    9+15+17
         * Contempt     R12A+R14A
         */
        /// <summary>
        /// Link between an Emotion and the corresponding ActionUnits (with associated Intensity)
        /// </summary>
        private readonly Dictionary<Emotions, List<Tuple<ActionUnits, float>>> emotionToActionUnits = new Dictionary<Emotions, List<Tuple<ActionUnits, float>>>()
        {
            { Emotions.None, null },
            { Emotions.Happiness, new List<Tuple<ActionUnits, float>>()
                {
                    new Tuple<ActionUnits, float>((ActionUnits)6, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)12, FACSIntensityDefault),
                }
            },
            { Emotions.Sadness, new List<Tuple<ActionUnits, float>>()
                {
                    new Tuple<ActionUnits, float>((ActionUnits)1, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)4, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)15, FACSIntensityDefault),
                }
            },
            { Emotions.Surprise, new List<Tuple<ActionUnits, float>>()
                {
                    new Tuple<ActionUnits, float>((ActionUnits)1, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)2, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)5, FACSIntensityB),
                    new Tuple<ActionUnits, float>((ActionUnits)26, FACSIntensityDefault),
                }
            },
            { Emotions.Fear, new List<Tuple<ActionUnits, float>>()
                {
                    new Tuple<ActionUnits, float>((ActionUnits)1, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)2, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)4, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)5, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)7, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)20, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)26, FACSIntensityDefault),
                }
            },
            { Emotions.Anger, new List<Tuple<ActionUnits, float>>()
                {
                    new Tuple<ActionUnits, float>((ActionUnits)4, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)5, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)7, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)23, FACSIntensityDefault),
                }
            },
            { Emotions.Disgust, new List<Tuple<ActionUnits, float>>()
                {
                    new Tuple<ActionUnits, float>((ActionUnits)9, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)15, FACSIntensityDefault),
                    new Tuple<ActionUnits, float>((ActionUnits)17, FACSIntensityDefault),
                }
            },
            { Emotions.Contempt, new List<Tuple<ActionUnits, float>>()
                {
                // TODO add laterality (left/right) for every ActionUnit
                    new Tuple<ActionUnits, float>((ActionUnits)121, FACSIntensityA),
                    new Tuple<ActionUnits, float>((ActionUnits)141, FACSIntensityA),
                }
            },
        };

        // Link between a ActionUnit and the corresponding Blendshapes
        private readonly Dictionary<ActionUnits, List<string>> actionUnitToBlendshapes = new Dictionary<ActionUnits, List<string>>()
        {
            { ActionUnits.NeutralFace, null },
            { ActionUnits.InnerBrowRaiser, new List<string>() { "Brow_Raise_Inner_L", "Brow_Raise_Inner_R" } },
            { ActionUnits.OuterBrowRaiser, new List<string>() { "Brow_Raise_Outer_L", "Brow_Raise_Outer_R" } },
            { ActionUnits.BrowLowerer, new List<string>() { "Brow_Drop_L", "Brow_Drop_R" } },
            { ActionUnits.UpperLidRaiser, new List<string>() { "Eye_Wide_L", "Eye_Wide_R" } },
            { ActionUnits.CheekRaiser, new List<string>() { "Cheek_Raise_L", "Cheek_Raise_R" } },
            { ActionUnits.LidTightener, new List<string>() { "Eye_Squint_L", "Eye_Squint_R" } },
            { ActionUnits.LipsTowardEachOther, new List<string>() { "Mouth_Lips_Tuck" } },
            { ActionUnits.NoseWrinkler, new List<string>() { "Nose_Scrunch" } },
            { ActionUnits.UpperLipRaiser, new List<string>() { "Mouth_Top_Lip_Up" } },
            { ActionUnits.NasolabialDeepener, new List<string>() { "Nose_Nostrils_Flare" } },
            { ActionUnits.LipCornerPuller, new List<string>() { "Mouth_Smile" } },
            { ActionUnits.SharpLipPuller, new List<string>() { "Mouth_Up" } },
            { ActionUnits.Dimpler, new List<string>() { "Mouth_Lips_Tuck" } },
            { ActionUnits.LipCornerDepressor, new List<string>() { "Mouth_Frown" } },
            { ActionUnits.LowerLipDepressor, new List<string>() { "Mouth_Bottom_Lip_Down" } },
            { ActionUnits.ChinRaiser, new List<string>() { "Mouth_Up" } },// TODO not exact
            { ActionUnits.LipPucker, new List<string>() { "Mouth_Pucker" } },
            { ActionUnits.TongueShow, null },// TODO missing
            { ActionUnits.LipStretcher, new List<string>() { "Mouth_Down" } },// TODO not exact
            { ActionUnits.NeckTightener, null },// TODO missing
            { ActionUnits.LipFunneler, new List<string>() { "Mouth_Pucker_Open" } },
            { ActionUnits.LipTightener, new List<string>() { "Mouth_Lips_Tight" } },
            { ActionUnits.LipPressor, new List<string>() { "Mouth_Lips_Tight" } },
            { ActionUnits.LipsPart, new List<string>() { "Mouth_Lips_Part" } },
            { ActionUnits.JawDrop, new List<string>() { "Mouth_Open" } },
            { ActionUnits.MouthStretch, new List<string>() { "Mouth_Lips_Open" } },// TODO not exact
            { ActionUnits.LipSuck, new List<string>() { "Mouth_Bottom_Lip_Under" } },

            // Extras
            { ActionUnits.LidDroop, new List<string>() { "Eye_Blink" } },
            { ActionUnits.Slit, new List<string>() { "Eye_Blink" } },
            { ActionUnits.EyesClosed, new List<string>() { "Eye_Blink" } },
            { ActionUnits.Squint, new List<string>() { "Eye_Squint_L", "Eye_Squint_R" } },
            { ActionUnits.Blink, new List<string>() { "Eye_Blink" } },
            { ActionUnits.Wink, new List<string>() { "Eye_Blink_R" } },

            // Specials Left/Right
            { ActionUnits.LipCornerPullerRight, new List<string>() { "Mouth_Snarl_Upper_R" } },
            { ActionUnits.DimplerRight, new List<string>() { "Mouth_Dimple_R" } },
        };

        // Link between a Viseme and the corresponding Blendshapes
        private readonly Dictionary<Visemes, List<string>> visemeToBlendshapes = new Dictionary<Visemes, List<string>>()
        {
            { Visemes.sil , null },
            { Visemes.PP , new List<string>() { "Mouth_Lips_Tight" } },
            { Visemes.FF , new List<string>() { "Mouth_Top_Lip_Under" } },
            { Visemes.TH , new List<string>() { "Mouth_Lips_Part" } },
            { Visemes.DD , new List<string>() { "Mouth_Top_Lip_Up" } },
            { Visemes.kk , null },// TODO missing
            { Visemes.CH , new List<string>() { "Tight-O" } },
            { Visemes.SS , new List<string>() { "Mouth_Bottom_Lip_Down" } },
            { Visemes.nn , new List<string>() { "Mouth_Top_Lip_Up" } },
            { Visemes.RR , new List<string>() { "Mouth_Bottom_Lip_Down" } },
            { Visemes.aa , null },// TODO missing
            { Visemes.E , new List<string>() { "Wide" } },
            { Visemes.ih , new List<string>() { "Dental_Lip" } },
            { Visemes.oh , new List<string>() { "Tight-O" } },
            { Visemes.ou , new List<string>() { "Tight-O" } },
        };

        // List of blendshape that are used for procedrual micro expressions with their maximum weight
        private readonly List<Tuple<float, List<string>>> microExpressionBlendshapes = new List<Tuple<float, List<string>>>()
        {
            {new Tuple<float, List<string>>(80, new List<string>() { "Open" } ) },
            {new Tuple<float, List<string>>(10, new List<string>() { "Brow_Raise_Inner_L","Brow_Raise_Inner_R" } ) },
            {new Tuple<float, List<string>>(10, new List<string>() { "Brow_Raise_Outer_L","Brow_Raise_Outer_R" } ) },
            {new Tuple<float, List<string>>(10, new List<string>() { "Brow_Drop_L","Brow_Drop_R" } ) },
            {new Tuple<float, List<string>>(10, new List<string>() { "Brow_Raise_L","Brow_Raise_R" } ) },
            {new Tuple<float, List<string>>(10, new List<string>() { "Eye_Blink_L","Eye_Blink_R" } ) },
            {new Tuple<float, List<string>>(20, new List<string>() { "Eye_Wide_L","Eye_Wide_R" } ) },
            {new Tuple<float, List<string>>(10, new List<string>() { "Eye_Squint_L","Eye_Squint_L" } ) },
            {new Tuple<float, List<string>>(20, new List<string>() { "Nose_Scrunch" } ) },
            {new Tuple<float, List<string>>(20, new List<string>() { "Nose_Flanks_Raise" } ) },
            {new Tuple<float, List<string>>(30, new List<string>() { "Cheek_Raise_L", "Cheek_Raise_R" } ) },
            {new Tuple<float, List<string>>(30, new List<string>() { "Cheek_Suck" } ) },
            {new Tuple<float, List<string>>(10, new List<string>() { "Cheek_Blow_L", "Cheek_Blow_R" } ) },
            {new Tuple<float, List<string>>(10, new List<string>() { "Mouth_Smile" } ) },
            {new Tuple<float, List<string>>(10, new List<string>() { "Mouth_Frown" } ) },
            {new Tuple<float, List<string>>(10, new List<string>() { "Mouth_Pucker" } ) },
            {new Tuple<float, List<string>>(30, new List<string>() { "Mouth_Widen" } ) },
            {new Tuple<float, List<string>>(10, new List<string>() { "Mouth_Plosive" } ) },
            {new Tuple<float, List<string>>(10, new List<string>() { "Mouth_Open" } ) },
            {new Tuple<float, List<string>>(20, new List<string>() { "Mouth_Bottom_Lip_Bite" } ) },
        };

        private void AwakeData()
        {
            // Consistency checks
            Assert.AreEqual(emotionToActionUnits.Count, Enum.GetValues(typeof(Emotions)).Length);
            Assert.AreEqual(actionUnitToBlendshapes.Count, Enum.GetValues(typeof(ActionUnits)).Length);
            Assert.AreEqual(visemeToBlendshapes.Count, Enum.GetValues(typeof(Visemes)).Length);
        }
    }

}