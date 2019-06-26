/*
              }        
            catch  (ChatWsFunctionException err){E(err);}
            catch (ChatDisconnectedException err){E(err);ChatDisconnected();}
            catch (Exception err) {        E(err);  }

 */
using JobInfo.WS_JobInfo;

namespace JobInfo
{
    public class ObjOu
    {
        public WS_JobInfo.Obj ou;
        public WS_JobInfo.Obj parent_ou;

        public ObjOu(WS_JobInfo.Obj ou_, WS_JobInfo.Obj parent_ou_)
        {
            this.ou = ou_;
            this.parent_ou = parent_ou_;
        }
    }
}