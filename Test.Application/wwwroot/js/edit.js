const disableForm = function () {
    var elements = document.getElementById('editForm').elements;

    for (let i = 0; i < elements.length; i++) {
        elements[i].readOnly = true;
    }

    document.getElementById('saveButton').hidden = true;
    //document.getElementById('notifyButton').hidden = false;
}

const hideNotifyButton = function () {
    document.getElementById('notifyButton').hidden = true;
}