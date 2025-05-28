// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function FillCities(lstCountryCtrl, lstCityId) {
    var lstCities = $("#" + lstCityId);
    lstCities.empty();
    lstCities.append($('<option/>',
        {
            value: null,
            text: "Select City"
        }));
    var selectedContry = lstCountryCtrl.options[lstCountryCtrl.selectedIndex].value;
    if (selectedContry != null && selectedContry != '') {
        $.getJSON('/Customer/getcitiesbycountry', { CountryId: selectedcountry }, function (cities) {
            if (cities != null && !jQuery.isEmptyObject(cities)) {
                $.each(cities, function (index, city) {
                    lstCities.append($('<option/>',
                        {
                            value: city.value,
                            text: city.text
                        }));
                });
            };
        });
    }
    return;
}

$(".custom-file-input").on("change", function () {
    var fileName = $(this).val().split("\\").pop();
    document.getElementById('PreviewPhoto').src = window.URL.createObjectURL(this.files[0]);
    document.getElementById('PhotoUrl').value = fileName;
});

function ShowCreateModalFrom() {
    $("#DivCreateDialogHolder").modal('show');
    return;
}

function submitModalForm() {
    var btnSubmit = document.getElementById('btnSubmit');
    btnSubmit.click();
}

function refreshCountruyList() {
    var btnBack = document.getElementById('dubBackBtn');
    btnBack.click();
    FillCountries("lstCountryId");
}

function FillCountries(lstCountryId) {
    var lstCountries = $("#" + lstCountryId);
    lstCountries.empty();
    lstCountries.append($('<option/>',
        {
            value: null,
            text: "Select Country"
        }));
    $.getJSON("/country/GetCountries", function (countries) {
        if (selectedContry != null && !jQuery.isEmptyObject(countries)) {
            $.each(countries, function (index, country) {
                lstCountries.append($('<option/>',
                    {
                        value: country.value,
                        text: country.text
                    }));
            });
        };
    });
    return;
}