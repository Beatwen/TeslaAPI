namespace TeslaAPI.Component
{
    using System;
    using System.Security.Cryptography;
    public class EcKeyGenerator
    {
        private string EcPrivateKey = "MHcCAQEEIEHw43vIrh3sCFLg8aZJOipIECda0Hw7FSKValx2QVvwoAoGCCqGSM49AwEHoUQDQgAE635xi2MkA0mhCLo2ZMQEbA3HgrMimEtHj5we3lSN+Nt+snVnZJxM9VvqYjzGbHSKoMnqnaiylHJgdbUSPWXWxw==";
    }
        //    public static string GenerateEcPublicKey()
        //    {
        //        using (ECDsa ecDsa = ECDsa.Create(ECCurve.NamedCurves.nistP256))
        //        {
        //            if (ecDsa == null)
        //            {
        //                throw new InvalidOperationException("Failed to create ECDsa key pair.");
        //            }
        //            byte[] privateKeyBytes = ecDsa.ExportECPrivateKey();
        //            string privateKeyPem = Convert.ToBase64String(privateKeyBytes);

        //            byte[] publicKeyBytes = ecDsa.ExportSubjectPublicKeyInfo();
        //            string publicKeyPem = Convert.ToBase64String(publicKeyBytes);

        //            return publicKeyPem + " private key = " + privateKeyPem;

        //        }
        //    }
        //}

    }
