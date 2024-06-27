$('#btnEditApartments').click(function () {
    HideAllEdit();
    $('#apartmentsEditBlock').css('display', 'block');
    loadApartments();
});

function loadApartments() {
    $.ajax({
        url: '/Apartments/Get',
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: handleApartmentsResponse,
        error: handleApartmentsError
    });
}

function handleApartmentsResponse(response) {
    if (!response || response.length === 0) {
        alert('В доме нет квартир');
    } else {
        updateApartmentsTable(response);
    }
}

function handleApartmentsError() {
    alert('Unable to read the data');
}

function updateApartmentsTable(apartments) {
    let tableRows = '';
    apartments.forEach(apartment => {
        tableRows += `<tr>`;
        tableRows += `<td style="display:none">${apartment.id}</td>`;
        const checked = ownedApartmentsMap[apartment.id] ? 'checked' : '';
        tableRows += `<td><input type="checkbox" id="apartmentCheckbox_${apartment.id}" value="${apartment.id}" ${checked}/></td>`;
        tableRows += `<td>${apartment.number}</td>`;
        tableRows += `<td>${apartment.residentsNumber}</td>`;
        tableRows += `</tr>`;
    });
    $('#apartmentsEditTableBody').html(tableRows);
}

$('#saveApartments').click(function () {
    var selectedApartments = getSelectedApartmentsData();
    updateLodgerApartments(selectedApartments);
});

function getSelectedApartmentsData() {
    var selectedApartments = [];

    $('input[type=checkbox]').each(function () {
        if (this.checked) {
            var $row = $(this).closest('tr');
            var apartmentId = $row.find('td:eq(0)').text();
            var apartmentNumber = $row.find('td:eq(2)').text();
            var residentsNumber = $row.find('td:eq(3)').text();
            selectedApartments.push({
                Id: apartmentId,
                Number: apartmentNumber,
                ResidentsNumber: residentsNumber
            });
        }
    });

    return selectedApartments;
}

function updateLodgerApartments(selectedApartments) {
    var ownerId = $('#Id').val();

    var formData = {
        id: ownerId,
        selectedApartments: selectedApartments,
    };

    $.ajax({
        url: '/Apartments/UpdateApartments',
        type: 'POST',
        data: formData,
        success: handleUpdateApartmentsSuccess,
        error: handleUpdateApartmentsError
    });
}

function handleUpdateApartmentsSuccess(response) {
    $('#apartmentsEditBlock').css('display', 'none');
    Edit($('#Id').val());
    ownedApartmentsMap = {};
}

function handleUpdateApartmentsError(error) {
    alert('Error: ' + error);
}

$('#hideApartments').click(function () {
    $('#apartmentsEditBlock').css('display', 'none');
    $('#apartmentsOwnershipPercentageBlock').css('display', 'none');
    ownedApartmentsMap = {};
});

function EditPercentageApartment(id) {
    HideAllEdit();
    $.ajax({
        url: '/Apartments/Edit?id=' + id,
        type: 'GET',
        success: function (response) {
            if (response) {
                $('#apartmentsOwnershipPercentageBlock').css('display', 'block');
                $('#OwnedApartmentIdEdit').val(response.id);
                $('#ApartmentNumberEdit').val(response.apartment.number);
                $('#OwnershipPercentageAdd').val(response.ownershipPercentage);
            }
        },
        error: function (error) {
            alert('Error: ' + error);
        }
    });
}

$('#saveOwnershipPercentage').click(function () {
    var formData = {
        id: $('#OwnedApartmentIdEdit').val(),
        ownershipPercentage: $('#OwnershipPercentageAdd').val(),
        ownerId: $('#Id').val()
    };

    $.ajax({
        url: '/Apartments/Update',
        type: 'POST',
        data: formData,
        success: function (response) {
            if (!response.success) {
                alert(response.message);
            } else {
                $('#apartmentsOwnershipPercentageBlock').css('display', 'none');
                var id = $('#Id').val();
                Edit(id);
            }
        },
        error: function (error) {
            alert('Error: ' + error);
        }
    });
});

$('#hideOwnershipPercentage').click(function () {
    $('#apartmentsOwnershipPercentageBlock').css('display', 'none');
});

function GetApartments(callback) {
    $.ajax({
        url: '/Apartments/Get',
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        dataType: 'json',
        success: function (response) {
            if (response == null || response == undefined) {
                alert('Unable to read the data');
            }
            else if (response.length == 0) {
                alert('В доме нет квартир');
            }
            else {
                callback(response);
            }
        },
        error: function () {
            alert('Unable to read the data');
        }
    });
}