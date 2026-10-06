namespace DongTaErp.Application.Common.Models;

public class ErrorHelpers
{
    public static string NotFoundWithId(string entityName, object entityId)
    {
        return $"{entityName} with id {entityId} not found";
    }
}
