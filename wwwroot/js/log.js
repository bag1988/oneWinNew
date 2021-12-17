var log = {
    getLog: function () {
        var userName = document.querySelector("select[name=userName]").value;
        var dateRequset = document.querySelector("input[name=dateRequset]").value;
        var dateTimeStart = document.querySelector("input[name=dateTimeStart]").value;
        var dateTimeStop = document.querySelector("input[name=dateTimeStop]").value;
        var controllerName = document.querySelector("select[name=controllerName]").value;
        var otdelName = document.querySelector("select[name=otdelName]").value;
        var actionName = document.querySelector("select[name=actionName]").value;        
        if (dateRequset == "") {
            createErrorWindow("Выберите дату!");
            return;
        }
        $.ajax({
            type: "post",
            url: "/admin/log/viewPartial",
            data: {
                "userName": userName, "dateRequset": dateRequset, "dateTimeStart": dateTimeStart, "dateTimeStop": dateTimeStop, "controllerName": controllerName, "otdelName": otdelName, "actionName": actionName },
            success: function (response) {
                document.querySelector("#viewPartial").innerHTML = response;
            },
            failure: function (response) {
                createErrorWindow(response.responseText);
            },
            error: function (response) {
                if (response.status == "403")
                    response.responseText = "Нет доступа!";
                createErrorWindow(response.responseText);
            }
        });
    },
    getRequest: function (elm) {
        $.ajax({
            type: "post",
            url: "/admin/log/getRequest",
            data: { "strRequest": elm.getAttribute("data-request") },
            success: function (response) {
                createErrorWindow(response, "Список параметров");
            },
            failure: function (response) {
                createErrorWindow(response.responseText);
            },
            error: function (response) {
                if (response.status == "403")
                    response.responseText = "Нет доступа!";
                createErrorWindow(response.responseText);
            }
        });
    },
    loadSearchField: function () {
        var dateRequset = document.querySelector("input[name=dateRequset]").value;
        document.querySelector("input[name=dateTimeStart]").value = "";
        document.querySelector("input[name=dateTimeStop]").value = "";
        if (dateRequset == "") {
            return;
        }
        $.ajax({
            type: "post",
            url: "/admin/log/getUserName",
            data: { "date": dateRequset },
            success: function (response) {
                var userName = document.querySelector("select[name=userName]");
                userName.innerHTML = ("<option value=''>----</option>");
                response.forEach((e) => {
                    userName.innerHTML += ("<option value='" + e + "'>" + e + "</option>");
                });
            }
        });
        $.ajax({
            type: "post",
            url: "/admin/log/geControllerName",
            data: { "date": dateRequset },
            success: function (response) {
                var userName = document.querySelector("select[name=controllerName]");
                userName.innerHTML = ("<option value=''>----</option>");
                response.forEach((e) => {
                    userName.innerHTML += ("<option value='" + e.addressController + "'>" + e.nameController + "</option>");
                });
            }
        });
        $.ajax({
            type: "post",
            url: "/admin/log/getOtdelName",
            data: { "date": dateRequset },
            success: function (response) {
                var userName = document.querySelector("select[name=otdelName]");
                userName.innerHTML = ("<option value=''>----</option>");
                response.forEach((e) => {
                    userName.innerHTML += ("<option value='" + e.key + "'>" + e.value + "</option>");
                });
            }
        });
        $.ajax({
            type: "post",
            url: "/admin/log/geActionName",
            data: { "date": dateRequset },
            success: function (response) {
                var userName = document.querySelector("select[name=actionName]");
                userName.innerHTML = ("<option value=''>----</option>");
                response.forEach((e) => {
                    userName.innerHTML += ("<option value='" + e + "'>" + e + "</option>");
                });
            }
        });
    }
}