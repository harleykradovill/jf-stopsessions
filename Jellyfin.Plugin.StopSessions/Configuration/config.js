var StopSessionsConfigurationPage = {
            pluginUniqueId: "28a6125c-cb57-4e8b-b031-421ac55cfa91",

            txtCleanValue: document.querySelector("#txtCleanValue"),
            txtCleanUnit: document.querySelector("#txtCleanUnit")
        };

        window.addEventListener("pageshow", function (_) {
            Dashboard.showLoadingMsg();
            console.log('pageshow');
            window.ApiClient.getPluginConfiguration(StopSessionsConfigurationPage.pluginUniqueId).then(function (config) {
                StopSessionsConfigurationPage.txtCleanValue.value = config.PausedValue ?? 3600;
                StopSessionsConfigurationPage.txtCleanUnit.value = config.PausedUnit ?? "Seconds";
                Dashboard.hideLoadingMsg();
            });
        });

        document.querySelector(".esqConfigurationForm").addEventListener("submit", function(e){
            e.preventDefault();
            Dashboard.showLoadingMsg();

            window.ApiClient.getPluginConfiguration(StopSessionsConfigurationPage.pluginUniqueId).then(function (config) {
                config.PausedValue = parseInt(StopSessionsConfigurationPage.txtCleanValue.value);
                config.PausedUnit = StopSessionsConfigurationPage.txtCleanUnit.value;
                window.ApiClient.updatePluginConfiguration(StopSessionsConfigurationPage.pluginUniqueId, config).then(Dashboard.processPluginConfigurationUpdateResult);
            });

            return false;
        });