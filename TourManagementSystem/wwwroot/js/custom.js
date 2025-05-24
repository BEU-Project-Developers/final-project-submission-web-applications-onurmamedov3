/* JS Document */

/******************************

[Table of Contents]

1. Vars and Inits
2. Set Header
3. Init Home Slider
4. Init Menu
5. Init Search Tabs (from original code)
6. Init CTA Slider
7. Init Testimonials Slider
8. Init Search Form (Header Search Icon)


******************************/

$(document).ready(function () {
	"use strict";

	/* 1. Vars and Inits */
	var menu = $('.menu');
	var menuActive = false;
	var header = $('.header');
	var searchActive = false; // For header search icon

	setHeader();

	$(window).on('resize', function () {
		setHeader();
	});

	$(document).on('scroll', function () {
		setHeader();
	});

	initHomeSlider();
	initMenu();
	initSearchTabs(); // Renamed from initSearch for clarity if theme had initSearch for header
	initCtaSlider();
	initTestSlider();
	initHeaderSearchForm(); // Renamed from initSearchForm

	/* 2. Set Header */
	function setHeader() {
		if (window.innerWidth < 992) {
			if ($(window).scrollTop() > 100) {
				header.addClass('scrolled');
			}
			else {
				header.removeClass('scrolled');
			}
		}
		else {
			if ($(window).scrollTop() > 100) {
				header.addClass('scrolled');
			}
			else {
				header.removeClass('scrolled');
			}
		}
		if (window.innerWidth > 991 && menuActive) {
			closeMenu();
		}
	}

	/* 3. Init Home Slider */
	function initHomeSlider() {
		if ($('.home_slider').length) {
			var homeSlider = $('.home_slider');
			homeSlider.owlCarousel(
				{
					items: 1,
					loop: true,
					autoplay: true, // Changed to true for auto-play
					autoplayTimeout: 5000, // 5 seconds
					autoplayHoverPause: true,
					smartSpeed: 1200,
					dotsContainer: '#home_slider_custom_dots' // Corrected selector
				});

			if ($('.home_slider_prev').length) {
				var prev = $('.home_slider_prev');
				prev.on('click', function () { homeSlider.trigger('prev.owl.carousel'); });
			}
			if ($('.home_slider_next').length) {
				var next = $('.home_slider_next');
				next.on('click', function () { homeSlider.trigger('next.owl.carousel'); });
			}
			if ($('.home_slider_custom_dot').length) {
				$('.home_slider_custom_dot').on('click', function () {
					$('.home_slider_custom_dot').removeClass('active');
					$(this).addClass('active');
					homeSlider.trigger('to.owl.carousel', [$(this).index(), 300]);
				});
			}
			homeSlider.on('changed.owl.carousel', function (event) {
				$('.home_slider_custom_dot').removeClass('active');
				$('#home_slider_custom_dots li').eq(event.page.index).addClass('active'); // Corrected selector
			});
			function setAnimation(_elem, _InOut) {
				var animationEndEvent = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
				_elem.each(function () {
					var $elem = $(this);
					var $animationType = 'animated ' + $elem.data('animation-' + _InOut);
					$elem.addClass($animationType).one(animationEndEvent, function () {
						$elem.removeClass($animationType);
					});
				});
			}
			homeSlider.on('change.owl.carousel', function (event) {
				var $currentItem = $('.home_slider_item', homeSlider).eq(event.item.index);
				var $elemsToanim = $currentItem.find("[data-animation-out]");
				setAnimation($elemsToanim, 'out');
			});
			homeSlider.on('changed.owl.carousel', function (event) {
				var $currentItem = $('.home_slider_item', homeSlider).eq(event.item.index);
				var $elemsToanim = $currentItem.find("[data-animation-in]");
				setAnimation($elemsToanim, 'in');
			})
		}
	}

	/* 4. Init Menu */
	function initMenu() {
		if ($('.hamburger').length && $('.menu').length) {
			var hamb = $('.hamburger');
			var close = $('.menu_close_container'); // Assuming this is the close button in the menu

			hamb.on('click', function () { if (!menuActive) { openMenu(); } else { closeMenu(); } });
			close.on('click', function () { if (!menuActive) { openMenu(); } else { closeMenu(); } });
		}
	}
	function openMenu() { menu.addClass('active'); menuActive = true; }
	function closeMenu() { menu.removeClass('active'); menuActive = false; }

	/* 5. Init Search Tabs (for Home Page and Offers Page if structure is identical) */
	function initSearchTabs() {
		// This function handles tab switching for any element with class .search_tab
		// Ensure your Offers page search tabs have the same classes if you want this to apply there too.
		// If Offers page tabs are different, they might need their own JS in offers_custom.js
		if ($('.search_tab').length) {
			$('.search_tab').on('click', function () {
				var $thisTab = $(this);
				$('.search_tab').removeClass('active'); // Deactivate all tabs
				$thisTab.addClass('active'); // Activate clicked tab

				// Find the corresponding search panel
				// Assumes search panels are siblings of search_tabs_container or in a predictable structure
				var $searchTabsContainer = $thisTab.closest('.search_tabs_container');
				var $searchPanelsContainer = $searchTabsContainer.siblings('.search_panel, .active'); // Find panels related to this tab set

				if (!$searchPanelsContainer.length) {
					// A more robust way if panels are not direct siblings of container:
					// Find the main search section and then the panels within it.
					var $mainSearchBlock = $thisTab.closest('.search'); // or .col.fill_height containing both tabs and panels
					$searchPanelsContainer = $mainSearchBlock.find('.search_panel');

				}

				var clickedIndex = $thisTab.parent().children('.search_tab').index($thisTab);


				$searchPanelsContainer.removeClass('active');
				$($searchPanelsContainer[clickedIndex]).addClass('active');
			});
		}
	}


	/* 6. Init CTA Slider */
	function initCtaSlider() {
		if ($('.cta_slider').length) {
			var ctaSlider = $('.cta_slider');
			ctaSlider.owlCarousel(
				{
					items: 1, loop: true, autoplay: true, nav: false, dots: false, smartSpeed: 1200
				});
			if ($('.cta_slider_prev').length) { $('.cta_slider_prev').on('click', function () { ctaSlider.trigger('prev.owl.carousel'); }); }
			if ($('.cta_slider_next').length) { $('.cta_slider_next').on('click', function () { ctaSlider.trigger('next.owl.carousel'); }); }
		}
	}

	/* 7. Init Testimonials Slider */
	function initTestSlider() {
		if ($('.test_slider').length) {
			var testSlider = $('.test_slider');
			testSlider.owlCarousel(
				{
					loop: true, nav: false, dots: false, smartSpeed: 1200, margin: 30,
					responsive: { 0: { items: 1 }, 480: { items: 1 }, 768: { items: 2 }, 992: { items: 3 } }
				});
			if ($('.test_slider_prev').length) { $('.test_slider_prev').on('click', function () { testSlider.trigger('prev.owl.carousel'); }); }
			if ($('.test_slider_next').length) { $('.test_slider_next').on('click', function () { testSlider.trigger('next.owl.carousel'); }); }
		}
	}

	/* 8. Init Header Search Form (the icon in the nav bar) */
	function initHeaderSearchForm() {
		if ($('.search_form').length) // This refers to the header search form
		{
			var searchForm = $('.search_form'); // Header search form
			var searchButton = $('.content_search'); // Header search icon

			searchButton.on('click', function (event) {
				event.stopPropagation();
				if (!searchActive) {
					searchForm.addClass('active');
					searchActive = true;
					$(document).one('click', function closeForm(e) {
						if ($(e.target).hasClass('search_content_input')) { $(document).one('click', closeForm); }
						else { searchForm.removeClass('active'); searchActive = false; }
					});
				}
				else { searchForm.removeClass('active'); searchActive = false; }
			});
		}
	}
});