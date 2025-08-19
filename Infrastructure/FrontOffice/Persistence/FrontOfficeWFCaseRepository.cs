using Infrastructure.Persistence;

namespace Infrastructure.FrontOffice.Persistence;

public class FrontOfficeWFCaseRepository : WFCaseRepositoryBase<FrontOfficeDbContext>
{
    public FrontOfficeWFCaseRepository(FrontOfficeDbContext context) : base(context) { }
}
