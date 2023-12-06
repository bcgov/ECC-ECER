using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace ECER.Utilities.Hosting;

public static class RouteBuildingExtensions
{
    public static RouteHandlerBuilder WithOpenApi(this RouteHandlerBuilder builder, string summary, string description, string operationId){
        return builder.WithOpenApi(op =>
        {
            op.OperationId = operationId;
            op.Summary = summary;
            op.Description = description;
            return op;
        });
    }
}
