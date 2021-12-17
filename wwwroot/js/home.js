//отправка в отдел
function sendToOtdel() {
    if (!checkSelectDoc())
        return;        
    var dialogWindow = createWindow("Передать в отдел");
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Дата передачи в отдел", "dateToOtdel", "date", true));
    var dialogSave = dialogWindow.querySelector("button[name=dialogSave]");
        
    var arrayProc = new Array();

    getSelectDoc().forEach(e => {
        arrayProc.push(e.getAttribute("data-id"));
    });
    
    dialogSave.addEventListener("click", x => {
        var data = { "dateSend": dialogWindow.querySelector("input[name=dateToOtdel]").value, "arrayProc": arrayProc };
        var url = '/home/sendToOtdel';
        sendAjaxToHome(url, data, dialogSave);
    });

    var updateButton = createButton("updateDate", "Исправить дату");


    updateButton.addEventListener("click", x => {
        var data = { "dateSend": dialogWindow.querySelector("input[name=dateToOtdel]").value, "arrayProc": arrayProc };
        var url = '/home/replaceDateToOtdel';        
        sendAjaxToHome(url, data, updateButton);
    });


    dialogWindow.querySelector(".modal-footer").insertAdjacentElement('afterbegin', updateButton);

    $(dialogWindow).modal('show');
}

//возврат из отдел
function returnFromDept() {
    if (!checkSelectDoc())
        return;
    var dialogWindow = createWindow("Вернуть из отдела");
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Дата передачи из отдел", "dateFromOtdel", "date", true));
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Решение администрации №", "decisionNumber", "text"));
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Решение от", "dateDecision", "date"));
    dialogWindow.querySelector(".modal-body").appendChild(labelAndSelect("Вопрос решен", "issueResolved", "Положительно,Отрицательно,Отказано в осуществлении,Отказано в приёме,Переадресовано,Отозвано"));
    

    var dialogSave = dialogWindow.querySelector("button[name=dialogSave]");
        
    var arrayProc = new Array();
    getSelectDoc().forEach(e => {
        arrayProc.push(e.getAttribute("data-id"));
    });

    

    dialogSave.addEventListener("click", x => {
        var data = {
            "dateSend": dialogWindow.querySelector("input[name=dateFromOtdel]").value
            , "arrayProc": arrayProc
            , "decisionNumber": dialogWindow.querySelector("input[name=decisionNumber]").value
            , "dateDecision": dialogWindow.querySelector("input[name=dateDecision]").value
            , "issueResolved": dialogWindow.querySelector("select[name=issueResolved]").value
        };
        var url = '/home/returnFromDept';
        sendAjaxToHome(url, data, dialogSave);
    });

    var updateButton = createButton("updateDate", "Исправить дату");


    updateButton.addEventListener("click", x => {
        var data = {
            "dateSend": dialogWindow.querySelector("input[name=dateFromOtdel]").value
            , "arrayProc": arrayProc
            , "decisionNumber": dialogWindow.querySelector("input[name=decisionNumber]").value
            , "dateDecision": dialogWindow.querySelector("input[name=dateDecision]").value
            , "issueResolved": dialogWindow.querySelector("select[name=issueResolved]").value
        };
        var url = '/home/replaceDateFromOtdel';
        sendAjaxToHome(url, data, updateButton);
    });


    dialogWindow.querySelector(".modal-footer").insertAdjacentElement('afterbegin', updateButton);

    $(dialogWindow).modal('show');
}


//возврат из отдел и выдача заявителю
function returnFromDeptAndIssuance() {
    if (!checkSelectDoc())
        return;
    var dialogWindow = createWindow("возврат из отдел и выдача заявителю");
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Дата передачи из отдел", "dateFromOtdel", "date", true));
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Решение администрации №", "decisionNumber", "text"));
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Решение от", "dateDecision", "date"));
    dialogWindow.querySelector(".modal-body").appendChild(labelAndSelect("Вопрос решен", "issueResolved", "Положительно,Отрицательно,Отказано в осуществлении,Отказано в приёме,Переадресовано,Отозвано"));
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Дата выдачи заявителю", "issueDate", "date", true));
    dialogWindow.querySelector(".modal-body").appendChild(labelAndSelect("Выдано", "evaluationNotification", "Выдано на руки,По почте,В виде электронного документа,Курьером,Иным способом"));
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Дело №", "caseNumber", "text"));

    var dialogSave = dialogWindow.querySelector("button[name=dialogSave]");

    var arrayProc = new Array();
    getSelectDoc().forEach(e => {
        arrayProc.push(e.getAttribute("data-id"));
    });

    dialogSave.addEventListener("click", x => {
        var data = {
            "dateSend": dialogWindow.querySelector("input[name=dateFromOtdel]").value
            , "arrayProc": arrayProc
            , "decisionNumber": dialogWindow.querySelector("input[name=decisionNumber]").value
            , "dateDecision": dialogWindow.querySelector("input[name=dateDecision]").value
            , "issueResolved": dialogWindow.querySelector("select[name=issueResolved]").value
            , "issueDate": dialogWindow.querySelector("input[name=issueDate]").value
            , "caseNumber": dialogWindow.querySelector("input[name=caseNumber]").value
            , "evaluationNotification": dialogWindow.querySelector("select[name=evaluationNotification]").value
        };
        var url = '/home/returnFromDeptAndIssuance';
        sendAjaxToHome(url, data, dialogSave);
    });
    //var updateButton = createButton("updateDate", "Исправить дату");
    //updateButton.addEventListener("click", x => {
    //    var data = {
    //        "dateSend": dialogWindow.querySelector("input[name=dateFromOtdel]").value
    //        , "arrayProc": arrayProc
    //        , "decisionNumber": dialogWindow.querySelector("input[name=decisionNumber]").value
    //        , "dateDecision": dialogWindow.querySelector("input[name=dateDecision]").value
    //        , "issueResolved": dialogWindow.querySelector("select[name=issueResolved]").value
    //        , "issueDate": dialogWindow.querySelector("input[name=issueDate]").value
    //        , "caseNumber": dialogWindow.querySelector("input[name=caseNumber]").value
    //        , "evaluationNotification": dialogWindow.querySelector("select[name=evaluationNotification]").value
    //    };
    //    var url = '/home/FromDeptAndIssuance';
    //    sendAjaxToHome(url, data, updateButton);
    //});
    //dialogWindow.querySelector(".modal-footer").insertAdjacentElement('afterbegin', updateButton);

    $(dialogWindow).modal('show');
}

//выдача заявителю
function issuanceApplicant() {
    if (!checkSelectDoc())
        return;
    var dialogWindow = createWindow("Выдача заявителю");   
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Дата выдачи заявителю", "issueDate", "date", true));
    dialogWindow.querySelector(".modal-body").appendChild(labelAndSelect("Выдано", "evaluationNotification", "Выдано на руки,По почте,В виде электронного документа,Курьером,Иным способом"));
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Дело №", "caseNumber", "text"));
    dialogWindow.querySelector(".modal-body").appendChild(labelAndSelect("Вопрос решен", "issueResolved", "Положительно,Отрицательно,Отказано в осуществлении,Отказано в приёме,Переадресовано,Отозвано"));

    var dialogSave = dialogWindow.querySelector("button[name=dialogSave]");

    var arrayProc = new Array();
    getSelectDoc().forEach(e => {
        arrayProc.push(e.getAttribute("data-id"));
    });

    dialogSave.addEventListener("click", x => {
        var data = {
            "arrayProc": arrayProc
            , "issueResolved": dialogWindow.querySelector("select[name=issueResolved]").value
            , "issueDate": dialogWindow.querySelector("input[name=issueDate]").value
            , "caseNumber": dialogWindow.querySelector("input[name=caseNumber]").value
            , "evaluationNotification": dialogWindow.querySelector("select[name=evaluationNotification]").value
        };
        var url = '/home/issuanceApplicant';
        sendAjaxToHome(url, data, dialogSave);
    });
    var updateButton = createButton("updateDate", "Исправить дату");
    updateButton.addEventListener("click", x => {
        var data = {
            "arrayProc": arrayProc
            , "issueResolved": dialogWindow.querySelector("select[name=issueResolved]").value
            , "issueDate": dialogWindow.querySelector("input[name=issueDate]").value
            , "caseNumber": dialogWindow.querySelector("input[name=caseNumber]").value
            , "evaluationNotification": dialogWindow.querySelector("select[name=evaluationNotification]").value
        };
        var url = '/home/replaceIssuanceApplicant';
        sendAjaxToHome(url, data, updateButton);
    });
    dialogWindow.querySelector(".modal-footer").insertAdjacentElement('afterbegin', updateButton);

    $(dialogWindow).modal('show');
}

//вернуть в поступившие
function returnToReceived() {
    if (!checkSelectDoc())
        return;
    var arrayProc = new Array();
    getSelectDoc().forEach(e => {
        arrayProc.push(e.getAttribute("data-id"));
    });

    var dialogWindow = createWindow("Вернуть в поступившие");
    dialogWindow.querySelector(".modal-body").innerHTML = "<strong>Вернуть в поступившие " + arrayProc.length+" записей</strong>";

    var dialogSave = dialogWindow.querySelector("button[name=dialogSave]");

   

    dialogSave.addEventListener("click", x => {
        var data = {
            "arrayProc": arrayProc           
        };
        var url = '/home/returnToReceived';
        sendAjaxToHome(url, data, dialogSave);
    });
    $(dialogWindow).modal('show');
}


//Приостановить
function stopStatement() {
    if (!checkSelectDoc())
        return;
    var dialogWindow = createWindow("Приостановить");
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Дата остановки", "dateStop", "date", true));
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Причина остановки", "reason", "text"));
    var dialogSave = dialogWindow.querySelector("button[name=dialogSave]");

    var arrayProc = new Array();

    getSelectDoc().forEach(e => {
        arrayProc.push(e.getAttribute("data-id"));
    });

    dialogSave.addEventListener("click", x => {
        var data = { "dateSend": dialogWindow.querySelector("input[name=dateStop]").value, "arrayProc": arrayProc, "reason": dialogWindow.querySelector("input[name=reason]").value };
        var url = '/home/stopStatement';
        sendAjaxToHome(url, data, dialogSave);
    });

    $(dialogWindow).modal('show');
}

//Возобновить
function resumeStatement() {
    if (!checkSelectDoc())
        return;
    var dialogWindow = createWindow("Возобновить");
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Дата остановки", "dateStart", "date", true));
    var dialogSave = dialogWindow.querySelector("button[name=dialogSave]");

    var arrayProc = new Array();

    getSelectDoc().forEach(e => {
        arrayProc.push(e.getAttribute("data-id"));
    });

    dialogSave.addEventListener("click", x => {
        var data = { "dateSend": dialogWindow.querySelector("input[name=dateStart]").value, "arrayProc": arrayProc };
        var url = '/home/resumeStatement';
        sendAjaxToHome(url, data, dialogSave);
    });

    $(dialogWindow).modal('show');
}

//Направить уведомление
function sendNotification() {
    if (!checkSelectDoc())
        return;
    var dialogWindow = createWindow("Направить уведомление");
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Дата направления уведомления", "dateSend", "date", true));
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Номер реестра", "registryNumber", "text"));
    var dialogSave = dialogWindow.querySelector("button[name=dialogSave]");

    var arrayProc = new Array();

    getSelectDoc().forEach(e => {
        arrayProc.push(e.getAttribute("data-id"));
    });

    dialogSave.addEventListener("click", x => {
        var data = { "dateSend": dialogWindow.querySelector("input[name=dateSend]").value, "arrayProc": arrayProc, "registryNumber": dialogWindow.querySelector("input[name=registryNumber]").value };
        var url = '/home/sendNotification';
        sendAjaxToHome(url, data, dialogSave);
    });

    $(dialogWindow).modal('show');
}

//удалить
function deleteStatement() {
    if (!checkSelectDoc())
        return;
    var arrayProc = new Array();
    getSelectDoc().forEach(e => {
        arrayProc.push(e.getAttribute("data-id"));
    });

    var dialogWindow = createWindow("Удаление");
    dialogWindow.querySelector(".modal-body").innerHTML = "<strong>Удалить " + arrayProc.length + " записей</strong>";

    var dialogSave = dialogWindow.querySelector("button[name=dialogSave]");

    dialogSave.addEventListener("click", x => {
        var data = {
            "arrayProc": arrayProc
        };
        var url = '/home/deleteStatement';
        sendAjaxToHome(url, data, dialogSave);
    });
    $(dialogWindow).modal('show');
}

//Устанавливаем видимость столбцов главной таблицы
function setColumnTable() {
    
    var dialogWindow = createWindow("Настройка столбцов");

    var innerStr = '<div class="form-group" id="viewColumnGroup">';

    var tdArray = document.querySelectorAll("th[data-col]");
    var nameColumn = {
        "DocNo": "Номер заявления", "LName": "Заявитель", "OrgName": "Организация", "PhoneNo": "Телефон", "Address": "Адрес", "GettingDate": "Дата обращения", "OutDeptDate": "Дата передачи в отдел", "ReturnInDeptDate": "Дата возврата из отдела"
        , "NotificationDate": "Дата уведомления", "IssueDate": "Дата выдачи", "MustBeReady": "Дата исполнения", "PerformerName": "Исполнитель", "Number": "Номер процедуры", "Registrator": "Регистратор", "Solution": "Решение"
        , "Notes": "Коментарий", "DateSsolutions": "№ и дата решения"};
    var input;

    for (var n in nameColumn) {
        input = document.querySelector("th[data-col=\"" + n + "\"");
        innerStr += '<div class="form-check">';
        innerStr += '<label class="form-check-label"><input class="form-check-input" ' + (!input ? "" : "checked") + ' type="checkbox" value="' + n + '"> ' + nameColumn[n] + '</label>';
        innerStr += '</div>';
    }
    innerStr += '</div>';

    dialogWindow.querySelector(".modal-body").innerHTML = innerStr;

    var dialogSave = dialogWindow.querySelector("button[name=dialogSave]");
    
    dialogSave.addEventListener("click", x => {
        var viewNameColumn = new Array();
        document.querySelector("#viewColumnGroup").querySelectorAll("input[type=checkbox]:checked").forEach(e => {
            viewNameColumn.push(e.getAttribute("value"));
        });
        var data = { "viewNameColumn": viewNameColumn };
        var url = '/home/setColumnTable';

        loadingButton(dialogSave);
        
        $.ajax({
            type: "post",
            url: url,
            data: data,
            success: function (response) {
                updateViewTable();
                removeDialog();

            },
            failure: function (response) {
                createErrorWindow(response.responseText);
            },
            error: function (response) {
                if (response.status == "403")
                    response.responseText = "Нет доступа!";
                createErrorWindow(response.responseText);
            },
            complete: function () { loadingButton(dialogSave);}
        });
    });

    $(dialogWindow).modal('show');
}


//поиск по запросам
function searchRequest() {


    var dialogWindow = createWindow("Поиск по запросам");
    dialogWindow.querySelector(".modal-body").appendChild(labelAndInput("Фамилия", "lnameSearch", "text"));
    var dialogSave = dialogWindow.querySelector("button[name=dialogSave]");
    dialogSave.innerHTML = "Поиск";
    dialogSave.addEventListener("click", x => {
        loadingButton(dialogSave);
        var data = { "lnameSearch": dialogWindow.querySelector("input[name=lnameSearch]").value };
        var url = '/home/searchRequest';

        $.ajax({
            type: "post",
            url: url,
            data: data,
            success: function (response) {

                if (response) {
                    
                    var innerStr = "";

                    for(var key in response) {

                        innerStr += "<p>";
                        innerStr += "<b>" + key + " район</b><br/>";
                        response[key].forEach(x => {
                            innerStr += x + "<br/>";
                        });
                        innerStr += "</p>";
                    }
                    createErrorWindow(innerStr, "Результат поиска");
                    

                }
                

            },
            failure: function (response) {
                createErrorWindow(response.responseText);
            },
            error: function (response) {
                if (response.status == "403")
                    response.responseText = "Нет доступа!";
                createErrorWindow(response.responseText);
            },
            complete: function () {
                loadingButton(dialogSave);
                removeDialog();
            }
        });
    });

    $(dialogWindow).modal('show');
}


//отправка ajax запроса
function sendAjaxToHome(url, data, button) {
    
    if (button) {
        loadingButton(button);
    }
    $.ajax({
        type: "post",
        url: url,
        data: data,
        success: function (response) {
            if (response)
                createErrorWindow(response, "Сообщение");
            removeDialog();
            updateViewTable();
        },
        failure: function (response) {
            createErrorWindow(response.responseText);
        },
        error: function (response) {
            if (response.status == "403")
                response.responseText = "Нет доступа!";
            createErrorWindow(response.responseText);
        },
        complete: function () {
            if (button) {
                loadingButton(button);
            }
        }
    });
}


//проверка, был ли выбран хоть один элемент
function checkSelectDoc() {    
    if (getSelectDoc().length == 0) {
        createErrorWindow("Вы не выбрали ни одной записи для совершения операции!");
        return false;
    }
    return true;
}
function getSelectDoc() {
    return document.querySelectorAll("input[data-id]:checked");
}

//обновление таблицы
function updateViewTable() {
    
    $.ajax({
        type: "post",        
        url: "home/viewChildTable",
        data: { "quertyString": window.location.search },
        beforeSend: function () { document.querySelector("div[name=loadingTableView]").classList.add("d-flex"); },
        success: function (response) {
            document.querySelector("div[name=viewTable").innerHTML = response;
            //getColumnTable();
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
}

function reportForPeriod(typeReports) {
    var button = document.querySelector("[onclick=\"reportForPeriod('" + typeReports + "')\"]");
    loadingButton(button);
    $.ajax({
        type: "post",
        url: "home/reportForPeriod",
        data: { "typeReports": typeReports },
        success: function (response) {
            createErrorWindow("Отчет сформирован:<br/><a href='returnFile/?urlFile=" + response + "'>Просмотр</a>", "Сообщение");
        },
        failure: function (response) {
            createErrorWindow(response.responseText);
        },
        error: function (response) {
            if (response.status == "403")
                response.responseText = "Нет доступа!";
            createErrorWindow(response.responseText);
        },
        complete: function () { loadingButton(button); }
    });
}

