using System;
using System.Security.Cryptography;

public class EcKeyGenerator
{
    public static string GenerateEcPublicKey()
    {
        using (ECDsa ecDsa = ECDsa.Create(ECCurve.NamedCurves.nistP256))
        {
            if (ecDsa == null)
            {
                throw new InvalidOperationException("Failed to create ECDsa key pair.");
            }

            byte[] publicKeyBytes = ecDsa.ExportSubjectPublicKeyInfo();
            string publicKeyPem = Convert.ToBase64String(publicKeyBytes);

            return publicKeyPem;
        }
    }
}

