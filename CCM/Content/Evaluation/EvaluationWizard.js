"use strict";
function scroll_to_class(element_class, removed_height) {
    var scroll_to = $(element_class).offset().top - removed_height;
    if ($(window).scrollTop() != scroll_to) {
        $('.form-wizard').stop().animate({ scrollTop: scroll_to }, 0);
    }
}

function bar_progress(progress_line_object, direction) {
    var number_of_steps = progress_line_object.data('number-of-steps');
    var now_value = progress_line_object.data('now-value');
    var new_value = 0;
    if (direction == 'right') {
        new_value = now_value + (100 / number_of_steps);
    }
    else if (direction == 'left') {
        new_value = now_value - (100 / number_of_steps);
    }
    progress_line_object.attr('style', 'width: ' + new_value + '%;').data('now-value', new_value);
}




jQuery(document).ready(function () {
  
   
    $("#AskforSubQuestionDiv").hide();
    $("#formPreviewSection").hide();
    $('#MainAwnser').multiselect({
        includeSelectAllOption: false,
        enableCaseInsensitiveFiltering: true,
        enableFiltering: true,
        buttonWidth: '540px',
        maxHeight: 200,
    })
    $('#SubAnswer').multiselect({
        includeSelectAllOption: false,
        enableCaseInsensitiveFiltering: true,
        enableFiltering: true,
        buttonWidth: '540px',
        maxHeight: 200,
    });

    /*
        Form
    */
    $('.form-wizard fieldset:first').fadeIn('slow');

    $('.form-wizard .required').on('focus', function () {
        $(this).removeClass('input-error');
    });

    // next step
    $('.form-wizard .btn-next').on('click', function () {
        var parent_fieldset = $(this).parents('fieldset');
        var next_step = true;
        // navigation steps / progress steps
        var current_active_step = $(this).parents('.form-wizard').find('.form-wizard-step.active');
        var progress_line = $(this).parents('.form-wizard').find('.form-wizard-progress-line');

        // fields validation
        parent_fieldset.find('.required').each(function () {
            if ($(this).val() == "") {
                $(this).addClass('input-error');
                next_step = false;
            }
            else {
                $(this).removeClass('input-error');
            }
        });
        // fields validation

        if (next_step) {
            parent_fieldset.fadeOut(400, function () {
                // change icons
                current_active_step.removeClass('active').addClass('activated').next().addClass('active');
                // progress bar
                bar_progress(progress_line, 'right');
                // show next step
                $(this).next().fadeIn();
                // scroll window to beginning of the form
                scroll_to_class($('.form-wizard'), 20);
            });
        }

    });

    // previous step
    $('.form-wizard .btn-previous').on('click', function () {
        // navigation steps / progress steps
        var current_active_step = $(this).parents('.form-wizard').find('.form-wizard-step.active');
        var progress_line = $(this).parents('.form-wizard').find('.form-wizard-progress-line');

        $(this).parents('fieldset').fadeOut(400, function () {
            // change icons
            current_active_step.removeClass('active').prev().removeClass('activated').addClass('active');
            // progress bar
            bar_progress(progress_line, 'left');
            // show previous step
            $(this).prev().fadeIn();
            // scroll window to beginning of the form
            scroll_to_class($('.form-wizard'), 20);
        });
    });

    // submit
    $('.form-wizard').on('submit', function (e) {

        // fields validation
        $(this).find('.required').each(function () {
            if ($(this).val() == "") {
                e.preventDefault();
                $(this).addClass('input-error');
            }
            else {
                $(this).removeClass('input-error');
            }
        });
        // fields validation

    });


});



var $dropzone = $('.image_picker'),
    $droptarget = $('.drop_target'),
    $dropinput = $('#inputFile'),
    $dropimg = $('.image_preview'),
    $remover = $('[data-action="remove_current_image"]');

$dropzone.on('dragover', function () {
    $droptarget.addClass('dropping');
    return false;
});

$dropzone.on('dragend dragleave', function () {
    $droptarget.removeClass('dropping');
    return false;
});

$dropzone.on('drop', function (e) {
    $droptarget.removeClass('dropping');
    $droptarget.addClass('dropped');
    $remover.removeClass('disabled');
    e.preventDefault();

    var file = e.originalEvent.dataTransfer.files[0],
        reader = new FileReader();

    reader.onload = function (event) {
        $dropimg.css('background-image', 'url(' + event.target.result + ')');
    };

    console.log(file);
    reader.readAsDataURL(file);

    return false;
});

$dropinput.change(function (e) {
    $droptarget.addClass('dropped');
    $remover.removeClass('disabled');
    $('.image_title input').val('');

    var file = $dropinput.get(0).files[0],
        reader = new FileReader();

    reader.onload = function (event) {
        $dropimg.css('background-image', 'url(' + event.target.result + ')');
    }

    reader.readAsDataURL(file);
});

$remover.on('click', function () {
    $dropimg.css('background-image', '');
    $droptarget.removeClass('dropped');
    $remover.addClass('disabled');
    $('.image_title input').val('');
});

$('.image_title input').blur(function () {
    if ($(this).val() != '') {
        $droptarget.removeClass('dropped');
    }
});



var QuestionList;
var FormPreviewHtml;
var FinalPreviewList = [];
//var GetFormId = function () {
//    var BillingCategoryId = $("#BillingCategoryId").val();
//    var Name = $("#FormName").val();
//    if (BillingCategoryId.trim() != "" && Name.trim() != "") {
//        //if ($("#HiddenEvaluationFormId").val() == "") { 
//        $.ajax({
//            type: "Post",
//            url: "/Evaluation/getEvaluationFormId",
//            data: { BillingCategoryId: BillingCategoryId, Name: Name },
//            success: function (data) {
//                if (data != 0) {
                 
//                    $("#HiddenEvaluationFormId").val(data);
//                    $("#formTitle").text($("#FormName").val());
//                }

//            },
//            error: function (error) {
//                alert('failed');
//            }
//        })
//        //}
//        //} else {

//        //    return false;
//        //}
//    }
//}

$("input[name='askSubQuestion']").change(function () {
    $(".addLivePreviewbtn").attr("disabled", true);
    if ($("#MainQuestion").val() != "" && $("input[name='askSubQuestion']:checked").val() == null) {
        $("#MainAnswerTypeDiv").show();
        $("#mainAnswerDiv").show();
        $("#subQuestionDiv").hide();
        $("#SubAnswerTypeDiv").hide();
    } else {
        ResetSubQuestion();
        $("#MainAnswerTypeDiv").hide();
        $("#mainAnswerDiv").hide();
        $("#MainAwnser").val("");
        $("#MainAwnser").multiselect('refresh');
        if ($("#MainQuestion").val() != "" && $("input[name='askSubQuestion']:checked").val() != null) {
            $("#subQuestionDiv").show();
            //$("#SubAnswerTypeDiv").show();
        } else {
            $("#subQuestionDiv").hide();
            $("#SubAnswerTypeDiv").hide();
        }

    }
    var currentValue = $("input[name='askSubQuestion']:checked").val();
    if (currentValue != null && currentValue != "text") {
       
    } else {
       
        $("#MainAwnser").val("");
        $("#MainAwnser").multiselect('refresh');

    }

    var MainQuestionId = $("#MainQuestion :selected").val();
     FinalPreviewList = FinalPreviewList.filter(x => x.QuestionId != MainQuestionId);
})




$("input[name='AnswerType']").change(function () {
    var currentValue = $("input[name='AnswerType']:checked").val();
    var MainAnswers = $("#MainAwnser :selected").val()

    if (currentValue != null && MainAnswers != null && currentValue != "text") {
        $(".addLivePreviewbtn").attr("disabled", false);
    } else {
        $(".addLivePreviewbtn").attr("disabled", true);

    }

    if (currentValue != null && currentValue != "text") {

        $("#mainAnswerDiv").show();
        $(".IncludeDateMain").show();
    } else {
        $(".IncludeDateMain").hide();
        $("input[name='IncludeDateMain']:checked").prop('checked', false);
        $("#mainAnswerDiv").hide();
        $("#MainAwnser").val("");
        $("#MainAwnser").multiselect('refresh');
        $(".addLivePreviewbtn").attr("disabled", false);
    }

    
})
$("input[name='SubAnswerType']").change(function () {
    var currentValue = $("input[name='SubAnswerType']:checked").val();
    if (currentValue != null && currentValue != "text") {
        $("#SubAnswerDiv").show();
        $(".IncludeDateSub").show();
    } else {
        $("#SubAnswerDiv").hide();
        $("#SubAnswer").val("");
        $(".IncludeDateSub").hide();
        $("input[name='IncludeDatesub']:checked").prop('checked', false);
        $("#SubAnswer").multiselect('refresh');

    }
})
$(document).on('click', '#MainQuestion', function () {
    $(".tickmark").each(function () {
        $($(this)).attr('disabled', 'disabled');
            //.siblings().removeAttr('disabled');
    })
 
});
$(document).on('change', '#MainQuestion', function () {

    if ($(this).val() != "") {
        $("#AskforSubQuestionDiv").show();

        //$('#MainQuestion :selected').addClass("tickmark");
        //$(".tickmark").each(function () {
        //    $($(this)).attr('disabled', 'disabled');
        //    //.siblings().removeAttr('disabled');
        //})

        
        $("#formPreviewSection").show();
    } else {
        $("#AskforSubQuestionDiv").hide();
        $("#formPreviewSection").hide();
    }
 
    if ($("#MainQuestion").val() != "" && $("input[name='askSubQuestion']:checked").val() == null) {
        $("#MainAnswerTypeDiv").show();
    } else {
        $("#MainAnswerTypeDiv").hide();
    }
    $("#MainQuestion").attr("disabled", true);
});

$("#MainAwnser").change(function () {
    var AnswerType = $("input[name='AnswerType']:checked").val();
    var currentValue = $("#MainAwnser :selected").val()
    if (currentValue != null && AnswerType != null && currentValue != "text") {
        $(".addLivePreviewbtn").attr("disabled", false);
    } else {
        $(".addLivePreviewbtn").attr("disabled", true);

    }
})

$("#MainAwnser ,input[name='IncludeDateMain']").change(function () {
    var MainQuestionId = $("#MainQuestion :selected").val();
    var haveSubQuestion = $("input[name='askSubQuestion']:checked").val();
    var MainAwnser = $("#MainAwnser").val();
    var MainQuestion = $("#MainQuestion :selected").text();
    var haveDateTime = "no";
    var AnswerType = $("input[name='AnswerType']:checked").val();
    var exist = FinalPreviewList.find(x => x.QuestionId === MainQuestionId);
    if (exist != null) {
        FinalPreviewList = FinalPreviewList.filter(y => y.QuestionId != MainQuestionId);
    }
    var FormId = $("#HiddenEvaluationFormId").val();
    if (AnswerType != "text" && haveSubQuestion == null) {
        var answerList = [];
        $("#MainAwnser option:selected").each(function () {
          
                var ans = {
                    AnswerId:$(this).val(),
                    Answer:$(this).text()
                }
                answerList.push(ans);
         
        });

        if ($("input[name='IncludeDateMain']:checked").val() != null) {
        
            haveDateTime = "yes";
        }
        var obj = {
            QuestionId: MainQuestionId,
            MainQuestion: MainQuestion,
            haveSubQuestion:"no" ,
            AnswerType: AnswerType,
            MainAnswer: answerList,
            sortIndex: FinalPreviewList.length,
            haveDateTime: haveDateTime
        }
        FinalPreviewList.push(obj);
        console.log(FinalPreviewList);
        GetFormPreview();
    }
    
})
$("input[name='AnswerType']").change(function () {
    var MainQuestionId = $("#MainQuestion :selected").val();
    var haveSubQuestion = $("input[name='askSubQuestion']:checked").val();
    var MainAwnser = $("#MainAwnser").val();
    var MainQuestion = $("#MainQuestion :selected").text();

    var AnswerType = $("input[name='AnswerType']:checked").val();
    var exist = FinalPreviewList.find(x => x.QuestionId === MainQuestionId);
    if (exist != null) {
        FinalPreviewList = FinalPreviewList.filter(y => y.QuestionId != MainQuestionId);
    }
     //Case 1 With No SubQuestion and Type Text;
    var FormId = $("#HiddenEvaluationFormId").val();
    if (AnswerType == "text") {
        var obj = {
            QuestionId: MainQuestionId,
            MainQuestion: MainQuestion,
            haveSubQuestion: "no",
            subQuestions: [],
            AnswerType: "text",
            MainAnswer: [],
            sortIndex: FinalPreviewList.length,
              haveDateTime: "no"
        }
        FinalPreviewList.push(obj);
        console.log(FinalPreviewList);
        GetFormPreview();
      
    }
})

$("input[name='SubAnswerType']").change(function () {
    var currentValue = $("input[name='SubAnswerType']:checked").val();
    var MainAnswers = $("#SubQuestion :selected").val()

    if (currentValue != null && MainAnswers != null && currentValue == "text") {
        $(".addLivePreviewbtn").attr("disabled", false);
        $("#addNewSubQuestionBtn").attr("disabled", false);
    } else {
        $(".addLivePreviewbtn").attr("disabled", true);
        $("#addNewSubQuestionBtn").attr("disabled", true);


    }
})
$("#SubQuestion").change(function () {
    if ($("#SubQuestion").val() != "") {
        //$("#addNewSubQuestionBtn").attr("disabled", false);
        $("#SubQuestion").attr("disabled", true);
        //$('#SubQuestion :selected').addClass("tickmark");
        $("#SubAnswerTypeDiv").show();
    } else {
        $("#addNewSubQuestionBtn").attr("disabled", true);
        $("#SubAnswerTypeDiv").hide();

    }
})


//for text type
$("input[name='SubAnswerType']").change(function () {
    var MainQuestionId = $("#MainQuestion :selected").val();
    var haveSubQuestion = $("input[name='askSubQuestion']:checked").val();
    var MainAwnser = $("#MainAwnser").val();
    var MainQuestion = $("#MainQuestion :selected").text();
    var AnswerType = $("input[name='SubAnswerType']:checked").val();

    var subQuestionsList = [];
    var exist = FinalPreviewList.find(x => x.QuestionId === MainQuestionId);
    if (exist != null && AnswerType == "text") {
        var subQuestionExist = exist.subQuestions.find(x => x.QuestionId === $("#SubQuestion :selected").val());
        if (subQuestionExist == null) {
            if (AnswerType == "text" && haveSubQuestion != null) {

                var subQuestion = {
                    QuestionId: $("#SubQuestion :selected").val(),
                    Question: $("#SubQuestion :selected").text(),
                    AnswerType: AnswerType,
                    answers: [],
                    haveDateTime: "no"
                }
                var newdata = exist.subQuestions;
                newdata.push(subQuestion);
                subQuestionsList = newdata;
            }
        } else {
            exist = exist.subQuestions.filter(x => x.QuestionId != $("#SubQuestion :selected").val());
            var subQuestion = {
                QuestionId: $("#SubQuestion :selected").val(),
                Question: $("#SubQuestion :selected").text(),
                AnswerType: AnswerType,
                answers: [],
                haveDateTime: "no"
            }
            exist.push(subQuestion);
            subQuestionsList = exist;
        }
        //FinalPreviewList = FinalPreviewList.filter(y => y.QuestionId != MainQuestionId);
    } else {
        
    }
 
    var FormId = $("#HiddenEvaluationFormId").val();
    if (AnswerType == "text" && haveSubQuestion != null) {
        var QuestionExist = FinalPreviewList.find(x => x.QuestionId === MainQuestionId);
        if (QuestionExist == null) {
            subQuestionsList = [];

            var subQuestion = {
                QuestionId: $("#SubQuestion :selected").val(),
                Question: $("#SubQuestion :selected").text(),
                AnswerType: AnswerType,
                answers: [],
                haveDateTime: "no"
            }
            subQuestionsList.push(subQuestion);

            var obj = {
                QuestionId: MainQuestionId,
                MainQuestion: MainQuestion,
                haveSubQuestion: "yes",
                subQuestions: subQuestionsList,
                AnswerType: "none",
                MainAnswer: [],
                sortIndex: FinalPreviewList.length,
                  haveDateTime: "no"
            }
        } else {
            FinalPreviewList = FinalPreviewList.filter(x => x.QuestionId != MainQuestionId)

            var obj = {
                QuestionId: MainQuestionId,
                MainQuestion: MainQuestion,
                haveSubQuestion: "yes",
                subQuestions: subQuestionsList,
                AnswerType: "none",
                MainAnswer: [],
                sortIndex: FinalPreviewList.length,
                haveDateTime: "no"
            }
        }
        FinalPreviewList.push(obj);
        console.log(FinalPreviewList);
        GetFormPreview();

    }
})

//for  radio and checkBox Type

    $("#SubAnswer").change(function () {

        var currentValue = $("input[name='SubAnswerType']:checked").val();
        var subAnswers = $("#SubAnswer :selected").val()

        if (currentValue != null && subAnswers != null && currentValue != "text") {
            $(".addLivePreviewbtn").attr("disabled", false);
            $("#addNewSubQuestionBtn").attr("disabled", false);
        } else {
            $(".addLivePreviewbtn").attr("disabled", true);
            $("#addNewSubQuestionBtn").attr("disabled", true);


        }
    })
$("#SubAnswer , input[name='IncludeDatesub']").change(function () {

    var MainQuestionId = $("#MainQuestion :selected").val();
    var haveSubQuestion = $("input[name='askSubQuestion']:checked").val();
    var MainAwnser = $("#MainAwnser").val();
    var MainQuestion = $("#MainQuestion :selected").text();
    var AnswerType = $("input[name='SubAnswerType']:checked").val();
    var subQuestionsList = [];
    var haveDateTime = "no";
    if ($("input[name='IncludeDatesub']:checked").val() != null) {
        haveDateTime = "yes";
    }
    var exist = FinalPreviewList.find(x => x.QuestionId === MainQuestionId);
    if (exist != null && AnswerType != "text") {
        var subQuestionExist = exist.subQuestions.find(x => x.QuestionId === $("#SubQuestion :selected").val());
        if (subQuestionExist == null) {
            if (AnswerType != "text" && haveSubQuestion != null) {

                var answerList = [];
                $("#SubAnswer option:selected").each(function () {

                    var ans = {
                        AnswerId: $(this).val(),
                        Answer: $(this).text(),
                        type: AnswerType
                    }
                    answerList.push(ans);

                });
                var subQuestion = {
                    QuestionId: $("#SubQuestion :selected").val(),
                    Question: $("#SubQuestion :selected").text(),
                    AnswerType: AnswerType,
                    answers: answerList,
                    type: AnswerType,
                    haveDateTime: haveDateTime
                }
                var newdata = exist.subQuestions;
                newdata.push(subQuestion);
                subQuestionsList = newdata;
            }
        } else {
            exist = exist.subQuestions.filter(x => x.QuestionId != $("#SubQuestion :selected").val());
            var answerList = [];
            $("#SubAnswer option:selected").each(function () {

                var ans = {
                    AnswerId: $(this).val(),
                    Answer: $(this).text(),
                    type: AnswerType
                }
                answerList.push(ans);

            });
            var subQuestion = {
                QuestionId: $("#SubQuestion :selected").val(),
                Question: $("#SubQuestion :selected").text(),
                AnswerType: AnswerType,
                answers: answerList,
                haveDateTime: haveDateTime
            }
            exist.push(subQuestion);
            subQuestionsList = exist;
        }
    } else {

    }
    var FormId = $("#HiddenEvaluationFormId").val();
    if (AnswerType != "text" && haveSubQuestion != null) {
        var QuestionExist = FinalPreviewList.find(x => x.QuestionId === MainQuestionId);
        if (QuestionExist == null) {
            subQuestionsList = [];
            var answerList = [];
            $("#SubAnswer option:selected").each(function () {

                var ans = {
                    AnswerId: $(this).val(),
                    Answer: $(this).text(),
                    type: AnswerType
                }
                answerList.push(ans);

            });
            var subQuestion = {
                QuestionId: $("#SubQuestion :selected").val(),
                Question: $("#SubQuestion :selected").text(),
                AnswerType: AnswerType,
                answers: answerList,
                haveDateTime: haveDateTime
            }
            subQuestionsList.push(subQuestion);

            var obj = {
                QuestionId: MainQuestionId,
                MainQuestion: MainQuestion,
                haveSubQuestion: "yes",
                subQuestions: subQuestionsList,
                AnswerType: "none",
                MainAnswer: [],
                sortIndex: FinalPreviewList.length,
                haveDateTime: "no"
            }
        } else {
            FinalPreviewList = FinalPreviewList.filter(x => x.QuestionId != MainQuestionId)
            var answerList = [];
            $("#SubAnswer option:selected").each(function () {

                var ans = {
                    AnswerId: $(this).val(),
                    Answer: $(this).text(),
                    type: AnswerType
                }
                answerList.push(ans);

            });
            var obj = {
                QuestionId: MainQuestionId,
                MainQuestion: MainQuestion,
                haveSubQuestion: "yes",
                subQuestions: subQuestionsList,
                AnswerType: "none",
                MainAnswer: [],
                sortIndex: FinalPreviewList.length,
                haveDateTime: "no"
            }
        }
        FinalPreviewList.push(obj);
        console.log(FinalPreviewList);
        GetFormPreview();

    }

})
var PreviousType = "";
const SubIndex = ["a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z"];
var GetFormPreview = function () {
    var ul = document.createElement("ul");
    ul.setAttribute("class", "list-unstyled");
    FormPreviewHtml = null
    $("#formPreview").html("");
    var titleLi = document.createElement('li');
    var h1 = document.createElement('h1');
    h1.innerText = $("#FormName").val();
    h1.setAttribute("class", "text-center");
    titleLi.appendChild(h1);
    titleLi.setAttribute("id", "formTitle");
    ul.appendChild(titleLi);
    var z = 0;
    for (var i = 0; i < FinalPreviewList.length; i++) {
        if (FinalPreviewList[i].haveSubQuestion == "no" && FinalPreviewList[i].AnswerType == "text") {
            // Text Types added un UL 
            var li = document.createElement("li");
            var input = document.createElement("INPUT");
            var div = document.createElement('div');
            var label = document.createElement('label');
            div.setAttribute("class", "form-group");
            label.setAttribute("class", "");
            label.innerText = i + 1 + ") " + FinalPreviewList[i].MainQuestion;
            label.setAttribute("class", "mainQuestion-label");
            input.setAttribute("type", "text");
            input.setAttribute("class", "form-control");
            div.appendChild(label);
            div.appendChild(input);
            li.appendChild(div);
            ul.appendChild(li);
            $("#formPreview").append(ul);
        }

        if (FinalPreviewList[i].haveSubQuestion == "no" && FinalPreviewList[i].AnswerType != "text") {
            var li = document.createElement("li");
            var questionDiv = document.createElement('div');
            var br = document.createElement('br');
            questionDiv.setAttribute("class", "form-group");
            var questionLabel = document.createElement('label');
            questionLabel.innerText = i + 1 + ") " + FinalPreviewList[i].MainQuestion;
            questionLabel.setAttribute("class","mainQuestion-label")
            questionDiv.appendChild(questionLabel);
            li.appendChild(questionDiv);
            var outerDiv = document.createElement('div');
            outerDiv.setAttribute("class", "form-group");
        
            for (var j = 0; j < FinalPreviewList[i].MainAnswer.length; j++) {
              
                var innerDiv = document.createElement('div');
                if (FinalPreviewList[i].AnswerType == "radio") {
                    innerDiv.setAttribute("class", "dynamic-inputs");
                } else {
                    innerDiv.setAttribute("class", "dynamic-inputs-checkbox");
                }

                var label = document.createElement('label');
                var input = document.createElement("INPUT");
                label.innerText = FinalPreviewList[i].MainAnswer[j].Answer;
                input.setAttribute("type", FinalPreviewList[i].AnswerType);
                //input.setAttribute("class", "form-control");
                if (FinalPreviewList[i].AnswerType == "radio") {
                    innerDiv.appendChild(label);
                    innerDiv.appendChild(input);
                } else {
                    innerDiv.appendChild(input);
                    innerDiv.appendChild(label);
                }

                outerDiv.appendChild(innerDiv);
               
            }
            if (FinalPreviewList[i].haveDateTime == "yes") {
                var dateRow = document.createElement('div');
                dateRow.setAttribute("class", "row");
                var dateDiv = document.createElement('div');
                dateDiv.setAttribute("class", "col-md-4");
                var dateLabel = document.createElement('label');
                dateLabel.innerText = "Select Date";
                var dateInput = document.createElement("INPUT");
                dateInput.setAttribute("class", "datepicker");
                dateDiv.appendChild(dateLabel);
                dateDiv.appendChild(dateInput);
                dateRow.appendChild(dateDiv)
                outerDiv.appendChild(dateRow)
            }
            
            li.appendChild(outerDiv);
            ul.appendChild(li);
            $("#formPreview").append(ul);
            //UpdateDatePicker();


        }

        if (FinalPreviewList[i].haveSubQuestion == "yes" && FinalPreviewList[i].AnswerType == "none" && FinalPreviewList[i].subQuestions.length > 0 && FinalPreviewList[i].MainAnswer.length == 0) {
                
            var li = document.createElement("li");
            var MainquestionDiv = document.createElement('div');
            MainquestionDiv.setAttribute("class", "form-group");
            var MainquestionLabel = document.createElement('label');
            MainquestionLabel.innerText = i + 1 + ") " + FinalPreviewList[i].MainQuestion;
            MainquestionLabel.setAttribute("class", "mainQuestion-label");
            MainquestionDiv.appendChild(MainquestionLabel);
            li.appendChild(MainquestionDiv);
            var outerDiv = document.createElement('div');
            outerDiv.setAttribute("class", "form-group subQuestion-margin");
            for (var j = 0; j < FinalPreviewList[i].subQuestions.length; j++) {
     
                if (FinalPreviewList[i].subQuestions[j].AnswerType == "text") {
                   
                        var label = document.createElement('label');
                        var input = document.createElement("INPUT");
                    label.innerText = SubIndex[j]+") " + FinalPreviewList[i].subQuestions[j].Question;
                    label.setAttribute("class", "subQuestion-label");
                    input.setAttribute("type", FinalPreviewList[i].subQuestions[j].AnswerType);
                        input.setAttribute("class", "form-control");
                        outerDiv.appendChild(label);
                        outerDiv.appendChild(input);
                } else {

                        var label = document.createElement('label');
                    label.innerText = SubIndex[j] + ") " + FinalPreviewList[i].subQuestions[j].Question;
                    label.setAttribute("class", "subQuestion-label");
                    outerDiv.appendChild(label);
                   
                    for (var k = 0; k < FinalPreviewList[i].subQuestions[j].answers.length; k++) {
                        var elementdiv = document.createElement('div');
                        if (FinalPreviewList[i].subQuestions[j].answers[k].type=="radio") {
                            elementdiv.setAttribute("class", "dynamic-inputs");
                        } else {
                            elementdiv.setAttribute("class", "dynamic-inputs-checkbox");
                        }
                        
                            var label = document.createElement('label');
                            var input = document.createElement("INPUT");
                            label.innerText = FinalPreviewList[i].subQuestions[j].answers[k].Answer;
                            input.setAttribute("type", FinalPreviewList[i].subQuestions[j].answers[k].type);
                        if (FinalPreviewList[i].subQuestions[j].answers[k].type == "radio") {

                            elementdiv.appendChild(input);
                            elementdiv.appendChild(label);
                        }
                        else {
                            elementdiv.appendChild(input);
                            elementdiv.appendChild(label);
                        }
                    
                        outerDiv.appendChild(elementdiv);
                    }
                    if (FinalPreviewList[i].subQuestions[j].haveDateTime == "yes") {
                        var dateRow = document.createElement('div');
                        dateRow.setAttribute("class", "row");
                        var dateDiv = document.createElement('div');
                        dateDiv.setAttribute("class", "col-md-4");
                        var dateLabel = document.createElement('label');
                        dateLabel.innerText = "Select Date";
                        var dateInput = document.createElement("INPUT");
                        dateInput.setAttribute("class", "datepicker");
                        dateDiv.appendChild(dateLabel);
                        dateDiv.appendChild(dateInput);
                        dateRow.appendChild(dateDiv)
                        outerDiv.appendChild(dateRow)
                    }
                }
                   
            }
            li.appendChild(outerDiv);
                ul.appendChild(li);
            $("#formPreview").append(ul);
          
            }
      
        }

    if (FinalPreviewList.length > 0) {
        $("#saveFormbtn").show();
    }
    UpdateDatePicker();
    }


$(document).on('click', '.addLivePreviewbtn', function () {
    $('#MainQuestion :selected').addClass("tickmark");
    $(".tickmark").each(function () {
        $($(this)).attr('disabled', 'disabled');
        //.siblings().removeAttr('disabled');
    })

    ResetMainQuestion();
});

var ResetMainQuestion = function(){

    $("#MainQuestion").val("");
    $("#AskforSubQuestionDiv").hide();
    $("#MainAnswerTypeDiv").hide();
    $("#mainAnswerDiv").hide();
    $("#subQuestionDiv").hide();
    $("#SubAnswerDiv").hide();
    $("#MainAwnser").val('');
    $("#SubQuestion").val('');
    $("#MainAwnser").multiselect('refresh');
    $("#SubAnswer").val('');
    $("#SubAnswer").multiselect('refresh');

    
    $("#MainQuestion").attr("disabled", false);
    $("#addNewSubQuestionBtn").attr("disabled", true);

    $("input[name='AnswerType']:checked").prop('checked', false);
    $("input[name='SubAnswerType']:checked").prop('checked', false);
    $("input[name='IncludeDateMain']:checked").prop('checked', false);
    $("input[name='IncludeDatesub']:checked").prop('checked', false);
    $("input[name='IncludeDateMain']:checked").prop('checked', false);
    $("#SubQuestion").attr("disabled", false);
    $(".addLivePreviewbtn").attr("disabled", true);
    if ($("input[name='askSubQuestion']:checked").val() != null) {
        $("#askSubQuestion").trigger('click');
    }

    $("#SubQuestion option").removeClass("Subtickmark");
}
$("#addNewSubQuestionBtn").click(function () {
    $('#SubQuestion :selected').addClass("Subtickmark");
    $(".Subtickmark").each(function () {
        $($(this)).attr('disabled', 'disabled');
    })
    ResetSubQuestion(); 
})

var ResetSubQuestion = function () {
    $("#SubAnswerDiv").hide();
    $("#SubAnswer").val('');
    $("#SubAnswer").multiselect('refresh');
    $("#SubQuestion").attr("disabled", false);
    $("#SubQuestion").val("");
    $("input[name='SubAnswerType']:checked").prop('checked', false);
    $("#addNewSubQuestionBtn").attr("disabled", true);
    $("input[name='IncludeDatesub']:checked").prop('checked', false);
    $(".IncludeDateSub").hide();
    $("#SubAnswerTypeDiv").hide();
    $(".addLivePreviewbtn").attr("disabled", true);


}


$("#saveFormbtn").click(function () {

    var BillingCategoryId = $("#BillingCategoryId").val();
   var Name = $("#FormName").val();
    console.log(FinalPreviewList);
    var FinalPreviewListStringify = JSON.stringify(FinalPreviewList);
    if (BillingCategoryId.trim() != "" && Name.trim() != "" && FinalPreviewList.length > 0) {
      
        $.ajax({
            type: "Post",
            url: "/Evaluation/SaveEvaluationForm",
            data: { BillingCategoryId: BillingCategoryId, formName: Name, formData: FinalPreviewListStringify },
            success: function (data) {
                swal({
                    title: 'Saved!',
                    text: 'Form Configuration Saved Successfully',
                    type: 'success',
                    icon: "success",
                    showCancelButton: false,
                    allowOutsideClick: false,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'ok',

                    //allowOutsideClick: false,
                    //showLoaderOnConfirm: false
                }).then(function () {
                    location.reload();
                });
            },


            error: function (error) {
                alert('failed');
            }
        })

    }
     
  
})

$("#nextbtnEvalutionForm").click(function () {

    $("#formTitle").text($("#FormName").val());
})
$("#removeCurrentQuestion").click(function () {

    $("#SubQuestion option").removeClass("Subtickmark");
    $("#SubQuestion option").attr("disabled", false);

    var MainQuestionId = $("#MainQuestion :selected").val();
    FinalPreviewList = FinalPreviewList.filter(x => x.QuestionId !== MainQuestionId)


    ResetMainQuestion();
    GetFormPreview();
})

$("#ResetSubQuestionBtn").click(function () {
    //$("#SubQuestion :selected").removeClass("Subtickmark");

    var MainQuestionId = $("#MainQuestion :selected").val();
    var mainQuestion = FinalPreviewList.find(x => x.QuestionId === MainQuestionId);
    if (mainQuestion != null) {
        if (mainQuestion.subQuestions.length > 0) {
            var subQuestion = mainQuestion.subQuestions.filter(x => x.QuestionId != $("#SubQuestion :selected").val());

            mainQuestion.subQuestions = subQuestion;
            FinalPreviewList = FinalPreviewList.filter(x => x.QuestionId !== MainQuestionId)
            FinalPreviewList.push(mainQuestion);
        }
    }

    ResetSubQuestion();
    GetFormPreview();
})
var UpdateDatePicker = function () {
    $(".datepicker").each(function () {
        $(this).datepicker({
            //uiLibrary: 'bootstrap'
        });
    })

 
}