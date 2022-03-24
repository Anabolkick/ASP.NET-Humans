(function ($) {
    function Index() {
        var $this = this;
        function initialize() {

            $(".popup").on('click', function (e) {
                modelPopup(this);
            });

            function modelPopup(reff) {
                var url = $(reff).data('url');

                $.get(url).done(function (data) {
                    debugger;
                    $('#modal-create-edit-user').find(".modal-dialog").html(data);
                    $('#modal-create-edit-user > .modal', data).modal("show");
                });

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
                if (response != null) {
                    alert("Name : " + response.Name + ", Designation : " + response.Designation + ", Location :" + response.Location);
                } else {
                    alert("Something went wrong");
                }
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


//$('#login-btn').click(function () {
//    debugger;
//    $.ajax({
//        type: "POST",
//        url: '@Url.Action("Account/Login","LoginForm")',

//        //success: function (msg) {

//        //    alert("111 success");
//        //},
//        //error: function (req, status, error) {
//        //    alert(22225, error);
//        //}
//    })
//        .done(function () {
//            alert("success");
//        })
//        .fail(function () {
//            alert(44444, "error");
//        });
//    function LoginData() {

//        var data = {
//            Email_Login: $("#Email_Login").val(),
//            Password: $("#Password").val(),
//            IsRemember: $("#IsRemember").val()
//        };
//        addAntiForgeryToken(data);
//        return data;
//    }

//});
