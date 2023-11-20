using System.Globalization;
using ECER.Utilities.DataverseSdk.Model;

namespace ECER.Resources.Accounts.Registrants;

internal sealed class RegistrantRepository : IRegistrantRepository
{
    private readonly EcerContext context;

    public RegistrantRepository(EcerContext context)
    {
        this.context = context;
    }

    public async Task<string> Create(NewRegistrantRequest request)
    {
        await Task.CompletedTask;
        var contact = new Contact(Guid.NewGuid())
        {
            FirstName = "first",
            LastName = "last",
            Birthdate = DateTime.Parse("2000-01-01", CultureInfo.InvariantCulture)
        };

        context.AddObject(contact);

        context.SaveChanges();

        return contact.Id.ToString();
    }
}
