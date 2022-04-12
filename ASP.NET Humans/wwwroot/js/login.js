function ShowLoginModal() {
    $.ajax({
        type: "GET",
        url: "/Account/Login",
        success: function (response) {
            $("#LoginModal").find(".LoginModal").html(response);
            $("#LoginModal").modal('show');
        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
};

$("#login-btn").click(function () {
    var model = new Object();
    model.Email_Login = $("#Email_Login").val();
    model.Password = $("#Password").val();
    model.IsRemember = $("#IsRemember").val();
    model.ReturnUrl = $("#ReturnUrl").val();


    $.ajax({
        type: "POST",
        url: "/Account/Login",
        data: model,

        success: function (response) {
            sessionStorage.setItem("login_reload", "true");
          
            if (model.ReturnUrl != "") {
                location.href = model.ReturnUrl;
            }
            else {
                location.reload();
            }
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

