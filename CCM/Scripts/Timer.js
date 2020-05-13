var h1 = document.getElementsByTagName('time')[0],
    seconds = 0, minutes = 0, hours = 0, t;

function add() {
    if ($("#ControlTimerHidden").data("value") == null || $("#ControlTimerHidden").data("value") == '') {
        seconds++;
        if (seconds >= 60) {
            seconds = 0;
            minutes++;
            if (minutes >= 60) {
                minutes = 0;
                hours++;
            }
        }

        h1.style.color = "White";
        $("#ccmTimer").css("color", "White");
        $("#ccmTimer").css("font-weight", "400");
    } else {
        h1.style.color = "red";
        $("#ccmTimer").css("color", "red");
        $("#ccmTimer").css("font-weight", "700");
    }
    h1.textContent = (hours ? (hours > 9 ? hours : "0" + hours) : "00") + ":" +
        (minutes ? (minutes > 9 ? minutes : "0" + minutes) : "00") + ":" +
        (seconds > 9 ? seconds : "0" + seconds);
    timer();


}

function timer() {
    t = setTimeout(add, 1000);
}

timer();