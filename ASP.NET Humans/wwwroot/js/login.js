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


    $.ajax({
        type: "POST",
        url: "/Account/Login",
        data: model,

        success: function (response) {
            sessionStorage.setItem("login_reload", "true");
            location.reload();

        },
        failure: function (response) {
            FailToast();
            alert(response.responseText);
        },
        error: function (response) {
            FailToast();
            alert(response.responseText);
        }
    });
});
