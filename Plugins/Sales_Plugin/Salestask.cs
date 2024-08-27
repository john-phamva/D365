using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using System.ServiceModel;
using Microsoft.Xrm.Sdk.Query;

namespace Sales_Plugin
{
    public class Salestask : IPlugin
    {
        public void Execute(IServiceProvider serviceProvider)
        {
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
            {
                Entity account = (Entity)context.InputParameters["Target"];
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

                try
                {
                    // Retrieve attribute values with null checks
                    var accountName = account.GetAttributeValue<string>("name") ?? "Unknown";
                    var accountTelephone = account.GetAttributeValue<string>("telephone1") ?? "No Telephone";
                    var contactLookup = account.GetAttributeValue<EntityReference>("primarycontactid");
                    var creditLimit = account.GetAttributeValue<Money>("creditlimit");

                    // Trace the account information
                    tracingService.Trace("Account Name: {0}", accountName);
                    tracingService.Trace("Account Telephone: {0}", accountTelephone);
                    tracingService.Trace("Primary Contact ID: {0}", contactLookup?.Id.ToString() ?? "No Contact");
                    tracingService.Trace("Credit Limit: {0}", creditLimit?.Value.ToString("C") ?? "No Credit Limit");

                    // Query the related contact
                    if (contactLookup != null)
                    {
                        QueryExpression contactQuery = new QueryExpression("contact")
                        {
                            ColumnSet = new ColumnSet("firstname", "lastname", "jobtitle", "parentcustomerid"),
                            Criteria = new FilterExpression()
                            {
                                Conditions =
            {
                new ConditionExpression("contactid", ConditionOperator.Equal, contactLookup.Id)
            }
                            }
                        };

                        // Execute the query and trace results
                        EntityCollection results = service.RetrieveMultiple(contactQuery);
                        foreach (var contact in results.Entities)
                        {
                            string firstName = contact.GetAttributeValue<string>("firstname") ?? "No First Name";
                            string lastName = contact.GetAttributeValue<string>("lastname") ?? "No Last Name";
                            string jobTitle = contact.GetAttributeValue<string>("jobtitle") ?? "No Job Title";

                            // Tracing the contact details
                            tracingService.Trace("Contact: {0} {1}, Job Title: {2}", firstName, lastName, jobTitle);
                        }
                    }

                    // Create a follow-up task
                    Entity followup = new Entity("task")
                    {
                        ["subject"] = "Send a new email",
                        ["description"] = "Follow up with the customer",
                        ["scheduledstart"] = DateTime.Now,
                        ["scheduledend"] = DateTime.Now.AddDays(1),
                        ["category"] = context.PrimaryEntityName,
                        ["regardingobjectid"] = new EntityReference("account", context.OutputParameters.Contains("id") ? (Guid)context.OutputParameters["id"] : Guid.Empty)
                    };

                    // Trace and create the follow-up task
                    tracingService.Trace("FollowupPlugin: Creating the task");
                    service.Create(followup);

                }

                catch (FaultException<OrganizationServiceFault> ex)
                {
                    throw new InvalidPluginExecutionException(" error", ex);
                }
                catch (Exception ex)
                {
                    tracingService.Trace("Followup: {0}", ex.ToString());
                }
            }
        }
    }
}
