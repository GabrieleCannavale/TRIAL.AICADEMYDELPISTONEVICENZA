using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Definizione del namespace e della classe del plugin
namespace TRIAL.AICADEMYDELPISTONE.PLUGINS
{
    public class PreCreate : IPlugin
    {
        // Implementazione del metodo Execute richiesto dall'interfaccia IPlugin
        public void Execute(IServiceProvider serviceProvider)
        {
            // Ottenimento del servizio di tracciamento per il logging
            ITracingService tracingService = (ITracingService)serviceProvider.GetService(typeof(ITracingService));

            // Ottenimento del contesto di esecuzione del plugin
            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));

            // Creazione del factory per il servizio di organizzazione
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));

            // Creazione del servizio di organizzazione per l'utente corrente
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            try
            {
                // Verifica se i parametri di input contengono l'entità da creare e se è del tipo corretto
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity)
                {
                    // Ottieni l'entità appena creata
                    Entity target = (Entity)context.InputParameters["Target"];
                    

                    // Verifica se l'entità contiene l'attributo "par_datadiacquisto"
                    if (target.Contains("par_dataacquisto"))
                    {
                        // Ottieni il valore dell'attributo "par_datadiacquisto"
                        DateTime dataAcquisto = target.GetAttributeValue<DateTime>("par_dataacquisto");

                        // Log della data di acquisto
                        tracingService.Trace($"La data di acquisto: {dataAcquisto.Date}");

                        // Verifica se la data di acquisto è valida
                        if (dataAcquisto != DateTime.MinValue && dataAcquisto.Date >= DateTime.Today)
                        {
                            // Se la data di acquisto non è valida, lancia un'eccezione
                            throw new Exception("La data di acquisto deve essere minore o uguale alla data odierna");
                        }
                    }

                    if (target.LogicalName.Equals("par_automobile"))
                    {

                        // Verifica il campo di ricerca per il contatto
                        if (target.Attributes.Contains("par_contactid"))
                        {
                            EntityReference contactReference = (EntityReference)target.Attributes["par_contactid"];
                            Guid contactId = contactReference.Id;

                            // Eseguire una query per ottenere il contatto dalla tabella Contact
                            ColumnSet columns = new ColumnSet("par_blacklist");
                            Entity contactEntity = service.Retrieve("contact", contactId, columns);

                            // Ottenere il valore di par_blacklist dal contatto
                            bool isBlacklisted = contactEntity.Contains("par_blacklist") ? (bool)contactEntity["par_blacklist"] : false;

                            // Verificare se il contatto è nella blacklist
                            if (isBlacklisted)
                            {
                                // Il contatto è nella blacklist, impedisci la creazione dell'Automobile
                                throw new InvalidPluginExecutionException("Il cliente è nella blacklist. Non può acquistare un automobile.");
                            }
                            else
                            {
                                tracingService.Trace("Il contatto non è presente nella black list");
                            }
                        }
                        
                        // Altrimenti, gestisci il caso in cui il campo "par_contactid" non è presente nell'entità "Automobile"
                    }
                }
            }
            catch (Exception e)
            {
                // Se si verifica un'eccezione, lancia un'eccezione di esecuzione del plugin con un messaggio di errore
                throw new InvalidPluginExecutionException($"Errore: {e.Message}");
            }
            
        }
    }
}

