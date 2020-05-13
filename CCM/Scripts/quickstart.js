//$(function () {
//    log('Requesting Capability Token...');
//    var device;
//  $.getJSON('/token')
//      .done(function (data) {
         
//      log('Got a token.');
//      console.log('Token: ' + data.token);

//      // Setup Twilio.Device
//      Twilio.Device.setup(data.token, { closeProtection:true });

//      Twilio.Device.ready(function (device) {
//        log('Twilio.Device Ready!');
//        document.getElementById('call-controls').style.display = 'block';
//      });

//      Twilio.Device.error(function (error) {
//        log('Twilio.Device Error: ' + error.message);
//      });

//      Twilio.Device.connect(function (conn) {
//          log('Successfully established call!');
//          $(".btnclosetwilio").hide(); 
//          $("#" + selectedbuttonid + "call").hide();
//          $("#" + selectedbuttonid + "hang").show();
//          $("#" + selectedbuttonid + "mute").show();
//          $("#" + selectedbuttonid + "unmute").show();
         
          
//        //document.getElementById('button-call').style.display = 'none';
//        //document.getElementById('button-hangup').style.display = 'inline';
//      });

//          Twilio.Device.disconnect(function (conn) {
//              log('Please wait Your call is saving now.');
//              var callid = conn.mediaStream.callSid;
//              var callto = conn.message.To;
//              setTimeout(function () {
//                  savecall(callid, callto);
//                  log('Call ended.');
//                  //$("#" + selectedbuttonid + "call").show();
//                  //$("#" + selectedbuttonid + "hang").hide();
//                  //$("#" + selectedbuttonid + "mute").hide();
//                  //$("#" + selectedbuttonid + "unmute").hide();
//                  //document.getElementById('button-call').style.display = 'inline';
//                  //document.getElementById('button-hangup').style.display = 'none';
//                  //document.getElementById('button-mute').style.display = 'none';
//                  //document.getElementById('button-unmute').style.display = 'none';
//                  window.location.reload(true);
//              }, 5000);

             
             
       
//      });
//    })

//    .fail(function () {
//      log('Could not get a token from server!');
//    });
//    $(".makeaphonecall").click(function () {
//        var tonumber = $(this).attr("data-number");
//        $("#txtPhoneNumber").val(tonumber);
//        var id = $(this).attr("data-id");
//        selectedbuttonid = id;
//        var params = {
//            To: tonumber
          
//        };

//        console.log('Calling ' + params.To + '...');
//        Twilio.Device.connect(params);
//    });
//    $(".textthisnumber").click(function () {
//        var tonumber = $(this).attr("data-number");
//        $("#txtPhoneNumber").val(tonumber);
        
//    });
    
//    $(".hangupphonecall").click(function () {
//        var tonumber = $(this).attr("data-number");
//        var id = $(this).attr("data-id");
//        selectedbuttonid = id;
//        log('Hanging up...');
//        Twilio.Device.disconnectAll();
        
//    });
//    $(".mute").click(function () {
//        var tonumber = $(this).attr("data-number");
//        var id = $(this).attr("data-id");
//        selectedbuttonid = id;
//        log('Call Muted...');
//        Twilio.Device.activeConnection().mute(true);

//    });
//    $(".unmute").click(function () {
//        var tonumber = $(this).attr("data-number");
//        var id = $(this).attr("data-id");
//        selectedbuttonid = id;
//        log('Call Un Muted...');
//        Twilio.Device.activeConnection().mute(false);

//    });
    
//  // Bind button to make call
//  document.getElementById('button-call').onclick = function () {
//    var params = {
//      To: document.getElementById('phone-number').value
//    };

//    console.log('Calling ' + params.To + '...');
//      Twilio.Device.connect(params);
//             document.getElementById('button-call').style.display = 'none';
//      document.getElementById('button-hangup').style.display = 'inline';
//      document.getElementById('button-mute').style.display = 'inline';
//      document.getElementById('button-unmute').style.display = 'inline';
//      $(".btnclosetwilio").hide(); 
//  };

//  // Bind button to hangup call
//  document.getElementById('button-hangup').onclick = function () {
//    log('Hanging up...');
//      Twilio.Device.disconnectAll();
//      document.getElementById('button-call').style.display = 'inline';
//      document.getElementById('button-hangup').style.display = 'none';
//      document.getElementById('button-mute').style.display = 'none';
//      document.getElementById('button-unmute').style.display = 'none';
     

//    };
//    document.getElementById('button-mute').onclick = function () {
//        log('Call Muted...');
//        Twilio.Device.activeConnection().mute(true);
        
//    };
//    document.getElementById('button-unmute').onclick = function () {
//        log('Call Un Muted...');
//        Twilio.Device.activeConnection().mute(false);

//    };

//});

//// Activity log
//function log(message) {
//  var logDiv = document.getElementById('log');
//  logDiv.innerHTML += '<p>&gt;&nbsp;' + message + '</p>';
//  logDiv.scrollTop = logDiv.scrollHeight;
//}
