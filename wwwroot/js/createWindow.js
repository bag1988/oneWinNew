function loadingButton(button) {
    if (button) {
        if (button.getAttribute("disabled")) {
            button.innerHTML = button.getAttribute("data-val");
            button.removeAttribute("disabled");
            button.removeAttribute("data-val");
        }
        else {

            button.setAttribute("disabled", true);
            button.setAttribute("data-val", button.innerHTML);
            button.innerHTML = '<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Загрузка...';
        }
    }
}

function createErrorWindow(errrorMessage, nameTitle) {   
    if (!nameTitle)
        nameTitle = "<font style='color: red;'>Ошибка</font>";
    var dialogWindow = createWindow(nameTitle, "errorDialog");
    dialogWindow.querySelector(".modal-body").innerHTML = errrorMessage;
    $(dialogWindow).modal('show');
    return dialogWindow;
}

function createDeleteWindow() {    
    var dialogWindow = createWindow("Удаление", "deleteDialog");
    dialogWindow.querySelector(".modal-body").innerHTML = "Подтвердите удаление";
    $(dialogWindow).modal('show');
    var button = dialogWindow.querySelector("button[name=dialogSave]");
    button.innerHTML = "Подтверждаю удаление";

    return button;
}
function createMessageWindow(message) {
    var dialogWindow = createWindow("Сообщение", "messageDialog");
    dialogWindow.querySelector(".modal-body").innerHTML = message;
    $(dialogWindow).modal('show');
    var button = dialogWindow.querySelector("button[name=dialogSave]");

    return button;
}

function createWindow(nameTitle, nameDialog) {
   
    nameDialog = removeDialog(nameDialog);
    var innerHtml = '<div class="modal fade" name="' + nameDialog + '" tabindex="-1" role="dialog" aria-hidden="true"><div class="modal-dialog modal-dialog-centered" style="max-width:1000px;" role="document" >';
    innerHtml += '<div class="modal-content"><div class="modal-header"><h5 class="modal-title">' + nameTitle + '</h5><button type="button" class="close" data-dismiss="modal" aria-label="Close">';
    innerHtml += '<span aria-hidden="true">&times;</span></button></div><div class="modal-body"></div><div class="modal-footer" style="display:block;">';
    if (nameDialog != "errorDialog")
        innerHtml += '<button type="button" name="dialogSave" class="btn btn-primary">Сохранить</button>';
    innerHtml += '<button type="button" class="btn btn-secondary" data-dismiss="modal">Закрыть</button></div></div></div></div> ';
    $('body').append(innerHtml);
    return document.querySelector("div[name=" + nameDialog + "]");
}

function removeDialog(nameDialog) {
    if (!nameDialog)
        nameDialog = "dialogWindow";    
    if (document.querySelector("div[name=" + nameDialog + "]")) {
        $("div[name=" + nameDialog + "]").modal('hide');
        document.body.removeChild(document.querySelector("div[name=" + nameDialog + "]"));
    }
    return nameDialog;
}

function createInput(nameInput, typeInput, valueInput) {    
    var newInput = document.createElement("input");
    newInput.setAttribute("class", "form-control");
    if (nameInput) {
        newInput.setAttribute("name", nameInput);
    }    
    if (typeInput == "date") {
        newInput.setAttribute("type", "date");
        if (valueInput) {
            newInput.value = new Date().toISOString().substring(0, 10);
        }
    }   
    if (typeInput == "text") {
        newInput.setAttribute("type", "text");
        if (valueInput) {
            newInput.value = valueInput;
        }
    }
   
    return newInput;
}

function createButton(nameButton, textButton, typeButton) {
    var newButton = document.createElement("button");
    newButton.setAttribute("class", "btn btn-success");
    if (nameButton) {
        newButton.setAttribute("name", nameButton);
    }
    if (textButton) {
        newButton.innerText = textButton;
    }
    if (typeButton) {
        newButton.setAttribute("type", typeButton);
    }
    else
        newButton.setAttribute("type", "button");       
    return newButton;
}

function createSelect(nameSelect, valueSelect) {
    var newSelect = document.createElement("select");
    newSelect.setAttribute("class", "form-control");
    if (nameSelect) {
        newSelect.setAttribute("name", nameSelect);
    }        
    if (valueSelect) {       
        valueSelect.split(",").forEach(x => {
            var newOption = document.createElement("option");
            newOption.innerText = x;
            newSelect.appendChild(newOption);
        });
    }
    return newSelect;
}
function createLabel(textLabel) {
    var newLabel = document.createElement("label");
    newLabel.setAttribute("class", "col-form-label col-md-3");
    newLabel.innerText = textLabel;
    return newLabel;
}
function createDivRow() {
    var newDiv = document.createElement("div");
    newDiv.setAttribute("class", "form-group row");
    return newDiv;
}
function createDivCol() {
    var newDivCol = document.createElement("div");
    newDivCol.setAttribute("class", "col-md-9");
    return newDivCol;
}
function labelAndInput(textLabel, nameInput, typeInput, valueInput) {
    var newDiv = createDivRow();    
    var newDivCol = createDivCol();
    newDivCol.appendChild(createInput(nameInput, typeInput, valueInput));        
    newDiv.appendChild(createLabel(textLabel));
    newDiv.appendChild(newDivCol);
    return newDiv;
}
function labelAndSelect(textLabel, nameSelect, valueSelect) {
    var newDiv = createDivRow();
    var newDivCol = createDivCol();
    newDivCol.appendChild(createSelect(nameSelect, valueSelect));
    newDiv.appendChild(createLabel(textLabel));
    newDiv.appendChild(newDivCol);
    return newDiv;
}