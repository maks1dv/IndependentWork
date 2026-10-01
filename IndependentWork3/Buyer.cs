using System;

namespace IndependentWork3;

public class Buyer(string identity, string name)
{
    public string IdentityGuid { get; } = !string.IsNullOrWhiteSpace(identity)
        ? identity
        : throw new ArgumentNullException(nameof(identity));

    public string Name { get; } = !string.IsNullOrWhiteSpace(name)
        ? name
        : throw new ArgumentNullException(nameof(name));
}