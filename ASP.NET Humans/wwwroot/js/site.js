function openField(input) {
    $(input).prop('readonly', false);
}


function FailToast(text = 'Something went wrong!') {
    let t = new Toast({
        title: 'Error!',
        text: text,
        theme: 'danger',
        autohide: true,
        interval: 5000
    });
    t._show();
    return false;
}


function SuccessToast(text = 'You have successfully logged in.') {
    let t = new Toast({
        title: 'Success!',
        text: text,
        theme: 'success',
        autohide: true,
        interval: 5000
    });
    t._show();
    return false;
}

function GetAntiForgeryToken() {
    var tokenField = $("input[type='hidden'][name$='RequestVerificationToken']");
    if (tokenField.length == 0) {
        return null;
    } else {
        return {
            name: tokenField[0].name,
            value: tokenField[0].value
        };
    }
}

$.ajaxPrefilter(
    function (options, localOptions, jqXHR) {
        if (options.type !== "GET") {
            var token = GetAntiForgeryToken();
            if (token !== null) {
                if (options.data.indexOf("X-Requested-With") === -1) {
                    options.data = "X-Requested-With=XMLHttpRequest" + ((options.data === "") ? "" : "&" + options.data);
                }
                options.data = options.data + "&" + token.name + '=' + token.value;
            }
        }
    }
);