/* ws note - this customization was taken from jqtouch. it is used to override jquerymobile button ui to create ios style back button*/

$(document).ready(function () {
    iOSBStyleOverride();

    //update pointed back button style
    $('a').bind('click', function () {
        setTimeout(function () {
            iOSBStyleOverride();
        }, 250);
    });
});

function iOSBStyleOverride() {
    $("a.iosbackbutton").buttonMarkup({ corners: false });
    $("a.iosbackbutton > span").removeClass("ui-btn-inner");
    $("a.iosbackbutton").removeClass("ui-btn").removeClass("ui-btn-left").removeClass("ui-btn-up-a");
}
