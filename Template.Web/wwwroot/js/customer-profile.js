﻿
import '../lib/toastr.js/toastr.min.js';

document.addEventListener("DOMContentLoaded", function () {
    const deleteBtn = document.getElementById("deleteUser");
    const lockBtn = document.getElementById("lockUser");

    if (deleteBtn) {
        deleteBtn.addEventListener("click", function () {
            const id = deleteBtn.dataset.id;
            const url = `/api/customer/user?id=${id}`;
            deleteConfirm(url);
        });
    }

    if (lockBtn) {
        lockBtn.addEventListener("click", function () {
            const id = lockBtn.dataset.id;
            lockUnlock(id);
        });
    }
});


function deleteConfirm(url) {
    Swal.fire({
        title: "هل أنت متأكد؟",
        text: "لن تتمكن من التراجع عن هذا الإجراء!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "نعم، احذفها!",
        cancelButtonText: "إلغاء",
        customClass: {
            title: "swal-title",
            htmlContainer: "swal-text",
            confirmButton: "btn btn-sm  btn-error jump",
            cancelButton: "btn btn-sm  btn-2 jump"
        }
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (response) {
                    if (response.success) {
                        sessionStorage.setItem('toastr-success-message', response.message);

                        // I need to logout the user how
                        window.location.href = '/customer/home/index';
                    } else {
                        toastr.error(response.message || "حدث خطأ غير متوقع.");
                    }
                },
                error: function (xhr) {
                    // Handle server-side error (status 500, etc.)
                    const response = xhr.responseJSON;
                    if (response && response.message) {
                        toastr.error(response.message); // Display the error message using toastr
                    } else {
                        toastr.error("حدث خطأ غير معروف."); // Fallback message
                    }
                }
            });
        }
    });
}
