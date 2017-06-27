ModifyTabButton
===============

Revit add-in demonstrating how to add a custom button to the Modify tab via the .NET UI Automation library and trigger Revit API functionality from it via the Idling event.

## Flaw

As pointed out by Anthony Tiefenbach [@ttiefenbach](https://github.com/ttiefenbach) 
in [issue #1](https://github.com/jeremytammik/ModifyTabButton/issues/1), the fundamental architecture of this add-in is probably flawed:

[Q] When I try to use your sample code, I get this error in the journal file: "Invalid call to Revit API! Revit is currently not within an API context."  It occurs in the `OnUiElementActivated` function at the line:
```
_controlledApp.Idling += new EventHandler<IdlingEventArgs>( OnButtonIdling );
```
I'm testing in Revit 2016.

[A] The error message is probably absolutely correct, and the architecture of this app flawed. It could probably be solved as follows: on startup, implement and connect an external event. in `OnUiElementActivated `, raise the external event. Revit became stricter on enforcing the valid API context as time went by, and that is good, and uncovered this flaw. Capisci? Ciao!
