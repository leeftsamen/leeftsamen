// <copyright file="IRandomGenerator.cs" company="LeeftSamen B.V.">
// Copyright © 2015-2016 LeeftSamen B.V. All rights reserved.
// </copyright>

namespace LeeftSamen.Portal.Services
{
    public interface IRandomGenerator
    {
        string GenerateRandomToken();
    }
}