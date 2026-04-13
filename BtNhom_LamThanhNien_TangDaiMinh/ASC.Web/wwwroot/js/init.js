(function ($) {
  $(function () {
    $(".sidenav").sidenav();
    $(".parallax").parallax();
    $(".collapsible").collapsible();
  });
})(jQuery);

window.history.pushState(null, "", window.location.href);
window.onpopstate = function () {
  window.history.pushState(null, "", window.location.href);
};

document.addEventListener("contextmenu", function (event) {
  event.preventDefault();
});
