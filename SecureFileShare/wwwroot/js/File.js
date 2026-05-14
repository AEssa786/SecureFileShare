
let currentFileId = null;
let currentSearch = "";
let currentSort = "FileName";
let currentDir = "asc";

function openPopUp(buttonElement) {
    currentFileId = buttonElement.dataset.fileId;
    console.log("Setting active file ID to: " + currentFileId);

    var popUp = buttonElement.parentElement.querySelector(".popUp");

    // Close any other open pop-ups first (optional but cleaner)
    document.querySelectorAll('.popUp.show').forEach(p => {
        if (p !== popUp) p.classList.remove('show');
    });

    popUp.classList.toggle("show");

    if (popUp.classList.contains("show")) {
        popUp.querySelector("#searchInput").focus();
    }
}

function updateTable() {
    $.get("/File/GetFilesPartial", { search: currentSearch, sortColumn: currentSort, direction: currentDir }, function (data) {
        $("#fileTableBody").html(data);
    });
}

$(document).ready(function () {
    $("#searchInput").on("keyup", function () {
        const query = $(this).val();

        // Only search if 3+ characters
        if (query.length > 2) {
            $.get("/File/SearchUsers", { query: query }, function (data) {
                // Clear old results
                $("#searchResults").empty();

                // If there are no results, show a message
                if (data.length === 0) {
                    $("#searchResults").append("<li>No users found.</li>");
                    return;
                }
                //If there are results, add them to the list with data attributes for user ID
                else {
                    data.forEach(user => {
                        $("#searchResults").append(
                            `<li class="searchResult" data-user-id="${user.id}">${user.userName} (${user.firstName} ${user.lastName})</li>`
                        );
                    });
                }
            });
        } else {
            // Clear results if query too short
            $("#searchResults").empty();
        }
    });

    document.getElementById("searchResults").addEventListener("click", function (event) {
        if (event.target && event.target.nodeName === "LI") {

            const recipientId = event.target.dataset.userId; // Get recipient ID from data attribute

            if (!currentFileId) {
                alert("No file selected.");
                return;
            }

            $.post("/File/Share", { recipientId: recipientId, fileId: currentFileId }, function (response) {
                if (response.success) {
                    alert("File shared successfully!");
                } else {
                    alert("Error sharing file: " + response.message);
                }
                document.querySelectorAll('.popUp.show').forEach(p => p.classList.remove('show'));
            });

        }
    });

    $(".sortable").on("click", function () {
        const column = $(this).data("column");
        if (currentSort === column) {
            currentDir = currentDir === "asc" ? "desc" : "asc";
        } else {
            currentSort = column;
            currentDir = "asc";
        }

        $(".sortable span").text(""); // Clear all sort indicators
        $("#icon-" + column).text(currentDir === "asc" ? "▲" : "▼"); // Set indicator for current column
        updateTable();
    });

    $("#fileSearch").on("keyup", function () {
        currentSearch = $(this).val();
        updateTable();
    });

});
