namespace Infrastructure.Persistence;

public class FrontOfficeWFCaseRepository: WFCaseRepositoryBase<FrontOfficeDbContext>
{
    public FrontOfficeWFCaseRepository(FrontOfficeDbContext context) : base(context) { }
}
