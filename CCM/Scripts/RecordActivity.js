
var otherPreviousValue;

$(document).on('click', '.GetValue-this', function () {
    otherPreviousValue = $(this).parent().parent().data('initialvalue');
})

//dont remove this recordactivity
function recordactivity() {
    debugger;
}

$(document).on("focusin", 'input[type=text],input[type=number],input[type=select],input[type=textarea],select,textarea', function (e) {
    //$('input,select,textarea').focusin(function () {

    if ($(this).data('categeryValue') != null) {
        console.log($(this).data('categeryValue'));
    }
    lastinputvalue = $(this).val();
    lastinputname = $(this).attr("name");
    lastinputtype = $(this).get(0).type;
    if (lastinputtype == "select-one") {
        lastinputvalue = $(this).find("option:selected").text();
    }
});

$(document).on("focusout", 'input[type=text],input[type=number],input[type=select],input[type=textarea],select,textarea', function (e) {
    //$('input,select,textarea').focusout(function () {
    debugger;

    if (!$(this).hasClass("notRecord")) {

        var HiddenContainerId = $("#ControlTimerHidden").data("value");
        //alert(HiddenContainerId);
        if (HiddenContainerId == null || HiddenContainerId == '') {
            if (lastinputvalue != $(this).val() && lastinputname == $(this).attr("name") && lastinputtype != "select-one") {
                var alreadyval = $("#activityperformed").val();
                if (alreadyval != "") {
                    alreadyval = alreadyval + "\n";
                }
                alreadyval = alreadyval + "Change value of " + lastinputname + "\n From \n" + lastinputvalue + "\n To \n" + $(this).val();
                $("#activityperformed").val(alreadyval);
            }
            else {
                var newvalue = "";
                if (lastinputtype == "select-one") {
                    newvalue = $(this).find("option:selected").text();
                }
                if (lastinputvalue != newvalue && lastinputname == $(this).attr("name") && lastinputtype == "select-one") {
                    var alreadyval = $("#activityperformed").val();
                    if (alreadyval != "") {
                        alreadyval = alreadyval + "\n ";
                    }
                    alreadyval = alreadyval + "Change value of " + lastinputname + " \n From \n" + lastinputvalue + "\n To \n" + newvalue;
                    $("#activityperformed").val(alreadyval);
                }
            }
        } else {
            if (lastinputvalue != $(this).val() && lastinputname == $(this).attr("name") && lastinputtype != "select-one") {
                var alreadyval = $("#BillingCategoryactivityRecord" + HiddenContainerId).val();
                if (alreadyval != "") {
                    alreadyval = alreadyval + "\n";
                }
                alreadyval = alreadyval + "Change value of " + lastinputname + "\n From \n" + lastinputvalue + "\n To \n" + $(this).val();

                $("#BillingCategoryactivityRecord" + HiddenContainerId).val(alreadyval);
            }
            else {

                var newvalue = "";
                if (lastinputtype == "select-one") {
                    newvalue = $(this).find("option:selected").text();
                }
                if (lastinputvalue != newvalue && lastinputname == $(this).attr("name") && lastinputtype == "select-one") {
                    var alreadyval = $("#BillingCategoryactivityRecord" + HiddenContainerId).val();
                    if (alreadyval != "") {
                        alreadyval = alreadyval + "\n ";
                    }
                    alreadyval = alreadyval + "Change value of " + lastinputname + " \n From \n" + lastinputvalue + "\n To \n" + newvalue;
                    $("#BillingCategoryactivityRecord" + HiddenContainerId).val(alreadyval);
                }
            }
        }

    }
});



window.onbeforeunload = reviewTimePost;
var reviewid = ReviewIdForRecordActivity;
function reviewTimePost() {
    debugger;
    if (reviewid == "" || reviewid == null) {
        reviewid = 0;
    }

    //var browser = get_browser();
    //var asynctype = true;
    //if (browser.name == 'Firefox') {
    //    asynctype = false;
    //}
    reviewTimePostOther();

    var istimesavedforthispage = null;
    istimesavedforthispage = $("#time").html();
    if (istimesavedforthispage != null) {
        $.ajax({
            url: "/PatientProfile/ReviewTimePost",
            data: { "reviewId": reviewid, "activity": $("#activityperformed").val() },
            type: "POST",
            //async: asynctype
        });
 
    }
    //reviewTimePostOther();
}


var reviewTimePostOther = function () {

    var ActivityListSterilized;
    var ActivitesList = [];
    //var reviewid = $("#SelectedCategory").data('id');
    $(".GetBillingCodesForActivityRecord").each(function () {
        var reviewid = $(this).val();
        //var BillingCategoryId = currentCategoryId;
        var BillingCategoryId = $(this).data('billingcategoryid');
        console.log(reviewid);
        if (reviewid != null && reviewid != "") {
            var Activity = $("#BillingCategoryactivityRecord" + BillingCategoryId).val();
            if (Activity == null || Activity == "") {
                Activity = "";
            }

            var obj = {
                ReviewId: reviewid,
                BillingCategoryId: BillingCategoryId,
                Activity: Activity
            }
            ActivitesList.push(obj);

        }
    })
    console.log(ActivitesList);

    ActivityListSterilized = JSON.stringify(ActivitesList);
    if (ActivitesList.length > 0) {
        $.ajax({
            url: "/PatientProfile/ReviewTimePostOtherCategories",
            data: { "reviewId": reviewid, "activity": ActivityListSterilized },
            type: "POST",
        });
    }

}











//var reviewTimePostOther = function () {
//    if ($("#SelectedCategory").val().toLowerCase() != "ccm") {
//        var currentCategoryId = $("#SelectedCategory").data('value');
//        if (currentCategoryId != "" || currentCategoryId != null) {

//            var reviewid = $("#SelectedCategory").data('id');
//            var BillingCategoryId = currentCategoryId;
//            var Activity = $("#BillingCategoryactivityRecord" + BillingCategoryId).val();
//            if (Activity == null || Activity == "") {
//                Activity = "";
//            }
//            var obj = {
//                BillingCategoryId: BillingCategoryId,
//                Activity: Activity
//            }
//            var ActivityListSterilized = JSON.stringify(obj);
//            $.ajax({
//                url: "/PatientProfile/ReviewTimePostOtherCategories",
//                data: { "reviewId": reviewid, "activity": ActivityListSterilized },
//                type: "POST",
//            });
//            $("#BillingCategoryactivityRecord" + BillingCategoryId).val("");
//        }
//    }
//}



$(document).on("click", 'input[type=text],input[type=number],input[type=radio],input[type=checkbox]', function (e) {

    var target = e.target;


    if (target.className.indexOf("notRecord") < 0) {

        if (target.className.indexOf("logdetails") >= 0) {
            var buttontitleorvalue = "";
            buttontitleorvalue = target.innerHTML;
            if (buttontitleorvalue == "") {
                buttontitleorvalue = target.value;
            }
            //else {

            //}
            var alreadyval = $("#activityperformed").val();
            if (alreadyval != "") {
                alreadyval = alreadyval + "\n ";
            }
            alreadyval = alreadyval + "Clicked on " + buttontitleorvalue;
            $("#activityperformed").val(alreadyval);
        }
        if (target.type == "checkbox") {
            var HiddenContainerId = $("#ControlTimerHidden").data("value");
            if (HiddenContainerId != '') {
                var alreadyval = $("#BillingCategoryactivityRecord" + HiddenContainerId).val();
                if (alreadyval != "") {
                    alreadyval = alreadyval + "\n ";
                }
                var checkedold = false;
                if (target.checked == false) {
                    checkedold = true;
                }
                if ($(this).parent().parent().data('initialvalue') != null) {
                    if ($(this).parent().parent().data('initialvalue') != 'noinitialvalue') {
                        var PreviousCheckboxAnswers = $(this).parent().parent().data('initialvalue');

                        var NewCheckboxAnswers = "";
                        var checkboxTypes = $(this).parent().parent().find('input');
                        checkboxTypes.each(function () {
                            if ($(this).is(":checked")) {
                                //alert($(this).val());
                                NewCheckboxAnswers += $(this).val() + "\n";
                            }
                        });
                        $(this).parent().parent().data('initialvalue', NewCheckboxAnswers);
                        alreadyval = alreadyval + "Change value of " + $(this).data('recordquestion') + "\n From \n" + PreviousCheckboxAnswers + "\n To \n" + NewCheckboxAnswers

                    }
                    else if ($(this).parent().parent().data('initialvalue') == 'noinitialvalue') {
                        var NewCheckboxAnswers = "";
                        var checkboxTypes = $(this).parent().parent().find('input');
                        checkboxTypes.each(function () {
                            if ($(this).is(":checked")) {
                                //alert($(this).val());
                                NewCheckboxAnswers += $(this).val() + "\n";
                            }
                        });
                        $(this).parent().parent().data('initialvalue', NewCheckboxAnswers);
                        alreadyval = alreadyval + "Change value of " + $(this).data('recordquestion') + "\n From \n" + '' + "\n To \n" + NewCheckboxAnswers
                    }
                }
                else {
                    alreadyval = alreadyval + "Change value of " + target.name + "\n From \n" + checkedold + "\n To \n" + target.checked;
                }



                $("#BillingCategoryactivityRecord" + HiddenContainerId).val(alreadyval);
            }
            else {
                var alreadyval = $("#activityperformed").val();
                if (alreadyval != "") {
                    alreadyval = alreadyval + "\n ";
                }
                var checkedold = false;
                if (target.checked == false) {
                    checkedold = true;
                }
                alreadyval = alreadyval + "Change value of " + target.name + "\n From \n" + checkedold + "\n To \n" + target.checked;
                $("#activityperformed").val(alreadyval);
            }
        }
        if (target.type == "radio") {
            var HiddenContainerId = $("#ControlTimerHidden").data("value");
            if (HiddenContainerId != '') {
                var alreadyval = $("#BillingCategoryactivityRecord" + HiddenContainerId).val();
                if (alreadyval != "") {
                    alreadyval = alreadyval + "\n ";
                }
                var checkedold = false;
                if (target.checked == false) {
                    checkedold = true;
                }
                //alert($(this).parent().parent().data('initialvalue'));

                if ($(this).parent().parent().data('initialvalue') != null) {
                    if ($(this).parent().parent().data('initialvalue') != 'noinitialvalue') {
                        var CurrentOldValue = $(this).parent().parent().data('initialvalue');
                        var CurrentNewValue = $(this).val();
                        $(this).parent().parent().data('initialvalue', CurrentNewValue);
                        alreadyval = alreadyval + "Change value of " + $(this).data('recordquestion') + "\n From \n" + CurrentOldValue + "\n To \n" + CurrentNewValue;
                    } else if ($(this).parent().parent().data('initialvalue') == 'noinitialvalue') {

                        var CurrentNewValue = $(this).val();
                        $(this).parent().parent().data('initialvalue', CurrentNewValue);
                        alreadyval = alreadyval + "Change value of " + $(this).data('recordquestion') + "\n From \n" + "Null" + "\n To \n" + CurrentNewValue;

                    }

                }
                else {
                    alreadyval = alreadyval + "Change value of " + target.name + "\n From \n" + checkedold + "\n To \n" + target.checked;

                }

                $("#BillingCategoryactivityRecord" + HiddenContainerId).val(alreadyval);
            }
            else {
                var alreadyval = $("#activityperformed").val();
                if (alreadyval != "") {
                    alreadyval = alreadyval + "\n ";
                }
                var checkedold = false;
                if (target.checked == false) {
                    checkedold = true;
                }

                alreadyval = alreadyval + "Change value of " + target.name + "\n From \n" + checkedold + "\n To \n" + target.checked;
                $("#activityperformed").val(alreadyval);
            }
        }

        //var SetActivityData = BillingCategoryIdsListForActivityRecord;
        //SetActivityData.forEach(item => {

        //    if (target.className.indexOf("BillingCategoryactivityRecord" + item) >= 0) {
        //        var buttontitleorvalue = "";

        //        buttontitleorvalue = target.innerHTML;
        //        if (buttontitleorvalue == "") {
        //            buttontitleorvalue = target.value;
        //        }

        //        //else {

        //        //}
        //        var alreadyval = $("#BillingCategoryactivityRecord" + item).val();
        //        if (alreadyval != "") {
        //            alreadyval = alreadyval + "\n ";
        //        }
        //        alreadyval = alreadyval + "Clicked on " + buttontitleorvalue;
        //        $("#BillingCategoryactivityRecord" + item).val(alreadyval);
        //    }
        //    if (target.type == "checkbox") {
        //        var alreadyval = $("#BillingCategoryactivityRecord" + item).val();
        //        if (alreadyval != "") {
        //            alreadyval = alreadyval + "\n ";
        //        }
        //        var checkedold = false;
        //        if (target.checked == false) {
        //            checkedold = true;
        //        }

        //        alreadyval = alreadyval + "Change value of " + target.name + "\n From \n" + checkedold + "\n To \n" + target.checked;
        //        $("#BillingCategoryactivityRecord" + item).val(alreadyval);
        //    }
        //    if (target.type == "radio") {
        //        var alreadyval = $("#BillingCategoryactivityRecord" + item).val();
        //        if (alreadyval != "") {
        //            alreadyval = alreadyval + "\n ";
        //        }
        //        var checkedold = false;
        //        if (target.checked == false) {
        //            checkedold = true;
        //        }

        //        alreadyval = alreadyval + "Change value of " + target.name + "\n From \n" + checkedold + "\n To \n" + target.checked;
        //        $("#BillingCategoryactivityRecord" + item).val(alreadyval);
        //    }
        //});
    }
});