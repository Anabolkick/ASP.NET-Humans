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
                //(function (data) {
                //    $().modal("show");
                //});
            }


            $("#login-btn").click(function () {
                debugger;
                $("#LoginModalForm").submit();

            });
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