////import '../lib/toastr.js/toastr.min.js';


let dataTable;

var $ = jQuery.noConflict();

document.addEventListener("DOMContentLoaded", function () {
    loadDataTable(
        'admin',
        'user',
        null, null
    );
    const searchInput = document.getElementById('searchTable');
    const noRecordsRow = document.getElementById('noRecordsRow');

    function filterUsers() {
        const filter = searchInput.value.toLowerCase();
        let visibleCount = 0;

        // Select the rows dynamically (important!)
        const rows = document.querySelectorAll("#listTable tbody .userRow");

        rows.forEach(function (row) {
            const name = row.cells[1].textContent.toLowerCase();
            const email = row.cells[2].textContent.toLowerCase();

            if (name.includes(filter) || email.includes(filter)) {
                row.style.display = "";
                visibleCount++;
            } else {
                row.style.display = "none";
            }
        });

        // Show "No Records" message if all rows are hidden
        noRecordsRow.style.display = visibleCount === 0 ? "" : "none";
    }

    // Listen for search input
    searchInput.addEventListener("keyup", filterUsers);

    // Optionally, apply filter on page load if there's an initial value
    if (searchInput.value.trim() !== "") {
        filterUsers();
    }

    // Dummy function for the Add User button
    document.getElementById('addBtn').addEventListener('click', function () {
        alert('إضافة مستخدم جديد');
    });



});


function loadDataTable(area, controller, filterProperty, filter) {
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


            console.log(items);
            // Clear table before adding new rows
            $("#listTable tbody").empty();

            if (items.length === 0) {
                $("#noRecordsRow").show(); // Show "No Records" row
            } else {
                $("#noRecordsRow").hide(); // Hide "No Records" row
            }


            items.forEach(user => {
                let lockUnlockString = Boolean(user.isLocked) ? "فك القفل" : "قفل";
                let buttonColor = Boolean(user.isLocked) ? "btn-error " : "btn-3";

                let rowHtml = `
    <tr class="userRow">
        <td>${user.id}</td>
        <td>${user.name}</td>
        <td>${user.email}</td>
        <td>${user.phone || "لا يوجد"}</td>
        <td>${Array.isArray(user.role) ? user.role.join('/') : user.role || "N/A"}</td>
        <td class="row-actions">
            <a onclick="lockUnlock(${user.id})" class="action-btn btn btn-sm ${buttonColor} jump ">${lockUnlockString}</a>
            <a class="action-btn btn btn-sm btn-2 jump ">تعديل الدور</a>
            <a class="action-btn btn btn-sm btn-acc jump ">تفاصيل</a>
        </td>
    </tr>
    `;

                $("#listTable tbody").append(rowHtml);
            });

        },
        error: function (xhr, status, error) {
            console.error("AJAX Error:", status, error, xhr.responseText);
            Swal.fire("Error", `خطأ في استرجاع البيانات: ${xhr.responseText}`, "error");
        }
    });

};

function lockUnlock(userId) {
    $.ajax({
        type: 'POST',
        url: `/api/admin/user/LockUnLock?userId=${userId}`, // Note the explicit route segment
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        success: function (data) {
            if (data.success) {
                console.log("Success:", data.message);
                toastr.success(data.message);

                    $("#listTable tbody").empty(); // Clear table before reloading
                    loadDataTable('admin', 'user', null, null); // Reload data

            } else {
                console.log("Error:", data.message);
            }
        },
        error: function (xhr, status, error) {
            console.log("AJAX Error:", xhr.responseText);
            toastr.error("An error occurred.");
        }
    });
}





