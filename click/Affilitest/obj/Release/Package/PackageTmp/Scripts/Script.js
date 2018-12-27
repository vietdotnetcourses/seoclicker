$(function () {
    $(".select2").select2({width: '100%'});
    $(".select2Country").select2();
});

function postData(url, data, callback) {
    $.ajax({
        url: url,
        data: data,
        type: 'POST',
        crossDomain: true,
        xhrFields: {
            withCredentials: true
        }
    }).success(callback);
}

function postDataJson(url, data, callback) {
    $.ajax({
        url: url,
        data: data,
        contentType: 'application/json; charset=utf-8',
        type: 'POST',
        crossDomain: true, // set this to ensure our $.ajaxPrefilter hook fires
        xhrFields: {
            withCredentials: true
        }
    }).success(callback);
}

function openNav() {
    document.getElementById("mySidenav").style.width = "250px";
}

function closeNav() {
    document.getElementById("mySidenav").style.width = "0";
}


