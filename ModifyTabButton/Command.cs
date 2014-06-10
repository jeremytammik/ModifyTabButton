#region Namespaces
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion // Namespaces

namespace ModifyTabButton
{
  class Command
  {
    const string _prompt = 
      "Button {0} clicked in view '{1}' in document '{2}'.";

    public static void OnButton( 
      int button_nr, 
      View view )
    {
      Document doc = view.Document;

      string msg = string.Format( 
        _prompt, button_nr, view.Name, doc.Title );

      TaskDialog.Show( App.Caption, msg );
    }
  }
}
