//Responsive Quote Carousel
$(document).ready(function () {


    checkSize();
    $(window).resize(checkSize);
    //Set the carousel options
    $('#quote-carousel').carousel({
        pause: true,
        interval: 4000,
    });
    if (document.URL.indexOf("ReturnUrl") !== -1 && document.URL.indexOf("Account/Manage") === -1) openLoginModal();

    $("#filter-tb").on("change", function (e) {
        $("#state-filter").children(".checkbox-container").each(function (index, item) {
            if (!$(item).attr('state').toLowerCase().includes($("#filter-tb").val()))
                $(item).hide();
            else
                $(item).show();
        });
    });

    $(".checkbox-container input").change(function () {
        applyFilters();
    });

    $(".loginBox input").keypress(function (e) {
        if (e.which === 10 || e.which === 13) {
            loginAjax();
        }
    });

    $(".registerBox input").keypress(function (e) {
        if (e.which === 10 || e.which === 13) {
            registerAjax();
        }
    });

    $(".forgetBox input").keypress(function (e) {
        if (e.which === 10 || e.which === 13) {
            forgotAjax();
        }
    });

    $("a.compact-fav").click(function () {
        var elm = $(this);
        $.get({
            url: elm.attr("link"),
            data: {
                AuctionId: $(this).attr("AuctionId")
            },
            success: function (data) {
                if (data === "added") {
                    $("body").overhang({
                        type: "success",
                        message: "Added to your favorites",
                        duration: 1
                    });
                    elm.html('<i class="fa fa-heart" aria-hidden="true"></i>');
                } else {
                    $("body").overhang({
                        type: "success",
                        message: "Removed from your favorites",
                        duration: 1
                    });
                    elm.html('<i class="fa fa-heart-o" aria-hidden="true"></i>');
                }
            }
        }).fail(function (data) {
            $("body").overhang({
                type: "error",
                message: "Failed to add to your favorites",
                duration: 1
            });
        });
    });

    window.page = 1;
    window.loading = false;

    $(window).on('scroll', function () {
        var compact = $("#drop7").siblings().find("li a.selected").attr("displaykey") === "Compact";
        var pos = compact ? 200 : 800;
        if (!window.loading && $(window).scrollTop() > $(document).height() - $(window).height() - pos) {
            window.loading = true;
            $.post({
                url: "/Auctions/IndexAjax",
                data: {
                    skip: window.page * 20,
                    cbmin: $(".currentbid-filter-min").val(),
                    cbmax: $(".currentbid-filter-max").val(),
                    mbmin: $(".minbid-filter-min").val(),
                    mbmax: $(".minbid-filter-max").val(),
                    states: $(".state-checkbox-container input:checked")
                        .map(function (index, item) { return $(item).parent().attr("state"); }).get().join(','),
                    propertyType: $(".property-checkbox-container input:checked")
                        .map(function (index, item) { return $(item).parent().attr("state"); }).get().join(','),
                    displaykey: $("#drop7").siblings().find("li a.selected").attr("displaykey"),
                    favonly: $(".filter-fav").html().indexOf("fa-heart-o") === -1
                },
                success: function (data) {
                    if (compact)
                        $(".results-table").append(data);
                    else
                        $(".results").append(data);
                    window.loading = false;
                    $(".auctionimg").click(function () {
                        $("#imgOverlay img").attr("src", $(this).attr("href"));
                        $("#imgOverlay").css("display", "flex");
                        $("body").css("overflow", "hidden");
                    });
                    $("#imgOverlay").click(function () {
                        $(this).css("display", "none");
                        $("body").css("overflow", "auto");
                    })

                    $(".auctionimg").bind('mouseenter', function () {
                        $(this).find(".hint").css("display", "flex");
                    }).on('mouseleave', function () {
                        $(this).find(".hint").css("display", "none");
                    });

                    $("a.compact-fav").click(function () {
                        var elm = $(this);
                        $.get({
                            url: elm.attr("link"),
                            data: {
                                AuctionId: $(this).attr("AuctionId")
                            },
                            success: function (data) {
                                if (data === "added") {
                                    $("body").overhang({
                                        type: "success",
                                        message: "Added to your favorites",
                                        duration: 1
                                    });
                                    elm.html('<i class="fa fa-heart" aria-hidden="true"></i>');
                                } else {
                                    $("body").overhang({
                                        type: "success",
                                        message: "Removed from your favorites",
                                        duration: 1
                                    });
                                    elm.html('<i class="fa fa-heart-o" aria-hidden="true"></i>');
                                }
                            }
                        }).fail(function (data) {
                            $("body").overhang({
                                type: "error",
                                message: "Failed to add to your favorites",
                                duration: 1
                            });
                        });
                    });

                    if ($(".btn-view-auction").length !== 0) {
                        $(".btn-view-auction").click(function () {
                            $.get($(this).attr("partial-link")).done(function (e) {
                                $("#quickViewOverlay div")[0].innerHTML = e;
                                $("#gallery a").click(function () {
                                    $("#gallery a").removeClass("active");
                                    $("#display-img").css("background-image", 'url(' + $(this).attr("image") + ')');
                                    $(this).addClass("active");
                                });
                                $("#quickViewOverlay .container-fluid").append('<div class="close">Close</div>');
                                if ($(".close").length !== 0) {
                                    $(".close").click(function () {
                                        $("#quickViewOverlay").css("display", "none");
                                        $("body").css("overflow", "auto");
                                    });
                                }
                            });
                            $("body").css("overflow", "hidden");
                            $("#quickViewOverlay").show();
                        });
                    }
                }
            }).fail(
                function () {
                    //alert("found all!");
                });
            window.page++;
        }
    }).scroll();

    $(".auctionimg").click(function () {
        $("#imgOverlay img").attr("src", $(this).attr("href"));
        $("#imgOverlay").css("display", "flex");
        $("body").css("overflow", "hidden");
    });
    $("#imgOverlay").click(function () {
        $(this).css("display", "none");
        $("body").css("overflow", "auto");
    })

    $(".auctionimg").bind('mouseenter', function () {
        $(this).find(".hint").css("display", "flex");
    }).on('mouseleave', function () {
        $(this).find(".hint").css("display", "none");
    });

    if ($(".currentbid-filter-min").length !== 0) {
        window.slider1 = $("#currentbid-filter-slider").bootstrapSlider({
            tooltip: "hide"
        }).on('change',
            function (a) {
                $(".currentbid-filter-min").val(a.value["newValue"][0]);
                $(".currentbid-filter-max").val(a.value["newValue"][1]);
            });
        $(".currentbid-filter-min").val(window.slider1[0].value.split(',')[0]);
        $(".currentbid-filter-max").val(window.slider1[0].value.split(',')[1]);

        $('.currentbid-filter-min, .currentbid-filter-max').on('input', function () {
            window.slider1.bootstrapSlider("setValue",
                new Array(parseInt($('.currentbid-filter-min').val()),
                    parseInt($('.currentbid-filter-max').val())));
        });

        window.slider2 = $("#minbid-filter-slider").bootstrapSlider({
            tooltip: "hide"
        }).on('change',
            function (a) {
                $(".minbid-filter-min").val(a.value["newValue"][0]);
                $(".minbid-filter-max").val(a.value["newValue"][1]);
            });
        $(".minbid-filter-min").val(window.slider2[0].value.split(',')[0]);
        $(".minbid-filter-max").val(window.slider2[0].value.split(',')[1]);

        $('.minbid-filter-min, .minbid-filter-max').on('input', function () {
            window.slider2.bootstrapSlider("setValue",
                new Array(parseInt($('.minbid-filter-min').val()),
                    parseInt($('.minbid-filter-max').val())));
        });
    }

    if ($("#gallery").length !== 0) {
        $("#gallery a").click(function () {
            $("#gallery a").removeClass("active");
            $("#display-img").css("background-image", 'url(' + $(this).attr("image") + ')');
            $(this).addClass("active");
        });
    }

    if ($("#count-down").length !== 0) {
        var countDownDate = new Date($("#count-down").attr("time")).getTime();

        var x = setInterval(function () {

            var now = new Date().getTime();
            var distance = countDownDate - now;

            var days = Math.floor(distance / (1000 * 60 * 60 * 24));
            var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
            var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
            var seconds = Math.floor((distance % (1000 * 60)) / 1000);

            $("#count-down")[0].innerHTML =
                days + " <span>Days</span> " +
                hours + " <span>Hours</span> " +
                minutes + " <span>Minutes</span> " +
                seconds + " <span>Seconds</span>";

            if (distance < 0) {
                clearInterval(x);
                $("#count-down")[0].innerHTML = "EXPIRED";
            }
        }, 1000);
    }

    if ($(".btn-view-auction").length !== 0) {
        $(".btn-view-auction").click(function () {
            $.get($(this).attr("partial-link")).done(function (e) {
                $("#quickViewOverlay div")[0].innerHTML = e;
                $("#gallery a").click(function () {
                    $("#gallery a").removeClass("active");
                    $("#display-img").css("background-image", 'url(' + $(this).attr("image") + ')');
                    $(this).addClass("active");
                });
                $("#quickViewOverlay .container-fluid").append('<div class="close">Close</div>');
                if ($(".close").length !== 0) {
                    $(".close").click(function () {
                        $("#quickViewOverlay").css("display", "none");
                        $("body").css("overflow", "auto");
                    });
                }
            });
            $("body").css("overflow", "hidden");
            $("#quickViewOverlay").show();
        });
    }
});

function mouseWheelZoom(e) {
    window.zoomFactor += e.deltaY === -100 ? 0.1 : -0.1;
    console.log(window.zoomFactor);
    mouseMoveZoom(e.offsetX, e.offsetY);
    e.stopPropagation();
}

function mouseOutZoom(e) {
    var c = document.getElementById("display-img");
    var ctx = c.getContext("2d");
    ctx.clearRect(0, 0, c.width, c.height);
    $("#display-img-zoom").hide();
}

function mouseOverZoom(e) {
    console.log("mouseOverZoom triggered!");

    window.zoomFactor = 1;

    var c = document.getElementById("display-img");
    var ctx = c.getContext("2d");
    window.zoomImg = new Image();
    window.zoomImg.dataset.w = c.offsetWidth;
    window.zoomImg.dataset.h = c.offsetHeight;
    window.zoomImg.onload = function () {
        window.maxw = this.dataset.w,
            window.maxh = this.dataset.h,
            window.aspect = this.width / this.height;
    };
    window.zoomImg.src = $("#display-img").css("background-image").substring(5, $("#display-img").css("background-image").length - 2);
    mouseMoveZoom(e.offsetX, e.offsetY);
    $("#display-img-zoom").show();
}

function mouseMoveZoom(eOffsetX, eOffsetY) {
    console.log("mouseMoveZoom triggered!");
    var c = document.getElementById("display-img");
    var ctx = c.getContext("2d");
    ctx.clearRect(0, 0, c.width, c.height);
    ctx.beginPath();
    var rectx = 100 * window.zoomFactor;
    recty = 100 * window.zoomFactor;
    ctx.rect(eOffsetX - (rectx / 2), eOffsetY - (recty / 2), rectx, recty);

    c2 = document.getElementById('display-img-zoom'), ctx2 = c2.getContext('2d');
    ctx2.fillStyle = "white";
    ctx2.fillRect(0, 0, 400, 400);
    ctx2.fillStyle = "black";
    ctx2.font = "30px Arial";
    ctx2.fillText("Loading...", 130, 190);
    if (window.maxw < window.maxh * window.aspect) {
        ctx2.drawImage(window.zoomImg,
            ((eOffsetX - (rectx / 2)) * window.zoomImg.width / window.maxw),
            ((eOffsetY - (recty / 2) - ((400 - (window.maxw / window.aspect)) / 2)) * window.zoomImg.height / (window.maxw / window.aspect)),
            rectx * (window.zoomImg.width / 400), recty * (window.zoomImg.height / 400) * window.aspect,
            0, 0,
            400, 400);
    } else {
        ctx2.drawImage(window.zoomImg,
            ((eOffsetX - (rectx / 2) - ((400 - (window.maxh * window.aspect)) / 2)) * window.zoomImg.width / (window.maxh * window.aspect)),
            ((eOffsetY - (recty / 2)) * window.zoomImg.height / window.maxh),
            rectx * (window.zoomImg.width / 400) / window.aspect, recty * (window.zoomImg.height / 400),
            0, 0,
            400, 400);
    }
    ctx.stroke();
}

function showRegisterForm() {
    $('.loginBox, .registerBox, .confirmBox, .forgetBox').fadeOut('fast', function () {
        $('.registerBox').fadeIn('fast');
        $('.login-footer').fadeOut('fast', function () {
            $('.register-footer').fadeIn('fast');
        });
        $('.modal-title').html('Register');
    });
    $('.error').removeClass('alert alert-danger').html('');

}

function showLoginForm() {
    $('.loginBox, .registerBox, .confirmBox, .forgetBox').fadeOut('fast', function () {
        $('.loginBox').fadeIn('fast');
        $('.register-footer').fadeOut('fast', function () {
            $('.login-footer').fadeIn('fast');
        });

        $('.modal-title').html('Login');
    });
    $('.error').removeClass('alert alert-danger').html('');
}

function showForgetForm() {
    $('.loginBox, .registerBox, .confirmBox, .forgetBox').fadeOut('fast', function () {
        $('.forgetBox').fadeIn('fast');
        $('.register-footer').fadeOut('fast', function () {
            $('.login-footer').fadeIn('fast');
        });
        $('.modal-title').html('Send forget Password email');
    });
    $('.error').removeClass('alert alert-danger').html('');
}

function showConfirmForm() {
    $('.loginBox, .registerBox, .confirmBox, .forgetBox').fadeOut('fast', function () {
        $('.confirmBox').fadeIn('fast');
        $('.register-footer, .login-footer').fadeOut('fast', function () {
            $('.resend-footer').fadeIn('fast');
        });

        $('.modal-title').html('Confirm email');
    });
    $('.error').removeClass('alert alert-danger').html('');
}

function openLoginModal() {
    if (window.confirmEmail) {
        openConfirmModal();
        return;
    }
    showLoginForm();
    setTimeout(function () { $('#loginModal').modal('show'); }, 230);
}

function openRegisterModal() {
    if (window.confirmEmail) {
        openConfirmModal();
        return;
    }
    showRegisterForm();
    setTimeout(function () { $('#loginModal').modal('show'); }, 230);
}

function openConfirmModal() {
    $("#confirm-email").val($("#reg-email").val());
    showConfirmForm();
    setTimeout(function () { $('#loginModal').modal('show'); }, 230);
}

function resendCode() {
    $.get({
        url: "/auth/sendConfirmation",
        data: {
            email: $("#reg-email").val(),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]', $(".loginBox")).val()
        },
        success: function (data) {
            $("body").overhang({
                type: "success",
                message: "Confirmation email resent",
                duration: 4,
                overlay: true
            });
        }
    }).fail(function (data) {
        shakeModal("Invalid operation!");
        $("body").overhang({
            type: "error",
            message: "Invalid operation!",
            overlay: true,
            closeConfirm: true
        });
    });
}

function forgotAjax() {
    $.get({
        url: "/auth/sendforgetpassword",
        data: {
            email: $("#forget-email").val(),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]', $(".forgetBox")).val(),
        },
        success: function (data) {
            $("body").overhang({
                type: "success",
                message: "Forget password email sent!",
                duration: 4
            });
        }
    }).fail(function (data) {
        shakeModal("Invalid Email Address!");
    });
}

function confirmAjax() {
    $.get({
        url: "/auth/ConfirmEmail",
        data: {
            email: $("#reg-email").val(),
            code: $("#confirm-code").val().trim(),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]', $(".loginBox")).val(),
            ajax: true
        },
        success: function (data) { window.location.replace("/auctions"); }
    }).fail(function (data) {
        shakeModal("Invalid Code!");
        $("body").overhang({
            type: "error",
            message: "Invalid confirmation code!",
            overlay: true,
            closeConfirm: true
        });
    });
}

function loginAjax() {
    $.post({
        url: "/auth/login",
        data: {
            email: $("#login-email").val(),
            password: $("#login-password").val(),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]', $(".loginBox")).val(),
            ReturnUrl: document.URL.substring(document.URL.indexOf("ReturnUrl") + 10)
        },
        success: function (data) {
            window.location.replace(data);
        }
    }).fail(function (data) {
        shakeModal((data.responseText === undefined || data.responseText === "") ? "something went wrong!" : data.responseText);
    });
}

function changePassword() {
    $.post({
        url: "/account/changePassword",
        data: {
            currentpass: $("#current-password").val(),
            pass: $("#new-password").val(),
            passConfirmation: $("#new-password-confirm").val(),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]', $("#changePasswordWrapper")).val()
        },
        success: function (data) {
            $("body").overhang({
                type: "success",
                message: "Password changed successfully!",
                duration: 4,
                overlay: true
            });
        }
    }).fail(function (data) {
        $("body").overhang({
            type: "error",
            message: "Password didn't match or didn't meet the minimum of 8 characters with proper complexity",
            duration: 4,
            overlay: true
        });
    }).always(function (data) {
        $("#current-password").val("");
        $("#new-password").val("");
        $("#new-password-confirm").val("");
    });
}

function registerAjax(layout = "big") {
    $.post({
        url: "/auth/register",
        data: {
            email: $("#reg-email").val(),
            password: $("#reg-password").val(),
            passwordConfirmation: $("#reg-password_confirmation").val(),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]', $(".registerBox")).val()
        },
        success: function (data) {
            //email code confirmation
            if (layout == "mob") {
                document.getElementById('id03').style.display = 'block'
                $("#confirm-email").val($("#reg-email").val());
            }
            else {
                openConfirmModal();
            }
            
            $("body").overhang({
                type: "success",
                message: "Kindly enter the code sent you at " + $("#reg-email").val() + " or click on the link in the email",
                duration: 4,
                overlay: true
            });
            $("#confirm-code").focus();
            window.confirmEmail = true;
        }
    }).fail(function (data) {
        shakeModal((data.responseText === undefined || data.responseText === "") ? "something went wrong!" : data.responseText);
    });
}

function resetPasswordAjax() {
    $("#reset-error").html("");
    $.post({
        url: "/auth/resetPassword",
        data: {
            email: $("#reset-email").val(),
            password: $("#reset-password").val(),
            passwordConfirmation: $("#reset-password_confirmation").val(),
            code: $("#code").val(),
            __RequestVerificationToken: $('input[name="__RequestVerificationToken"]', $(".resetBox")).val()
        },
        success: function (data) {
            $("body").overhang({
                type: "success",
                message: "Your password has been changed successfully!",
                duration: 4,
                overlay: true
            });
            setTimeout(function () {
                window.location.replace("/");
            }, 2000);
        }
    }).fail(function (data) {
        $("#reset-error").html((data.responseText === undefined || data.responseText === "") ? "something went wrong!" : data.responseText);
    });
}

function shakeModal(error) {
    $('#loginModal .modal-dialog').addClass('shake');
    $('.error').addClass('alert alert-danger').html(error);
    $('input[type="password"]').val('');
    setTimeout(function () {
        $('#loginModal .modal-dialog').removeClass('shake');
    }, 1000);
}

function changeFavFilter(e) {
    if (e.html().indexOf("fa-heart-o") === -1) {
        e.html('<i class="fa fa-heart-o" aria-hidden="true"></i>');
    } else {
        e.html('<i class="fa fa-heart" aria-hidden="true"></i>');
    }
    applyFilters($('#drop6').siblings().find('li a.selected').attr('sortkey'),
        $('#drop7').siblings().find('li a.selected').attr('displaykey'));
}

function applyFilters(sortby, displaykey) {
    var fav = $(".filter-fav").html().indexOf("fa-heart-o") === -1;
    window.location.href = "/Auctions/Index?" +
        "cbmin=" + $(".currentbid-filter-min").val() +
        "&cbmax=" + $(".currentbid-filter-max").val() +
        "&mbmin=" + $(".minbid-filter-min").val() +
        "&mbmax=" + $(".minbid-filter-max").val() +
        "&states=" + $(".state-checkbox-container input:checked")
            .map(function (index, item) { return $(item).parent().attr("state"); }).get().join(',') +
        "&propertyType=" + $(".property-checkbox-container input:checked")
            .map(function (index, item) { return $(item).parent().attr("state"); }).get().join(',') +
        "&sellerType=" + $(".seller-checkbox-container input:checked")
            .map(function (index, item) { return $(item).parent().attr("state"); }).get().join(',') +
        "&sortby=" + (sortby === undefined ? "bm" : sortby) +
        "&displaykey=" + (displaykey === undefined ? "Comfortable" : displaykey) +
        "&favonly=" + fav;
}

function checkSize() {
    if ($(window).width() <= 600) {
        $("#mobile_display").show();
        $("#full_display").hide();
        $("#mob-terms").show();
        $("#big-terms").hide();
       
        $("#for_small_display").show();
        $("#for_big_display").hide();
        $('#site').attr('disabled', 'disabled');
        $('#mobile').removeAttr('disabled');

        $('#w3-mob').removeAttr('disabled');
        $('#big-login > #login-email').attr("id", "login-email-disable");
        $('#big-login > #login-password').attr("id", "login-password-disable");
        $('#mob-login > #login-email-mob').attr("id", "login-email");
        $('#mob-login > #login-password-mob').attr("id", "login-password");

        $('#big-reg > #reg-email').attr("id", "reg-email-disable");
        $('#big-reg > #reg-password').attr("id", "reg-password-disable");
        $('#big-reg > #reg-password_confirmation').attr("id", "reg-password_confirmation-disable");
        $('#mob-reg > #reg-email-mob').attr("id", "reg-email");
        $('#mob-reg > #reg-password-mob').attr("id", "reg-password");
        $('#mob-reg > #reg-password_confirmation-mob').attr("id", "reg-password_confirmation");

        $('#big-confirm > #confirm-email').attr("id", "confirm-email-disable");
        $('#big-confirm > #confirm-code').attr("id", "confirm-code-disable");
        $('#mob-confirm > #confirm-email-mob').attr("id", "confirm-email");
        $('#mob-confirm > #confirm-code-mob').attr("id", "confirm-code");

        $('#big-forget > #forget-email').attr("id", "forget-email-disable");
        $('#mob-forget > #forget-email-mob').attr("id", "forget-email");
    }
    else {
        $('#w3-mob').attr('disabled', 'disabled');
        $('#mob-login > #login-email').attr("id", "login-email-mob");
        $('#mob-login > #login-password').attr("id", "login-password-mob");
        $('#big-login > #login-email-disable').attr("id", "login-email");
        $('#big-login > #login-password-disable').attr("id", "login-password");

        $('#mob-reg > #reg-email').attr("id", "reg-email-mob");
        $('#mob-reg > #reg-password').attr("id", "reg-password-mob");
        $('#mob-reg > #reg-password_confirmation').attr("id", "reg-password_confirmation-mob");
        $('#big-reg > #reg-email-disable').attr("id", "reg-email");
        $('#big-reg > #reg-password-disable').attr("id", "reg-password");
        $('#big-reg > #reg-password_confirmation-disable').attr("id", "reg-password_confirmation");

        $('#mob-confirm > #confirm-email').attr("id", "confirm-email-mob");
        $('#mob-confirm > #confirm-code').attr("id", "confirm-code-mob");
        $('#big-confirm > #confirm-email-disable').attr("id", "confirm-email");
        $('#big-confirm > #confirm-code-disable').attr("id", "confirm-code");

        $('#big-forget > #forget-email-disable').attr("id", "forget-email");
        $('#mob-forget > #forget-email').attr("id", "forget-email-mob");

        document.getElementById('id01').style.display = 'none'
        document.getElementById('id02').style.display = 'none'
        document.getElementById('id03').style.display = 'none'
        document.getElementById('id04').style.display = 'none'
        $("#mob-terms").hide();
        $("#big-terms").show();
        $("#mobile_display").hide();
        $("#full_display").show();
       
        $("#for_big_display").show();
        $("#for_small_display").hide();
        $('#site').removeAttr('disabled');
        $('#mobile').attr('disabled', 'disabled');
    }
}