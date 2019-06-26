/*
              }        
            catch  (ChatWsFunctionException err){E(err);}
            catch (ChatDisconnectedException err){E(err);ChatDisconnected();}
            catch (Exception err) {        E(err);  }

 */
using System;
using System.Windows.Forms;
using JobInfo.WS_JobInfo;

namespace JobInfo
{
    internal class onEditBoxChanged
    {
        private TextBox textBox;
        String FirtValue;
        WS_JobInfo.User user;

        public onEditBoxChanged(TextBox textBox)
        {
            this.textBox = textBox;
            FirtValue = textBox.Text;
        }

        internal bool isChanged()
        {
            return FirtValue != textBox.Text;
        }

        internal WS_JobInfo.User GetChangedUser()
        {
            return user;
        }

        internal void SetChangedUser(WS_JobInfo.User User)
        {
            user = User;
        }

        internal string GetNewValue()
        {
            return textBox.Text;
        }
        internal string CancelChanges()
        {
            return textBox.Text=FirtValue;
        }
        
    }
}