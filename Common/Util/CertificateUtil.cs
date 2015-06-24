using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using Common.Models;

namespace Common.Util
{
    public class CertificateUtil
    {
        public static List<CertificateModel> GetCertificateList()
        {
            List<CertificateModel> list = new List<CertificateModel>();
            X509Store store = new X509Store(StoreLocation.CurrentUser);

            store.Open(OpenFlags.ReadOnly);
            string name;
            foreach (X509Certificate2 mCert in store.Certificates)
            {
                name = mCert.Subject;
                name = name.Substring(name.IndexOf("CN=") + 3);
                name = name.Split(',')[0];
                
                list.Add(new CertificateModel { Name = name, SerialNumber = mCert.SerialNumber });
            }
            return list;
        }

        public static X509Certificate2 GetCertificate(string serialNumber)
        {
            List<string> list = new List<string>();
            X509Store storeCurUser = new X509Store(StoreLocation.CurrentUser);

            storeCurUser.Open(OpenFlags.ReadOnly);
            
            foreach (X509Certificate2 mCert in storeCurUser.Certificates)
            {
                if (mCert.SerialNumber.Contains(serialNumber))
                {
                    return mCert;
                }
            }

            X509Store storeMachine = new X509Store(StoreLocation.LocalMachine);

            storeMachine.Open(OpenFlags.ReadOnly);

            foreach (X509Certificate2 mCert in storeMachine.Certificates)
            {
                if (mCert.SerialNumber.Contains(serialNumber))
                {
                    return mCert;
                }
            }

            return null;
        }
    }
}
