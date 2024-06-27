function DeleteParkingSpace(id) {
    if (confirm('Вы уверены, что хотите отменить бронь на парковочное место?')) {
        $.ajax({
            url: '/ParkingSpaces/Delete?id=' + id,
            type: 'POST',
            success: function (response) {
                if (response == null || response == undefined) {
                    alert('Не получилось отменить бронь');
                }
                else {
                    var id = $('#Id').val();
                    Edit(id);
                }
            },
            error: function () {
                alert('Не получилось отменить бронь');
            }
        });
    }
}

$('#btnEditParkingSpaces').click(handleBtnEditParkingSpacesClick);

function handleBtnEditParkingSpacesClick() {
    HideAllEdit();
    $('#parkingSpacesEditBlock').css('display', 'block');
    var id = $('#Id').val();
    getParkingSpacesData(id);
}

function getParkingSpacesData(id) {
    $.ajax({
        url: '/ParkingSpaces/Get?id=' + id,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        datatype: 'json',
        success: function (response) {
            handleParkingSpacesDataResponse(response);
        },
        error: function () {
            alert('Unable to read the data');
        }
    });
}

function handleParkingSpacesDataResponse(response) {
    if (response == null || response == undefined) {
        alert('Unable to read the data');
    } else if (response.length == 0) {
        alert('Все парковочные места заняты');
    } else {
        displayParkingSpaces(response);
    }
}

function displayParkingSpaces(parkingSpaces) {
    var object = '';
    $.each(parkingSpaces, function (index, ownedParkingSpace) {
        object += '<tr>';
        object += '<td style="display:none">' + ownedParkingSpace.id + '</td>';
        var checked = ownedParkingSpacesMap[ownedParkingSpace.id] ? 'checked' : '';
        object += '<td><input type="checkbox" id="carCheckbox_' + ownedParkingSpace.id + '" value="' + ownedParkingSpace.id + '" ' + checked + '/></td>';
        object += '<td>' + ownedParkingSpace.number + '</td>';
    });
    $('#parkingSpacesEditTableBody').html(object);
}

function saveParkingSpaces() {
    var selectedParkingSpaces = getSelectedParkingSpacesData();

    var ownerId = $('#Id').val();

    var formData = {
        id: ownerId,
        selectedParkingSpaces: selectedParkingSpaces,
    };

    updateParkingSpaces(formData, ownerId);
}

function getSelectedParkingSpacesData() {
    var selectedParkingSpaces = [];

    $('input[type=checkbox]').each(function () {
        if (this.checked) {
            var $row = $(this).closest('tr');
            var parkingSpaceId = $row.find('td:eq(0)').text();
            var number = $row.find('td:eq(2)').text();
            selectedParkingSpaces.push({
                Id: parkingSpaceId,
                Number: number
            });
        }
    });

    return selectedParkingSpaces;
}

function updateParkingSpaces(formData, ownerId) {
    $.ajax({
        url: '/ParkingSpaces/Update',
        type: 'POST',
        data: formData,
        success: function (response) {
            $('#parkingSpacesEditBlock').css('display', 'none');
            Edit(ownerId);
            ownedParkingSpacesMap = {};
        },
        error: function (error) {
            alert('Error: ' + error);
        }
    });
}

$('#saveParkingSpaces').click(saveParkingSpaces);

$('#hideParkingSpaces').click(function () {
    $('#parkingSpacesEditBlock').css('display', 'none');
});