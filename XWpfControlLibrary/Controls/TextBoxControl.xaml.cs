using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfEmoticons.Controls
{
  /// <summary>
  /// Logica di interazione per TextBoxControl.xaml
  /// </summary>
  public partial class TextBoxControl : UserControl
  {
    bool m_IgnoreChanges = false;

    public TextBoxControl()
    {
      InitializeComponent();
      txtTextbox.Document = new FlowDocument();
      txtTextbox.AcceptsReturn = false;
    }

    public bool AcceptsReturn {
      get { return (bool)GetValue(AcceptsReturnProperty); }
      set { SetValue(AcceptsReturnProperty, value); }
    }

    public static readonly DependencyProperty AcceptsReturnProperty =
      DependencyProperty.Register("AcceptsReturn", typeof(bool), typeof(TextBoxControl),
        new UIPropertyMetadata(false, OnAcceptsReturnChanged));

    static void OnAcceptsReturnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TextBoxControl This = (TextBoxControl)d;
      This.txtTextbox.AcceptsReturn = (bool)e.NewValue;
    }

    private void txtTextbox_TextChanged(object sender, TextChangedEventArgs e)
    {
      if (!m_IgnoreChanges){
        m_IgnoreChanges = true;
        EmoticonsHelper.ParseText(txtTextbox);
        m_IgnoreChanges = false;
      }
    } // txtTextbox_TextChanged

    private void UserControl_GotFocus(object sender, RoutedEventArgs e)
    {
      txtTextbox.Focus();
    } // UserControl_GotFocus

    public string GetPlainText()
    {
      return EmoticonsHelper.GetPlainText(txtTextbox.Document);
    } // GetPlainText

        public void Clear()
    {
      txtTextbox.Document = new FlowDocument();
    } // Clear
  }
}
