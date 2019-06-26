using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JobInfo
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }




        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            foreach (ListViewItem r in listView_chats.SelectedItems)
            {
                if (listView_chats.SelectedItems != null)
                    switch (sender)
                    {
                        case ListView i:
                            if (r.Tag != null)
                            {
                                long id = (r.Tag as Chat).ObjId.ObjId;
                                bool b_changed = job.Chat_Selected(id);
                                if (b_changed)
                                {
                                    DeleteSubscribe();
                                }
                                tbl_ChatUserInfo cui = job.Chat_GetMyStatistic(id);
                                Chat_Select(id);

                                WS_JobInfo.Obj[] r1 = job.GetMessages(id, cui); //msg_inChat =



                                ShowJob(job, 0, listBox_msg, r1);

                            }
                            else
                            {
                                //   panel4.Controls.Clear();
                            }
                            break
                                ;
                        default:
                            break
                                ;
                    }
            }*/

        }

      /*
        private void listView1_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            //https://stackoverflow.com/questions/20012097/overriding-the-drawitem-event-on-a-winform-listview-control
            ListView listView = (ListView)sender;
            Chat chat = e.Item.Tag as Chat;
            if (chat == null)
            {
                e.DrawDefault = true;
            }
            else
            {
                // Check if e.Item is selected and the ListView has a focus.
                /*    if (!listView.Focused && e.Item.Selected)
                    {
                        Rectangle rowBounds = e.Bounds;
                        int leftMargin = e.Item.GetBounds(ItemBoundsPortion.Label).Left;
                        Rectangle bounds = new Rectangle(leftMargin, rowBounds.Top,(int) e.Graphics.VisibleClipBounds.Width - leftMargin, rowBounds.Height);
                        e.Graphics.FillRectangle(SystemBrushes.Highlight, bounds);

                        if (chat != null)
                        {
                            TextRenderer.DrawText(e.Graphics, chat.statistic.CountNew.ToString(), listView.Font, bounds, SystemColors.HighlightText,
                            TextFormatFlags.SingleLine | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
                        }
                        
            }
                else
                {
                    Rectangle rowBounds = e.Bounds;


                    int leftMargin = e.Item.GetBounds(ItemBoundsPortion.Label).Left;
                    Rectangle bounds = new Rectangle(leftMargin, rowBounds.Top, (int)e.Graphics.VisibleClipBounds.Width - leftMargin, rowBounds.Height);
                    //     e.Graphics.FillRectangle(Brushes.AliceBlue, bounds);
                    //     e.DrawFocusRectangle();
                    //     e.DrawBackground();

                    string text = chat.ObjId.GetText();

                    int right = (int)e.Graphics.VisibleClipBounds.Width;
                    int pixelToCounter = 25;

                    Font fCounter = listView.Font;
                    Size fsize = TextRenderer.MeasureText(text, fCounter);
                    if (fsize.Width > right - pixelToCounter - leftMargin)
                    {
                        if (text.Length >= 25)
                            text = text.Substring(0, 25) + "...";
                    }
                  

                    System.Drawing.Color c = System.Drawing.Color.Black;
                    Font f = listView.Font;
                    if (!listView.Focused && e.Item.Selected)
                    {
                        c = System.Drawing.Color.Blue;
                    }

                    if (e.Item.Selected)
                    {
                        c = System.Drawing.Color.Blue;
                        //     f = new Font(listView.Font, FontStyle.Bold); 
                    }

                    TextRenderer.DrawText(e.Graphics, text, f, bounds, c, TextFormatFlags.SingleLine | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);

                    Rectangle boundsCounter = new Rectangle(right - pixelToCounter, rowBounds.Top, right - leftMargin - pixelToCounter, rowBounds.Height);
                    if (chat.statistic != null)
                    {
                        TextRenderer.DrawText(e.Graphics, chat.statistic.CountNew.ToString(), listView.Font, boundsCounter, SystemColors.WindowText, TextFormatFlags.SingleLine | TextFormatFlags.GlyphOverhangPadding | TextFormatFlags.VerticalCenter | TextFormatFlags.WordEllipsis);
                    }

                    //     e.DrawDefault = true;
                }
            }
        }
        */
    }
}
