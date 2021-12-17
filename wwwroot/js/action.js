//добавляем роли к странице
function AddRoleAction(idAction) {
    var button = document.querySelector("[onclick=\"AddRoleAction('" + idAction + "')\"]");
    loadingButton(button);

    $.ajax({
        type: "post",
        url: "/admin/action/getRoleList",
        success: function (response) {
            var dialogWindow = createWindow("Добавить роли к действию");

            var arrayRole = Array.from(document.querySelector("#viewRoles" + idAction).querySelectorAll("b")).map(x => x.innerHTML);
            
            var innerStr = '<div class="form-group" id="viewRoleList">';
            for (var key in response) {
                innerStr += '<div class="form-check">';
                innerStr += '<label class="form-check-label"><input class="form-check-input"' + (arrayRole.includes(response[key]) ? "checked" : "") + ' type="checkbox" value="' + key + '"> ' + response[key] + '</label>';
                innerStr += '</div>';                
            }
            innerStr += '</div>';

            dialogWindow.querySelector(".modal-body").innerHTML = innerStr;

            var dialogSave = dialogWindow.querySelector("button[name=dialogSave]");

            dialogSave.addEventListener("click", x => {
                var roleList = new Array();
                document.querySelector("#viewRoleList").querySelectorAll("input[type=checkbox]:checked").forEach(e => {
                    roleList.push(e.getAttribute("value"));
                });
                var data = { "idAction": idAction, "roleList": roleList };
                var url = '/admin/action/AddRoleAction';

                loadingButton(dialogSave);

                $.ajax({
                    type: "post",
                    url: url,
                    data: data,
                    success: function (response) {
                        location.reload();
                    },
                    failure: function (response) {
                        createErrorWindow(response.responseText);
                    },
                    error: function (response) {
                        if (response.status == "403")
                            response.responseText = "Нет доступа!";
                        createErrorWindow(response.responseText);
                    },
                    complete: function () { loadingButton(dialogSave); }
                });
            });

            $(dialogWindow).modal('show');

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


//добавляем роли к контроллеру
function AddRoleController(idController) {
    var button = document.querySelector("[onclick=\"AddRoleController('" + idController + "')\"]");
    loadingButton(button);

    $.ajax({
        type: "post",
        url: "/admin/action/getRoleList",
        success: function (response) {
            var dialogWindow = createWindow("Добавить роли к контроллеру");
            var arrayRole = Array.from(document.querySelector("#viewRoles" + idController).querySelectorAll("b")).map(x => x.innerHTML);
            var innerStr = '<div class="form-group" id="viewRoleList">';
            for (var key in response) {
                innerStr += '<div class="form-check">';
                innerStr += '<label class="form-check-label"><input class="form-check-input"' + (arrayRole.includes(response[key]) ? "checked" : "") + ' type="checkbox" value="' + key + '"> ' + response[key] + '</label>';
                innerStr += '</div>';
            }
            innerStr += '</div>';

            dialogWindow.querySelector(".modal-body").innerHTML = innerStr;

            var dialogSave = dialogWindow.querySelector("button[name=dialogSave]");

            dialogSave.addEventListener("click", x => {
                var roleList = new Array();
                document.querySelector("#viewRoleList").querySelectorAll("input[type=checkbox]:checked").forEach(e => {
                    roleList.push(e.getAttribute("value"));
                });
                var data = { "idController": idController, "roleList": roleList };
                var url = '/admin/action/AddRoleController';

                loadingButton(dialogSave);

                $.ajax({
                    type: "post",
                    url: url,
                    data: data,
                    success: function (response) {
                        location.reload();
                    },
                    failure: function (response) {
                        createErrorWindow(response.responseText);
                    },
                    error: function (response) {
                        if (response.status == "403")
                            response.responseText = "Нет доступа!";
                        createErrorWindow(response.responseText);
                    },
                    complete: function () { loadingButton(dialogSave); }
                });
            });

            $(dialogWindow).modal('show');

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

//удаляем запись
function deleteAction(id, typeRole) {
    var button = createDeleteWindow();
    button.addEventListener("click", (e) => {
        loadingButton(button);
        var url = '/admin/action/Delete';
        $.ajax({
            type: "post",
            url: url,
            data: { "id": id, "typeRole": typeRole},
            success: function (response) {
                removeDialog("deleteDialog");
                createErrorWindow("Удалено", "Сообщение");
                location.reload();
            },
        });
    });
}