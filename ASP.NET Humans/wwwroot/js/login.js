(function ($) {
    function Index() {
        var $this = this;
        function initialize() {

            $(".popup").on('click', function (e) {
                modelPopup(this);
            });

            function modelPopup(reff) {
                var url = $(reff).data('url');
                $.get(url).done();
            }
        }

        $this.init = function () {
            initialize();
        };
    }
    $(function () {
        var self = new Index();
        self.init();
    });
}(jQuery));



$("#login-btn").click(function () {
    var model = new Object();
    model.Email_Login = $("#Email_Login").val();
    model.Password = $("#Password").val();
    model.IsRemember = $("#IsRemember").val();
    if (model != null) {
        $.ajax({
            type: "POST",
            url: "/Account/Login",
            data: model,
            success: function (response) {
                location.reload();
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    }
});

function SuccessToast() {
    let t = new Toast({
        title: 'Success!',
        text: 'You have successfully logged in.',
        theme: 'success',
        autohide: true,
        interval: 5000
    });
    t._show();
    return false;
}

function FailToast() {
    let t = new Toast({
        title: 'Success!',
        text: 'Something went wrong',
        theme: 'danger',
        autohide: true,
        interval: 5000
    });
    t._show();
    return false;
}