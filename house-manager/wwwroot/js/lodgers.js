$(document).ready(function () {
    GetLodgers();
});

var ownedApartmentsMap = {};
var ownedCarsMap = {};
var ownedParkingSpacesMap = {};

/*Read Data*/
function GetLodgers() {
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
    $.ajax({
        url: '/Lodgers/GetLodgers',
        type: 'GET',
        dataType: 'Json',
        contentType: 'application/json;charset=utf-8',
        success: function (response) {
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
        },
        error: function (error) {
            alert(error);
        }
    });
}

$('#btnAdd').click(function () {
    ClearData();
    $('#LodgerModalAdd').modal('show');
});


/*Insert Data*/
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
                alert(response);
            }
        },
        error: function () {
            alert('Unable to save the data');
        }
    });
}

/*Edit Data*/
function Edit(id) {
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
            else {

                $('#LodgerModalEdit').modal('show');
                $('#Update').css('display', 'block');
                $('#Id').val(response.id);
                $('#SurnameEdit').val(response.surname);
                $('#NameEdit').val(response.name);
                $('#PathronymicEdit').val(response.pathronymic);
                $('#PassportNumberEdit').val(response.passportNumber);

                var object = '';
                $.each(response.ownedApartments, function (index, item) {
                    ownedApartmentsMap[item.apartment.id] = item;
                });

                $.each(response.ownedCars, function (index, item) {
                    ownedCarsMap[item.car.id] = item;
                });

                $.each(response.ownedParkingSpaces, function (index, item) {
                    ownedParkingSpacesMap[item.parkingSpace.id] = item;
                });

                $.each(response.ownedApartments, function (index, item) {
                    object += '<tr>';
/*                    object += '<td><input type="checkbox" id="apartmentCheckbox_' + item.apartment.id + '" value="' + item.apartment.id + '" /></td>';*/
                    object += '<td>' + item.apartment.number + '</td>';
                    object += '<td>' + item.ownershipPercentage + '</td>';
                    object += '<td>' + item.apartment.residentsNumber + '</td>';
                    object += '<td> <a href="#" class="btn btn-primary btn-sm" onclick="EditPercentageApartment(' + item.id + ')">Изменить долю</td>';
                });
                $('#apartmentsTableBody').html(object);

                object = '';
                $.each(response.ownedCars, function (index, item) {
                    object += '<tr>';
                    object += '<td>' + item.car.registrationNumber + '</td>';
                    object += '<td>' + item.car.brand + '</td>';
                    object += '<td> <a href="#" class="btn btn-primary btn-sm" onclick="EditCar(' + item.car.id + ')">Изменить</a> <a href="#" class="btn btn-danger btn-sm" onclick="DeleteCar(' + item.car.id + ')">Удалить</a> </td>';
                });
                $('#carsLength').html('Количество: ' + response.ownedCars.length + 'шт.');
                $('#carsTableBody').html(object);

                object = '';
                $.each(response.ownedParkingSpaces, function (index, item) {
                    object += '<tr>';
                    object += '<td>' + item.parkingSpace.number + '</td>';
                    object += '<td> <a href="#" class="btn btn-danger btn-sm" onclick="DeleteParkingSpace(' + item.parkingSpace.id + ')">Отменить бронь</a> </td>';
                });
                $('#parkingSpacesLength').html('Количество: ' + response.ownedParkingSpaces.length + 'шт.');
                $('#parkingSpacesTableBody').html(object);
            }
        },
        error: function () {
            alert('Unable to read the data');
        }
    });
}

/*Update Data*/
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
                alert(response);
            }
        },
        error: function () {
            alert('Unable to save the data');
        }
    });
}

/* Delete Data */
function Delete(id) {
    if (confirm('Вы уверены, что хотите удалить данные о жильце?')) {
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
        //$('#PassportNumber').val(error);
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
        //$('#PassportNumber').val(error);
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


// Edit Car
function EditCar(id) {
    // Make an AJAX request to get car data for the given ID
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
            console.log('Error: ' + error);
        }
    });
}

// Save Car
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
                alert(response.message);
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

// Hide Car Edit Block
$('#hideCar').click(function () {
    $('#carEditBlock').css('display', 'none');
    $('#carAddBlock').css('display', 'none');
});

// Add Car
$('#btnAddCar').click(function () {
    $('#carEditBlock').css('display', 'none');
    $('#carAddBlock').css('display', 'block');
    $('#carModalTitle').text('Add Car');
});

// Add Car
$('#addCar').click(function () {
    var registrationNumber = $('#RegistrationNumber').val();
    var brand = $('#Brand').val();
    var ownerId = $('#Id').val();  // Получаем Id жильца

    var formData = {
        car: {
            registrationNumber: registrationNumber,
            brand: brand
        },
        id: ownerId  // Передаем Id жильца
    };

    $.ajax({
        url: '/Lodgers/InsertCar',
        type: 'POST',
        data: formData,
        success: function (response) {
            // Handle the response
            $('#carAddBlock').css('display', 'none');
            var id = $('#Id').val();
            Edit(id);
        },
        error: function (error) {
            console.log('Error: ' + error);
        }
    });
});

// Delete Car
function DeleteCar(id) {
    if (confirm('Вы уверены, что хотите удалить данные о машине?')) {
        var ownerId = $('#Id').val();

        var formData = {
            id: id,
            ownerId: ownerId  // Передаем Id жильца
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

// Edit ApartmentsList
$('#btnEditApartments').click(function () {
    $('#apartmentsEditBlock').css('display', 'block');
    $.ajax({
        url: '/Lodgers/GetAllApartments',
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        datatype: 'json',
        success: function (response) {
            if (response == null || response == undefined) {
                alert('Unable to read the data');
            }
            else if (response.length == 0) {
                alert('В доме нет квартир)');
            }
            else {

                var object = '';

                $.each(response, function (index, apartment) {
                    object += '<tr>';
                    object += '<td style="display:none">' + apartment.id + '</td>';
                    var checked = ownedApartmentsMap[apartment.id] ? 'checked' : ''; // Проверяем, принадлежит ли текущая квартира пользователю
                    object += '<td><input type="checkbox" id="apartmentCheckbox_' + apartment.id + '" value="' + apartment.id + '" ' + checked + '/></td>';
                    object += '<td>' + apartment.number + '</td>';
/*                    object += '<td>' + 'значение для поля ownershipPercentage' + '</td>'; // добавьте соответствующее значение*/
                    object += '<td>' + apartment.residentsNumber + '</td>'; // добавьте соответствующее значение
                });
                $('#apartmentsEditTableBody').html(object);
            }
        },
        error: function () {
            alert('Unable to read the data');
        }
    });
});

// Save ApartmentsList

$('#saveApartments').click(function () {
    var selectedApartments = [];

    // Находим все отмеченные чекбоксы квартир и добавляем их в список selectedApartments
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

    var ownerId = $('#Id').val();

    var formData = {
        id: ownerId,
        selectedApartments: selectedApartments,
    };

    $.ajax({
        url: '/Lodgers/UpdateApartments',
        type: 'POST',
        data: formData,
        success: function (response) {
            $('#apartmentsEditBlock').css('display', 'none');
            Edit(ownerId);
            ownedApartmentsMap = {};
/*            alert('Рекомендуется изменить проценты собственности');*/
        },
        error: function (error) {
            console.log('Error: ' + error);
        }
    });
});

// Hide Edit ApartmentsList
$('#hideApartments').click(function () {
    $('#apartmentsEditBlock').css('display', 'none');
    $('#apartmentsOwnershipPercentageBlock').css('display', 'none');
    ownedApartmentsMap = {};
});

// Cars

// Edit CarsList
$('#btnEditCars').click(function () {
    $('#carsEditBlock').css('display', 'block');
    $.ajax({
        url: '/Lodgers/GetAllCars',
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        datatype: 'json',
        success: function (response) {
            if (response == null || response == undefined) {
                alert('Unable to read the data');
            }
            else if (response.length == 0) {
                alert('Нет машин)');
            }
            else {

                var object = '';

                $.each(response, function (index, car) {
                    object += '<tr>';
                    object += '<td style="display:none">' + car.id + '</td>';
                    var checked = ownedCarsMap[car.id] ? 'checked' : '';
                    object += '<td><input type="checkbox" id="carCheckbox_' + car.id + '" value="' + car.id + '" ' + checked + '/></td>';
                    object += '<td>' + car.registrationNumber + '</td>';
                    object += '<td>' + car.brand + '</td>';
                });
                $('#carsEditTableBody').html(object);
            }
        },
        error: function () {
            alert('Unable to read the data');
        }
    });
});

// Save CarsList

$('#saveCars').click(function () {
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

    var ownerId = $('#Id').val();

    var formData = {
        id: ownerId,
        selectedCars: selectedCars,
    };

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
            console.log('Error: ' + error);
        }
    });
});

// Hide Edit ApartmentsList
$('#hideCars').click(function () {
    $('#carsEditBlock').css('display', 'none');
});


// ParkingSpaces

function DeleteParkingSpace(id) {
    if (confirm('Вы уверены, что хотите удалить данные о парковочном месте?')) {
        $.ajax({
            url: '/Lodgers/DeleteParkingSpace?id=' + id,
            type: 'POST',
            success: function (response) {
                if (response == null || response == undefined) {
                    alert('Не получилось удалить данные о парковочном месте');
                }
                else {
                    var id = $('#Id').val();
                    Edit(id);
                }
            },
            error: function () {
                alert('Не получилось удалить данные о парковочном месте');
            }
        });
    }
}

// Edit parkingSpacesList
$('#btnEditParkingSpaces').click(function () {
    $('#parkingSpacesEditBlock').css('display', 'block');
    var id = $('#Id').val();
    $.ajax({
        url: '/Lodgers/GetAllParkingSpaces?id=' + id,
        type: 'GET',
        contentType: 'application/json;charset=utf-8',
        datatype: 'json',
        success: function (response) {
            if (response == null || response == undefined) {
                alert('Unable to read the data');
            }
            else if (response.length == 0) {
                alert('Все парковочные места заняты');
            }
            else {

                var object = '';

                $.each(response, function (index, ownedParkingSpace) {
                    object += '<tr>';
                    object += '<td style="display:none">' + ownedParkingSpace.id + '</td>';
                    var checked = ownedParkingSpacesMap[ownedParkingSpace.id] ? 'checked' : '';
                    object += '<td><input type="checkbox" id="carCheckbox_' + ownedParkingSpace.id + '" value="' + ownedParkingSpace.id + '" ' + checked + '/></td>';
                    object += '<td>' + ownedParkingSpace.number + '</td>';
                });
                $('#parkingSpacesEditTableBody').html(object);
            }
        },
        error: function () {
            alert('Unable to read the data');
        }
    });
});

// Save ParkingSpacesList

$('#saveParkingSpaces').click(function () {
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

    var ownerId = $('#Id').val();

    var formData = {
        id: ownerId,
        selectedParkingSpaces: selectedParkingSpaces,
    };

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
            console.log('Error: ' + error);
        }
    });
});

// Hide Edit ApartmentsList
$('#hideParkingSpaces').click(function () {
    $('#parkingSpacesEditBlock').css('display', 'none');
});

function EditPercentageApartment(id) {
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
            console.log('Error: ' + error);
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

$('#btnSearch').click(function () {
    $('#LodgerModalSearch').css('display', 'block');
});

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
            HideSearch();
        },
        error: function (error) {
            alert('Произошла ошибка при выполнении поиска: ' + error);
        }
    });
};

function HideSearch() {
    $('#LodgerModalSearch').css('display', 'none');
}

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
                callback(response); // Вызываем callback и передаем ему apartmentsList
            }
        },
        error: function () {
            alert('Unable to read the data');
        }
    });
}