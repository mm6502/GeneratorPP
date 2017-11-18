
$("#inputFile").change(function (e) {
    // toggle submit button 
    $("#uploadFile").prop("disabled", !($("#inputFile").val()));
    // fill in the filename
    var label = this.nextElementSibling,
        labelVal = label.innerHTML;

    var fileName;
    if (this.files && this.files.length > 1)
        fileName = (this.getAttribute("data-caption") || "").replace("{count}", this.files.length);
    else
        fileName = e.target.value.replace(/\\/g, "/").split("/").pop();

    if (fileName)
        label.querySelectorAll("span")[1].innerHTML = fileName;
    else
        label.innerHTML = labelVal;
});
