﻿//Inspiration: https://github.com/muaz-khan/RecordRTC/blob/master/RecordRTC-to-ASPNETMVC/RecordRTC_to_ASPNETMVC/Views/RecordRTC/Index.cshtml

// PostBlob method uses XHR2 and FormData to submit 
// recorded blob to the PHP server
function PostBlob(blob, fileType, fileName) {
    // FormData
    var formData = new FormData();
    formData.append(fileType, fileName);
    formData.append(fileType + '-blob', blob);

    // progress-bar
    //var hr = document.createElement('hr');
    //container.appendChild(hr);
    //var strong = document.createElement('strong');
    //strong.id = 'percentage';
    //strong.innerHTML = fileType + ' upload progress: ';
    //container.appendChild(strong);
    //var progress = document.createElement('progress');
    //container.appendChild(progress);

    // POST the Blob using XHR2
    xhr(callbackurl, formData, null, null, function (response) {
        //container.appendChild(document.createElement('hr'));
        //var mediaElement = document.createElement(fileType);

        //var source = document.createElement('source');
        //source.src = location.href + 'uploads/' + fName.replace(/"/g, '');

        //if (fileType == 'video') source.type = 'video/webm; codecs="vp8, vorbis"';
        //if (fileType == 'audio') source.type = !!navigator.mozGetUserMedia ? 'audio/ogg' : 'audio/wav';

        //mediaElement.appendChild(source);

        //mediaElement.controls = true;
        //container.appendChild(mediaElement);
        //mediaElement.play();

        //progress.parentNode.removeChild(progress);
        //strong.parentNode.removeChild(strong);
        //hr.parentNode.removeChild(hr);

       
        var parsedResponse = JSON.parse(response);


        if (parsedResponse.success) {
            window.location = parsedResponse.redirectUrl;
        }


    });
}

var record = document.getElementById('React');
var stop = document.getElementById('Stop');
//var deleteFiles = document.getElementById('delete');

//var audio = document.querySelector('audio');

var recordVideo = document.getElementById('record-video');
var preview = document.getElementById('preview');

var container = document.getElementById('container');

// if you want to record only audio on chrome
// then simply set "isFirefox=true"
var isFirefox = !!navigator.mozGetUserMedia;

var recordAudio, recordVideo;
record.onclick = function () {
    record.disabled = true;
    navigator.getUserMedia({
        audio: false,
        video: true
    }, function (stream) {

        if (youtubePlayer) {

            playVideo();
        }


        preview.src = window.URL.createObjectURL(stream);
        preview.play();

        // var legalBufferValues = [256, 512, 1024, 2048, 4096, 8192, 16384];
        // sample-rates in at least the range 22050 to 96000.
        recordAudio = RecordRTC(stream, {
            //bufferSize: 16384,
            //sampleRate: 45000,
            onAudioProcessStarted: function () {
                if (!isFirefox) {
                    recordVideo.startRecording();
                }
            }
        });

        if (isFirefox) {
            recordAudio.startRecording();
        }

        if (!isFirefox) {
            recordVideo = RecordRTC(stream, {
                type: 'video'
            });
            recordAudio.startRecording();
        }

        stop.disabled = false;
    }, function (error) {
        alert(JSON.stringify(error, null, '\t'));
    });
};

var fileName;
stop.onclick = function () {


    if (youtubePlayer) {
        stopVideo();
    }


    record.disabled = false;
    stop.disabled = true;

    preview.src = '';

    fileName = Math.round(Math.random() * 99999999) + 99999999;

    if (!isFirefox) {
        //recordAudio.stopRecording(function () {
        //    PostBlob(recordAudio.getBlob(), 'audio', fileName + '.wav');
        //});
    } else {
        recordAudio.stopRecording(function (url) {
            preview.src = url;
            PostBlob(recordAudio.getBlob(), 'video', fileName + '.webm');
        });
    }

    if (!isFirefox) {
        recordVideo.stopRecording(function () {
            PostBlob(recordVideo.getBlob(), 'video', fileName + '.webm');
        });
    }

};


function xhr(url, data, progress, percentage, callback) {
    var request = new XMLHttpRequest();
    request.onreadystatechange = function () {
        if (request.readyState == 4 && request.status == 200 && callback) {
            callback(request.responseText);
        }
    };

    if (url.indexOf('/RecordRTC/DeleteFile') == -1) {
        if (percentage) {
            request.upload.onloadstart = function () {
                percentage.innerHTML = 'Upload started...';
            };
        }

        if (progress) {
            request.upload.onprogress = function (event) {
                progress.max = event.total;
                progress.value = event.loaded;
                percentage.innerHTML = 'Upload Progress ' + Math.round(event.loaded / event.total * 100) + "%";
            };
        }

        if (percentage) {
            request.upload.onload = function () {
                percentage.innerHTML = 'Saved!';
            };
        }
    }

    request.open('POST', url);
    request.send(data);




}

window.onbeforeunload = function () {
    if (!!fileName) {
        //deleteAudioVideoFiles();



        //return 'It seems that you\'ve not deleted audio/video files from the server.';
    }
};

navigator.getUserMedia({
    audio: false,
    video: true
}, function (stream) { }, function (error) { });