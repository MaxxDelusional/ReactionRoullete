//Inspiration: https://github.com/muaz-khan/RecordRTC/blob/master/RecordRTC-to-ASPNETMVC/RecordRTC_to_ASPNETMVC/Views/RecordRTC/Index.cshtml

$(function () {

    var reactbutton = $("#React");
    var stopbutton = $("#Stop");
    
    var recorder;

    stopbutton.attr('disabled', true);

    reactbutton.click(function () {
        reactbutton.attr('disabled', true);
        

        navigator.getUserMedia({ audio: false, video: true }, function (stream) {
            recorder = RecordRTC(stream, { type: "video" });
            recorder.startRecording();

            stopbutton.attr('disabled', false);
        }, function (error) {
            alert(JSON.stringify(error, null, '    '));
            stopbutton.attr('disabled', false);
        });
    });

    stopbutton.click(function () {
        reactbutton.attr('disabled', false);
        stopbutton.attr('disabled', true);
    })
});