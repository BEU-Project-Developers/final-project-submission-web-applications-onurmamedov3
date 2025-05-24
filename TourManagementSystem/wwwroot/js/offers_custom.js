// File: wwwroot/js/offers_custom.js
$(document).ready(function () {
    "use strict";

    initParallax();
    initOffersIsotope(); // Initialize Isotope for the current offers_grid content

    // Date picker initialization (if using jQuery UI or similar)
    // Ensure jQuery UI is referenced in _Layout.cshtml or here.
    // if ($.fn.datepicker) {
    //     $('input[type="date"].search_input').each(function() {
    //         // If native date picker is okay, this isn't strictly needed.
    //         // But if you want consistent styling or more features, use a JS datepicker.
    //         // For native type="date", this block can be removed or adapted.
    //         // $(this).datepicker({
    //         //     dateFormat: "yy-mm-dd", // Important to match server expectation
    //         //     changeMonth: true,
    //         //     changeYear: true
    //         // });
    //     });
    // }


    /* Initialize Parallax */
    function initParallax() {
        if ($('.parallax-window').length) {
            $('.parallax-window').parallax();
        }
    }

    /* Initialize Isotope for Offers Grid */
    function initOffersIsotope() {
        var $grid = $('.offers_grid');
        if ($grid.length) {
            $grid.imagesLoaded(function () { // Ensure images are loaded before Isotope initializes
                $grid.isotope({
                    itemSelector: '.offers_item',
                    layoutMode: 'vertical',
                    getSortData: {
                        price: '[data-price] parseFloat',
                        stars: '[data-stars] parseInt',
                        name: '[data-name]'
                    },
                    sortBy: 'original-order'
                });
            });


            // Filter buttons click handler
            $('.offers_sorting .filter_btn').on('click', function () {
                var filterValue = $(this).attr('data-filter');
                $grid.isotope({ filter: filterValue });
                $(this).closest('ul').find('.filter_btn').removeClass('active_filter');
                $(this).addClass('active_filter');
            });

            // Sort buttons click handler
            $('.offers_sorting .sort_btn').on('click', function () {
                var sortOptions = $(this).data('isotope-option');
                if (sortOptions && sortOptions.sortBy) {
                    $grid.isotope({ sortBy: sortOptions.sortBy });
                    // Manage active classes for visual feedback
                    $(this).closest('ul').find('.sort_btn').removeClass('active_sort');
                    $(this).addClass('active_sort');
                }
            });
        }
    }

    // Enhanced Search Tab functionality for Offers page
    $('.search_tabs .search_tab').on('click', function () {
        var $thisTab = $(this);
        var $tabsContainer = $thisTab.closest('.search_tabs');
        var entityType = $thisTab.data('entitytype');

        // Update active tab
        $tabsContainer.find('.search_tab').removeClass('active');
        $thisTab.addClass('active');

        // Update active search panel
        // Assumes search panels are siblings of search_tabs_container or in a predictable structure
        var $searchPanelsContainer = $thisTab.closest('.search_tabs_container').siblings('.search_panel');
        if (!$searchPanelsContainer.length) {
            var $mainSearchBlock = $thisTab.closest('.col.fill_height'); // or .search_inner
            $searchPanelsContainer = $mainSearchBlock.find('.search_panel');
        }

        $searchPanelsContainer.removeClass('active');
        $('#' + entityType.toLowerCase() + '_search_panel').addClass('active');

        // IMPORTANT: If a tab is clicked, it implies the user wants to search for THAT entity type.
        // We should probably clear the grid or indicate loading for the new type.
        // A full page reload by submitting the corresponding form is often the simplest.
        // Alternatively, if not submitting, clear the grid and maybe fetch default data for the new type via AJAX.
        // For now, clicking a tab changes the active form but doesn't auto-submit or change displayed content.
        // User needs to click the "search" button within the newly activated panel.

        // Example: If you wanted clicking a tab to *immediately* filter or redirect:
        // var formToSubmit = $('#' + entityType.toLowerCase() + '_search_panel').find('form');
        // if (formToSubmit.length) {
        //     formToSubmit.submit(); // This would trigger a new GET request
        // } else {
        //    window.location.href = '/Offers?entityType=' + entityType; // Simpler redirect
        // }
    });

});