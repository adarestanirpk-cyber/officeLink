namespace Infrastructure.Persistence;

public class BackOfficeWFCaseRepository: WFCaseRepositoryBase<BackOfficeDbContext>
{
    public BackOfficeWFCaseRepository(BackOfficeDbContext context) : base(context) { }
}
