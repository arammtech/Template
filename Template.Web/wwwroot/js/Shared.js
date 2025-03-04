const theme = localStorage.getItem("theme");


// Function to hide the spinner and show content after the page loads

document.addEventListener('DOMContentLoaded', function () {
    document.getElementById('loading-spinner').style.display = 'none';
    document.getElementById('content').style.display = 'block';
    const body = document.body;

    if (theme) {
        if (theme == 'dark') {
            body.classList.add("dark");
         
        } else {
            body.classList.add("light");
        }
    }
    else {
        localStorage.setItem("theme", "light");
    }
   
});



