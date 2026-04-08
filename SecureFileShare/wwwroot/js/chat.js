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

                if (data.length === 0) {
                    $("#searchResults").append("<li>No users found.</li>");
                    return;
                }
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
            const recipientName = event.target.textContent; // Get recipient name from list item text

            document.getElementById("chatWith").textContent = `{recipientName}`; // Update chat header

            window.currentRecipientId = recipientId; // Store current recipient ID for sending messages

            $.get("/Chat/GetMessages", { recipientId: recipientId }, function (data) {
                const messagesContainer = document.getElementById("messages");
                messagesContainer.innerHTML = ""; // Clear old messages
                data.forEach(msg => {
                    const message = document.createElement("div");
                    if (msg.senderId === document.getElementById("chats").dataset.userId) {
                        message.classList.add("my-message");
                    }
                    else {
                        message.classList.add("other-message");
                    }
                    message.textContent = `${msg.content} (${new Date(msg.timestamp).toLocaleTimeString([], {
                        hour: '2-digit',
                        minute: '2-digit',
                        hour12: false
                    })})`;
                    messagesContainer.appendChild(message);
                });
            });

        }
    });

    //create connection to SignalR hub
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/chatHub")
        .build();

    // Listen for incoming messages
    connection.on("ReceiveMessage", (senderId, content, timestamp) => {
        const currentUserId = document.getElementById("chats").dataset.userId;
        const chatWithId = window.currentRecipientId;

        const isFromCurrentRecipient = senderId === chatWithId;
        const isFromCurrentUser = senderId === currentUserId && chatWithId;

        if (isFromCurrentRecipient || isFromCurrentUser) {
            const message = document.createElement("div");

            // Style message differently if it's from the current user or another user
            if (senderId === document.getElementById("chats").dataset.userId) {
                message.classList.add("my-message");
            }
            else {
                message.classList.add("other-message");
            }
            message.textContent = `${content} (${new Date(timestamp).toLocaleTimeString([], {
                hour: '2-digit',
                minute: '2-digit',
                hour12: false
            })})`;
            document.getElementById("messages").appendChild(message);
        }
        else {
            // Optionally, show a notification for messages from other users
            alert("New message from " + senderId);
        }
    });

    // Start the connection
    connection.start().catch(err => console.error(err.toString()));

    //Make user list clickable and add the clicked user's ID as the recipient ID
    document.getElementById("chatUsers").addEventListener("click", function (event) {
        if (event.target && event.target.nodeName === "LI") {

            const recipientId = event.target.dataset.userId; // Get recipient ID from data attribute
            const recipientName = event.target.textContent; // Get recipient name from list item text

            document.getElementById("chatWith").textContent = `${recipientName}`; // Update chat header

            window.currentRecipientId = recipientId; // Store current recipient ID for sending messages

            $.get("/Chat/GetMessages", { recipientId: recipientId }, function (data) {
                const messagesContainer = document.getElementById("messages");
                messagesContainer.innerHTML = ""; // Clear old messages
                data.forEach(msg => {
                    const message = document.createElement("div");
                    if (msg.senderId === document.getElementById("chats").dataset.userId) {
                        message.classList.add("my-message");
                    }
                    else {
                        message.classList.add("other-message");
                    }
                    message.textContent = `${msg.content} (${new Date(msg.timestamp).toLocaleTimeString([], {
                        hour: '2-digit',
                        minute: '2-digit',
                        hour12: false
                    })})`;
                    messagesContainer.appendChild(message);
                });
            });

        }
    });

    // Send message on button click
    document.getElementById("sendMessage").addEventListener("click", function (event) {

        const senderId = document.getElementById("chats").dataset.userId; // Get sender ID from data attribute
        const content = document.getElementById("messageInput").value;
        const recipientId = window.currentRecipientId; 

        connection.invoke("SendMessage", senderId, recipientId, content)
            .catch(err => console.error(err.toString()));

        document.getElementById("messageInput").value = "";

    });

    document.getElementById("messageInput").addEventListener("keydown", function (event) {

        if (event.key === "Enter") {
            document.getElementById("sendMessage").click();
        }
        

    });

});