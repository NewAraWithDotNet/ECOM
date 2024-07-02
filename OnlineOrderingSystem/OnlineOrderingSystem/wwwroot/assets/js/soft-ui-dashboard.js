// =========================================================
// Soft UI Dashboard - v1.0.7
// =========================================================
document.addEventListener("DOMContentLoaded", function () {
    const links = document.querySelectorAll(".nav-link");

    links.forEach(link => {
        link.addEventListener("click", function (event) {
            event.preventDefault();
            const url = this.getAttribute("data-url");

            fetch(url)
                .then(response => response.text())
                .then(data => {
                    document.getElementById("contentArea").innerHTML = data;
                })
                .catch(error => console.error('Error fetching the partial view:', error));
        });
    });
});
