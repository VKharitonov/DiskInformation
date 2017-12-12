using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace DiskInformation
{
   public class HDisk:INotifyPropertyChanged
    {
      public string dName{ get; set; }
      public string sDSize { get; set; }
      public string  dType{ get; set; }
      public UInt64 dSize { get; set; }
      public ObservableCollection<Partit> parts{ get; set; }

      

       public void spaceCount()
      {
          try
          {
              decimal d = 0;
              if (dSize / 1073741824 > 1)
              {
                  d = Math.Round((Convert.ToDecimal(dSize) / 1073741824), 2);
                  sDSize = d + " GB";
              }
              else
              {
                  d = Math.Round((Convert.ToDecimal(dSize) / 1048576), 2);
                  sDSize = d + " MB";
              }
          }
          catch (DivideByZeroException ex)
          {
              string exS = System.DateTime.Now + " Incorrect total disk size /n" + ex.Message;

              using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
              {
                  sw.Write(exS);
              }
          }
          catch (Exception ex)
          {
              string exS = System.DateTime.Now + " Unexpected error /n" + ex.Message;

              using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
              {
                  sw.Write(exS);
              }
          }
      }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }

    public class Partit:INotifyPropertyChanged
    {
        public string pName{ get; set; }
        public string pStatus{ get; set; }
        public string pType{ get; set; }
        public string fSyst{ get; set; }
        public string sPTotalSize{ get; set; }
        public string sPFreeSize{ get; set; }
        public string sPFreeProc{ get; set; }
        public UInt64 pTotalSize, pFreeSize;
        public decimal pFreeProc;
        
        public void spaceCount()
        {
            try
            {


                decimal d = 0;
                if (pTotalSize / 1073741824 > 1)
                {
                    d = Math.Round((Convert.ToDecimal(pTotalSize) / 1073741824), 2);
                    sPTotalSize = d + " GB";
                }
                else
                {
                    d = Math.Round((Convert.ToDecimal(pTotalSize) / 1048576), 2);
                    sPTotalSize = d + " MB";
                }

                if (pFreeSize / 1073741824 > 1)
                {
                    d = Math.Round((Convert.ToDecimal(pFreeSize) / 1073741824), 2);
                    sPFreeSize = d + " GB";
                }
                else
                {
                    d = Math.Round((Convert.ToDecimal(pFreeSize) / 1048576), 2);
                    sPFreeSize = d + " MB";
                }

                pFreeProc = Math.Round((Convert.ToDecimal(pFreeSize) * 100 / (Convert.ToDecimal(pTotalSize))), 2);
                sPFreeProc = pFreeProc + " %";

            }
            catch (DivideByZeroException ex)
            {
                string exS = System.DateTime.Now + " Incorrect total partition size /n" + ex.Message;

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
                {
                    sw.Write(exS);
                }
            }
            catch (Exception ex)
            {
                string exS = System.DateTime.Now + " Unexpected error /n" + ex.Message;

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
                {
                    sw.Write(exS);
                }
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
