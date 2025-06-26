var StopSessionsConfigurationPage = {
            pluginUniqueId: "28a6125c-cb57-4e8b-b031-421ac55cfa91",

            txtCleanSecs: document.querySelector("#txtCleanSecs")
        };

        window.addEventListener("pageshow", function (_) {
            Dashboard.showLoadingMsg();
            console.log('pageshow');
            window.ApiClient.getPluginConfiguration(StopSessionsConfigurationPage.pluginUniqueId).then(function (config) {
                StopSessionsConfigurationPage.txtCleanSecs.value = config.PausedSeconds || "3600";
                Dashboard.hideLoadingMsg();
            });
        });

        document.querySelector(".esqConfigurationForm").addEventListener("submit", function(e){
            e.preventDefault();
            Dashboard.showLoadingMsg();

            window.ApiClient.getPluginConfiguration(StopSessionsConfigurationPage.pluginUniqueId).then(function (config) {
                config.PausedSeconds = StopSessionsConfigurationPage.txtCleanSecs.value;
                window.ApiClient.updatePluginConfiguration(StopSessionsConfigurationPage.pluginUniqueId, config).then(Dashboard.processPluginConfigurationUpdateResult);
            });

            return false;
        });