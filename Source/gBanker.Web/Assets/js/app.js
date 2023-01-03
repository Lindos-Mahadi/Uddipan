
var app = {
    initFns: function () {
        this.globalControl();
    },
    globalControl: function () {
        if ($(".datepicker").length > 0) {
            $(".datepicker").datepicker({
                dateFormat: "dd-M-yy",
                showAnim: "scale"
            });
        }

        if ($(".datepicker_mc").length > 0) {
            $(".datepicker_mc").datepicker({
                dateFormat: "dd-M-yy",
                showAnim: "scale",
                changeMonth: true,
                changeYear: true,
                yearRange: '-10:+0'
            });
        }
    },

    showNotification: (resp) => {
        $("#loading").hide();
        var result = resp.Result;
        var msg = resp.Message;
        var css = "failed";
        if (result == "OK")
            css = "success";
        $("#dvMessage").attr('class', css);
        $("#dvMessage").html(msg);
        $("#dvMessage").show();        
        $("#dvMessageDown").attr('class', css);
        $("#dvMessageDown").html(msg);
        $("#dvMessageDown").show();
        if (result == "OK") {
            $("#dvMessage").toggle('fade', 1500);
            $("#dvMessageDown").toggle('fade', 1500);           
        }       
    }
}

$(function () {
    app.initFns()
})


var SyncedStatusConstants = {
    NOT_SYNCED: 'NOT SYNCED',
    SYNCED: 'SYNCED'
}
var MFFlagConstants = {
    Male: 'M',
    Female: 'F',
    Neutral: 'N'
}

var BulkSMSAuthConstants ={
    BulkSMSAuthClientKey : "KEY::nObMMomumwvXvlTy6KEfEbjKMbdsO"
}

var DueTypeConstants = {
    Current_Due: 'Current_Due',
    Duration_Over_Due:'Duration_Over_Due'
}

var MFIConstants = {
    Society_For_Social_Service_SSS: 126
}