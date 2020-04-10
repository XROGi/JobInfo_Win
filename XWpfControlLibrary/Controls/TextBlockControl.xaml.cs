using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfEmoticons.Controls
{
  /// <summary>
  /// Interaction logic for TextBlockControl.xaml
  /// </summary>
  public partial class TextBlockControl : UserControl
  {
    bool m_IgnoreChanges = false;

    public TextBlockControl()
    {
      InitializeComponent();
    }

    public string TextWrapping {
      get { return (string)GetValue(TextWrappingProperty); }
      set { SetValue(TextWrappingProperty, value); }
    }

    public static readonly DependencyProperty TextWrappingProperty =
      DependencyProperty.Register("TextWrapping", typeof(System.Windows.TextWrapping), typeof(TextBlockControl),
        new UIPropertyMetadata(System.Windows.TextWrapping.NoWrap, OnTextWrappingChanged));

    static void OnTextWrappingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TextBlockControl This = (TextBlockControl)d;
      This.txtText.TextWrapping = (System.Windows.TextWrapping)e.NewValue;
    }

    public string TextTrimming {
      get { return (string)GetValue(TextTrimmingProperty); }
      set { SetValue(TextTrimmingProperty, value); }
    }

    public static readonly DependencyProperty TextTrimmingProperty =
      DependencyProperty.Register("TextTrimming", typeof(System.Windows.TextTrimming), typeof(TextBlockControl),
        new UIPropertyMetadata(System.Windows.TextTrimming.None, OnTextTrimmingChanged));

    static void OnTextTrimmingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TextBlockControl This = (TextBlockControl)d;
      This.txtText.TextTrimming = (System.Windows.TextTrimming)e.NewValue;
    }

    public string Text {
      get { return (string)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty TextProperty =
      DependencyProperty.Register("Text", typeof(string), typeof(TextBlockControl),
        new UIPropertyMetadata(string.Empty, OnTextChanged));

    static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      TextBlockControl This = (TextBlockControl)d;
      if (!This.m_IgnoreChanges){
        This.m_IgnoreChanges = true;
        This.txtText.Text = (string)e.NewValue;
        EmoticonsHelper.ParseText(This.txtText);
        This.m_IgnoreChanges = false;
      }
    }
  }
}
