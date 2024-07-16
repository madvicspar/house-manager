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
        url: '/Lodgers/GetHouseAddress',
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
        url: '/Lodgers/Get',
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
                if (!response.success)
                    alert(response.message);
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

    ownedApartmentsMap = {};
    ownedCarsMap = {};
    ownedParkingSpacesMap = {};

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
        rowHtml += `<td><a href="#" class="btn btn-primary btn-sm" onclick="EditPercentageApartment(${item.id})">Изменить долю</a><a href="#" class="btn btn-danger btn-sm" onclick="DeleteApartment(${item.apartment.id})">Удалить</a></td>`;
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
            else if (!response.success) {
                if (typeof (response.message) == "string")
                    alert(response.message);
                else
                    alert(response.message.errorMessage);
                Edit(formData.id);
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
            object += '<td style="display:none">' + item.id + '</td>';
            object += '<td>' + item.surname + '</td>';
            object += '<td>' + item.name + '</td>';
            object += '<td>' + item.pathronymic + '</td>';
            object += '<td>' + item.passportNumber + '</td>';
            object += '<td> <a href="#" class="btn btn-primary btn-sm" onclick="Edit(' + item.id + ')">Изменить</a> <a href="#" class="btn btn-danger btn-sm" onclick="Delete(' + item.id + ')">Удалить</a> </td>';
        });
        $('#tblBody').html(object);
    }
}


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

$('#SearchBreak').click(function () {
    GetLodgers();
});