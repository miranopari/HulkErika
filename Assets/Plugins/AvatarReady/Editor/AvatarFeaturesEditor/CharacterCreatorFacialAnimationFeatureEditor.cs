using System;
using UnityEditor;
using UnityEngine;

namespace Inria.Avatar.AvatarReady
{
    [CustomEditor(typeof(CharacterCreatorFacialAnimationFeature))]
    public class CharacterCreatorFacialAnimationFeatureEditor : Editor
    {
        private CharacterCreatorFacialAnimationFeature _target;
        private CharacterCreatorFacialAnimationFeature Target
        {
            get
            {
                if (_target == null)
                    _target = target as CharacterCreatorFacialAnimationFeature;
                return _target;
            }
        }

        private static bool showEmotions;
        private static bool showVisemes;
        private static bool showActionUnits;

        private void OnEnable()
        {
            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        private void Update()
        {
            if (!Application.isPlaying)
                return;

            if (target == null)
                return;

            if (Target.EditorEmotionsWeight == null)
                Target.EditorEmotionsWeight = new float[Enum.GetValues(typeof(CharacterCreatorFacialAnimationFeature.Emotions)).Length];

            if (Target.EditorVisemesWeight == null)
                Target.EditorVisemesWeight = new float[Enum.GetValues(typeof(CharacterCreatorFacialAnimationFeature.Visemes)).Length];

            if (Target.EditorActionUnitsWeight == null)
                Target.EditorActionUnitsWeight = new float[Enum.GetValues(typeof(CharacterCreatorFacialAnimationFeature.ActionUnits)).Length];

            int i = 0;
            foreach (object emotion in Enum.GetValues(typeof(CharacterCreatorFacialAnimationFeature.Emotions)))
            {
                Target.SetEmotion((CharacterCreatorFacialAnimationFeature.Emotions)emotion, Target.EditorEmotionsWeight[i]);
                i++;
            }

            i = 0;
            foreach (object viseme in Enum.GetValues(typeof(CharacterCreatorFacialAnimationFeature.Visemes)))
            {
                Target.SetViseme((CharacterCreatorFacialAnimationFeature.Visemes)viseme, Target.EditorVisemesWeight[i]);
                i++;
            }

            i = 0;
            foreach (object actionUnit in Enum.GetValues(typeof(CharacterCreatorFacialAnimationFeature.ActionUnits)))
            {
                Target.SetActionUnit((CharacterCreatorFacialAnimationFeature.ActionUnits)actionUnit, Target.EditorActionUnitsWeight[i]);
                i++;
            }
        }

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            EditorGUI.BeginDisabledGroup(!Application.isPlaying);


            EditorGUILayout.Separator();

            // Emotions
            showEmotions = EditorGUILayout.Foldout(showEmotions, "Emotions");
            if (showEmotions)
            {
                if (Target.EditorEmotionsWeight == null)
                    Target.EditorEmotionsWeight = new float[Enum.GetValues(typeof(CharacterCreatorFacialAnimationFeature.Emotions)).Length];

                EditorGUI.indentLevel++;

                int i = 0;
                foreach (object emotion in Enum.GetValues(typeof(CharacterCreatorFacialAnimationFeature.Emotions)))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(emotion.ToString());
                    Target.EditorEmotionsWeight[i] = EditorGUILayout.Slider(Target.EditorEmotionsWeight[i], 0f, 100f);
                    EditorGUILayout.EndHorizontal();
                    i++;
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Separator();

            // Visemes
            showVisemes = EditorGUILayout.Foldout(showVisemes, "Visemes");
            if (showVisemes)
            {
                if (Target.EditorVisemesWeight == null)
                    Target.EditorVisemesWeight = new float[Enum.GetValues(typeof(CharacterCreatorFacialAnimationFeature.Visemes)).Length];

                EditorGUI.indentLevel++;

                int i = 0;
                foreach (var viseme in Enum.GetValues(typeof(CharacterCreatorFacialAnimationFeature.Visemes)))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(viseme.ToString());
                    Target.EditorVisemesWeight[i] = EditorGUILayout.Slider(Target.EditorVisemesWeight[i], 0f, 100f);
                    EditorGUILayout.EndHorizontal();
                    i++;
                }
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Separator();

            // ActionUnits
            showActionUnits = EditorGUILayout.Foldout(showActionUnits, "ActionUnits");
            if (showActionUnits)
            {
                if (Target.EditorActionUnitsWeight == null)
                    Target.EditorActionUnitsWeight = new float[Enum.GetValues(typeof(CharacterCreatorFacialAnimationFeature.ActionUnits)).Length];

                EditorGUI.indentLevel++;

                int i = 0;
                foreach (var actionUnit in Enum.GetValues(typeof(CharacterCreatorFacialAnimationFeature.ActionUnits)))
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(actionUnit.ToString());
                    Target.EditorActionUnitsWeight[i] = EditorGUILayout.Slider(Target.EditorActionUnitsWeight[i], 0f, 100f);
                    EditorGUILayout.EndHorizontal();
                    i++;
                }
                EditorGUI.indentLevel--;
            }


            EditorGUI.EndDisabledGroup();
        }
    }
}
