var Automobile = window.Automobile || {};

(function () {
    //#region Variabili Globali
    var formContext;
    var formType;

    var CrmFormType = {
        create: 1,
        update: 2
    };

    var CrmSaveMode = {
        autoSave: 70,
        save: 1,
        saveAndClose: 2
    };
    //#endregion

    this.preventAutoSave = function (executionContext) {
        var eventArgs = executionContext.getEventArgs();
        if (eventArgs.getSaveMode() === CrmSaveMode.autoSave) {
            eventArgs.preventDefault();
        }
    }

    this.preventSave = function (executionContext) {
        var eventArgs = executionContext.getEventArgs();
        if (eventArgs.getSaveMode() == CrmSaveMode.save || 
            eventArgs.getSaveMode() == CrmSaveMode.saveAndClose) {
            eventArgs.preventDefault();
        }
    }

    this.OnLoad = function (executionContext) {
		debugger;
        formContext = executionContext.getFormContext();
        formType = formContext.ui.getFormType();

        if (formType === CrmFormType.create) {
            // Aggiungi il gestore di eventi per controllare la targa ogni volta che cambia
            formContext.getAttribute("par_targa").addOnChange(checkLicensePlate);
        }
    };

    this.OnSave = function (executionContext) {
        formContext = executionContext.getFormContext();
        ormType = formContext.ui.getFormType();

       if (formType === CrmFormType.create) {
        var thisNome = formContext.getAttribute("par_name").getValue();
        alert(`Complimenti, hai salvato correttamente la nuova vettura ${thisNome}`)
        }

        if (formType === CrmFormType.update) {
            var thisNome = formContext.getAttribute("par_name").getValue();
            alert(`Complimenti, hai modificato correttamente la vettura ${thisNome}`)
        }
    }

    // Funzione per controllare la targa
    function checkLicensePlate() {
		debugger;
        var thisTarga = formContext.getAttribute("par_targa").getValue();

        Xrm.WebApi.retrieveMultipleRecords("par_automobile", `?$select=par_targa&$filter=par_targa eq '${thisTarga}'`).then(
			function success(results) {
             debugger;   
			 console.log(results);
                
                if (results.entities.length > 0) {
                    // Mostra un messaggio di errore se la targa è già presente
                    Xrm.Navigation.openAlertDialog({
                        text: "Attenzione, la targa inserita è già presente nel database!"
                    });

                    // Svuota il campo targa per evitare il salvataggio
                    formContext.getAttribute("par_targa").setValue(null);
                }
            },
            function(error) {
                console.log(error.message);
            }
        );
    }

}).call(Automobile);
