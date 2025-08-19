using Domain.Entities;

namespace Application.Interfaces;

public interface IWFCaseLinkService
{
    Task SendTestMessage();
    Task PublishLinkCreated(WFCaseLink entity);
}
