using System;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Pkcs;
using System.IO;

namespace Common.Util
{
    public class XmlSignUtil
    {
        public static byte[] SignFile(X509Certificate2 cert, byte[] data)
        {
            try
            {
                ContentInfo content = new ContentInfo(data);
                SignedCms signedCms = new SignedCms(content, false);
                if (VerifySign(data))
                {
                    signedCms.Decode(data);
                }

                CmsSigner signer = new CmsSigner(cert);
                signer.IncludeOption = X509IncludeOption.WholeChain;
                signedCms.ComputeSignature(signer);

                return signedCms.Encode();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao assinar arquivo. A mensagem retornada foi: " + ex.Message);
            }
        }

        public static bool VerifySign(byte[] data)
        {
            try
            {
                SignedCms signed = new SignedCms();
                signed.Decode(data);
            }
            catch
            {
                return false; // Arquivo não assinado
            }
            return true;
        }

        public static byte[] SignFile(string CertFile, string CertPass, byte[] data)
        {
            FileStream fs = new FileStream(CertFile, FileMode.Open);
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            X509Certificate2 cert = new X509Certificate2(buffer, CertPass);
            fs.Close();
            fs.Dispose();
            return SignFile(cert, data);
        }

        private static X509Certificate2 FindCertOnStore(int idx)
        {
            X509Store st = new X509Store(StoreLocation.CurrentUser);
            st.Open(OpenFlags.ReadOnly);
            X509Certificate2 ret = st.Certificates[idx];
            st.Close();
            return ret;
        }

        public static void SignXml(XmlDocument Doc, RSA Key)
        {
            // Check arguments.
            if (Doc == null)
                throw new ArgumentException("Doc");
            if (Key == null)
                throw new ArgumentException("Key");

            // Create a SignedXml object.
            SignedXml signedXml = new SignedXml(Doc);

            // Add the key to the SignedXml document.
            signedXml.SigningKey = Key;

            // Create a reference to be signed.
            Reference reference = new Reference();
            reference.Uri = "";

            // Add an enveloped transformation to the reference.
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            // Append the element to the XML document.
            Doc.DocumentElement.AppendChild(Doc.ImportNode(xmlDigitalSignature, true));
        }

        public string SignXml(string xmlString, string signTag, X509Certificate2 certificate)
        {
            try
            {
                // checking if there is a certified used on xml sign
                string _xnome = "";
                if (certificate != null)
                    _xnome = certificate.Subject.ToString();

                string x;
                x = certificate.GetKeyAlgorithm().ToString();

                // Create a new XML document.
                XmlDocument doc = new XmlDocument();

                // Format the document to ignore white spaces.
                doc.PreserveWhitespace = false;

                // Load the passed XML file using it’s name.
                try
                {
                    doc.LoadXml(xmlString);

                    // cheching the element will be sign
                    int tagQuantity = doc.GetElementsByTagName(signTag).Count;

                    if (tagQuantity == 0)
                    {
                        return "A tag de assinatura " + signTag.Trim() + " não existe";
                    }
                    else
                    {
                        if (tagQuantity > 1)
                        {
                            return "A tag de assinatura " + signTag.Trim() + " não é unica";
                        }
                        else
                        {
                            try
                            {
                                // Create a SignedXml object.
                                SignedXml signedXml = new SignedXml(doc);

                                // Add the key to the SignedXml document
                                signedXml.SigningKey = certificate.PrivateKey;

                                // Create a reference to be signed
                                Reference reference = new Reference();

                                XmlAttributeCollection tag = doc.GetElementsByTagName(signTag).Item(0).Attributes;
                                foreach (XmlAttribute xmlAttr in tag)
                                {
                                    if (xmlAttr.Name == "Id")
                                        reference.Uri = "#" + xmlAttr.InnerText;
                                }

                                // Felipe Hosomi - se reference.Uri == null, dá erro na assinatura
                                if (reference.Uri == null)
                                {
                                    reference.Uri = String.Empty;
                                }

                                // Add an enveloped transformation to the reference.
                                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                                reference.AddTransform(env);

                                XmlDsigC14NTransform c14 = new XmlDsigC14NTransform();
                                reference.AddTransform(c14);

                                // Add the reference to the SignedXml object.
                                signedXml.AddReference(reference);

                                // Create a new KeyInfo object
                                KeyInfo keyInfo = new KeyInfo();

                                // Load the certificate into a KeyInfoX509Data object
                                // and add it to the KeyInfo object.
                                keyInfo.AddClause(new KeyInfoX509Data(certificate));

                                // Add the KeyInfo object to the SignedXml object.
                                signedXml.KeyInfo = keyInfo;
                                signedXml.ComputeSignature();

                                // Get the XML representation of the signature and save
                                // it to an XmlElement object.
                                XmlElement xmlDigitalSignature = signedXml.GetXml();

                                // save element on XML
                                doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));
                                XmlDocument XMLDoc = new XmlDocument();
                                XMLDoc.PreserveWhitespace = false;
                                XMLDoc = doc;

                                // XML document already signed
                                return XMLDoc.OuterXml;
                            }
                            catch (Exception e)
                            {
                                return "Erro ao assinar o documento - " + e.Message;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    return "XML mal formado - " + e.Message;
                }
            }
            catch (Exception e)
            {
                return "Problema ao acessar o certificado digital" + e.Message;
            }
        }
    }
}
