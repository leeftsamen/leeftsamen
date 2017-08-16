// <copyright file="RandomGenerator.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    using System;
    using System.Security.Cryptography;

    public class RandomGenerator : IRandomGenerator
    {
        public string GenerateRandomToken()
        {
            var data = new byte[24]; // Divisible by 3 so we don't get base64 padding characters.
            new RNGCryptoServiceProvider().GetBytes(data);
            return Base64UrlEncode(data);
        }

        private static string Base64UrlEncode(byte[] data)
        {
            var base64EncodedText = Convert.ToBase64String(data);

            base64EncodedText = base64EncodedText.Replace("=", string.Empty).Replace('+', '-').Replace('/', '_');
            base64EncodedText =
                base64EncodedText.PadRight(base64EncodedText.Length + ((4 - (base64EncodedText.Length % 4)) % 4), '=');

            return base64EncodedText;
        }
    }
}