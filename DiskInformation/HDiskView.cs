using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Management;

namespace DiskInformation
{
    class HDiskView : INotifyPropertyChanged
    {

        public ObservableCollection<HDisk> disks { get; set; }
        public ObservableCollection<Partit> partitions{ get; set; }

        public HDiskView()
        {
            disks = new ObservableCollection<HDisk>();
            partitions = new ObservableCollection<Partit>();
            readInfo();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        public void readInfo()
        {
            disks.Clear();
            partitions.Clear();
            /* Получаем все диски, имеющиеся в системе, затем для каждого получаются параметры и список разделов.
            После перебора всех разделов определяется "неразмеченное пространство", которое добавляется в виде отдельного раздела
             для дальнейшего отображения.
             
             */
            try
            {
                using (ManagementObjectSearcher driveQuery = new ManagementObjectSearcher("select * from Win32_DiskDrive"))
                {
                    foreach (ManagementObject d in driveQuery.Get())
                    {
                        HDisk hd = new HDisk();
                        UInt64 usSp = 0;
                        hd.parts = new ObservableCollection<Partit>();
                        hd.dName = "Disk " + Convert.ToUInt32(d.Properties["Signature"].Value);
                        hd.dType = Convert.ToString(d.Properties["MediaType"].Value);
                        hd.dSize = Convert.ToUInt64(d.Properties["Size"].Value);
                        hd.spaceCount();

                        string partitionQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_DiskDriveToDiskPartition", d.Path.RelativePath);
                        using (ManagementObjectSearcher partitionQuery = new ManagementObjectSearcher(partitionQueryText))
                        {
                            foreach (ManagementObject p in partitionQuery.Get())
                            {
                                string logicalDriveQueryText = string.Format("associators of {{{0}}} where AssocClass = Win32_LogicalDiskToPartition", p.Path.RelativePath);
                                using (ManagementObjectSearcher logicalDriveQuery = new ManagementObjectSearcher(logicalDriveQueryText))
                                {
                                    Partit part = new Partit();

                                    if (logicalDriveQuery.Get().Count == 0)
                                    {
                                        part.pName = Convert.ToString(p.Properties["Name"].Value);
                                        part.pType = Convert.ToString(p.Properties["Type"].Value);
                                        part.pStatus = Convert.ToString(d.Properties["Status"].Value);
                                        part.fSyst = "";
                                        part.pTotalSize = part.pFreeSize = Convert.ToUInt64(p.Properties["Size"].Value);
                                        part.spaceCount();
                                    }
                                    else
                                    {
                                        foreach (ManagementObject ld in logicalDriveQuery.Get())
                                        {
                                            part.pName = Convert.ToString(ld.Properties["VolumeName"].Value) + "(" + Convert.ToString(ld.Properties["DeviceId"].Value) + ")";

                                            UInt32 driveType = Convert.ToUInt32(ld.Properties["DriveType"].Value);
                                            switch (driveType)
                                            {
                                                case 0:
                                                    part.pType = "Unknown";
                                                    break;
                                                case 1:
                                                    part.pType = "No Root Directory ";
                                                    break;
                                                case 2:
                                                    part.pType = "Removable Disk";
                                                    break;
                                                case 3:
                                                    part.pType = "Local Disk";
                                                    break;
                                                case 4:
                                                    part.pType = "Network Drive";
                                                    break;
                                                case 5:
                                                    part.pType = "Compact Disc";
                                                    break;
                                                case 6:
                                                    part.pType = "RAM Disk";
                                                    break;
                                                default:
                                                    part.pType = "";
                                                    break;
                                            }
                                            part.pStatus = Convert.ToString(d.Properties["Status"].Value);
                                            part.fSyst = Convert.ToString(ld.Properties["FileSystem"].Value);
                                            part.pTotalSize = Convert.ToUInt64(ld.Properties["Size"].Value);
                                            part.pFreeSize = Convert.ToUInt64(ld.Properties["FreeSpace"].Value);
                                            part.spaceCount();


                                        }
                                    }
                                    usSp += part.pTotalSize;
                                    partitions.Add(part);
                                    hd.parts.Add(part);
                                }
                            }
                        }
                        if (hd.dSize - usSp > 0)
                        {
                            Partit UlPt = new Partit();
                            UlPt.pName = "Unallocated Space";
                            UlPt.pTotalSize = hd.dSize - usSp;
                            UlPt.spaceCount();
                            UlPt.pStatus = "Unallocated";
                            hd.parts.Add(UlPt);
                        }
                        disks.Add(hd);
                    }

                }
            }
            catch(ManagementException ex)
            {
                string exS = System.DateTime.Now + " Error in WMI querry /n"+ex.Message;
                                   
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
                {
                    sw.Write(exS);
                }
        
            }
            catch(Exception ex)
            {
                string exS = System.DateTime.Now + " Unexpected error /n" + ex.Message;

                using (System.IO.StreamWriter sw = new System.IO.StreamWriter("log.txt", true))
                {
                    sw.Write(exS);
                }
            }
        }
           
    }
}

