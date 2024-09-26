// /**
//  * Created by Pawel Homenko on  07/2022
//  */

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NatureManufacture.RAM.Editor
{
    public static class LayerMaskField
    {
        public static LayerMask ShowLayerMaskField(string label, LayerMask selected, bool showSpecial)
        {
            List<string> layers = new List<string>();
            List<int> layerNumbers = new List<int>();

            var selectedLayers = "";

            for (var i = 0; i < 32; i++)
            {
                var layerName = LayerMask.LayerToName(i);
                if (layerName != "")
                    if (selected == (selected | (1 << i)))
                    {
                        selectedLayers = selectedLayers == "" ? layerName : "Mixed";
                    }
            }

            //EventType lastEvent = Event.current.type;
            

            if (Event.current.type != EventType.MouseDown && Event.current.type != EventType.ExecuteCommand)
            {
                if (selected.value == 0)
                    layers.Add("Nothing");
                else if (selected.value == -1)
                    layers.Add("Everything");
                else
                    layers.Add(selectedLayers);

                layerNumbers.Add(-1);
            }

            layers.Add("   ");
            layerNumbers.Add(-4);
            if (showSpecial)
            {
                layers.Add((selected.value == 0 ? "[X] " : "      ") + "Nothing");
                layerNumbers.Add(-2);

                layers.Add((selected.value == -1 ? "[X] " : "      ") + "Everything");
                layerNumbers.Add(-3);
            }

            for (var i = 0; i < 32; i++)
            {
                var layerName = LayerMask.LayerToName(i);

                if (layerName != "")
                {
                    if (selected == (selected | (1 << i)))
                        layers.Add("[X] " + i + ": " + layerName);
                    else
                        layers.Add("     " + i + ": " + layerName);

                    layerNumbers.Add(i);
                }
            }

            var preChange = GUI.changed;

            GUI.changed = false;

            var newSelected = 0;

            if (Event.current.type == EventType.MouseDown) newSelected = -1;

            newSelected = EditorGUILayout.Popup(label, newSelected, layers.ToArray(), EditorStyles.layerMaskField);

            if (GUI.changed && newSelected >= 0)
            {
                if (showSpecial && newSelected == 1)
                {
                    selected = 0;
                }
                else if (showSpecial && newSelected == 2)
                {
                    selected = -1;
                }
                else
                {
                    if (selected == (selected | (1 << layerNumbers[newSelected])))
                        selected &= ~(1 << layerNumbers[newSelected]);
                    else
                        selected = selected | (1 << layerNumbers[newSelected]);
                }
            }
            else
            {
                GUI.changed = preChange;
            }

            return selected;
        }
    }
}