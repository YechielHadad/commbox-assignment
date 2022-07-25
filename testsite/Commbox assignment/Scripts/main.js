"use strict";

const url = "../Handlers/NbaAjaxHandler.ashx/";

$(function () {
    $('#datepicker').datepicker({
        changeYear: true,
        showButtonPanel: true,
        dateFormat: 'yy',
        onClose: function () {
            var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
            $(this).datepicker('setDate', new Date(year, 1));
        }
    });
    $(".date-picker-year").focus(function () {
        $(".ui-datepicker-month").hide();
    });
});

function getTopTen(year) {
    $(".spninner-container").show();
    let uri = url + "?year=" + year;

    $.ajax({
        type: 'GET',
        url: uri,
        data: {
            ac: 1
        },
        success: function (data) {
            $('#result').empty();
            if (data) {
                let players = JSON.parse(data);
                players.forEach(function (item, index) {
                    let newId = 'top' + index;
                    $('#clone').clone().attr('id', newId).removeAttr("style").appendTo('#result');
                    $('#' + newId).find(".full-name").text(item.playerFullName);
                    $('#' + newId).find(".player-height").text(item.playerHeightInMeters ? "(" + item.playerHeightInMeters + ")" : "");
                    $('#' + newId).find(".card-position").text(item.playerPosition);
                    $('#' + newId).find(".team-name").text(item.team?.teamName);
                    $('#' + newId).find(".conference-name").text(item.team?.conferenceName);
                    $('#' + newId).find(".country").text(item.team?.teamCountry);
                    $('#' + newId).find(".date-of-birth").text(item.playerDateOfBirth);
                });
            }
        },
        error: function (xhr) {
            $("#result").hide();
            $("#error").show();
            $("#error").html(xhr.status + ":" + xhr.responseText);
        },
        complete: function (data) {
            $(".spninner-container").hide();
        }
    });
}

function apply() {
    var year = $("#datepicker").val();
    if (year) {
        $("#error").hide();
        getTopTen(year);
    }
    else {
        $("#error").show();
        $("#error").html("Choose Year");
    }
}