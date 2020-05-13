//$("input[type='file']").change(function () {
//    alert($(this).data('value'));
//    var variable = $(this).data('value');
//    var ListStringify = [];
//    for (var i = 0; i < $(this).get(0).files.length; i++) {
//        var Imgreader = new FileReader();


//        Imgreader.readAsDataURL($(this).get(0).files[i]);

//        Imgreader.onload = function (event) {
//            var image = new Image();
//            debugger;
//            image.onload = function () {
//                //document.getElementById("original-Img").src = image.src;
//                var canvas = document.createElement("canvas");
//                var context = canvas.getContext("2d");
//                canvas.width = image.width / 4;
//                canvas.height = image.height / 4;
//                context.drawImage(image,
//                    0,
//                    0,
//                    image.width,
//                    image.height,
//                    0,
//                    0,
//                    canvas.width,
//                    canvas.height
//                );


//                //document.getElementById("upload-Preview").src = canvas.toDataURL();
//                var ListStringifyOld = [];
//                if ($("#SaveImageDataForUplaodHidden").val() != "") {
//                    ListStringifyOld = JSON.parse($("#SaveImageDataForUplaodHidden").val());

//                }
//                var obj = {
//                    Variable: variable,
//                    ImageData: canvas.toDataURL()
//                }
//                ListStringifyOld.push(obj);

//                $("#SaveImageDataForUplaodHidden").val(JSON.stringify(ListStringifyOld));
                
//            }
//            image.src = event.target.result;

//        };

//    }

//});