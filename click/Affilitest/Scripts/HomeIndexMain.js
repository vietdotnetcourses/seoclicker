function HomeIndexMain() {
   
    var _urlContainerSelectorClass = ".urlContainer";
    var _urlSelectorClass = ".url";
    var _numberOfUrlID = "#numberOfUrl";
    var _numberOfClickID = "#numberOfClick00";
    // var _iphone12Items = ["iphone-12"];
    // var _iphone11Items = ["iphone-11.1", "iphone-11.2.1", "iphone-11.2.5", "iphone-11.3", "iphone-11.4"];
    //var _iphone10Items = ["iphone-10", "iphone-10.2", "iphone-10.2.1", "iphone-10.3", "iphone-10.3.1", "iphone-10.3.2"];
    //var _iphone9Items = ["iphone-9", "iphone-9.2.1", "iphone-9.3", "iphone-9.3.1"];
    //var _iphone1011Items = ["iphone-11.1", "iphone-10", "iphone-10.2", "iphone-10.2.1", "iphone-10.3", "iphone-10.3.1", "iphone-10.3.2", "iphone-11.2.1", "iphone-11.2.5", "iphone-11.3", "iphone-11.4"];
    //var _iphoneItems = ["iphone-11.1", "iphone-10", "iphone-9", "iphone-8", "iphone-7", "iphone-9.2.1", "iphone-9.3", "iphone-9.3.1", "iphone-10.2", "iphone-10.2.1", "iphone-10.3", "iphone-10.3.1", "iphone-10.3.2", "iphone-11.2.1", "iphone-11.2.5", "iphone-11.3", "iphone-11.4"];

    //var _android4Items = ["android-4.1", "android-4.4.2", "android-4.4.4", "android-4.4"];
    // var _android5Items = ["android-5", "android-5.1"];
    // var _android6Items = ["android-6", "android-6.0.1"];
    //var _android7Items = ["android-7", "android-7.1", "android-7.1.1", "android-7.1.2"];
    //var _android5678Items = ["android-5", "android-5.1", "android-6", "android-6.0.1", "android-7", "android-7.1", "android-7.1.1", "android-7.1.2", "android-8"];
    //var _android78Items = ["android-7", "android-7.1", "android-7.1.1", "android-7.1.2", "android-8"];
    //var _android9Items = ["android-9"];
    //var _androidItems = ["android-8", "android-7", "android-6", "android-4.1", "android-4.4", "android-4.4.2", "android-4.4.4", "android-5", "android-5.1", "android-7.1", "android-6.0.1", "android-7.1.1", "android-7.1.2"];
    // private function --------------------------------------
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
                //if ($(deviceSelector, ".urlContainer").val() == "rdiphone") {
                //    device = _iphoneItems[Math.floor(Math.random() * _iphoneItems.length)];
                //}
                //else if ($(deviceSelector, ".urlContainer").val() == "rdandroid") {
                //    device = _androidItems[Math.floor(Math.random() * _androidItems.length)];
                //}
                //else if ($(deviceSelector, ".urlContainer").val() == "rdandroid5678") {
                //    device = _android5678Items[Math.floor(Math.random() * _android5678Items.length)];
                //}
                //else if ($(deviceSelector, ".urlContainer").val() == "rdandroid78") {
                //    device = _android78Items[Math.floor(Math.random() * _android78Items.length)];
                //}
                //else if ($(deviceSelector, ".urlContainer").val() == "rdandroid7") {
                //    device = _android7Items[Math.floor(Math.random() * _android7Items.length)];
                //}
                //else if ($(deviceSelector, ".urlContainer").val() == "rdandroid9") {
                //    device = _android9Items[0];
                //}
                //else if ($(deviceSelector, ".urlContainer").val() == "rdandroid6") {
                //    device = _android6Items[Math.floor(Math.random() * _android6Items.length)];
                //}
                //else if ($(deviceSelector, ".urlContainer").val() == "rdandroid5") {
                //    device = _android5Items[Math.floor(Math.random() * _android5Items.length)];
                //}
                //else if ($(deviceSelector, ".urlContainer").val() == "rdandroid4") {
                //    device = _android4Items[Math.floor(Math.random() * _android4Items.length)];
                //}
                //else if ($(deviceSelector, ".urlContainer").val() == "rdip1011") {
                //    device = _iphone1011Items[Math.floor(Math.random() * _iphone1011Items.length)];
                //}
                //else if ($(deviceSelector, ".urlContainer").val() == "rdip10") {
                //    device = _iphone10Items[Math.floor(Math.random() * _iphone10Items.length)];
                //}
                //else if ($(deviceSelector, ".urlContainer").val() == "rdip11") {
                //    device = _iphone11Items[Math.floor(Math.random() * _iphone11Items.length)];
                //}
                //else if ($(deviceSelector, ".urlContainer").val() == "rdip11") {
                //    device = _iphone11Items[Math.floor(Math.random() * _iphone11Items.length)];
                //}
                //else if ($(deviceSelector, ".urlContainer").val() == "rdip9") {
                //    device = _iphone9Items[Math.floor(Math.random() * _iphone9Items.length)];
                //}
                //else {
                //    device = $(deviceSelector, ".urlContainer").val();
                //}
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
    function startOne(index) {

        $("#startbuttonLabel00" + index).html("Stop");
        var data = [];
        data.push({
            url: $("#url00" + index).val(),
            country: $("#country00" + index).val(),
            device: $("#device00" + index).val(),
            thread: $("#speed00" + index).val(),
            index: index,
            click: $("#numberOfClick00" + index).val(),
        });

        postDataJson(window.url, JSON.stringify(data), function (data) {
            if (data.Status == 0 && data.Data[0]) {
                if (updateRowData(data.Data[0], index)) {
                    setTimeout(function () { startOne(index); }, 5000);
                }
                else {
                    $("#infoRow00" + index).html("OK");
                    $("#startbuttonLabel00" + index).html("Start");
                }
            }
            else if (data.Status == 2) {
                $("#infoRow00" + index).html("Lỗi");
                $("#startbuttonLabel00" + index).html("Start");
            }
            else {
                $("#infoRow00" + index).html("Lỗi");
                $("#startbuttonLabel00" + index).html("Start");
            }
        });
    }
    function updateRowData(data, index) {
        var totalClick = 0;
        if (!data.isStop) {
            totalClick += parseInt(data.thread);
        }
        $("#numberOfClick00" + index).val(data.click);
        
        return parseInt(data.click) > 0;
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
        //$(".resultPartial").empty().append("<p>" + jsonData + "<p>");
        //times = checkTimes < 0 ? times : checkTimes;
        //times = (times - parseInt($("#speed001").val())) < 0 ? 0 : (times - parseInt($("#speed001").val()));
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
            data.push({ url: $(urlSelector, ".urlContainer").val(), country: $(countrySelector, ".urlContainer").val(), device: $(deviceSelector, ".urlContainer").val(), thread: $(speedSelector, ".urlContainer").val(), click: $(click, ".urlContainer").val(), textSelected: $(textSelectedselector, ".urlContainer").text() });
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
            //$(".numberOfClick", ".urlContainer").each(function (index, element) {
            //    if ($(element).val() == '') {
            //        turnOffStart();
            //        return;
            //        alertify
            //        .alert("Thông báo", "Vui lòng không được để trống trường nào.", function () {
            //            //alertify.message('OK');
            //        });

            //    }
            //});
            //$(".speed", ".urlContainer").each(function (index, element) {
            //    if ($(element).val() == '') {
            //        turnOffStart();
            //        return;
            //        alertify
            //        .alert("Thông báo", "Vui lòng không được để trống trường nào.", function () {
            //            //alertify.message('OK');
            //        });

            //    }
            //});
            //$this.button('loading');
            //setTimeout(function () {
            //    $this.button('reset');
            //}, 8000);
            //if ($("#url001").val() == '' || $("#country001").val() == '' || $("#device001").val() == '' || $("#numberOfClick").val() == '') {
            //    alertify
            //        .alert("Vui lòng không được để trống trường nào.", function () {
            //            //alertify.message('OK');
            //        });
            //    turnOffStart();
            //    return;
            //}

            //postData('https://affilitest.com/user/login', dataLogin, function (data) { // Use the jQuery promises interface
            //    postData('https://affilitest.com/api/v1/test', data, function (data) { // Use the jQuery promises interface
            //                var jsonData = JSON.parse(data); // Assume it returns a JSON string
            //                console.log(jsonData); // Do whatever you want with the data
            //                $(".resultPartial").empty().append("<p>" + jsonData + "<p>");
            //            });
            //});
            //Click(parseInt($("#numberOfClick").val()));
            //if (!Click()) {

            //    return;
            //}
            Click().then(function () {
                //saveHistory("Start");
                saveUrlToDB();
            }, function () {
                // function reject
            });
            // How to use the cross domain proxy

        });
        $('.startbutton').on('click', function () {

            var index = $(this).data('index');
            if (runningStatus[index]) {
                $("#startbuttonLabel00" + index).html("Start");
                return;
            } 
            if (!$("#url00" + index).val() || !$("#speed00" + index).val() || !$("#numberOfClick00" + index).val()) {
                alert("Hãy điền đủ thông tin!");
                return;
            }
            startOne(index);
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

          
            runningStatus.push(false);
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