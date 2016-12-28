/*
    Template: Hallooou HTML5 Responsive template
    Author: Mauritius D'Silva <hello@mauritiusdsilva.com>
    Theme URI: http://www.mauritiusdsilva.com/themes/hallooou
    Version: 1.0
*/


// jQuery to collapse the navbar on scroll
$(window).scroll(function() {
    if ($(".navbar").offset().top > 50) {
        $(".navbar-fixed-top").addClass("top-nav-collapse");
        $(".scroll-top").fadeIn('1000', "easeInOutExpo");
    } else {
        $(".navbar-fixed-top").removeClass("top-nav-collapse");
        $(".scroll-top").fadeOut('1000', "easeInOutExpo");
    }
});

// jQuery for page scrolling feature - requires jQuery Easing plugin
$(function() {
    $('.overlay-menu ul li a,.scroll-top a').bind('click', function(event) {
        var $anchor = $(this);
        $('html, body').stop().animate({
            scrollTop: $($anchor.attr('href')).offset().top
        }, 1500, 'easeInOutExpo');
        event.preventDefault();
    });
});


// WOW.js initialise
// WOW.js uses animate.css to animate/reveal elements.
// Browse the list of animation effects available here-> https://daneden.github.io/animate.css/
$(function() {
    wow = new WOW({
        boxClass: 'wow', // default
        animateClass: 'animated', // default
        offset: 0, // default
        mobile: true, // default
        live: true // default
    })
    wow.init();
});


// jQuery Parallax. More info here-> https://github.com/IanLunn/jQuery-Parallax
// $(function() {
//     // apply parallax effect only when body has the ".parallax-page" class
//     if ($('body').hasClass('parallax-page')) {
//         //.parallax(xPosition, speedFactor, outerHeight) options:
//         //xPosition - Horizontal position of the element
//         //inertia - speed to move relative to vertical scroll. Example: 0.1 is one tenth the speed of scrolling, 2 is twice the speed of scrolling
//         //outerHeight (true/false) - Whether or not jQuery should use it's outerHeight option to determine when a section is in the viewport

//         $('#parallax-slide').parallax("50%", 0.1);
//         $('#products').parallax("50%", 0.1);
//         $('#portfolio').parallax("50%", 0.1);
//         $('#page-aboutus').parallax("50%", 0.1);
//     }
// });


// Closes the Responsive Menu on Menu Item Click
$('.overlay-menu ul li a').click(function() {
    $('.button_container:visible').click();
});


// jQuery for page scrolling feature - requires jQuery Easing plugin
$(function() {
    $('a.page-scroll').bind('click', function(event) {
        var $anchor = $(this);
        $('html, body').stop().animate({
            scrollTop: $($anchor.attr('href')).offset().top
        }, 1500, 'easeInOutExpo');
        event.preventDefault();
    });
});


// Closes the Responsive Menu on Menu Item Click
$('.navbar-collapse ul li a').click(function() {
    $('.navbar-toggle:visible').click();
});

// Navigation show/hide
$('.toggle').click(function() {
    if ($('#overlay.open')) {
        $(this).toggleClass('active');
        $('#overlay').toggleClass('open');
    }
});


// Client testimonials
$(function() {

    var owl = $("#client-testimonials");

    owl.owlCarousel({
        navigation: false, // Show next and prev buttons
        slideSpeed: 300,
        paginationSpeed: 400,
        singleItem: true
    });

});


// Client Slider Carousel
$(function() {

    var owl = $("#technology-slider");

    owl.owlCarousel({
        items: 4, //5 items above 1000px browser width
        itemsDesktop: [1024, 4], //4 items between 1000px and 901px
        itemsDesktopSmall: [900, 3], // betweem 900px and 601px
        itemsTablet: [600, 2], //2 items between 600 and 480
        itemsMobile: [479, 2], //1 item between 480 and 0
        pagination: true, // Show pagination
        navigation: false // Show navigation
    });

});


// Screenshots Carousel
$(function() {

    var owl = $("#screenshots-carousel");

    owl.owlCarousel({
        items: 3, //5 items above 1000px browser width
        itemsDesktop: [1024, 4], //4 items between 1000px and 901px
        itemsDesktopSmall: [900, 2], // betweem 900px and 601px
        itemsTablet: [600, 2], //2 items between 600 and 480
        itemsMobile: [479, 1], //1 item between 480 and 0
        pagination: true, // Show pagination
        navigation: false // Show navigation
    });


    // Custom Navigation Events
    $("#screenshots-next").on('click', function() {
        owl.trigger('owl.next');
    })
    $("#screenshots-prev").on('click', function() {
        owl.trigger('owl.prev');
    })

});

// Open House Carousel
$(function() {

    var owl = $("#open-house-carousel");

    owl.owlCarousel({
        items: 3, //5 items above 1000px browser width
        itemsDesktop: [1024, 4], //4 items between 1000px and 901px
        itemsDesktopSmall: [900, 2], // betweem 900px and 601px
        itemsTablet: [600, 2], //2 items between 600 and 480
        itemsMobile: [479, 1], //1 item between 480 and 0
        pagination: true, // Show pagination
        navigation: false // Show navigation
    });


    // Custom Navigation Events
    $("#open-house-next").on('click', function() {
        owl.trigger('owl.next');
    })
    $("#open-house-prev").on('click', function() {
        owl.trigger('owl.prev');
    })

});



// Counter
// $(function() {

//     $('.counter-section').on('inview', function(event, visible, visiblePartX, visiblePartY) {
//         if (visible) {
//             $(this).find('.timer').each(function() {
//                 var $this = $(this);
//                 $({
//                     Counter: 0
//                 }).animate({
//                     Counter: $this.text()
//                 }, {
//                     duration: 2000,
//                     easing: 'swing',
//                     step: function() {
//                         $this.text(Math.ceil(this.Counter));
//                     }
//                 });
//             });
//             $(this).off('inview');
//         }
//     });

// });


// Carousel Slider
$(function() {
    $('.carousel').carousel({
        interval: 8000 //changes the speed
    })
});


// YouTube Player
// $(function() {
//     $(".player").mb_YTPlayer();

//     $('#video-play').click(function(event) {
//         event.preventDefault();
//         if ($(this).hasClass('fa-play')) {
//             $('.player').playYTP();
//         } else {
//             $('.player').pauseYTP();
//         }
//         $(this).toggleClass('fa-play fa-pause');
//         return false;
//     });

//     $('#video-volume').click(function(event) {
//         event.preventDefault();
//         $('.player').toggleVolume();
//         $(this).toggleClass('fa-volume-off fa-volume-up');
//         return false;
//     });
// });


// // HTML5 Player
// $(function() {

//     var vid = $("#html5-video").get(0);

//     $('#html5-video-play').click(function(event) {
//         event.preventDefault();
//         if (vid.paused) {
//             vid.play();
//         } else {
//             vid.pause();
//         }
//         $(this).toggleClass('fa-play fa-pause');
//         return false;
//     });

//     $('#html5-video-volume').click(function(event) {
//         event.preventDefault();
//         if (vid.muted) {
//             vid.muted = false;
//         } else {
//             vid.muted = true;
//         }
//         $(this).toggleClass('fa-volume-off fa-volume-up');
//         return false;
//     });
// });


// Lightbox
$(function() {
     $('#screenshots-carousel').magnificPopup({
        delegate: 'a',
        type: 'image',
        tLoading: 'Loading image #%curr%...',
        mainClass: 'mfp-with-zoom mfp-img-mobile',
        gallery: {
            enabled: true,
            navigateByImgClick: true,
            preload: [0, 1] // Will preload 0 - before current, and 1 after the current image
        },
        image: {
            verticalFit: true,
            tError: '<a href="%url%">The image #%curr%</a> could not be loaded.',
            titleSrc: function(item) {
                return item.el.attr('title') + '<small>by bARk</small>';
            }
        },
        zoom: {
			enabled: true,
			duration: 300, // don't foget to change the duration also in CSS
			opener: function(element) {
				return element.find('img');
			}
		}
    });

    $('#open-house-carousel').magnificPopup({
        delegate: 'a',
        type: 'image',
        tLoading: 'Loading image #%curr%...',
        mainClass: 'mfp-with-zoom mfp-img-mobile',
        gallery: {
            enabled: true,
            navigateByImgClick: true,
            preload: [0, 1] // Will preload 0 - before current, and 1 after the current image
        },
        image: {
            verticalFit: true,
            tError: '<a href="%url%">The image #%curr%</a> could not be loaded.',
            titleSrc: function(item) {
                return item.el.attr('title') + '<small>at <a target="_blank" href="https://www.kth.se/social/course/DH2413/page/agi16-open-house/">AGI16 Open House</a></small>';
            }
        },
        zoom: {
			enabled: true,
			duration: 300, // don't foget to change the duration also in CSS
			opener: function(element) {
				return element.find('img');
			}
		}
    });

});
