using UnityEngine;
using UnityEditor;
using System.Linq;

namespace GOAP_S.UI
{
    public class FieldSelectMenu_GS : ScriptableWizard
    {
        //Content fields
        private int _selected_field_index = -1;
        private string _selected_field_path = "";

        //Funciotnality Methods =======
        public void CreateDropdown()
        {
            DisplayWizard<FieldSelectMenu_GS>("Field Select Menu");
        }



    }
}
