// Wait until the DOM is ready
$(document).ready(function () {
    // Attach event listener to the search input
    $("#searchInput").on("keyup", function () {
        const query = $(this).val();

        // Only search if 3+ characters
        if (query.length > 2) {
            $.get("/Chat/SearchUsers", { query: query }, function (data) {
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

    // Make search results clickable and add the clicked user's ID as the recipient ID
    document.getElementById("searchResults").addEventListener("click", function (event) {
        if (event.target && event.target.nodeName === "LI") {

            const recipientId = event.target.dataset.userId; // Get recipient ID from data attribute
            const recipientName = event.target.textContent; // Get recipient name from list item text

            document.getElementById("chatWith").textContent = `${recipientName}`; // Update chat header

            window.currentRecipientId = recipientId; // Store current recipient ID for sending messages

            //Add recipient to chat list if not already there (new conversations)
            if (!document.querySelector(`#chatUsers li[data-user-id="${recipientId}"]`)) {
                const newUserLi = document.createElement("li");
                newUserLi.dataset.userId = recipientId;
                newUserLi.classList.add("chatUser");
                newUserLi.textContent = `${recipientName}`;
                document.getElementById("chatUsers").appendChild(newUserLi);
            }

            //Add chat history to the chat box
            $.get("/Chat/GetMessages", { recipientId: recipientId }, function (data) {
                const messagesContainer = document.getElementById("messages");
                messagesContainer.innerHTML = ""; // Clear old messages

                // Add each message to the chat box, styling differently if it's from the current user or another user
                // Also format the timestamp to only show hours and minutes
                data.forEach(msg => {
                    appendMessage(msg);
                });
                connection.invoke("MarkAllRead", recipientId).catch(err => console.error(err.toString()));
            });
            
        }
    });

    //create connection to SignalR hub
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    // Listen for incoming messages
    connection.on("ReceiveMessage", (msg) => {
        console.log("Raw Message from Hub:", msg);

        const currentUserId = document.getElementById("chats").dataset.userId.toLowerCase();
        const chatWithId = (window.currentRecipientId || "").toLowerCase();
        const msgSenderId = msg.senderId.toLowerCase();

        console.log(`Comparing: Sender(${msgSenderId}) | Me(${currentUserId}) | ActiveChat(${chatWithId})`);

        const isFromCurrentUser = msgSenderId === currentUserId;
        const isFromCurrentChatPartner = msgSenderId === chatWithId;

        let existingUserLi = null;
        document.querySelectorAll("#chatUsers li").forEach(li => {
            if (li.dataset.userId && li.dataset.userId.toLowerCase() === msgSenderId) {
                existingUserLi = li;
            }
        });

        if (!isFromCurrentUser && !existingUserLi) {
            console.log("New user detected, adding to sidebar...");
            existingUserLi = document.createElement("li");
            existingUserLi.dataset.userId = msg.senderId;
            existingUserLi.classList.add("chatUser");
            existingUserLi.textContent = msg.senderName;
            document.getElementById("chatUsers").appendChild(existingUserLi);
        }

        if (isFromCurrentUser || isFromCurrentChatPartner) {
            console.log("Appending message to active chat window...");
            appendMessage(msg);
            if (isFromCurrentChatPartner) {
                connection.invoke("MarkAsRead", msg.messageId || msg.MessageId)
                    .catch(err => console.error(err.toString()));
            }
        }
        else {
            console.log("Background message! Applying unread class to:", existingUserLi);
            if (existingUserLi) {
                existingUserLi.classList.add("unread");
            }
        }
    });

    connection.on("MessageRead", (messageId) => {
        const tick = document.querySelector(`[data-msg-id="${messageId}"] .status-tick`);
        if (tick) {
            tick.textContent = "✓✓";
        }
    });

    connection.on("AllMessagesRead", (userId) => {
        if (window.currentRecipientId && window.currentRecipientId.toLowerCase() === userId.toLowerCase()) {
            document.querySelectorAll(".status-tick").forEach(tick => tick.textContent = " ✓✓");
        }
    });

    // Start the connection
    connection.start().catch(err => console.error(err.toString()));

    //Make user list clickable and add the clicked user's ID as the recipient ID
    document.getElementById("chatUsers").addEventListener("click", function (event) {
        if (event.target && event.target.nodeName === "LI") {

            const recipientId = event.target.dataset.userId; // Get recipient ID from data attribute
            const recipientName = event.target.textContent; // Get recipient name from list item text
            if (event.target.classList.contains("unread")) {
                event.target.classList.remove("unread");
            }

            document.getElementById("chatWith").textContent = `${recipientName}`; // Update chat header

            window.currentRecipientId = recipientId; // Store current recipient ID for sending messages

            $.get("/Chat/GetMessages", { recipientId: recipientId }, function (data) {
                const messagesContainer = document.getElementById("messages");
                messagesContainer.innerHTML = ""; // Clear old messages
                data.forEach(msg => {
                    appendMessage(msg);
                });
                connection.invoke("MarkAllRead", recipientId).catch(err => console.error(err.toString()));
            });
        }
    });

    // Send message on button click
    document.getElementById("sendMessage").addEventListener("click", function (event) {

        const senderId = document.getElementById("chats").dataset.userId; // Get sender ID from data attribute
        const content = document.getElementById("messageInput").value;
        const recipientId = window.currentRecipientId; 

        connection.invoke("SendMessage", recipientId, content)
            .catch(err => console.error(err.toString()));

        document.getElementById("messageInput").value = "";

    });

    // Send message on Enter key press
    document.getElementById("messageInput").addEventListener("keydown", function (event) {

        if (event.key === "Enter") {
            document.getElementById("sendMessage").click();
        }

    });

    document.getElementById("uploadFileNew").addEventListener("click", function (e) {
        e.preventDefault();
        document.getElementById("hiddenFileInput").click();
        openPopUp();
    })

    document.getElementById("hiddenFileInput").addEventListener("change", function () {

        const file = this.files[0];
        if (file) {
            const recipientId = window.currentRecipientId;
            const formData = new FormData();
            formData.append("file", file);
            formData.append("recipientId", recipientId);

            const input = document.getElementById("messageInput");
            const originalPlaceholder = input.placeholder;
            input.placeholder = "Uploading file...";
            input.disabled = true;

            $.ajax({
                url: "/Chat/SendFile",
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,
                success: function (ChatDTO) {
                    console.log("File sent successfully");
                },
                error: function () {
                    console.error("Error sending file");
                },
                complete: function () {
                    document.getElementById("hiddenFileInput").value = "";
                    input.placeholder = originalPlaceholder;
                    input.disabled = false;
                }
            });
        }

    });

});


function loadUserFiles() {
    var fileList = document.getElementById("existingFiles");

    fileList.innerHTML = "<li>Loading...</li>"; // Clear existing list

    $.get("/Chat/GetUserFiles", function (data) {
        fileList.innerHTML = ""; // Clear loading message
        data.forEach(file => {
            const fileItem = document.createElement("li");
            fileItem.classList.add("file-option");
            fileItem.textContent = `${file.fileName}`;

            fileItem.onclick = function () {
                console.log(`File ID = ${file.fileId}`)
                console.log(`File URL = ${file.fileUrl}`)
                shareExistingFile(file.fileId);
                openPopUp();
            }

            fileList.appendChild(fileItem);
        })

    });
}

function shareExistingFile(fileId) {
    const recipientId = window.currentRecipientId;

    if (!recipientId) {
        alert("Please select a chat partner before sharing a file.");
        return;
    }

    $.post("/Chat/SelectFile", { fileId: fileId, recipientId: recipientId }, function (ChatDTO) {
        console.log("File shared successfully");
    }).fail(function (errorThrown) {
        console.error(`Error sharing file ${errorThrown}`);
    });

}

// Function to toggle the visibility of the pop-up
function openPopUp() {
    var popUp = document.getElementById("popUp");
    popUp.classList.toggle("show");
        if (popUp.classList.contains("show")) {
            loadUserFiles();
        }
}


function appendMessage(msg) {
    const messagesContainer = document.getElementById("messages");
    const currentUserId = document.getElementById("chats").dataset.userId.toLowerCase();

    const messageDiv = document.createElement("div");
    const isMe = msg.senderId.toLowerCase() === currentUserId;
    messageDiv.classList.add(isMe ? "my-message" : "other-message");

    const time = new Date(msg.timestamp).toLocaleTimeString([], {
        hour: '2-digit', minute: '2-digit', hour12: false
    });

    messageDiv.setAttribute("data-msg-id", msg.messageId || msg.MessageId);

    // Check for FileURL (C# case) or fileURL (JSON case)
    const url = msg.fileURL || msg.fileUrl;

    if (url) {
        const link = document.createElement("a");
        link.href = url;
        link.target = "_blank";
        link.textContent = msg.content;
        link.style.color = "inherit";
        messageDiv.appendChild(link);
    } else {
        const textSpan = document.createElement("span");
        textSpan.textContent = msg.content;
        messageDiv.appendChild(textSpan);
    }

    if (isMe) {
        const statusSpan = document.createElement("span");
        statusSpan.classList.add("status-tick");
        statusSpan.textContent = msg.isRead ? "✓✓" : "✓";
        messageDiv.appendChild(statusSpan);
    }

    // Add timestamp
    const timeNode = document.createTextNode(` (${time})`);
    messageDiv.appendChild(timeNode);

    messagesContainer.appendChild(messageDiv);
    messagesContainer.scrollTop = messagesContainer.scrollHeight;
}