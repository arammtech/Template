//import '../lib/toastr.js/toastr.min.js';


let dataTable;

var $ = jQuery.noConflict();


$(document).ready(function () {
    loadDataTable(
        'admin',
        'user',
        null,null,
        _getTableStructure(),
        _mapTable
    );
});


//export function loadDataTable(area, controller, filterProperty, filter, columns, mapData) {
     function loadDataTable(area, controller, filterProperty, filter, columns, mapData) {
    let url = `/api/${area}/${controller}`;
    //let url = `/${area}/${controller}/GetAll`;
    if (filterProperty && filter) {
        url += `?filterProperty=${filterProperty}&filter=${filter}`;
    }


    // Make the AJAX request
    $.ajax({
        type: 'GET',
        url: url,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            let items = response.data ? response.data : response;
            let tabledata = items.map(item => mapData(item));

            // If a DataTable instance already exists, destroy it before re-initializing
            if ($.fn.DataTable.isDataTable('#tblGeneric')) {
                $('#tblGeneric').DataTable().clear().destroy();
            }

            let table = $('#tblGeneric').DataTable({
                data: tabledata,
                columns: columns,
                autoWidth: false,
                dom: 'lrtip',
                pageLength: 5,
                lengthChange: true,
                lengthMenu: [
                    [5, 10, 25, 50, -1],
                    [5, 10, 25, 50, "All"]
                ],
                language: {
                    search: "بحث:",
                    lengthMenu: "عرض _MENU_ سجل لكل صفحة",
                    info: "عرض _START_ إلى _END_ من _TOTAL_ سجل",
                    infoEmpty: "لا توجد سجلات متاحة",
                    infoFiltered: "(تمت التصفية من إجمالي _MAX_ سجل)",
                    zeroRecords: "لم يتم العثور على سجلات",
                },
            });

            //$('.table-search-box input').on('keyup', function () {
            //    table.search($(this).val()).draw();
            //});

            //$('#add-button').on('click', function () {
            //    window.location.href = `/Admin/${controller}/Create`;
            //});

        },
        error: function (xhr, status, error) {
            console.error("Error retrieving data:", error);
            Swal.fire("Error", "خطأ في استرجاع البيانات", "error");
        }
    });

};


function _getTableStructure() {
    return [
        { data: 'name', 'width': '20%', data: 'name' },
        { data: 'email', 'width': '30%', data: 'email' },
        { data: 'phone', 'width': '15%', data: 'phone' },
        { data: 'role', 'width': '15%', data: 'role' },
        {
            title: 'action',
            data: 'id',
            orderable: false,
            render: function (data) {
                let lockUnlockString = "Lock";
                let lockUnlockIcon = "lock";
                let buttonColor = "success";

                console.log(data);

                if (data.isLocked) {
                    lockUnlockString = "Unlock";
                    lockUnlockIcon = "unlock";
                    buttonColor = "danger";
                }

                return `

                        <div class="btn-group" role="group">
                            <a onclick="lockUnlock(${data.id})" class="btn btn-${buttonColor} mx-2" style="width: 100px">
                                <i class="bi bi-${lockUnlockIcon}-fill"></i> ${lockUnlockString}
                            </a>
                            <a href="/admin/user/roleManagement?userId=${data.id}" class="btn btn-warning">
                                <i class="bi bi-pencil-square"></i> Permissions
                            </a>
                        </div>`;

            },
            'width': '20%',
            orderable: false
        }
    ];
}

function _mapTable(item) {
    return {
        name: item.name,
        email: item.email,
        phone: item.phone,
        role: item.role,
        id: item.id
    };
}




function lockUnlock(userId) {
    $.ajax({
        type: 'POST',
        url: '/Admin/User/LockUnlock',
        data: JSON.stringify(userId),
        contentType: 'application/json',
        success: function (data) {
            if (data.success) {
                dataTable.ajax.reload();
                toastr.success(data.message);
            }
            else {
                toastr.error(data.message);
            }
        }
    });
}




