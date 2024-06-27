function EditCar(id) {
    HideAllEdit();
    $.ajax({
        url: '/Cars/Edit?id=' + id,
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
        url: '/Cars/Update',
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
    $('#carAddBlock').css('display', 'none');
});

$('#hideCarEdit').click(function () {
    $('#carEditBlock').css('display', 'none');
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
        url: '/Cars/Insert',
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
            url: '/Cars/Delete',
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
        url: '/Cars/Get',
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
        url: '/Cars/UpdateCars',
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