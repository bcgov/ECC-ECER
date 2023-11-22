using System.Diagnostics.CodeAnalysis;
using ECER.Utilities.DataverseSdk.Model;
using Microsoft.Xrm.Sdk;

namespace ECER.Utilities.DataverseSdk;

public static class ExtensionMethods
{
    public static void AddLink([NotNull] this EcerContext context, Entity sourceEntity, string relationshipName, Entity targetEntity)
    {
        context.AddLink(sourceEntity, new Relationship(relationshipName), targetEntity);
    }
}
