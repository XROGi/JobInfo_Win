/*
              }        
            catch  (ChatWsFunctionException err){E(err);}
            catch (ChatDisconnectedException err){E(err);ChatDisconnected();}
            catch (Exception err) {        E(err);  }

 */
using System;

namespace JobInfo
{
    internal class LogData
    {
        private DateTime DateEvent;
        public  string MessageText;
        private string MessageText2;

        public LogData(DateTime now, string v)
        {
            this.DateEvent = now;
            this.MessageText = v;
        }

        public string DateEventString { get { return DateEvent.ToString("yyyy/MM/dd hh:mm:ss"); } internal set { MessageText2= value; } }

        internal string GetTextInfo()
        {
            return DateEventString + "\t" + MessageText;
        }
    }
}