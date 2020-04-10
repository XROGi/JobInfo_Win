using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows;

namespace WpfEmoticons.Controls
{
  public class EmoticonsHelper
  {
        public static int SizeImotic = 24;
  public  static Dictionary<string, string> m_Emoticons = null;
    //static Dictionary<string, string> m_EmoticonsReverse = null;

    private static void InitEmoticons()
    {
      if (m_Emoticons != null)
        return;

      m_Emoticons = new Dictionary<string, string>();
        /*    //     m_Emoticons["^^"] = "images/emoticons/01.png";
            m_Emoticons["<smile id=\"01\">^_^</smile>"]   = "images/emoticons/01.png";
            m_Emoticons["<smile id=\"02\">:D</smile>"]    = "images/emoticons/02.png";
            m_Emoticons["<smile id=\"02\">:-D</smile>"]   = "images/emoticons/02.png";
            m_Emoticons["<smile id=\"03\">:D</smile>"]    = "images/emoticons/03.png";
            m_Emoticons["<smile id=\"03\">:-D</smile>"]   = "images/emoticons/03.png";
            m_Emoticons["<smile id=\"04\">;)</smile>"]    = "images/emoticons/04.png";
            m_Emoticons["<smile id=\"04\">;-)</smile>"]   = "images/emoticons/04.png";
            m_Emoticons["<smile id=\"05\">:)</smile>"]    = "images/emoticons/05.png";
            m_Emoticons["<smile id=\"05\">:-)</smile>"]   = "images/emoticons/05.png";
            m_Emoticons["<smile id=\"06\">:D</smile>"]    = "images/emoticons/06.png";
            m_Emoticons["<smile id=\"06\">:-D</smile>"]   = "images/emoticons/06.png";

            m_Emoticons["<smile id=\"07\">8)</smile>"]    = "images/emoticons/07.png";
            m_Emoticons["<smile id=\"07\">8-)</smile>"]   = "images/emoticons/07.png";
            m_Emoticons["<smile id=\"08\">:p</smile>"]    = "images/emoticons/08.png";
            m_Emoticons["<smile id=\"08\">:-p</smile>"]   = "images/emoticons/08.png";
            m_Emoticons["<smile id=\"09\">:D</smile>"]    = "images/emoticons/09.png";
            m_Emoticons["<smile id=\"09\">:-D</smile>"]   = "images/emoticons/09.png";
            m_Emoticons["<smile id=\"10\">:o</smile>"]    = "images/emoticons/10.png";
            m_Emoticons["<smile id=\"10\">:-o</smile>"]   = "images/emoticons/10.png";
            m_Emoticons["<smile id=\"11\">:D</smile>"]    = "images/emoticons/11.png";
            m_Emoticons["<smile id=\"11\">:-D</smile>"]   = "images/emoticons/11.png";
            m_Emoticons["<smile id=\"12\">:(</smile>"]    = "images/emoticons/12.png";
            m_Emoticons["<smile id=\"12\">:-(</smile>"]   = "images/emoticons/12.png";
            m_Emoticons["<smile id=\"13\">:'(</smile>"]   = "images/emoticons/13.png";
            m_Emoticons["<smile id=\"13\">:'-(</smile>"]  = "images/emoticons/13.png";
            m_Emoticons["<smile id=\"14\">:@</smile>"]    = "images/emoticons/14.png";
            m_Emoticons["<smile id=\"14\">:-@</smile>"]   = "images/emoticons/14.png";
            m_Emoticons["<smile id=\"14\">>:@</smile>"]   = "images/emoticons/14.png";
            m_Emoticons["<smile id=\"14\">>:-@</smile>"]  = "images/emoticons/14.png";
            m_Emoticons["<smile id=\"14\">>_<</smile>"]   = "images/emoticons/14.png";
                              
            */

            
       m_Emoticons["^_^"] = "images/emoticons/01.png";
  //     m_Emoticons["^^"] = "images/emoticons/01.png";
       m_Emoticons[":D"] = "images/emoticons/02.png";
      m_Emoticons[":-D"] = "images/emoticons/02.png";
            m_Emoticons[":#D"] = "images/emoticons/03.png";
            m_Emoticons[":#-D"] = "images/emoticons/03.png";
      m_Emoticons[";)"] = "images/emoticons/04.png";
      m_Emoticons[";-)"] = "images/emoticons/04.png";
      m_Emoticons[":)"] = "images/emoticons/05.png";
      m_Emoticons[":-)"] = "images/emoticons/05.png";
            m_Emoticons["8|"] = "images/emoticons/06.png";
            m_Emoticons["*-|"] = "images/emoticons/06.png";

      m_Emoticons["8)"] = "images/emoticons/07.png";
      m_Emoticons["8-)"] = "images/emoticons/07.png";
      m_Emoticons[":p"] = "images/emoticons/08.png";
      m_Emoticons[":-p"] = "images/emoticons/08.png";
            m_Emoticons[":%"] = "images/emoticons/09.png";
            m_Emoticons[":-%"] = "images/emoticons/09.png";
      m_Emoticons[":o"] = "images/emoticons/10.png";
      m_Emoticons[":-o"] = "images/emoticons/10.png";
            m_Emoticons[":-["] = "images/emoticons/11.png";
            m_Emoticons[":["] = "images/emoticons/11.png";
      m_Emoticons[":("] = "images/emoticons/12.png";
      m_Emoticons[":-("] = "images/emoticons/12.png";
      m_Emoticons[":'("] = "images/emoticons/13.png";
      m_Emoticons[":'-("] = "images/emoticons/13.png";
      m_Emoticons[":@"] = "images/emoticons/14.png";
      m_Emoticons[":-@"] = "images/emoticons/14.png";
      m_Emoticons[">:@"] = "images/emoticons/14.png";
      m_Emoticons[">:-@"] = "images/emoticons/14.png";
 //     m_Emoticons[">_<"] = "images/emoticons/14.png";

         /*   */
//            m_EmoticonsReverse = new Dictionary<string, string>();
//      foreach (string k in m_Emoticons.Keys)
//        m_EmoticonsReverse[m_Emoticons[k]] = k;
    } // InitEmoticons


   public static string ReplaceEmoticToHTMLLink(String inStr)
   {
            string ret = inStr;
            foreach ( var t in m_Emoticons)
            {  
                ret = ret.Replace(t.Key, "<img src=\"" + Environment.CurrentDirectory+"\\"+t.Value.Replace("/","\\")+ "\"/>");
            }
            return ret; 
   }

     public static string GetPlainText(FlowDocument doc)
    {
      InitEmoticons();
      StringBuilder result = new StringBuilder();

      foreach (Block b in doc.Blocks) {
                try
                {
                    Paragraph p = b as Paragraph;
                    if (p != null)
                    {
                        foreach (Inline i in p.Inlines)
                        {
                            if (i is Run)
                            {
                                Run r = i as Run;
                                result.Append(r.Text);
                            }
                            else if (i is InlineUIContainer)
                            {
                                InlineUIContainer ic = i as InlineUIContainer;
                                if (ic.Child is Image)
                                {
                                    BitmapImage img = (ic.Child as Image).Source as BitmapImage;
                                    if (img == null)
                                    {
                                        // Неизвестная картинка
                                    }
                                    else
                                    {
                                        foreach( var r in m_Emoticons)
                                        {
                                            if (r.Value == img.UriSource.ToString())
                                            {
                                                result.Append(r.Key);
                                                break;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                    result.AppendLine();
                }catch (Exception err)
                {

                }
      }
      return result.ToString();
    } // GetPlainText

    private static int FindFirstEmoticon(string text, int startIndex, out string emoticonFound)
    {
      InitEmoticons();
      emoticonFound = string.Empty;
      int minIndex = -1;
      foreach (string e in m_Emoticons.Keys) {
        int index = text.IndexOf(e, startIndex);
        if (index >= 0) {
          if (minIndex < 0 || index < minIndex) {
            minIndex = index;
            emoticonFound = e;
          }
        }
      }
      return minIndex;
    } // FindFirstEmoticon

    public static void ParseText(FrameworkElement element)
    {
      InitEmoticons();
      TextBlock textBlock = null;
      RichTextBox textBox = element as RichTextBox;
      if (textBox == null)
        textBlock = element as TextBlock;
      
      if (textBox == null && textBlock == null)
        return;

      if (textBox != null){
        FlowDocument doc = textBox.Document;
        for (int blockIndex=0; blockIndex < doc.Blocks.Count; blockIndex++){
          Block b = doc.Blocks.ElementAt(blockIndex);
          Paragraph p = b as Paragraph;
          if (p != null) {          
            ProcessInlines(textBox, p.Inlines);
          }
        }
      }else{
        ProcessInlines(null, textBlock.Inlines);
      }
    } // ParseText

    private static void ProcessInlines(RichTextBox textBox, InlineCollection inlines)
    {
      for (int inlineIndex=0; inlineIndex < inlines.Count; inlineIndex++){
        Inline i = inlines.ElementAt(inlineIndex);
        if (i is Run) {
          Run r = i as Run;
          string text = r.Text;
          string emoticonFound = string.Empty;
          int index = FindFirstEmoticon(text, 0, out emoticonFound);
          if (index >= 0) {
            TextPointer tp = i.ContentStart;
            bool reposition = false;                
            while (!tp.GetTextInRun(LogicalDirection.Forward).StartsWith(emoticonFound)) 
              tp = tp.GetNextInsertionPosition(LogicalDirection.Forward);
            TextPointer end = tp;
            for (int j=0; j<emoticonFound.Length; j++)
              end = end.GetNextInsertionPosition(LogicalDirection.Forward);
            TextRange tr = new TextRange(tp, end);
            if (textBox != null)
              reposition = textBox.CaretPosition.CompareTo(tr.End) == 0;
            tr.Text = string.Empty;

            string imageFile = m_Emoticons[emoticonFound];

                  //         InsertImage(imageFile, tr);
               //      InsertImage(imageFile, tp);

                  
                        Image image = new Image();
                        BitmapImage bimg = new BitmapImage();
                        bimg.BeginInit();
                        bimg.UriSource = new Uri(imageFile, UriKind.Relative);
                        bimg.EndInit();
                        image.Source = bimg;
                        image.Width = 24;

                        InlineUIContainer iui = new InlineUIContainer(image, tp);
                        iui.BaselineAlignment = BaselineAlignment.TextBottom;


                        if (textBox != null && reposition)
              textBox.CaretPosition = tp.GetNextInsertionPosition(LogicalDirection.Forward);
          }
        }
      }
    } // ProcessInlines

        private static void InsertImage(string imageFile, TextPointer tp)
        {
            Image image = new Image();
            BitmapImage bimg = new BitmapImage();
            bimg.BeginInit();
            bimg.UriSource = new Uri(imageFile, UriKind.Relative);
            bimg.EndInit();
            image.Source = bimg;
            image.Width = SizeImotic;

            InlineUIContainer iui = new InlineUIContainer(image, tp);
            iui.BaselineAlignment = BaselineAlignment.TextBottom;
        }
    }
}
