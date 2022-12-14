
// 文件加入队列出错时触发，包括大小限制，类型限制，空文件等均会触发 
function fileQueueError(file, errorCode, message) {
    if (errorCode == SWFUpload.QUEUE_ERROR.QUEUE_LIMIT_EXCEEDED) {
        alert("只能选择一个文件.");
        return;
    }
    if (!queueErrorArray) {
        queueErrorArray = [];
    }
    var errorFile = {
        file: file,
        code: errorCode,
        error: ''
    };
    switch (errorCode) {
        case SWFUpload.QUEUE_ERROR.FILE_EXCEEDS_SIZE_LIMIT:
            errorFile.error = '文件大小超出限制.';
            break;
        case SWFUpload.QUEUE_ERROR.INVALID_FILETYPE:
            errorFile.error = '只能上传图片类型的文件.';
            break;
        case SWFUpload.QUEUE_ERROR.ZERO_BYTE_FILE:
            errorFile.error = '文件为空文件.';
            break;
        default:
            alert('加载入队列出错.');
            break;
    }
    queueErrorArray.push(errorFile);
}

/** 
* 选择文件对话框关闭时触发,报告所选文件个数、加入上传队列文件数及上传队列文件总数 
* @param numSelected 选择的文件数目 
* @param numQueued 加入队列的文件数目 
* @param numTotalInQueued 上传文件队列中文件总数 
*/
function fileDialogComplete(numSelected, numQueued, numTotalInQueued) {
    var swfupload = this;
    if (queueErrorArray && queueErrorArray.length) {
        //        var table = $('<table><tr><td>文件</td><td>大小</td></tr></table>');
        //        for (var i in queueErrorArray) {
        //            var tr = $('<tr></tr>');
        //            var info = '<td>' + queueErrorArray[i].file.name + '<span style="color:red">('
        //                        + queueErrorArray[i].error + ')</span></td>'
        //                        + '<td>' + queueErrorArray[i].file.size + 'bytes</td>';
        //            table.append(tr.append(info));
        //        }
        alert(queueErrorArray[0].error);
        queueErrorArray = [];


    } else {
        this.startUpload();
    }
}

/** 
* 文件上传过程中定时触发,更新进度显示 
* @param file 上传的文件 
* @param bytesCompleted 已上传大小 
* @param bytesTotal 文件总大小 
*/
function uploadProgress(file, bytesLoaded) {

    try {
        var percent = Math.ceil((bytesLoaded / file.size) * 100);

        var progress = new FileProgress(file, this.customSettings.upload_target);
        progress.setProgress(percent);
        if (percent === 100) {
            progress.setStatus("开始上传...");
            progress.toggleCancel(false, this);
        } else {
            progress.setStatus("正在上传中...");
            progress.toggleCancel(true, this);
        }
    } catch (ex) {
        this.debug(ex);
    }
}


/** 
* 文件上传完毕并且服务器返回200状态码时触发,此时文件的上传周期并未完成, 
* 不能在此事件监听函数开始下一个文件的上传 
* @param file 上传的文件 
* @param serverData 服务器在执行完接收文件方法后返回的数据 
* @param response Boolean类型,表示是否服务器返回数据 
*/
function uploadTemplateBgSuccess(file, serverData) {
    try {
        var progress = new FileProgress(file, this.customSettings.upload_target);
        progress.setStatus("");
        $("#fmSrc").val(serverData);
       // var litterpic=$("<img>")
        $("#littlepic").attr("src", serverData);
        $("#littlepic").show();
        $("#delpic").show();
       // $("#templatetd").append(litterpic);
        progress.toggleCancel(false);

    } catch (ex) {
        this.debug(ex);
    }
}

function uploadSuccess(file, serverData) {
    try {
        //SearchImage.SearchImage(serverData);
        //  serverData = "../.."+serverData;
        var progress = new FileProgress(file, this.customSettings.upload_target);
        progress.setStatus("");
        $("#fmSrc").val(serverData);
        var smallimg = $("<img>");
//        var delimg = "<a id=\"delpic\" href='javascript:void(0)' onclick='RemovePic()'>删除</a>";
        $("#smallpic").empty();
        smallimg.attr("src", serverData);
        $("#smallpic").append(smallimg);
//        $("#smallpic").append(delimg);
        $("#smallpic").show();
        swfu.setButtonImageURL('../images/reUpload.jpg');
        progress.toggleCancel(false);

    } catch (ex) {
        this.debug(ex);
    }
}


/** 
* 在一个上传周期结束后触发(uploadError及uploadSuccess均触发) 
* 在此可以开始下一个文件上传(通过上传组件的uploadStart()方法) 
* @param file 上传完成的文件对象 
*/
function uploadComplete(file) {
    try {
        if (this.getStats().files_queued > 0) {
            this.startUpload();
        } else {
            var progress = new FileProgress(file, this.customSettings.upload_target);
            progress.setComplete();
            progress.toggleCancel(false);
        }
    } catch (ex) {
        this.debug(ex);
    }
}

/** 
* 文件上传被中断或是文件没有成功上传时会触发该事件均触发) 
* 在此可以开始下一个文件上传(通过上传组件的uploadStart()方法) 
* @param file 上传完成的文件对象 
*/
function uploadError(file, errorCode, message) {
    var progress;
    try {
        switch (errorCode) {
            case SWFUpload.UPLOAD_ERROR.FILE_CANCELLED:
                try {
                    progress = new FileProgress(file, this.customSettings.upload_target);
                    progress.setCancelled();
                    progress.setStatus("文件已取消");
                    progress.toggleCancel(false);
                    message = "文件已取消";
                }
                catch (ex1) {
                    this.debug(ex1);
                }
                break;
            case SWFUpload.UPLOAD_ERROR.UPLOAD_STOPPED:
                try {
                    progress = new FileProgress(file, this.customSettings.upload_target);
                    progress.setCancelled();
                    progress.setStatus("文件上传已停止");
                    progress.toggleCancel(true);
                    message = "文件上传已停止";
                }
                catch (ex2) {
                    this.debug(ex2);
                }
            case SWFUpload.UPLOAD_ERROR.UPLOAD_LIMIT_EXCEEDED:
                message = "文件上传大小出错";
                break;
        }
        alert(message);

    } catch (ex3) {
        this.debug(ex3);
    }

}



/* ******************************************
*	FileProgress Object
*	Control object for displaying file info
* ****************************************** */

function FileProgress(file, targetID) {
    this.fileProgressID = "divFileProgress";

    this.fileProgressWrapper = document.getElementById(this.fileProgressID);
    if (!this.fileProgressWrapper) {
        this.fileProgressWrapper = document.createElement("div");
        this.fileProgressWrapper.className = "progressWrapper";
        this.fileProgressWrapper.id = this.fileProgressID;

        this.fileProgressElement = document.createElement("div");
        this.fileProgressElement.className = "progressContainer";

        var progressCancel = document.createElement("a");
        progressCancel.className = "progressCancel";
        progressCancel.href = "#";
        progressCancel.style.visibility = "hidden";
        progressCancel.appendChild(document.createTextNode(" "));

        var progressText = document.createElement("div");
        progressText.className = "progressName";
        // progressText.appendChild(document.createTextNode(file.name));

        var progressBar = document.createElement("div");
        progressBar.className = "progressBarInProgress";

        var progressStatus = document.createElement("div");
        progressStatus.className = "progressBarStatus";
        progressStatus.innerHTML = "&nbsp;";
        this.fileProgressElement.appendChild(progressCancel);
        this.fileProgressElement.appendChild(progressText);
        this.fileProgressElement.appendChild(progressStatus);
        this.fileProgressElement.appendChild(progressBar);

        this.fileProgressWrapper.appendChild(this.fileProgressElement);

        document.getElementById(targetID).appendChild(this.fileProgressWrapper);
        fadeIn(this.fileProgressWrapper, 0);

    } else {
        this.fileProgressElement = this.fileProgressWrapper.firstChild;
        //  this.fileProgressElement.childNodes[1].firstChild.nodeValue = file.name;
    }

    this.height = this.fileProgressWrapper.offsetHeight;

}
FileProgress.prototype.setProgress = function (percentage) {
    this.fileProgressElement.className = "progressContainer green";
    this.fileProgressElement.childNodes[3].className = "progressBarInProgress";
    this.fileProgressElement.childNodes[3].style.width = percentage + "%";
};
FileProgress.prototype.setComplete = function () {
    this.fileProgressElement.className = "progressContainer blue";
    this.fileProgressElement.childNodes[3].className = "progressBarComplete";
    this.fileProgressElement.childNodes[3].style.width = "";

};
FileProgress.prototype.setError = function () {
    this.fileProgressElement.className = "progressContainer red";
    this.fileProgressElement.childNodes[3].className = "progressBarError";
    this.fileProgressElement.childNodes[3].style.width = "";

};
FileProgress.prototype.setCancelled = function () {
    this.fileProgressElement.className = "progressContainer";
    this.fileProgressElement.childNodes[3].className = "progressBarError";
    this.fileProgressElement.childNodes[3].style.width = "";

};
FileProgress.prototype.setStatus = function (status) {
    this.fileProgressElement.childNodes[2].innerHTML = status;
};

FileProgress.prototype.toggleCancel = function (show, swfuploadInstance) {
    this.fileProgressElement.childNodes[0].style.visibility = show ? "visible" : "hidden";
    if (swfuploadInstance) {
        var fileID = this.fileProgressID;
        this.fileProgressElement.childNodes[0].onclick = function () {
            swfuploadInstance.cancelUpload(fileID);
            return false;
        };
    }
};



function RemovePic() {

    var imgsrc = $("#fmSrc").val();
    //if (clientType == undefined) {var clientType = 0; }
    if (imgsrc != null && imgsrc != "undefined" && imgsrc != "") {
        $.ajax({
            url: "../UploadHandler.ashx",
            type: "POST",
            dataType: "text",
            data: { "action": "delete", "del": imgsrc},
            success: function (msg) {
                if (msg == "true") {
                    $("#smallpic").empty();
                    $("#littlepic").hide();
                    $("#fmSrc").val("");
                    $("#delpic").hide();
                }
                else {
                    alert("移除图片失败");
                }
            },
            error: function (xmlHttpRequest, error) {
                alert(error);
            }
        });
    }
}




function fadeIn(element, opacity) {
    var reduceOpacityBy = 5;
    var rate = 30; // 15 fps


    if (opacity < 100) {
        opacity += reduceOpacityBy;
        if (opacity > 100) {
            opacity = 100;
        }

        if (element.filters) {
            try {
                element.filters.item("DXImageTransform.Microsoft.Alpha").opacity = opacity;
            } catch (e) {
                // If it is not set initially, the browser will throw an error.  This will set it if it is not set yet.
                element.style.filter = 'progid:DXImageTransform.Microsoft.Alpha(opacity=' + opacity + ')';
            }
        } else {
            element.style.opacity = opacity / 100;
        }
    }

    if (opacity < 100) {
        setTimeout(function () {
            fadeIn(element, opacity);
        }, rate);
    }
}

