using Microsoft.Xrm.Sdk;

namespace ECER.Utilities.ObjectStorage.Providers.Dataverse;
public record DataverseDescriptor(Entity Entity, string? PropertyName = null) : Descriptor;
