$(document).ready(function () {
    $('[data-toggle="popover"]').popover({
        placement: 'top',
        html: true,
        sanitize: false
    });
    $('[data-toggle="tooltip"]').tooltip(
        { placement: "right" });
    if (document.querySelector("#adminProcList")) showAdminProc();
    if (document.querySelector("#HeadsID")) {
        $("#HeadsID").on('change keyup select', function () { getSectionsList(); });
        if (document.querySelector("#DocRegList"))
            $("#SectionID").on('change keyup select', function () { getAdminProcListForFiltr(); });
    }
    if (document.querySelector("#Curators_Area_Id")) {
        $("#Curators_Area_Id").on('change keyup select', function () { getCuratorList(); });
    }
    if (document.querySelector("#Department_Curators_Area_Id")) {
        $("#Department_Curators_Area_Id").on('change keyup select', function () { getCuratorList("Department_"); });

        if (document.querySelector("#Department_Curator_Id")) {
            $('#Department_Curator_Id').on('change keyup select', function () { getDepartmentList(); });
        }

    }
    if (document.querySelector("#CuratorFiltr")) {
        $("#CuratorFiltr").on('change keyup select', function () { getDepartmentList(); });

        if (document.querySelector("#DepartamentFiltr")) {
            $('#DepartamentFiltr').on('change keyup select', function () { getPerformerList(); });
        }
    }
    if (document.querySelector("#IdDep")) {
        $("#IdDep").on('change keyup select', function () { getDocForDeptList(); });
        $('#IdDoc').on('change keyup select', function () { getComment(); });
        getComment();
    }
    if (document.querySelector(".viewDoc")) viewProcDoc();
    if (document.querySelector("#viewCurator")) getListCurator();
    if (document.querySelector("#monthCalendar")) {
        $("#monthCalendar").on('change keyup select', function () { getCurrentDate(); });
        $("#yearCalendar").on('change keyup select', function () { getCurrentDate(); });
        getCurrentDate();
    }
    if (document.querySelector(".guideList")) selectGuideList();
    var docHeight = document.body.offsetHeight;
    if (document.querySelector(".nav-green")) {
        window.addEventListener('scroll', function () {
            if (window.scrollY > 70) {
                document.querySelector('.nav-green').classList.add('fixed-top');
                navbar_height = document.querySelector('.navbar').offsetHeight;
                document.querySelector("main[role=main]").style.paddingTop = navbar_height + 'px';
            } else {
                document.querySelector('.nav-green').classList.remove('fixed-top');
                document.querySelector("main[role=main]").removeAttribute("style");
            }
            if (document.querySelector(".bottomFixed")) {
                if ((window.pageYOffset + window.innerHeight - 60) >= (docHeight - document.querySelector("footer").offsetHeight))
                    document.querySelector(".bottomFixed").setAttribute("style", "position: relative;");
                else
                    document.querySelector(".bottomFixed").removeAttribute("style");
            }
        });
    }

    if (document.querySelector("select[name=TypeReg]") && document.querySelector("input[name=OrgName]")) {
        var select = document.querySelector("select[name=TypeReg]");
        select.addEventListener("change", () => {
            if (select.value == 2 || select.value == 5 || select.value == 6 || select.value == 8)
                document.querySelector("input[name=OrgName]").parentNode.parentNode.removeAttribute("style");
            else
                document.querySelector("input[name=OrgName]").parentNode.parentNode.setAttribute("style", "display:none;");
        });
        if (select.value == 2 || select.value == 5 || select.value == 6 || select.value == 8)
            document.querySelector("input[name=OrgName]").parentNode.parentNode.removeAttribute("style");
        else
            document.querySelector("input[name=OrgName]").parentNode.parentNode.setAttribute("style", "display:none;");
    }

    //подсвечиваем поля со значением
    if (document.querySelector("#DocNoFilter")) {
        var tr = document.querySelector("#DocNoFilter").closest("tr");
        tr.querySelectorAll("input").forEach(x => {
            if (x.getAttribute("type") != "checkbox") {
                if (x.value) {
                    x.parentNode.parentNode.classList.add("alert-th");
                }
                else
                    x.parentNode.parentNode.classList.remove("alert-th");
            }
        });

        document.querySelector(".card-body").querySelectorAll("input").forEach(x => {
            if (x.getAttribute("type") != "submit") {
                if (x.value) {
                    x.parentNode.classList.add("alert-th");
                }
                else
                    x.parentNode.classList.remove("alert-th");
            }
        });

        document.querySelector(".card-body").querySelectorAll("select").forEach(x => {
            if (x.value) {
                x.parentNode.classList.add("alert-th");
            }
            else
                x.parentNode.classList.remove("alert-th");
        });

    }


    if (document.querySelector("#selectAllCheck")) {
        document.querySelector("#selectAllCheck").addEventListener("change", () => {
            selectAllCheck();
        });
    }

    if (document.querySelector(".navbar-brand") && document.querySelector("[name=otdel]")) {
        document.querySelector(".navbar-brand").innerHTML = document.querySelector("[name=otdel]").innerHTML;
    }

    if (document.querySelector("#Address")) {
        getAddressName();
    }
    if (document.querySelector("#PassIssuer") || document.querySelector("#DocIssuer")) {
        getOrgIssueName();
    }
    if (document.querySelector("#MobPhone")) {
        maskPhone(document.querySelector("#MobPhone"));
    }
    if (document.querySelector("[accesskey]")) {
        $(window).keyup(function (event) {
            if (event.target.getAttribute("type") != "text" && event.target.getAttribute("type") != "date" && event.target.getAttribute("type") != "number") {
                if (document.querySelector("[accesskey=\"" + (event.key) + "\"]"))
                    document.querySelector("[accesskey=\"" + (event.key) + "\"]").click();
            }
        });
        document.querySelectorAll("[accesskey]").forEach(i => {
            if (i.getAttribute("accesskey") != "")
                i.innerHTML = i.innerHTML + " (" + i.getAttribute("accesskey") + ")";
        });

    }
    hiddenAction();

    if (document.querySelector("#OrganisationID") && document.querySelector("#Sent")) {
        getOrgZaprName();
        //document.querySelector("#OrganisationID").addEventListener("change", () => {
        //    getOrgZapr();
        //});
        getOrgZapr();
    }
    if (document.querySelector("input[name=dateRequset]") && document.querySelector("select[name=userName]")) {
        document.querySelector("input[name=dateRequset]").addEventListener("change", () => {
            log.loadSearchField();
        });
    }
});

//получаем организацию для запроса
function getOrgZapr() {
    var nameOrg = document.querySelector("#orgsZapr_Name").value;
    var list = document.querySelector("#OrgZaprList");
    if (nameOrg) {
        var idOrg;
        if (list) {
            list.querySelectorAll("option").forEach((i) => {
                if (i.innerText == nameOrg) {
                    idOrg = i.getAttribute("data-value");
                    document.querySelector("#OrganisationID").value = idOrg;
                }
            });
        }
        idOrg = document.querySelector("#OrganisationID").value;
        
        if (idOrg) {
            $.ajax({
                type: "post",
                url: "/orgzapr/getOrgZapr",
                data: { "idOrg": idOrg },
                success: function (response) {
                    if (response) {
                        
                        document.querySelector("label[name=emailOrg]").innerHTML = response.e_mail || "нет email";
                        document.querySelector("label[name=adresOrg]").innerHTML = response.postAddress || "нет адреса";
                    }
                }
            });
        }
    }
}

//скрываем недопустимые ссылки для пользователя
function hiddenAction() {
    $.ajax({
        type: "post",
        url: "/admin/action/getPageUser",
        success: function (response) {
            if (!response.roleName) {
                var arrayAction = document.querySelectorAll("a:not(table[name=viewTableHome] a)");
                arrayAction.forEach(x => {
                    if (x.getAttribute("href") != null && x.getAttribute("href") != "#" && x.getAttribute("href") != "/Identity/Account/Logout") {
                        var mat = x.getAttribute("href").match(/^(?:\/)?(admin|identity)?\/(\w+)(?:\/)?(\w+)?/);

                        if (mat) {
                            if (!mat[3]) {
                                mat[0] = mat[0] + "/index";
                                mat[3] = "index";
                            }

                            if (!response.url.includes(("/" + mat[2].toLowerCase() + "/" + mat[3].toLowerCase()))) {

                                x.setAttribute("style", "display:none;");
                            }
                        }
                    }
                });

                var arraySpan = document.querySelectorAll("span[onclick]");
                arraySpan.forEach(x => {
                    if (x.getAttribute("onclick") != null) {
                        var mat = x.getAttribute("onclick").match(/^(\w+)/);
                        if (mat) {
                            if (!response.addressAction.includes(mat[1].toLowerCase()))
                                x.setAttribute("style", "display:none;");
                        }
                    }
                });

                hiddenDropdownList();

            }

        }
    });
}

function hiddenDropdownList() {
    document.querySelectorAll("li.dropdown").forEach(x => {
        var b = true;
        x.querySelectorAll("a.dropdown-item").forEach(a => {
            if (!a.getAttribute("style"))
                b = false;
        });
        if (b)
            x.setAttribute("style", "display:none;");
    });
}

//подсказка список органов выдавших документы
function getOrgIssueName() {
    var input = document.querySelector("#PassIssuer") || document.querySelector("#DocIssuer");
    var datalist = document.createElement("datalist");
    if (!input.parentNode.querySelector("datalist")) {
        input.parentNode.append(datalist);
    }
    else
        datalist = input.parentNode.querySelector("datalist");
    input.setAttribute("list", "PassIssuerList");
    datalist.setAttribute("id", "PassIssuerList");

    input.addEventListener("keyup", (e) => {
        $.ajax({
            type: "post",
            url: "/OrgIssues/getOrgIssue",
            data: { "nameOrgIssue": e.target.value },
            success: function (response) {
                datalist.innerHTML = "";
                response.forEach(i => {
                    datalist.innerHTML += '<option>' + i + '</option > ';
                });
            }
        });
    });
}

//подсказки при вводе адреса
function getAddressName() {
    var input = document.querySelector("#Address");
    var datalist = document.createElement("datalist");
    if (!input.parentNode.querySelector("datalist")) {
        input.parentNode.append(datalist);
    }
    else
        datalist = input.parentNode.querySelector("datalist");
    input.setAttribute("list", "AddressList");
    input.addEventListener("change", () => { getHomeForAddress(); });
    datalist.setAttribute("id", "AddressList");

    input.addEventListener("keyup", () => {
        $.ajax({
            type: "post",
            url: "/address/getAddress",
            data: { "nameAddress": document.querySelector("#Address").value },
            success: function (response) {
                datalist.innerHTML = "";
                response.forEach(i => {
                    datalist.innerHTML += '<option>' + i + '</option > ';
                });
            }
        });
    });
}

//получаем список домов
function getHomeForAddress() {
    if (document.querySelector("#Home")) {
        var input = document.querySelector("#Home");
        input.value = "";
        var datalist = document.createElement("datalist");
        if (!input.parentNode.querySelector("datalist")) {
            input.parentNode.append(datalist);
        }
        else
            datalist = input.parentNode.querySelector("datalist");
        input.setAttribute("list", "nDomList");
        input.addEventListener("change", () => { getFlatForAddress(); });
        datalist.setAttribute("id", "nDomList");
        $.ajax({
            type: "post",
            url: "/address/getHomeForAddress",
            data: { "nameAddress": document.querySelector("#Address").value },
            success: function (response) {
                datalist.innerHTML = "";
                response.forEach(i => {
                    datalist.innerHTML += '<option>' + i + '</option > ';
                });
            }
        });
    }
}

//получаем список квартир
function getFlatForAddress() {
    if (document.querySelector("#Flat")) {
        var input = document.querySelector("#Flat");
        input.value = "";
        var datalist = document.createElement("datalist");
        if (!input.parentNode.querySelector("datalist")) {
            input.parentNode.append(datalist);
        }
        else
            datalist = input.parentNode.querySelector("datalist");
        input.setAttribute("list", "nFlatList");
        datalist.setAttribute("id", "nFlatList");
        $.ajax({
            type: "post",
            url: "/address/getFlatForAddress",
            data: { "nameAddress": document.querySelector("#Address").value, "nDom": document.querySelector("#Home").value },
            success: function (response) {
                datalist.innerHTML = "";
                response.forEach(i => {
                    datalist.innerHTML += '<option>' + i + '</option > ';
                });
            }
        });

    }
}

//загружаем список раздела для страницы административные процедуры
function getSectionsList() {
    var url = '/adminproc/sectionsList';
    var idHead = document.getElementById("HeadsID").value;
    $.ajax({
        type: "post",
        url: url,
        data: { "idHead": idHead },
        success: function (response) {
            $('#SectionID').empty();
            $('#SectionID').append('<option value="">----</option>');
            $.each(response, function (i, row) {
                $('#SectionID').append('<option value="' + row.value + '">' + row.text + '</option>');
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
//Список процедур для фильтра
function getAdminProcListForFiltr() {
    var url = '/adminproc/adminProcList';
    var idSection = document.getElementById("SectionID").value;
    $.ajax({
        type: "post",
        url: url,
        data: { "idSection": idSection },
        success: function (response) {
            var items = '';
            $('#DocRegList').empty();
            $('#DocRegList').append('<option value="">----</option>');
            $.each(response, function (i, row) {
                $('#DocRegList').append('<option value="' + row.value + '">' + row.text + '</option>');
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

//загружаем список кураторов для страницы добавления отдела
function getCuratorList(department) {
    var url = '/area/getCuratorList';
    var idArea = document.getElementById(department + "Curators_Area_Id").value;
    $.ajax({
        type: "post",
        url: url,
        data: { "idArea": idArea },
        success: function (response) {
            var items = '';
            $('#' + department + 'Curator_Id').empty();
            $.each(response, function (i, row) {
                $('#' + department + 'Curator_Id').append('<option value="' + row.value + '">' + row.text + '</option>');
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

//загружаем список отделов для страницы добавления исполнителя
function getDepartmentList() {
    var url = '/area/getDepartmentList';
    var idCurator = "";
    if (document.getElementById("Department_Curator_Id"))
        idCurator = document.getElementById("Department_Curator_Id").value;
    if (document.getElementById("CuratorFiltr"))
        idCurator = document.getElementById("CuratorFiltr").value;
    $.ajax({
        type: "post",
        url: url,
        data: { "idCurator": idCurator },
        success: function (response) {
            var select = document.querySelector('#DepartamentFiltr') || document.querySelector('#Department_ID');
                        
            select.innerHTML="";
            if (document.querySelector('#DepartamentFiltr')) {
                select.innerHTML = '<option value="">----</option>';
            }

            response.forEach((e) => {
                select.insertAdjacentHTML("beforeend", '<option value="' + e.value + '">' + e.text + '</option>');
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

//загружаем список исполнителей для фильтра
function getPerformerList() {
    var url = '/area/getPerformerList';
    var idDepartment = document.getElementById("DepartamentFiltr").value;
    $.ajax({
        type: "post",
        url: url,
        data: { "idDepartment": idDepartment },
        success: function (response) {
            var select = $('#PerformerFiltr');
            select.empty();
            select.append('<option value="">----</option>');
            $.each(response, function (i, row) {
                select.append('<option value="' + row.value + '">' + row.text + '</option>');
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

//загружаем список процедур для страницы добавления комментария
function getDocForDeptList() {
    var url = '/area/getDocForDeptList';
    var idDepartment = document.getElementById("IdDep").value;
    $.ajax({
        type: "post",
        url: url,
        data: { "idDepartment": idDepartment },
        success: function (response) {
            var items = '';
            $('#IdDoc').empty();
            $.each(response, function (i, row) {
                $('#IdDoc').append('<option value="' + row.value + '">' + row.text + '</option>');
            });
            getComment();
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

//загружаем комментарий
function getComment() {
    var url = '/area/getDocForComment';
    var IdDoc = document.getElementById("IdDoc").value;
    var IdDep = document.getElementById("IdDep").value;
    document.getElementById("Text").value = "";
    document.getElementById("Email").value = "";
    document.getElementById("Id").value = "";
    document.querySelector("a[name=deleteComment]").removeAttribute("href");
    $.ajax({
        type: "post",
        url: url,
        data: { "IdDoc": IdDoc, "IdDep": IdDep },
        success: function (response) {
            document.getElementById("Text").value = response.text;
            document.getElementById("Email").value = response.email;
            document.getElementById("Id").value = response.id;
            document.querySelector("a[name=deleteComment]").setAttribute("href", "deleteComment?idComment=" + response.id);
        }
    });
}

//загружаем список праздничных дней
function getCurrentDate() {
    var url = '/calendar/getCurrentDate';
    var monthCalendar = document.getElementById("monthCalendar").value;
    var yearCalendar = document.getElementById("yearCalendar").value;
    $.ajax({
        type: "post",
        url: url,
        data: { "yearCalendar": yearCalendar, "monthCalendar": monthCalendar },
        success: function (response) {
            document.getElementById("viewCurrentDay").innerHTML = response;
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
//загружаем список праздничных дней
function saveCurrentDate() {
    var url = '/calendar/saveCurrentDate';
    var monthCalendar = document.getElementById("monthCalendar").value;
    var yearCalendar = document.getElementById("yearCalendar").value;
    var daysCalendar = document.getElementById("daysCalendar").value;
    daysCalendar = daysCalendar.replaceAll(" ", "");

    $.ajax({
        type: "post",
        url: url,
        data: { "yearCalendar": yearCalendar, "monthCalendar": monthCalendar, "daysCalendar": daysCalendar },
        success: function (response) {
            getCurrentDate();
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

//выделяем все элементы
function selectAllCheck() {
    if (document.getElementById("selectAllCheck").checked == false)
        document.querySelectorAll("input[data-id]").forEach(x => x.checked = false);
    else
        document.querySelectorAll("input[data-id]").forEach(x => x.checked = true);
}

//показываем и скрываем список административных процедур
function showAdminProc() {
    var arrayAdminProc = document.querySelector("#adminProcList").querySelectorAll("span");
    arrayAdminProc.forEach(function (x) {
        x.addEventListener('click', (event) => {
            var childUl = x.parentNode.querySelector("ol");
            if (childUl.style.display == 'none')
                childUl.style.display = "block";
            else
                childUl.style.display = "none";

        });
    });

    var selectedList = document.querySelector("#adminProcList").querySelectorAll("[data-selected]");
    selectedList.forEach(function (x) {
        x.addEventListener('change', (event) => {
            var url = '/adminproc/updateSelected';
            $.ajax({
                type: "post",
                data: { "idDoc": x.getAttribute('data-selected') },
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

        });
    });
    var pmcList = document.querySelector("#adminProcList").querySelectorAll("[data-pmc]");
    pmcList.forEach(function (x) {
        x.addEventListener('change', (event) => {
            var url = '/adminproc/updatePMC';
            $.ajax({
                type: "post",
                data: { "idDoc": x.getAttribute('data-pmc') },
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

        });
    });
}

//просмотр справочника документов
function viewProcDoc() {
    var docArray = document.querySelector(".viewDoc").querySelectorAll("span[data-doc]");
    if (docArray.length > 0) {
        docArray.forEach(x => {
            x.addEventListener("click", (e) => {

                document.querySelector(".viewDoc").querySelectorAll("span[data-doc]").forEach(s => { s.parentElement.removeAttribute("style"); });
                document.querySelector(".viewDoc").querySelectorAll("div[name=actionMenu]").forEach(s => { s.setAttribute("style", "display:none;"); });

                x.parentElement.setAttribute("style", "background:#F5F5F5");
                x.parentElement.querySelector("div[name=actionMenu]").removeAttribute("style");
                ajaxViewDoc(x.getAttribute("data-doc"));
            });
        });
    }
}
function ajaxViewDoc(idDoc) {
    var controller = document.querySelector(".viewDoc").getAttribute("data-controller");
    var url = '/' + controller + '/details';
    document.getElementById('viewPartial').innerHTML = "";
    $.ajax({
        type: "post",
        url: url,
        data: { "idDoc": idDoc },
        success: function (response) {
            document.getElementById('viewPartial').insertAdjacentHTML('afterbegin', response);
        }
    });
    url = '/' + controller + '/viewPartial';
    $.ajax({
        type: "post",
        url: url,
        data: { "idDoc": idDoc },
        success: function (response) {
            document.getElementById('viewPartial').insertAdjacentHTML('beforeend', response);
            if (controller == "Head" || controller == "head") {
                selectGuideList();
            }
        }
    });

}

//добавление документа к процедуре
function addAdminProc() {
    $('#viewAdminProcModalForDoc').modal('show');
    document.getElementById("adminProcList").querySelectorAll("input[type=checkbox]").forEach((x) => {
        x.checked = false;
    });
    var arrayIdDocReg = document.getElementById('viewPartial').querySelectorAll("li[data-id]");
    arrayIdDocReg.forEach((x) => {
        var check = document.querySelector("input[name='" + x.getAttribute("data-id") + "']");
        if (check)
            check.checked = true;
    });
    var button = document.getElementById("viewAdminProcModalForDoc").querySelector("#saveButton");
    button.addEventListener("click", (e) => {

        var controller = document.querySelector(".viewDoc").getAttribute("data-controller");
        var url = '/' + controller + '/addAdminProc';
        var idDocReg = new Array();
        document.getElementById("adminProcList").querySelectorAll("input[type=checkbox]").forEach((x) => {
            if (x.checked)
                idDocReg.push(x.getAttribute("name"));
        });
        $.ajax({
            type: "post",
            url: url,
            data: {
                "idDoc": document.getElementById("idDoc").value, "idDocReg": idDocReg
            },
            success: function (response) {
                ajaxViewDoc(document.getElementById("idDoc").value);
                $('#viewAdminProcModalForDoc').modal('hide');
            },
        });

    }, { once: true });


}

//добавляем(редактируем) раздел
function addSection(idDoc, idSection) {
    if (idDoc) {
        $('#addSectionForHead').modal('show');
        document.querySelector("input[name='idHead']").value = idDoc;
        if (idSection) {
            document.querySelector(".modal-title").innerHTML = "Редактирование раздела";
            document.querySelector("input[name='idSection']").value = idSection;
            document.querySelector("textarea[name='nameSection']").value = document.querySelector("span[data-doc='" + idSection + "']").innerHTML;

            document.querySelector("input[name='numberSection']").value = document.querySelector("span[data-doc='" + idSection + "']").getAttribute("data-number");

        }
        else {
            document.querySelector("input[name='idSection']").value = "";
            document.querySelector("textarea[name='nameSection']").value = "";
            document.querySelector("input[name='numberSection']").value = "";
            document.querySelector(".modal-title").innerHTML = "Добавление раздела";
        }

        document.getElementById("addSectionForHead").querySelector("h4").innerHTML = document.querySelector("span[data-doc='" + idDoc + "']").innerHTML;
        var button = document.getElementById("addSectionForHead").querySelector("#saveButton");


        button.addEventListener("click", function handler() {

            var nameSection = document.querySelector("textarea[name='nameSection']").value;
            if (nameSection) {
                loadingButton(button);
                var url = '/head/CreateSection';
                $.ajax({
                    type: "post",
                    url: url,
                    data: {
                        "HeadID": idDoc, "SectionID": idSection, "Name": nameSection, "Number": document.querySelector("input[name='numberSection']").value
                    },
                    success: function (response) {
                        ajaxViewDoc(idDoc);
                        $('#addSectionForHead').modal('hide');
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
                this.removeEventListener('click', handler);
            }
            else { alert("Введите название!!!"); }
        });

    }
    else
        alert("Выберите главу!");
}

//удаляем раздел
function deleteSection(idSection) {
    var button = document.querySelector("span[data-del='" + idSection + "']");
    button.removeAttribute("style");
    setTimeout(() => button.setAttribute("style", "display:none"), 2000);
    button.addEventListener("click", (e) => {
        var url = '/head/DeleteSection';
        $.ajax({
            type: "post",
            url: url,
            data: { "idSection": idSection },
            success: function (response) {
                ajaxViewDoc(response);
                $('#addSectionForHead').modal('hide');
            },
        });
    }, { once: true });
}

//список кураторов
function getListCurator() {
    document.getElementById('viewPartial').innerHTML = "";
    var idArea = document.querySelector("select[name=areaName]").value;
    document.querySelector("a[name=editArea]").setAttribute("href", "/Area/CreateArea?idArea=" + idArea);
    var url = '/area/CuratorList';
    $.ajax({
        type: "post",
        url: url,
        data: { "idArea": idArea },
        success: function (response) {
            document.getElementById('viewCurator').innerHTML = response;
            var arraySpan = document.querySelector(".viewCurator").querySelectorAll("span[data-doc]");
            arraySpan.forEach(x => {
                x.addEventListener("click", e => {
                    getDepartmentForCurator(x.getAttribute("data-doc"));
                }, { once: true });
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
//список отделов
function getDepartmentForCurator(idCurator) {
    document.getElementById('viewPartial').innerHTML = "";
    var url = '/area/DepartmentList';
    $.ajax({
        type: "post",
        url: url,
        data: { "idCurator": idCurator },
        success: function (response) {
            document.getElementById('viewCurator').innerHTML = response;
            var arraySpan = document.querySelector(".viewCurator").querySelectorAll("span[data-doc]");
            arraySpan.forEach(x => {
                x.addEventListener("click", e => {
                    getPerformerForDepartment(x.getAttribute("data-doc"));
                }, { once: true });
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
//список исполнителей
function getPerformerForDepartment(idDepartment) {

    var url = '/area/PerformerList';
    $.ajax({
        type: "post",
        url: url,
        data: { "idDepartment": idDepartment },
        success: function (response) {
            document.getElementById('viewCurator').innerHTML = response;
            var arraySpan = document.querySelector(".viewCurator").querySelectorAll("span[data-doc]");
            arraySpan.forEach(x => {
                x.addEventListener("click", e => {
                    ajaxViewDoc(x.getAttribute("data-doc"));
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

//удаляем район
function deleteArea() {

    var button = document.querySelector("button[name=deleteArea]");
    button.removeAttribute("style");
    setTimeout(() => button.setAttribute("style", "display:none"), 2000);
    button.addEventListener("click", (e) => {
        var url = '/area/deleteArea';
        $.ajax({
            type: "post",
            url: url,
            data: { "idArea": document.querySelector("select[name=areaName]").value },
            success: function (response) {
                window.location.href = "area"
            },
        });
    });
}

//список процедур для ответственного, исполнителя
function viewAdminProcForPerformer(legal) {
    var idPerformer = document.querySelector("input[name=idPerformer]").value;
    var button = document.getElementById("viewAdminProcModalForDoc").querySelector("#saveButton");
    button.addEventListener("click", (e) => { addAdminProcForPerformer(legal); }, { once: true });
    var url = '/area/viewAdminProc';
    $.ajax({
        type: "post",
        url: url,
        data: { "idPerformer": idPerformer, "legal": legal },
        success: function (response) {
            document.getElementById('viewAdminProcModalForDoc').querySelector("div[name=viewAdminProc]").innerHTML = response;
            $('#viewAdminProcModalForDoc').modal('show');

            var arrayAdminProc = document.querySelector("#adminProcList").querySelectorAll("span");
            arrayAdminProc.forEach(function (x) {
                x.addEventListener('click', (event) => {
                    var childUl = x.parentNode.querySelector("ol");
                    if (childUl.style.display == 'none')
                        childUl.style.display = "block";
                    else
                        childUl.style.display = "none";

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

//добавление документа к процедуре
function addAdminProcForPerformer(legal) {
    var url = '/area/addAdminProc';
    var idPerformer = document.querySelector("input[name=idPerformer]").value;
    var idDocReg = new Array();
    document.getElementById("adminProcList").querySelectorAll("input[name=docReg]").forEach((x) => {
        if (x.checked)
            idDocReg.push(x.getAttribute("data-id"));
    });
    var idDocRes = new Array();
    document.getElementById("adminProcList").querySelectorAll("input[name=docRes]").forEach((x) => {
        if (x.checked)
            idDocRes.push(x.getAttribute("data-id"));
    });
    var idDocAcc = new Array();
    document.getElementById("adminProcList").querySelectorAll("input[name=docAcc]").forEach((x) => {
        if (x.checked)
            idDocAcc.push(x.getAttribute("data-id"));
    });
    $.ajax({
        type: "post",
        url: url,
        data: {
            "idPerformer": idPerformer, "idDocReg": idDocReg, "idDocRes": idDocRes, "idDocAcc": idDocAcc, "legal": legal
        },
        success: function (response) {
            ajaxViewDoc(idPerformer);
            $('#viewAdminProcModalForDoc').modal('hide');
        },
    });
}

//удаляем куратора
function deleteCurator(idCurator) {
    var button = document.querySelector("span[data-del='" + idCurator + "']");
    button.removeAttribute("style");
    setTimeout(() => button.setAttribute("style", "display:none"), 2000);
    button.addEventListener("click", (e) => {
        var url = '/area/deleteCurator';
        $.ajax({
            type: "post",
            url: url,
            data: { "idCurator": idCurator },
            success: function (response) {
                getListCurator();
            },
        });
    }, { once: true });
}

//удаляем отдел
function deleteDepartment(idDepartment) {
    var button = document.querySelector("span[data-del='" + idDepartment + "']");
    button.removeAttribute("style");
    setTimeout(() => button.setAttribute("style", "display:none"), 2000);
    button.addEventListener("click", (e) => {
        var url = '/area/deleteDepartment';
        $.ajax({
            type: "post",
            url: url,
            data: { "idDepartment": idDepartment },
            success: function (response) {
                getDepartmentForCurator(document.querySelector("input[name=idCurator]").value);
            },
        });
    }, { once: true });
}

//удаляем исполнителя
function deletePerformer(idPerformer) {
    var button = document.querySelector("span[data-del='" + idPerformer + "']");
    button.removeAttribute("style");
    setTimeout(() => button.setAttribute("style", "display:none"), 2000);
    button.addEventListener("click", (e) => {
        var url = '/area/deletePerformer';
        $.ajax({
            type: "post",
            url: url,
            data: { "idPerformer": idPerformer },
            success: function (response) {
                getPerformerForDepartment(document.querySelector("input[name=idDepartment]").value);
            },
        });
    }, { once: true });
}

//просмотр справочника документов
function selectGuideList() {
    var docArray = document.querySelector(".guideList").querySelectorAll("span[data-doc]");
    if (docArray.length > 0) {
        docArray.forEach(x => {
            x.addEventListener("click", (e) => {
                document.querySelector(".guideList").querySelectorAll("span[data-doc]").forEach(s => { s.parentElement.removeAttribute("style"); });
                document.querySelector(".guideList").querySelectorAll("div[name=actionMenu]").forEach(s => { s.setAttribute("style", "display:none;"); });
                x.parentElement.setAttribute("style", "background:#F5F5F5");
                x.parentElement.querySelector("div[name=actionMenu]").removeAttribute("style");
            });
        });
    }
}

//добавляем(редактируем) справочник
function createGuide(idDoc) {

    var nameTable = document.querySelector("input[name=nameTable]").value;
    if (!nameTable)
        alert("Выберите таблицу!!!!");
    else {
        document.querySelector("textarea[name=nameGuide]").value = "";
        document.querySelector(".modal-title").innerHTML = "";
        document.querySelector("input[name=idGuide]").value = "";
        var url = '/Guide/GetInfo';
        $.ajax({
            type: "post",
            url: url,
            data: { "idDoc": idDoc, "nameTable": nameTable },
            success: function (response) {
                if (response.name) {
                    document.querySelector("textarea[name=nameGuide]").value = response.name;
                    document.querySelector(".modal-title").innerHTML = response.nameTitle;
                    document.querySelector("input[name=idGuide]").value = response.id;
                }
                else
                    document.querySelector(".modal-title").innerHTML = response;
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
        $('#viewGuideCreate').modal('show');
        var button = document.getElementById("viewGuideCreate").querySelector("#saveButton");
        button.addEventListener("click", function handler() {
            loadingButton(button);
            var nameGuiede = document.querySelector("textarea[name=nameGuide]").value;
            var nameTable = document.querySelector("input[name=nameTable]").value;
            var idGuide = document.querySelector("input[name=idGuide]").value;
            if (nameGuiede) {
                var url = '/guide/Create';
                $.ajax({
                    type: "post",
                    url: url,
                    data: {
                        "doc": { "Name": nameGuiede, "id": idGuide || 0, "nameTitle": document.querySelector(".modal-title").innerHTML },
                        "nameTable": nameTable
                    },
                    success: function (response) {
                        loadingButton(button);
                        window.location.href = "guide?nameTable=" + nameTable;
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
                this.removeEventListener('click', handler);
            }
            else
                alert("Наименование не может быть пустым!!!!");

        });
    }
}

//удаляем справочник
function deleteGuide(id) {
    var nameTable = document.querySelector("input[name=nameTable]").value;
    if (!nameTable)
        alert("Выберите таблицу!!!!");
    else {
        var button = document.querySelector("span[data-del='" + id + "']");
        button.removeAttribute("style");
        setTimeout(() => button.setAttribute("style", "display:none"), 2000);
        button.addEventListener("click", (e) => {
            var url = '/guide/Delete';
            $.ajax({
                type: "post",
                url: url,
                data: { "idDoc": id, "nameTable": nameTable },
                success: function (response) {
                    window.location.href = "guide?nameTable=" + nameTable;
                },
            });
        }, { once: true });
    }
}

//отчеты
function getReports(typeReports) {
    var button = document.querySelector("button[onclick=\"getReports('" + typeReports + "')\"]");
    loadingButton(button);

    var startDate = document.querySelector("input[name=startDate]").value;
    var endDate = document.querySelector("input[name=endDate]").value;
    var radioOfSolutionType = document.querySelector("input[name=radioOfSolutionType]:checked").value;
    var radioOfDocType = document.querySelector("input[name=radioOfDocType]:checked").value;
    var typeDocReports = document.querySelector("select[name=typeReports]").value;
    var TypeOfReportLists = document.querySelector("select[name=TypeOfReportLists]").value;
    var TypeOfReportListSolutions = document.querySelector("select[name=TypeOfReportListSolutions]").value;

    var actionReports;
    var dataReports;

    switch (typeReports) {
        case "report": //Отчет по количеству принятых заявлений
            actionReports = "report";
            dataReports = { "startDate": startDate, "endDate": endDate, "RegType": typeDocReports, "radioOfDocType": radioOfDocType };
            break;
        case "reportGarden": //реестр детского сада
            actionReports = "reportGarden";
            dataReports = { "startDate": startDate, "endDate": endDate };
            break;
        case "reportHandred": //отчет по 110
            actionReports = "reportHandred";
            dataReports = { "startDate": startDate, "endDate": endDate };
            break;
        case "reportSolution": //отчет о решениях
            actionReports = "reportSolution";
            dataReports = { "startDate": startDate, "endDate": endDate, "radioOfSolutionType": radioOfSolutionType };
            break;
        case "reportCountSolution": //Отчет по количеству принятых решений
            actionReports = "reportCountSolution";
            dataReports = { "startDate": startDate, "endDate": endDate, "TypeOfReportLists": TypeOfReportLists, "TypeOfReportListSolutions": TypeOfReportListSolutions };
            break;
    }
    if (actionReports && dataReports) {
        var url = '/reports/' + actionReports;
        $.ajax({
            type: "post",
            url: url,
            data: dataReports,
            success: function (response) {
                loadingButton(button);
                window.location.href = "returnFile/index?urlFile=" + response;
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
}

//аналитика
function getAnalytics(typeReports) {
    var button = document.querySelector("button[onclick=\"getAnalytics('" + typeReports + "')\"]");
    loadingButton(button);

    var startDate = document.querySelector("input[name=startDate]").value;
    var endDate = document.querySelector("input[name=endDate]").value;

    var ProcedureName = document.querySelector("input[name=ProcedureName]").value;
    var RegistratorName = document.querySelector("input[name=RegistratorName]").value;
    var Chapter = document.querySelector("input[name=Chapter]").value;
    var radioOfDocTime = document.querySelector("input[name=radioOfDocTime]:checked").value;
    var DateList = document.querySelector("select[name=DateList]").value;
    var Person = document.querySelector("select[name=Person]").value;
    var ListQuestion = document.querySelector("select[name=ListQuestion]").value;

    var actionReports;
    var dataReports;

    switch (typeReports) {
        case "AnalyticsColHour": //Количество принятых заявлений в час
            actionReports = "AnalyticsColHour";
            dataReports = { "startDate": startDate, "endDate": endDate, "ProcedureName": ProcedureName, "RegistratorName": RegistratorName };
            break;
        case "AnalyticsChapter": //Аналитика по главам
            actionReports = "AnalyticsChapter";
            dataReports = { "startDate": startDate, "endDate": endDate, "DateList": DateList, "Person": Person, "Chapter": Chapter, "ListQuestion": ListQuestion, "radioOfDocTime": radioOfDocTime };
            break;
        case "AnalyticsDecision": //Аналитика по решениям
            actionReports = "AnalyticsDecision";
            dataReports = { "startDate": startDate, "endDate": endDate };
            break;
    }
    if (actionReports && dataReports) {
        var url = '/analytics/' + actionReports;
        $.ajax({
            type: "post",
            url: url,
            data: dataReports,
            success: function (response) {
                loadingButton(button);
                window.location.href = "returnFile/index?urlFile=" + response;
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
}