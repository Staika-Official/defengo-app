using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;
using Framework.UI;

//[UnityEditor.CustomEditor(typeof(UISequences))]
public class UIsequenceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //???????? ????????
        UISequences uISequences = (UISequences)target;

        //UISequences ???????? ????????
        base.OnInspectorGUI();

        switch (uISequences.sequenceType)
        {
            case SequenceType.OPEN_POPUP:
                {
                    PropertyAdd("openPopup.scaleCurve");
                    PropertyAdd("openPopup.startScale");
                    PropertyAdd("openPopup.endScale");
                    PropertyAdd("openPopup.duration");
                }
                break;
            case SequenceType.TOP_HEADER:
                {
                    PropertyAdd("openTopHeader.movePos");
                    PropertyAdd("openTopHeader.duration");
                    PropertyAdd("openTopHeader.ease");
                }
                break;
        }

        //???? ???????? ????
        if (GUILayout.Button("Replay"))
        {
            switch (uISequences.sequenceType)
            {
                case SequenceType.OPEN_POPUP:
                    uISequences.gameObject.transform.localScale = Vector3.one;
                    uISequences.Open_Popup().Restart();
                    break;
                case SequenceType.TOP_HEADER:
                    uISequences.Open_TopHeader().Restart();
                    break;
            }

        }

        if (GUILayout.Button("Reset Transform"))
        {
            Debug.Log("???? ??????");
        }


        void PropertyAdd(string data)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty(data));
        }

        serializedObject.ApplyModifiedProperties();

    }

}