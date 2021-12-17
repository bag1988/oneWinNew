function viewAdminProc() {
    var select = document.querySelector("select[name=TypeReg]");
    var legalStr = "false";
    if (select.value == 2 || select.value == 5 || select.value == 6 || select.value == 8)
        legalStr = "true";
    $.ajax({
        type: "post",
        url: "/create/viewAdminProc",
        data: { "legalStr": legalStr },
        success: function (response) {
            var dialogWindow = createWindow("Выбор административной процедуры");
            dialogWindow.querySelector(".modal-body").innerHTML = response;
            showAdminProc();
            dialogWindow.querySelector("button[name=dialogSave]").setAttribute("style", "display:none;");
            
            $(dialogWindow).modal('show');

            var arrayProc = dialogWindow.querySelector("#adminProcList").querySelectorAll("input[type=hidden]");

            arrayProc.forEach(x => {
                x.parentNode.setAttribute("style", "cursor:pointer;");
                x.parentNode.addEventListener("click", () => {
                    var regId = x.getAttribute("name");
                    document.querySelector("input[name=RegID]").value = regId;
                    removeDialog();                   
                    getDocForProc(regId);
                    getZaprForProc(regId);
                    $.ajax({
                        type: "post",
                        url: "/create/getAdminProcInfo",
                        data: { "RegId": regId },
                        success: function (response) {
                            document.querySelector("div[name=getAdminProc]").innerHTML = response;
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
                });
            });

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
//загружаем список документов для процедуры
function getDocForProc(regId) {
    $.ajax({
        type: "post",
        url: "/create/getDocForProc",
        data: {  "regId": regId  },
        success: function (response) {            
            document.querySelector("div[name=NameDocDopList]").innerHTML = response;
        },
        failure: function (response) {
            document.querySelector("div[name=NameDocDopList]").innerHTML =response.responseText;
        },
        error: function (response) {
            document.querySelector("div[name=NameDocDopList]").innerHTML =response.responseText;
        }
    });
}

//загружаем список документов для запросов
function getZaprForProc(regId) {
    $.ajax({
        type: "post",
        url: "/create/getZaprForProc",
        data: {"regId": regId },
        success: function (response) {
            document.querySelector("div[name=NameZpDopList]").innerHTML = response;
        },
        failure: function (response) {
            document.querySelector("div[name=NameZpDopList]").innerHTML =response.responseText;
        },
        error: function (response) {
            document.querySelector("div[name=NameZpDopList]").innerHTML =response.responseText;
        }
    });
}

//Сформировать документы
function generateDocument() {
    
    var button = document.querySelector("span[onclick=\"generateDocument()\"]");
    loadingButton(button);
    var regId = document.querySelector("input[name=RegistrationID]").value;
    $.ajax({
        type: "post",
        url: "/create/generateDocument",
        data: { "regId": regId },
        success: function (response) {
            location.reload();
            createErrorWindow("Документы сформированны! Если документы не были сформированны до, обновите страницу!", "Сообщение");
        },
        failure: function (response) {
            createErrorWindow(response.responseText);
        },
        error: function (response) {
            if (response.status == "403")
                response.responseText = "Нет доступа!";
            createErrorWindow(response.responseText);
        },
        complete: function()
        {
            loadingButton(button);
        }
    });
}

//Сохранить файлы
function saveAttachFile() {
    var button = document.querySelector("span[onclick=\"saveAttachFile()\"]");
    var regId = document.querySelector("input[name=RegistrationID]").value;   
    var fileList = document.querySelector("input[name=fileUrl]").files;
    if (fileList.length > 0) {
        loadingButton(button);
        var formData = new FormData();
        formData.append('registration', regId);
        for (var i = 0; i < fileList.length; i++) {
            formData.append('fileUrl', fileList[i]);
        }
        console.log(formData);
        $.ajax({
            type: "post",
            url: "/create/saveAttachFile",
            processData: false,
            contentType: false,
            data: formData,
            success: function (response) {
                document.querySelector("div[role=alert]").removeAttribute("style");
                setTimeout(() => document.querySelector("div[role=alert]").setAttribute("style", "display:none"), 2000);
                loadAttachFile();
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
                loadingButton(button);
            }
        });
    }
    else {
        createErrorWindow("Выберите файлы!");
    }
}

//Загрузить файлы
function loadAttachFile() {
    var regId = document.querySelector("input[name=RegistrationID]").value;
    $.ajax({
        type: "post",
        url: "/create/loadAttachFile",
        data: { "RegId": regId },
        success: function (response) {
            document.querySelector("div[name=loadAttachFile]").innerHTML = response;
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

//Удалить файлы
function deleteAttachFile(idFile) {    
    $.ajax({
        type: "post",
        url: "/create/deleteAttachFile",
        data: { "idFile": idFile },
        success: function (response) {
            loadAttachFile();
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

//удаляем состав семьи
function deleteFamily(idFamily) {
    var button = createDeleteWindow();
    button.addEventListener("click", (e) => {
        loadingButton(button);
        var url = '/family/deleteFamily';
        $.ajax({
            type: "post",
            url: url,
            data: { "idFamily": idFamily },
            success: function (response) {
                removeDialog("deleteDialog");
                createErrorWindow("Удалено", "Сообщение");
                loadFamily();
            },
        });
    }, { once: true });    
}

//удаляем состав семьи
function deleteTransfer(idMsg) {    
    var button = createDeleteWindow();
    button.addEventListener("click", (e) => {
        loadingButton(button);
        var url = '/family/deleteTransfer';
        $.ajax({
            type: "post",
            url: url,
            data: { "idMsg": idMsg },
            success: function (response) {
                removeDialog("deleteDialog");
                createErrorWindow("Удалено", "Сообщение");
                loadtransfer();
            },
        });
    }, { once: true });
}

//Загрузить состав семьи
function loadFamily() {
    var regId;
    if (document.querySelector("input[name=Registration_Id]"))
        regId = document.querySelector("input[name=Registration_Id]").value;
    if (document.querySelector("input[name=RegistrationID]"))
        regId = document.querySelector("input[name=RegistrationID]").value;
    $.ajax({
        type: "post",
        url: "/family/loadFamily",
        data: { "regId": regId },
        success: function (response) {
            document.querySelector("div[name=loadFamily]").innerHTML = response;            
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

//Copy transfer
function copyTransfer(idMsg) {
    var button = createMessageWindow("Копировать?");
    button.innerHTML = "Копировать";
    button.addEventListener("click", (e) => {
        loadingButton(button);
        $.ajax({
            type: "post",
            url: "/family/copyTransfer",
            data: { "idMsg": idMsg },
            success: function (response) {
                removeDialog("messageDialog");
                createErrorWindow("Скопировано", "Сообщение");
                loadtransfer();
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
    }, { once: true });
}

//Send transfer
function sendOneRequest(idMsg) {
    var button = document.querySelector("span[onclick=\"sendOneRequest('" + idMsg + "')\"]");
    loadingButton(button);
    $.ajax({
        type: "post",
        url: "/family/sendOneRequest",
        data: { "idMsg": idMsg },
        success: function (response) {
            var res = JSON.parse(response);
            if (res.file)
                window.open(res.file, '_blank');
            else
                createErrorWindow(res.message, "Сообщение");
            loadtransfer();
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

//Загрузить запросы
function loadtransfer() {
    var regId = document.querySelector("input[name=idTransfer]").value;    
    $.ajax({
        type: "post",
        url: "/family/loadTransfer",
        data: { "regId": regId },
        success: function (response) {
            document.querySelector("div[name=loadtransfer]").innerHTML = response;
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

//перенос в запросы
function TransferInRequest() {
    var button = document.querySelector("button[onclick=\"TransferInRequest()\"]");
    loadingButton(button);
    var regId;
    if (document.querySelector("input[name=Registration_Id]"))
        regId = document.querySelector("input[name=Registration_Id]").value;
    if (document.querySelector("input[name=RegistrationID]"))
        regId = document.querySelector("input[name=RegistrationID]").value;
    $.ajax({
        type: "post",
        url: "/family/TransferInRequest",
        data: { "regId": regId },
        success: function (response) {
            createErrorWindow("Отправлено", "Сообщение");
        },
        failure: function (response) {
            createErrorWindow(response.responseText);
        },
        error: function (response) {
            if (response.status == "403")
                response.responseText = "Нет доступа!";
            createErrorWindow(response.responseText);
        },
        complete: function()
        {
            loadingButton(button);
        }
    });

}


function maskPhone(input) {
    function mask(event) {
        if (event.keyCode == 8)
            return;
        if (this.value == "") {
            this.value = "+375-(__)-___-__-__";
            this.setSelectionRange(6, 6);
        }

        if (!/\d/.test(event.key) && event.key)
            return;


        var pos = this.selectionStart;
        var oldValue = this.value.replace("+375", "");
        oldValue = oldValue.replace(/\D/g, "");

        //console.log(this.value + " " + oldValue);

        if (event.type == "blur" && oldValue == "") {
            this.value = "";
            return;
        }
        var mats = oldValue.match(/^(\d{1,2})?(\d{1,3})?(\d{1,2})?(\d{1,2})?/);
        mats[1] = mats[1] ? mats[1] + "_".repeat((2 - mats[1].length)) : "_".repeat(2);
        mats[2] = mats[2] ? mats[2] + "_".repeat((3 - mats[2].length)) : "_".repeat(3);
        mats[3] = mats[3] ? mats[3] + "_".repeat((2 - mats[3].length)) : "_".repeat(2);
        mats[4] = mats[4] ? mats[4] + "_".repeat((2 - mats[4].length)) : "_".repeat(2);


        var newValue = "+375-(" + mats[1] + ")-" + mats[2] + "-" + mats[3] + "-" + mats[4];

        this.value = newValue;

        while (this.value[pos] == ")" || this.value[pos] == "-") {
            pos += 1;
        }
        this.setSelectionRange(pos, pos);


    }
    input.addEventListener("input", mask, false);
    input.addEventListener("focus", mask, false);
    input.addEventListener("blur", mask, false);
    input.addEventListener("keyup", mask, false)

}


//формируем запросы
function generateTransfer(calculate) {
    var s = "";
    if (calculate)
        s = "'true'";
    var button = document.querySelector("button[onclick=\"generateTransfer(" + s + ")\"]");
    loadingButton(button);
    var regId = document.querySelector("input[name=idTransfer]").value;
    var fullFamily = document.querySelector("input[name=fullFamily]").checked;
    $.ajax({
        type: "post",
        url: "/family/generateTransfer",
        data: { "regId": regId, "fullFamily": fullFamily, "calculate": calculate  },
        success: function (response) {
            createErrorWindow("Запросы сформированы", "Сообщение");
            loadtransfer();
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
            loadingButton(button);
        }
    });

}

function loginEsiful() {
    var url = '/create/loginEsiful/';
    var button = document.querySelector("[onclick=\"loginEsiful()\"]");
    loadingButton(button);
    $.ajax({
        type: "post",
        url: url,
        success: function (response) {
            var res = JSON.parse(response);            
            select_auth(res.signed, res.codeVerifer, res.enveloped);
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

function select_auth(signed, codeVerifier, urlAuth) {
    var url = "http://127.0.0.1:8084/select_auth";
    var button = document.querySelector("[onclick=\"loginEsiful()\"]");
    loadingButton(button);
    $.ajax({
        type: "post",
        data: JSON.stringify({
            "data": signed
        }),
        url: url,
        contentType: "application/json",
        success: function (response) {           
            $.ajax({
                type: "post",
                url: "/create/addSession",
                data: { "codeVerifier": codeVerifier},               
                success: function (response) {                   
                    window.location.href=urlAuth;
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

function getKeyAndCOK() {
    var url = "/create/callEsiful";
    $.ajax({
        type: "post",
        data: {
            "urlCall": "http://192.168.209.205:9999/api/v1/gencertreq", "dataCall": JSON.stringify({
                "key_param": {
                    "security_level": 128,
                    "iteration_count": 10000,
                    "users_num": 5,
                    "treshold_num": 3,
                    "passwords": [
                        "12345678",
                        "87654321",
                        "12345678",
                        "87654321",
                        "12345678"
                    ]
                },
                "cert_req_param": {
                    "type": "service_and_tls",
                    "cert_info": {
                        "common_name": "*.iis.corp.mgaon.by",
                        "description": "ПК \"Одно окно\"",
                        "organization": "КУП Минское городское агентство обслуживания населения",
                        "state": "Минский район",
                        "locality": "Минск",
                        "street": "ул. Максима Танка, 36/1, 220004",
                        "email": "info@mgaon.by",
                        "subject_alt_name": ["*.iis.corp.mgaon.by", "www.iis.corp.mgaon.by"]
                    }
                },
                "save_to_file": true
            }) },
        url: url,
        success: function (response) {
            console.log(response);

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




//BAUTH
function bauth() { 
    loadingButton(document.querySelector("[onclick=\"bauth()\"]"));
    terminal_proxy_bauth_app_prefetch();
   

}
function terminal_proxy_bauth_app_prefetch() {
    var url = "http://127.0.0.1:8084/api/v1/terminal_proxy_bauth_app_prefetch";
    $.ajax({
        type: "post",
        data: JSON.stringify({
            "stb": false
        }),
        url: url,
        contentType: "application/json",
        success: function (response) {
            var so_certificate = response.so_certificate;
            var cert_id = response.cert_id;
            var err = response.err;
            if (err=="0")
                bauth_init(so_certificate);
            else {
                createErrorWindow("Ошибка " + err);
                loadingButton(document.querySelector("[onclick=\"bauth()\"]"));
            }
                
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

var hreq;
function bauth_init(so_certificate) {

    var url = "/create/callEsiful";
    $.ajax({
        type: "post",
        data: {"urlCall": "http://192.168.209.205:48777/api/v1/bauth_init", "dataCall": JSON.stringify({ "so_certificate": so_certificate })},
        url: url,
        success: function (response) {
            var res = JSON.parse(response);
            hreq = res.hreq;
            var terminal_certificate = res.terminal_certificate;
            var cmd_to_card = res.cmd_to_card;
            //read_dg_init(hreq);
            terminal_proxy_bauth_init(terminal_certificate, cmd_to_card);

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

function terminal_proxy_bauth_init(terminal_certificate, cmd_to_card) {
    var url = "http://127.0.0.1:8084/api/v1/terminal_proxy_bauth_init";
    $.ajax({
        type: "post",
        data: JSON.stringify({
            "terminal_certificate": terminal_certificate,
            "cmd_to_card": cmd_to_card,
            "is_bilateral": true
        }),
        url: url,
        contentType: "application/json",
        success: function (response) {
            var card_response = response.card_response;
            bauth_process(card_response);
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

function bauth_process(card_response) {

    var url = "/create/callEsiful";
    $.ajax({
        type: "post",
        data: {
            "urlCall": "http://192.168.209.205:48777/api/v1/bauth_process", "dataCall": JSON.stringify({"hreq": hreq, "card_response": card_response}) },
        url: url,
        success: function (response) {

            var header_cmd_to_card = JSON.parse(response).header_cmd_to_card;
            var cmd_to_card = JSON.parse(response).cmd_to_card;
            var is_bauth_established = JSON.parse(response).is_bauth_established;

            if (!is_bauth_established) {
                terminal_proxy_bauth(header_cmd_to_card, cmd_to_card, false);
                //bauth_process(card_response);               
            }
            else {  
                read_dg_init();
            }

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

function terminal_proxy_bauth(header_cmd_to_card, cmd_to_card, nextBool) {
    var url = "http://127.0.0.1:8084/api/v1/terminal_proxy_bauth";
    $.ajax({
        type: "post",
        data: JSON.stringify({
            "header_cmd_to_card": header_cmd_to_card,
            "cmd_to_card": cmd_to_card
        }),
        url: url,
        contentType: "application/json",
        success: function (response) {
            var card_response = response.card_response;

            if (nextBool)
                read_dg_init();
            else
                bauth_process(card_response);
            //read_dg(card_response);
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


function read_dg_init() {

    var url = "/create/callEsiful";
    $.ajax({
        type: "post",
        data: {
            "urlCall": "http://192.168.209.205:48777/api/v1/read_dg_init", "dataCall": JSON.stringify({ "hreq": hreq, "data_groups_to_read": ["dg1", "dg2", "dg3", "dg4", "dg5"] })
        },
        url: url,
        success: function (response) {
            var header_cmd_to_card = JSON.parse(response).header_cmd_to_card;
            var cmd_to_card = JSON.parse(response).cmd_to_card;
            terminal_proxy_command(header_cmd_to_card, cmd_to_card, false);
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

function terminal_proxy_command(header_cmd_to_card, cmd_to_card, request) {
    var url = "http://127.0.0.1:8084/api/v1/terminal_proxy_command";
    $.ajax({
        type: "post",
        data: JSON.stringify({
            "header_cmd_to_card": header_cmd_to_card,
            "cmd_to_card": cmd_to_card
        }),
        url: url,
        contentType: "application/json",
        success: function (response) {
            var card_response = response.card_response;
            if (request)
                request_dg();
            else
                read_dg(card_response);
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

function read_dg(card_response) {

    var url = "/create/callEsiful";
    $.ajax({
        type: "post",
        data: {
            "urlCall": "http://192.168.209.205:48777/api/v1/read_dg", "dataCall": JSON.stringify({ "hreq": hreq, "card_response": card_response })
        },
        url: url,
        success: function (response) {

            var header_cmd_to_card = JSON.parse(response).header_cmd_to_card;
            var cmd_to_card = JSON.parse(response).cmd_to_card;
            var is_last_dg_readed = JSON.parse(response).is_last_dg_readed;

            if (!is_last_dg_readed) {
                terminal_proxy_command(header_cmd_to_card, cmd_to_card, false);
                            
            }
            else {
                request_dg();
            }
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
//terminal_proxy_command выполнять после read_dg
function request_dg() {
    var url = "/create/callEsiful";
    $.ajax({
        type: "post",
        data: {
            "urlCall": "http://192.168.209.205:48777/api/v1/request_dg", "dataCall": JSON.stringify({ "hreq": hreq })
        },
        url: url,
        success: function (response) {            
            var personal_data = JSON.parse(response).personal_data;
            var BE_Family_name = personal_data.BE_Family_name;
            var BE_Given_name = personal_data.BE_Given_name;
            var BE_Middle_name = personal_data.BE_Middle_name;
            var RU_Family_name = personal_data.RU_Family_name;
            var RU_Given_name = personal_data.RU_Given_name;
            var RU_Middle_name = personal_data.RU_Middle_name;
            var LA_Family_name = personal_data.LA_Family_name;
            var LA_Given_name = personal_data.LA_Given_name;

            for (var p in personal_data) {
                console.log(p);
                switch (p) {
                    case "BE:_Family_name": BE_Family_name = personal_data[p]; break;
                    case "BE:_Given_name": BE_Given_name = personal_data[p]; break;
                    case "BE:_Middle_name": BE_Middle_name = personal_data[p]; break;
                    case "RU:_Family_name": RU_Family_name = personal_data[p]; break;
                    case "RU:_Given_name": RU_Given_name = personal_data[p]; break;
                    case "RU:_Middle_name": RU_Middle_name = personal_data[p]; break;
                    case "LA:_Family_name": LA_Family_name = personal_data[p]; break;
                    case "LA:_Given_name": LA_Given_name = personal_data[p]; break;
                }
            }
            var BE_Place_of_birth = personal_data.BE_Place_of_birth; //"РЭСПУБЛІКА БЕЛАРУСЬ, Г МІНСК"
            var Citizenship = personal_data.Citizenship; //"BLR" гражданство
            var Date_of_expiry = personal_data.Date_of_expiry; // "20310907" действует до
            var Date_of_issuance = personal_data.Date_of_issuance; //"20210907" выдан
            var ID = personal_data.ID; //"4211092A028PB5"
            var Issuance_board = personal_data.Issuance_board; //"704"
            var Issuing_State = personal_data.Issuing_State; //"BLR"
            var RU_Place_of_birth = personal_data.RU_Place_of_birth; //"РЕСПУБЛИКА БЕЛАРУСЬ, Г МИНСК"
            var Serial_Number = personal_data.Serial_Number; //"BY0708986"
            var Sex = personal_data.Sex; //"F"
            var birth_date = personal_data.birth_date; //"19921021"

            getOrgIssueKod(Issuance_board);

            document.querySelector("#LName").value = RU_Family_name;
            document.querySelector("#FName").value = RU_Given_name;
            document.querySelector("#MName").value = RU_Middle_name;
            document.querySelector("#PassportNo").value = Serial_Number;
            document.querySelector("#PersonalNo").value = ID;
            document.querySelector("#PassIssuerDate").value = Date_of_issuance.replace(/^(\d{4})(\d{2})(\d{2})/, '$1-$2-$3');;

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
            loadingButton(document.querySelector("[onclick=\"bauth()\"]"));
        }
    });
}

function bauth_logout(hreq) {
    var url = "/create/callEsiful";
    $.ajax({
        type: "post",
        data: {
            "urlCall": "http://192.168.209.205:48777/api/v1/bauth_logout", "dataCall": JSON.stringify({ "hreq": hreq })
        },
        url: url,
        success: function (response) {
           
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

//орган выдавший документ по коду
function getOrgIssueKod(kod) {
    var input = document.querySelector("#PassIssuer");

    $.ajax({
        type: "post",
        url: "/OrgIssues/getOrgIssueKod",
        data: { "kod": kod },
        success: function (response) {
            input.value = response.name;
        }
    });
}

//подсказка при вводе организации для запроса
function getOrgZaprName() {
    var input = document.querySelector("#orgsZapr_Name");
    var datalist = document.createElement("datalist");
    if (!input.parentNode.querySelector("datalist")) {
        input.parentNode.append(datalist);
    }
    else
        datalist = input.parentNode.querySelector("datalist");
    input.setAttribute("list", "OrgZaprList");
    input.addEventListener("change", () => { getOrgZapr(); });
    input.addEventListener("keyup", () => { getOrgZapr(); });
    datalist.setAttribute("id", "OrgZaprList");

    input.addEventListener("keyup", () => {
        $.ajax({
            type: "post",
            url: "/OrgZapr/getOrgZaprName",
            data: { "nameOrg": document.querySelector("#orgsZapr_Name").value },
            success: function (response) {
                datalist.innerHTML = "";
                response.forEach(i => {
                    datalist.innerHTML += "<option data-value=" + i.id + ">" + i.name + "</option >";
                });
            }
        });
    });
}