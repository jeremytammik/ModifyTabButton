#region Namespaces
// BitmapImage requires PresentationCore
// System.Windows.Controls requires PresentationFramework

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;

#endregion // Namespaces

namespace ModifyTabButton
{
  class App : IExternalApplication
  {
    public static string Caption = "ModifyTabButton";

    UIControlledApplication _controlledApp;
    View _currentView;

    /// <summary>
    /// The button that was clicked, 1, 2 or 3.
    /// </summary>
    int _button_nr;

    Autodesk.Windows.RibbonPanel _modifyPanel;
    Autodesk.Windows.RibbonButton _button1;
    Autodesk.Windows.RibbonButton _button2;
    Autodesk.Windows.RibbonButton _button3;

    /// <summary>
    /// Set the visibility of our custom panel
    /// and its buttons.
    /// </summary>
    void SetCustomPanelVisible( bool a )
    {
      _modifyPanel.IsVisible = a;
      _button1.IsVisible = a;
      _button2.IsVisible = a;
      _button3.IsVisible = a;
    }

    /// <summary>
    /// Retrieve an embedded resource image
    /// for the button icons.
    /// </summary>
    static BitmapSource GetEmbeddedImage( string name )
    {
      try
      {
        Assembly a = Assembly.GetExecutingAssembly();
        Stream s = a.GetManifestResourceStream( name );
        return BitmapFrame.Create( s );
      }
      catch
      {
        return null;
      }
    }

    /// <summary>
    /// Create a basic ribbon button with an 
    /// identifying number and an image.
    /// </summary>
    static Autodesk.Windows.RibbonButton CreateButton(
      int nr,
      BitmapSource image )
    {
      string s = nr.ToString();

      Autodesk.Windows.RibbonButton b
        = new Autodesk.Windows.RibbonButton();

      b.Name = "_Button" + s;
      b.Id = "ID_MYBUTTON" + s;
      b.AllowInStatusBar = true;
      b.AllowInToolBar = true;
      b.GroupLocation = Autodesk.Private.Windows
        .RibbonItemGroupLocation.Middle;
      b.IsEnabled = true;
      b.IsToolTipEnabled = true;
      b.IsVisible = false;
      b.LargeImage = image;
      b.Image = image;
      b.ShowImage = true; //  true;
      b.ShowText = true;
      b.ShowToolTipOnDisabled = true;
      b.Text = "Button " + s;
      b.ToolTip = "Button " + s;
      b.MinHeight = 0;
      b.MinWidth = 0;
      b.Size = Autodesk.Windows.RibbonItemSize.Standard;
      b.ResizeStyle = Autodesk.Windows
        .RibbonItemResizeStyles.HideText;
      b.IsCheckable = true;
      b.Orientation = System.Windows.Controls
        .Orientation.Horizontal; // PresentationFramework
      b.KeyTip = "Button" + s;

      return b;
    }

    public Result OnStartup(
      UIControlledApplication a )
    {
      _controlledApp = a;

      // Locate and load button images.

      //string AddInPath = typeof( App ).Assembly.Location;
      //string iconPath = Path.GetDirectoryName( AddInPath ).ToLower();
      //string lastDir = new DirectoryInfo( iconPath ).Name;
      //iconPath = iconPath.Replace( "\\" + lastDir, "\\icons\\" );

      //Uri uriImage = new Uri( iconPath + "image.ico" );
      //BitmapImage image = new BitmapImage( uriImage ); // PresentationCore

      BitmapSource image = GetEmbeddedImage(
        "ModifyTabButton.icon.cartoon_house_16.ico" );

      // Add modify panel

      Autodesk.Windows.RibbonControl ribbon
        = Autodesk.Windows.ComponentManager.Ribbon;

      foreach( Autodesk.Windows.RibbonTab tab
        in ribbon.Tabs )
      {
        if( tab.Id == "Modify" )
        {
          _modifyPanel = new Autodesk.Windows.RibbonPanel();
          _modifyPanel.IsVisible = false;

          Autodesk.Windows.RibbonPanelSource source
            = new Autodesk.Windows.RibbonPanelSource();

          source.Name = "mymod";
          source.Id = "mymod";
          source.Title = "My Modify";
          _modifyPanel.Source = source;
          _modifyPanel.FloatingOrientation
            = System.Windows.Controls.Orientation.Vertical;

          _button1 = CreateButton( 1, image );
          _button2 = CreateButton( 2, image );
          _button3 = CreateButton( 3, image );

          Autodesk.Windows.ComponentManager.UIElementActivated
            += new EventHandler<
              Autodesk.Windows.UIElementActivatedEventArgs>(
                OnUiElementActivated );

          Autodesk.Windows.RibbonRowPanel rowPanel
            = new Autodesk.Windows.RibbonRowPanel();

          rowPanel.Items.Add( _button1 );
          rowPanel.Items.Add( new Autodesk.Windows.RibbonRowBreak() );
          rowPanel.Items.Add( _button2 );
          rowPanel.Items.Add( new Autodesk.Windows.RibbonRowBreak() );
          rowPanel.Items.Add( _button3 );

          _modifyPanel.Source.Items.Add( rowPanel );

          tab.Panels.Add( _modifyPanel );

          tab.Panels.CollectionChanged
            += new System.Collections.Specialized
              .NotifyCollectionChangedEventHandler(
                OnCollectionChanged );

          a.ViewActivated
            += new EventHandler<ViewActivatedEventArgs>(
              OnViewActivated );

          break;
        }
      }
      return Result.Succeeded;
    }

    public Result OnShutdown(
      UIControlledApplication a )
    {
      Autodesk.Windows.ComponentManager
        .UIElementActivated -= OnUiElementActivated;

      a.ViewActivated -= OnViewActivated;

      return Result.Succeeded;
    }

    /// <summary>
    /// React to Revit view activation.
    /// </summary>
    private void OnViewActivated(
      object sender,
      ViewActivatedEventArgs e )
    {
      _currentView = e.CurrentActiveView;
    }

    /// <summary>
    /// React to ribbon panel changes, triggered by 
    /// Revit element selection. We have no external
    /// command, hence no availability class, so we 
    /// use this to hide and show our custom panel.
    /// We have absolutely no access to the 
    /// Revit API in this method!
    /// </summary>
    private void OnCollectionChanged(
      object sender,
      System.Collections.Specialized
        .NotifyCollectionChangedEventArgs e )
    {
      if( e.NewItems != null )
      {
        bool visible = ( null != _currentView )
          && ( _currentView is View3D );

        SetCustomPanelVisible( visible );
      }
      if( e.OldItems != null )
      {
        SetCustomPanelVisible( false );
      }
    }

    /// <summary>
    /// React to UI element activation, 
    /// e.g. button click. We have absolutely 
    /// no access to the Revit PI in this method!
    /// </summary>
    void OnUiElementActivated(
      object sender,
      Autodesk.Windows.UIElementActivatedEventArgs e )
    {
      if( e.Item != null )
      {
        // We could just remember the button id here
        // and use one single Idling event handler
        // for both.

        if( e.Item.Id == "ID_MYBUTTON1"
          || e.Item.Id == "ID_MYBUTTON2"
          || e.Item.Id == "ID_MYBUTTON3" )
        {
          _button_nr = int.Parse(
            e.Item.Id.Substring( 11 ) );

          _controlledApp.Idling
            += new EventHandler<IdlingEventArgs>(
              OnButtonIdling );
        }
      }
    }

    /// <summary>
    /// Idling event handler for all buttons.
    /// This is where we have access to the Revit
    /// API again.
    /// </summary>
    void OnButtonIdling(
      object sender,
      IdlingEventArgs e )
    {
      UIApplication uiapp = sender as UIApplication;

      if( uiapp != null )
      {
        UIDocument uidoc = uiapp.ActiveUIDocument;
        Command.OnButton( _button_nr, uidoc.ActiveView );
      }

      _controlledApp.Idling -= OnButtonIdling;
    }
  }
}
