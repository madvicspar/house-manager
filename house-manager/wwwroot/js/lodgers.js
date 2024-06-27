$(document).ready(function () {
    LoadData();
    $('#btnAdd').click(showAddLodgerModal);
    $('#btnSearch').click(showSearchModal);
});

var ownedApartmentsMap = {};
var ownedCarsMap = {};
var ownedParkingSpacesMap = {};

function LoadData() {
    GetHouseAdress();
    GetLodgers();
}

function GetHouseAdress() {
    $.ajax({
        url: '/Lodgers/GetHouseAdress',
        type: 'GET',
        dataType: 'Json',
        contentType: 'application/json;charset=utf-8',
        success: function (response) {
            if (response == null || response == undefined || response.Length == 0) {
                $('#HouseAdress').html('Не указано');
            }
            else {
                $('#HouseAdress').html(response);
            }
        },
        error: function (error) {
            alert(error);
        }
    });
}

function GetLodgers() {
    $.ajax({
        url: '/Lodgers/GetLodgers',
        type: 'GET',
        dataType: 'Json',
        contentType: 'application/json;charset=utf-8',
        success: function (response) {
            DisplayLodgers(response);
        },
        error: function (error) {
            alert(error);
        }
    });
}

function showAddLodgerModal() {
    ClearData();
    $('#LodgerModalAdd').modal('show');
}

function showSearchModal() {
    ClearSearchData();
    $('#LodgerModalSearch').modal('show');
}

function Insert() {
    var result = Validate();
    if (!result)
        return result;

    var formData = new Object();
    formData.id = $('#Id').val();
    formData.surname = $('#Surname').val();
    formData.name = $('#Name').val();
    formData.pathronymic = $('#Pathronymic').val();
    formData.passportnumber = $('#PassportNumber').val();

    MakeInsert(formData);
}

function MakeInsert(formData) {
    $.ajax({
        url: '/Lodgers/Insert',
        data: formData,
        type: 'POST',
        success: function (response) {
            if (response == null || response == undefined || response.Length == 0) {
                alert('Unable to save the data');
            }
            else {
                HideModal();
                GetLodgers();
            }
        },
        error: function () {
            alert('Unable to save the data');
        }
    });
}

function Edit(id) {
    HideAllEdit();
    $.ajax({
        url: '/Lodgers/Edit?id=' + id,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        datatype: 'json',
        success: function (response) {
            if (response == null || response == undefined) {
                alert('Unable to read the data');
            }
            else if (response.length == 0) {
                alert('Data not available with the id' + id);
            }
            else
                showEditModal(response);
        },
        error: function () {
            alert('Unable to read the data');
        }
    });
}

function showEditModal(response) {
    $('#LodgerModalEdit').modal('show');
    $('#Update').css('display', 'block');
    $('#Id').val(response.id);
    $('#SurnameEdit').val(response.surname);
    $('#NameEdit').val(response.name);
    $('#PathronymicEdit').val(response.pathronymic);
    $('#PassportNumberEdit').val(response.passportNumber);

    updateOwnedItems(response.ownedApartments, 'apartmentsTableBody', 'apartment');
    updateOwnedItems(response.ownedCars, 'carsTableBody', 'car');
    updateOwnedItems(response.ownedParkingSpaces, 'parkingSpacesTableBody', 'parkingSpace');
    $('#carsLength').html(`Количество: ${response.ownedCars.length} шт.`);
    $('#parkingSpacesLength').html(`Количество: ${response.ownedParkingSpaces.length} шт.`);
}

function updateOwnedItems(items, tableBodyId, category) {
    let object = '';
    items.forEach(item => {
        updateOwnedItemMap(item, category);
        object += `<tr>${generateTableRow(item, category)}</tr>`;
    });
    $(`#${tableBodyId}`).html(object);
}

function generateTableRow(item, category) {
    let rowHtml = '';
    if (category === 'apartment') {
        rowHtml += `<td>${item.apartment.number}</td>`;
        rowHtml += `<td>${item.ownershipPercentage}</td>`;
        rowHtml += `<td>${item.apartment.residentsNumber}</td>`;
        rowHtml += `<td><a href="#" class="btn btn-primary btn-sm" onclick="EditPercentageApartment(${item.id})">Изменить долю</a></td>`;
    } else if (category === 'car') {
        rowHtml += `<td>${item.car.registrationNumber}</td>`;
        rowHtml += `<td>${item.car.brand}</td>`;
        rowHtml += `<td><a href="#" class="btn btn-primary btn-sm" onclick="EditCar(${item.car.id})">Изменить</a><a href="#" class="btn btn-danger btn-sm" onclick="DeleteCar(${item.car.id})">Удалить</a></td>`;
    } else if (category === 'parkingSpace') {
        rowHtml += `<td>${item.parkingSpace.number}</td>`;
        rowHtml += `<td><a href="#" class="btn btn-danger btn-sm" onclick="DeleteParkingSpace(${item.parkingSpace.id})">Отменить бронь</a></td>`;
    }
    return rowHtml;
}

function updateOwnedItemMap(item, category) {
    const itemId = category === 'apartment' ? item.apartment.id : category === 'car' ? item.car.id : item.parkingSpace.id;
    if (category === 'apartment') {
        ownedApartmentsMap[itemId] = item;
    } else if (category === 'car') {
        ownedCarsMap[itemId] = item;
    } else if (category === 'parkingSpace') {
        ownedParkingSpacesMap[itemId] = item;
    }
}

function Update() {
    var result = Validate();
    if (!result)
        return result;

    var formData = new Object();
    formData.id = $('#Id').val();
    formData.surname = $('#SurnameEdit').val();
    formData.name = $('#NameEdit').val();
    formData.pathronymic = $('#PathronymicEdit').val();
    formData.passportNumber = $('#PassportNumberEdit').val();

    $.ajax({
        url: '/Lodgers/Update',
        data: formData,
        type: 'POST',
        success: function (response) {
            if (response == null || response == undefined || response.Length == 0) {
                alert('Unable to save the data');
            }
            else if (!response.success)
                alert(response.message.errorMessage);
            else {
                HideModal();
                GetLodgers();
            }
        },
        error: function () {
            alert('Unable to save the data');
        }
    });
}

function Delete(id) {
    if (confirm('Вы уверены, что хотите удалить жильца?')) {
        $.ajax({
            url: '/Lodgers/Delete?id=' + id,
            type: 'POST',
            success: function (response) {
                if (response == null || response == undefined) {
                    alert('Не получилось удалить данные о жильце');
                }
                else {
                    var id = $('#Id').val();
                    GetLodgers();
                }
            },
            error: function () {
                alert('Не получилось удалить данные о жильце');
            }
        });
    }
}

function HideModal() {
    ClearData();
    $('#LodgerModalAdd').modal('hide');
    $('#LodgerModalEdit').modal('hide');
    $('#LodgerModalSearch').modal('hide');
    $('#carEditBlock').css('display', 'none');
    $('#carAddBlock').css('display', 'none');
}

function ClearData() {
    $('#Surname').val('');
    $('#Name').val('');
    $('#Pathronymic').val('');
    $('#PassportNumber').val('');

    $('#SurnameEdit').val('');
    $('#NameEdit').val('');
    $('#PathronymicEdit').val('');
    $('#PassportNumberEdit').val('');

    $('#Surname').css('border-color', 'lightgrey');
    $('#Name').css('border-color', 'lightgrey');
    $('#Pathronymic').css('border-color', 'lightgrey');
    $('#PassportNumber').css('border-color', 'lightgrey');

    $('#SurnameEdit').css('border-color', 'lightgrey');
    $('#NameEdit').css('border-color', 'lightgrey');
    $('#PathronymicEdit').css('border-color', 'lightgrey');
    $('#PassportNumberEdit').css('border-color', 'lightgrey');
}

function Validate() {
    var isValid = true;

    if ($('#Surname').val().trim() == "") {
        $('#Surname').css('border-color', 'Red');
    }
    else {
        $('#Surname').css('border-color', 'lightgrey');
    }

    if ($('#Name').val().trim() == "") {
        $('#Name').css('border-color', 'Red');
    }
    else {
        $('#Name').css('border-color', 'lightgrey');
    }

    if ($('#Pathronymic').val().trim() == "") {
        $('#Pathronymic').css('border-color', 'Red');
    }
    else {
        $('#Pathronymic').css('border-color', 'lightgrey');
    }

    if ($('#PassportNumber').val().trim() == "") {
        $('#PassportNumber').css('border-color', 'Red');
    }
    else {
        $('#PassportNumber').css('border-color', 'lightgrey');
    }

    if ($('#SurnameEdit').val().trim() == "") {
        $('#SurnameEdit').css('border-color', 'Red');
    }
    else {
        $('#SurnameEdit').css('border-color', 'lightgrey');
    }

    if ($('#NameEdit').val().trim() == "") {
        $('#NameEdit').css('border-color', 'Red');
    }
    else {
        $('#NameEdit').css('border-color', 'lightgrey');
    }

    if ($('#PathronymicEdit').val().trim() == "") {
        $('#PathronymicEdit').css('border-color', 'Red');
    }
    else {
        $('#PathronymicEdit').css('border-color', 'lightgrey');
    }

    if ($('#PassportNumberEdit').val().trim() == "") {
        $('#PassportNumberEdit').css('border-color', 'Red');
    }
    else {
        $('#PassportNumberEdit').css('border-color', 'lightgrey');
    }

    return isValid;
}

$('#Surname').change(function () {
    Validate();
})

$('#Name').change(function () {
    Validate();
})

$('#Pathronymic').change(function () {
    Validate();
})

$('#PassportNumber').change(function () {
    Validate();
})

$('#SurnameEdit').change(function () {
    Validate();
})

$('#NameEdit').change(function () {
    Validate();
})

$('#PathronymicEdit').change(function () {
    Validate();
})

$('#PassportNumberEdit').change(function () {
    Validate();
})

function DisplayLodgers(response) {
    if (response == null || response == undefined || response.Length == 0) {
        var object = '';
        object += '<tr>';
        object += 'td colspan="5">' + 'Lodgers not available' + '</td>';
        object += '</tr>';
        $('#tblBody').html(object);
    }
    else {
        var object = '';
        $.each(response, function (index, item) {
            object += '<tr>';
            object += '<td>' + item.id + '</td>';
            object += '<td>' + item.surname + '</td>';
            object += '<td>' + item.name + '</td>';
            object += '<td>' + item.pathronymic + '</td>';
            object += '<td>' + item.passportNumber + '</td>';
            object += '<td> <a href="#" class="btn btn-primary btn-sm" onclick="Edit(' + item.id + ')">Изменить</a> <a href="#" class="btn btn-danger btn-sm" onclick="Delete(' + item.id + ')">Удалить</a> </td>';
        });
        $('#tblBody').html(object);
    }
}

/* Apartments */

$('#btnEditApartments').click(function () {
    HideAllEdit();
    $('#apartmentsEditBlock').css('display', 'block');
    loadApartments();
});

function loadApartments() {
    $.ajax({
        url: '/Lodgers/GetAllApartments',
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
        url: '/Lodgers/UpdateApartments',
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
        url: '/Lodgers/EditOwnedApartment?id=' + id,
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
        url: '/Lodgers/UpdateOwnedApartment',
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
        url: '/Lodgers/GetAllApartments',
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


/* Cars */

function EditCar(id) {
    HideAllEdit();
    $.ajax({
        url: '/Lodgers/EditCar?id=' + id,
        type: 'GET',
        success: function (response) {
            if (response) {
                $('#CarIdEdit').val(response.id);
                $('#RegistrationNumberEdit').val(response.registrationNumber);
                $('#BrandEdit').val(response.brand);
                $('#carModalTitle').text('Edit Car');
                $('#carEditBlock').css('display', 'block');
            }
        },
        error: function (error) {
            alert('Не получилось получить данные о машине');
        }
    });
}

$('#saveCar').click(function () {
    var formData = {
        id: $('#CarIdEdit').val(),
        registrationNumber: $('#RegistrationNumberEdit').val(),
        brand: $('#BrandEdit').val()
    };

    $.ajax({
        url: '/Lodgers/UpdateCar',
        type: 'POST',
        data: formData,
        success: function (response) {
            if (!response.success) {
                if (typeof (response.message) == "string")
                    alert(response.message);
                else
                    alert(response.message.errorMessage);
            } else {
                $('#carEditBlock').css('display', 'none');
                var id = $('#Id').val();
                Edit(id);
            }
        },
        error: function (error) {
            alert('Error: ' + error);
        }
    });
});

$('#hideCar').click(function () {
    $('#carEditBlock').css('display', 'none');
    $('#carAddBlock').css('display', 'none');
});

$('#btnAddCar').click(function () {
    HideAllEdit();
    $('#carAddBlock').css('display', 'block');
});

$('#addCar').click(function () {
    var registrationNumber = $('#RegistrationNumber').val();
    var brand = $('#Brand').val();
    var ownerId = $('#Id').val();

    var formData = {
        car: {
            registrationNumber: registrationNumber,
            brand: brand
        },
        id: ownerId
    };

    $.ajax({
        url: '/Lodgers/InsertCar',
        type: 'POST',
        data: formData,
        success: function (response) {
            if (!response.success) {
                alert(response.message.errorMessage);
            }
            else {
                $('#carAddBlock').css('display', 'none');
                var id = $('#Id').val();
                Edit(id);
            }
        },
        error: function (error) {
            alert('Не получилось добавить машину');
        }
    });
});

function DeleteCar(id) {
    if (confirm('Вы уверены, что хотите удалить данные о машине?')) {
        var ownerId = $('#Id').val();

        var formData = {
            id: id,
            ownerId: ownerId
        };

        $.ajax({
            url: '/Lodgers/DeleteCar',
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response == null || response == undefined) {
                    alert('Не получилось удалить данные о машине');
                }
                else if (!response.success) {
                    alert('Эта машина принадлежит не только вам, поэтому удалить невозможно');
                }
                var id = $('#Id').val();
                Edit(id);
                ownedCarsMap = {};
            },
            error: function () {
                alert('Не получилось удалить данные о машине');
            }
        });
    }
}

$('#btnEditCars').click(handleBtnEditCarsClick);

function handleBtnEditCarsClick() {
    HideAllEdit();
    $('#carsEditBlock').css('display', 'block');
    getAllCarsData();
}

function getAllCarsData() {
    $.ajax({
        url: '/Lodgers/GetAllCars',
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        datatype: 'json',
        success: function (response) {
            handleAllCarsDataResponse(response);
        },
        error: function () {
            alert('Unable to read the data');
        }
    });
}

function handleAllCarsDataResponse(response) {
    if (response == null || response == undefined) {
        alert('Unable to read the data');
    } else if (response.length == 0) {
        alert('Нет машин');
    } else {
        displayCars(response);
    }
}

function displayCars(cars) {
    var object = '';
    $.each(cars, function (index, car) {
        object += '<tr>';
        object += '<td style="display:none">' + car.id + '</td>';
        var checked = ownedCarsMap[car.id] ? 'checked' : '';
        object += '<td><input type="checkbox" id="carCheckbox_' + car.id + '" value="' + car.id + '" ' + checked + '/></td>';
        object += '<td>' + car.registrationNumber + '</td>';
        object += '<td>' + car.brand + '</td>';
    });
    $('#carsEditTableBody').html(object);
}

$('#saveCars').click(handleSaveCarsClick);

function handleSaveCarsClick() {
    var selectedCars = retrieveSelectedCarsData();
    var ownerId = $('#Id').val();
    var formData = {
        id: ownerId,
        selectedCars: selectedCars,
    };
    updateCars(formData, ownerId);
}

function retrieveSelectedCarsData() {
    var selectedCars = [];
    $('input[type=checkbox]').each(function () {
        if (this.checked) {
            var $row = $(this).closest('tr');
            var carId = $row.find('td:eq(0)').text();
            var registrationNumber = $row.find('td:eq(2)').text();
            var brand = $row.find('td:eq(3)').text();
            selectedCars.push({
                Id: carId,
                RegistrationNumber: registrationNumber,
                Brand: brand
            });
        }
    });
    return selectedCars;
}

function updateCars(formData, ownerId) {
    $.ajax({
        url: '/Lodgers/UpdateCars',
        type: 'POST',
        data: formData,
        success: function (response) {
            $('#carsEditBlock').css('display', 'none');
            Edit(ownerId);
            ownedCarsMap = {};
        },
        error: function (error) {
            alert('Error: ' + error);
        }
    });
}

$('#hideCars').click(function () {
    $('#carsEditBlock').css('display', 'none');
});

/* Parking Spaces */

function DeleteParkingSpace(id) {
    if (confirm('Вы уверены, что хотите отменить бронь на парковочное место?')) {
        $.ajax({
            url: '/Lodgers/DeleteParkingSpace?id=' + id,
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
        url: '/Lodgers/GetAllParkingSpaces?id=' + id,
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
        url: '/Lodgers/UpdateParkingSpaces',
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

/* Search */

$('#Search').click(function () {
    var formData = {
        surname: $('#SurnameSearch').val(),
        name: $('#NameSearch').val(),
        pathronymic: $('#PathronymicSearch').val(),
        passportNumber: $('#PassportNumberSearch').val(),
        apartmentNumber: $('#ApartmentNumberSearch').val(),
        registrationNumber: $('#RegistrationNumberSearch').val(),
        brand: $('#BrandSearch').val(),
        parkingSpaceNumber: $('#ParkingSpaceSearch').val()
    };

    SearchLodgers(formData);
});

function SearchLodgers(criteria) {
    $.ajax({
        url: '/Lodgers/Search',
        type: 'GET',
        data: criteria,
        dataType: 'json',
        success: function (response) {
            DisplayLodgers(response);
            HideSearch();
        },
        error: function (error) {
            alert('Произошла ошибка при выполнении поиска: ' + error);
        }
    });
};

function HideSearch() {
    $('#LodgerModalSearch').modal('hide');
}

function HideAllEdit() {
    $('#apartmentsEditBlock').css('display', 'none');
    $('#apartmentsOwnershipPercentageBlock').css('display', 'none');
    $('#carEditBlock').css('display', 'none');
    $('#carsEditBlock').css('display', 'none');
    $('#carAddBlock').css('display', 'none');
    $('#parkingSpacesEditBlock').css('display', 'none');
    $('#parkingSpaces').css('display', 'none');
}

function ClearSearchData() {
    $('#SurnameSearch').val(''),
        $('#NameSearch').val(''),
        $('#PathronymicSearch').val(''),
        $('#PassportNumberSearch').val(''),
        $('#ApartmentNumberSearch').val(''),
        $('#RegistrationNumberSearch').val(''),
        $('#BrandSearch').val(''),
        $('#ParkingSpaceSearch').val('')
}

$('#SearchCancel').click(function () {
    GetLodgers();
});