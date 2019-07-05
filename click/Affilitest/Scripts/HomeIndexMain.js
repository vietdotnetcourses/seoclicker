function HomeIndexMain() {
   
    var _urlContainerSelectorClass = ".urlContainer";
    var _urlSelectorClass = ".url";
    var _numberOfUrlID = "#numberOfUrl";
    var _numberOfClickID = "#numberOfClick00";
   
    function Click() {
        var promiseCall = new Promise(function (resolve, reject) {
            var numberOfUrl = parseInt($(_numberOfUrlID).val());
            var data = [];
            var num = $(_urlSelectorClass, _urlContainerSelectorClass).length;
            var thread = 0;
            if (numberOfUrl < num) {
                alertify
                    .alert('Thông báo', "Không được bật quá url mà người quản trị cho phép bạn.", function () {
                        //alertify.message('OK');
                    });
            }
            for (var i = 1; i <= num; i++) {
                var device = "";
                let urlSelector = "#url00" + i.toString();
                let countrySelector = "#country00" + i.toString();
                let deviceSelector = "#device00" + i.toString();
                let speedSelector = "#speed00" + i.toString();
                let numberOfClick = _numberOfClickID + i.toString();
                thread = $(speedSelector, ".urlContainer").val();
                if (parseInt($(speedSelector, ".urlContainer").val()) > 20) {
                    alertify
                        .alert('Thông báo từ ban quản trị', " Tạm thời Không được để tốc độ lớn hơn 20.", function () {
                            //alertify.message('OK');
                        });
                    turnOffStart();
                }
                var device = $(deviceSelector).val();            
                data.push({
                    url: $(urlSelector, ".urlContainer").val(),
                    country: $(countrySelector, ".urlContainer").val(),
                    device: device,
                    thread: $(speedSelector, ".urlContainer").val(),
                    index: i,
                    click: $(numberOfClick, _urlContainerSelectorClass).val(),
                });
            }
            if ($("#stop").prop("checked") == false) {
                postDataJson(window.url, JSON.stringify(data), function (data) { // Use the jQuery promises interface
                    if (data.Status == 0) {
                        if (updateData(data.Data)) {
                            setTimeout(function () { Click(); }, 5000);
                        }
                        else {
                            turnOffStart();
                        }
                        resolve();
                    }
                    else if (data.Status == 2) {
                        window.location.href = data.LinkRedirect;
                    }
                    else {
                        turnOffStart();
                        alertify
                            .alert('Thông báo', data.Message, function () {
                                //alertify.message('OK');
                            });
                        reject();
                    }
                });
            }
        });
        return promiseCall;
    }
   
  
    function updateData(dataArray) {
        var isContinue = false;
        var totalClick = 0;
        //var jsonData = JSON.parse(data); // Assume it returns a JSON string
        dataArray.forEach(function (data) {
            let numberOfClickSelectorID = _numberOfClickID + data.index;
            let thread = data.thread;
            if (!data.isStop) {
                totalClick += parseInt(data.thread);
            }
            if (parseInt(data.click) > 0) {
                isContinue = true;
            }
            $(numberOfClickSelectorID, _urlContainerSelectorClass).val(data.click);
        });
        alertify.success('đã xong ' + totalClick + ' lần.');
        return isContinue;
    }
    function turnOffStart() {
        $("#cancel").addClass("hide");
        $("#submit").removeClass("hide");
        $("#stop").prop("checked", true);
    }

    function turnOnStart() {
        $("#cancel").removeClass("hide");
        $("#submit").addClass("hide");
        $("#stop").prop("checked", false);
    }

    function fillTextToAllTextbox(selector, value) {
        $(selector, ".urlContainer").val(value);
    }
    function fillTextToAllDropdown(selector, value) {
        $(selector, ".urlContainer").val(value).trigger("change");
    }


    function saveUrlToDB() {
        var data = [];
        var num = $(".url", ".urlContainer").length + 1;
        for (var i = 1; i < num; i++) {
            let urlSelector = "#url00" + i.toString();
            let countrySelector = "#country00" + i.toString();
            let deviceSelector = "#device00" + i.toString();
            let speedSelector = "#speed00" + i.toString();
            let click = "#numberOfClick00" + i.toString();
            let textSelectedselector = "#device00" + i.toString() + " option:selected";
            data.push({ url: $(urlSelector, ".urlContainer").val(), country: $(countrySelector, ".urlContainer").val(), device: $(deviceSelector, ".urlContainer").val(), thread: $(speedSelector, ".urlContainer").val(), click: $(click, ".urlContainer").val(), textSelected: $("#urlName00" + i).val() });
        }
        postDataJson(window.urlSaving, JSON.stringify(data), function (data) { // Use the jQuery promises interface
            console.log(data); // Do whatever you want with the data
            //alertify.success('đã lưu thành công');
        });
    }


    function updateCurrentClick(totalClick) {
        //var add = parseInt(totalClick);
        var selector = $("#currentClick");
        //var currentClick = parseInt(selector.text()) + add;
        selector.text(totalClick);
    }

    function setIntervalUpdateCurrentClick() {
        var interval = setInterval(function () {
            getCurrentClick();
        }, 30000);
    }

    function saveHistory(status) {
        var data = [];
        var num = $(".url", ".urlContainer").length + 1;
        for (var i = 1; i < num; i++) {
            let urlSelector = "#url00" + i.toString();
            let countrySelector = "#country00" + i.toString();
            let deviceSelector = "#device00" + i.toString();
            let speedSelector = "#speed00" + i.toString();
            let click = "#numberOfClick00" + i.toString();

            data.push({ url: $(urlSelector, ".urlContainer").val(), country: $(countrySelector, ".urlContainer").val(), device: $(deviceSelector, ".urlContainer").val(), thread: $(speedSelector, ".urlContainer").val(), click: $(click, ".urlContainer").val() });
        }
        var dataupload = { lstu: data, status: status }
        postDataJson(window.urlSaveHistory, JSON.stringify(dataupload), function (data) { // Use the jQuery promises interface
            if (data.Status == 0) {
                // success
            }
            else {
                alertify
                    .alert(data.Message, function () {
                        //alertify.message('OK');
                    });
            }
        });
    }

    function getCurrentClick() {
        postData(window.urlGetCurrentClick, {}, function (data) { // Use the jQuery promises interface
            if (data.Status == 0 && data.Data) {
                // success
                if (parseInt(data.Data[0].CurrentOfClick) > 0) {
                    updateCurrentClick(data.Data[0].CurrentOfClick);
                }
            }
            else {
                // error
            }
        });
    }

    function checkFieldsEmpty() {
        var result = false;
        $(".url", ".urlContainer").each(function (index, element) {
            if ($(element).val() == '') {
                result = true;
            }
        });
        $(".numberOfClick", ".urlContainer").each(function (index, element) {
            if ($(element).val() == '') {
                result = true;

            }
        });
        $(".speed", ".urlContainer").each(function (index, element) {
            if ($(element).val() == '') {
                result = true;

            }
        });
        return result;
    }
    // multi tab detection
    function register_tab_GUID() {
        // detect local storage available
        if (typeof (Storage) !== "undefined") {
            // get (set if not) tab GUID and store in tab session
            if (sessionStorage["tabGUID"] == null) sessionStorage["tabGUID"] = tab_GUID();
            var guid = sessionStorage["tabGUID"];

            // add eventlistener to local storage
            window.addEventListener("storage", storage_Handler, false);

            // set tab GUID in local storage
            localStorage["tabGUID"] = guid;
        }
    }

    function storage_Handler(e) {
        // if tabGUID does not match then more than one tab and GUID
        if (e.key == 'tabGUID') {
            if (e.oldValue != e.newValue) tab_Warning();
        }
    }

    function tab_GUID() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
            s4() + '-' + s4() + s4() + s4();
    }

    function tab_Warning() {
        //alert("Another tab is open!");
        window.location.href = "/aeonclick/notify/index";
    }

    // end private function ---------------------------

    // public function --------------------------------------------
    this.Initialize = function () {
        register_tab_GUID();
        $('#submit').on('click', function () {
            turnOnStart();
            var $this = $(this);
            if (checkFieldsEmpty()) {
                turnOffStart();
                alertify
                    .alert('Thông báo từ ban quản trị', "Vui lòng không được để trống trường nào.", function () {
                        //alertify.message('OK');
                    });
                return;
            }
        
            Click().then(function () {
                //saveHistory("Start");
                saveUrlToDB();
            }, function () {
                // function reject
            });
            // How to use the cross domain proxy

        });
        $('#btnSendChangePassword').on('click', function () {
            var data = { curentPassword: $("#current_password").val(), newPassword: $("#new_password").val(), passwordConfirm: $("#confirm_password").val() };
            if (data.curentPassword == "" || data.newPassword == "" || data.passwordConfirm == "") {
                alertify
                    .alert('Thông báo', "không được để trống trường nào", function () {

                    });
                return;
            }
            postData(window.urlChangePassword, data, function (data) { // Use the jQuery promises interface
                if (data.Status == 0) {
                    alertify
                        .alert('Thông báo', data.Message, function () {
                            //alertify.message('OK');
                            $('#changePasswordModal').modal('hide');
                        });
                }
                else if (data.Status == 2) {
                    window.location.href = data.LinkRedirect;
                }
                else {
                    alertify
                        .alert('Thông báo', data.Message, function () {
                            //alertify.message('OK');
                        });
                }
            });
        });

        $(".btnRefreshClick").on("click", function () {
            getCurrentClick();
        });
        $("#cancel").on("click", function () {
            //saveHistory("Stop");
            turnOffStart();
        });

        $(".btnAddUrl").on("click", function () {

          
            isRunning.push(false);
            var numberOfUrl = parseInt($("#numberOfUrl").val());
            var num = $(".url", ".urlContainer").length + 1;
            if (numberOfUrl == num) {
                $(this).addClass("hide");
            }

            $(".removeFormButton", ".urlContainer").remove();
            var i = $(".url", ".urlContainer").length + 1;
            var htmlTemp = $(".urlTemp").html();
            var regex = new RegExp("{{Index}}", "g");
            //htmlTemp = htmlTemp.replace("/{{Index}}/g", i);
            htmlTemp = htmlTemp.replace(regex, i);
            $(".urlContainer").append(htmlTemp);
            var selectorDevice = "#device00" + i;
            var selectorCountry = "#country00" + i;

            $(selectorDevice, ".urlContainer").select2();
            $(selectorCountry, ".urlContainer").select2({ dropdownCssClass: 'select2Country' });

        });

        $("#numberOfClickAll").on("change", function () {
            fillTextToAllTextbox(".numberOfClick", $(this).val());
        });
        $("#speedAll").on("change", function () {
            fillTextToAllTextbox(".speed", $(this).val());
        });
        $("#deviceAll").on("change", function () {
            fillTextToAllDropdown(".device", $(this).val());
        });
        $("#countryAll").on("change", function () {
            fillTextToAllDropdown(".country", $(this).val());
        });

        $(".urlContainer").on("click", ".removeFormButton", function () {
            var numberOfUrl = parseInt($("#numberOfUrl").val());
            var num = $(".url", ".urlContainer").length - 1;
            if (numberOfUrl > num) {
                $(".btnAddUrl").removeClass("hide");
            }

            var removeElement = $(this).parent().html();
            $(this).closest(".row").remove();
            var num = $(".url", ".urlContainer").length;
            var selector = ".removeFormButtonContainer00" + num.toString();
            $(selector, ".urlContainer").append(removeElement);

        });
        setIntervalUpdateCurrentClick();
    }

}

function updateRowData(data, index) {
    var totalClick = 0;
    if (!data.isStop) {
        totalClick += parseInt(data.thread);
    }
    $("#numberOfClick00" + index).val(data.click);

    return parseInt(data.click) > 0;
}
function startOne(i) {
    if (isRunning[i - 1]) {

        //if already running
        isRunning[i - 1] = false;
        $("#startbuttonLabel00" + i).html("Start");
        $("#infoRow00" + i).html("Đã dừng");
        return;
    } else {

        if (!$("#url00" + i).val() || !$("#speed00" + i).val() || !$("#numberOfClick00" + i).val()) {
            alert("Hãy điền đủ thông tin: url, số click và tốc độ!");
            return;
        }
        isRunning[i - 1] = true;
        $("#startbuttonLabel00" + i).html("Stop");
        $("#infoRow00" + i).html("Đang chạy");

         //countdown timer does its job here before starting

        var secondNumber = parseInt($("#timer00" + i).val());
        if (secondNumber && secondNumber > 0) {
            var myInterval = setInterval(function () {
                secondNumber -= 1;
                $("#timer00" + i).val(secondNumber);
                if (secondNumber <= 0) {
                    clearInterval(myInterval);
                    postrowData(i);
                }

            }, 1000);
        } else {
            //do it straightaway
            postrowData(i);
        }
       
      
    }

}
function postrowData(i) {


        if (!isRunning[i - 1]) {
            clearInterval(myIntervals[i - 1]);
            return;

        }
        var data = [];
        data.push({
            url: $("#url00" + i).val(),
            country: $("#country00" + i).val(),
            device: $("#device00" + i).val(),
            thread: $("#speed00" + i).val(),
            index: i,
            click: $("#numberOfClick00" + i).val(),
        });

        postDataJson(window.url, JSON.stringify(data), function (data) {
            if (data.Status == 0 && data.Data[0]) {
                if (updateRowData(data.Data[0], i)) {
                    var myinterval = setTimeout(function () { postrowData(i); }, 5000);
                    //myIntervals[i - 1] = myinterval;
                }
                else {
                    clearInterval(myIntervals[i - 1]);
                    $("#infoRow00" + i).html("Xong");
                    $("#startbuttonLabel00" + i).html("Start");
                }
            }
            else if (data.Status == 2) {
                $("#infoRow00" + i).html("Lỗi");
                $("#startbuttonLabel00" + i).html("Start");
            }
            else {
                $("#infoRow00" + i).html("Lỗi");
                $("#startbuttonLabel00" + i).html("Start");
            }
        });


}