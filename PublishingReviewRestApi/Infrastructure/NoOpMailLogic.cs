using PublicationPlanningContracts.BindingModels;
using PublicationPlanningBusinessLogic.MailLogic;

public class NoOpMailLogic : AbstractMailLogic
{
    public override void MailConfig(MailConfigBindingModel config) { /* no-op */ }
    public override void MailSendAsync(MailSendInfoBindingModel model) { /* no-op */ }
}
