function loadContacts() {
    $.ajax({
        url: "/ContactAjax/GetAllContacts",
        type: "GET",
        success: function (data) {
            $("#tblBody").empty();
            var currentUserRole = 'Staff'; // Replace with the actual implementation
            console.log(currentUserRole);

            $.each(data, function (index, item) {
                var row = `
          <tr>
            <td>${item.FName}</td>
            <td>${item.LName}</td>
            <td>
              <button class="${(item.IsActive ? " btn btn-success" : " btn btn-danger")} toggle-status-button" data-userid="${item.ContactId}" data-is-active="${item.IsActive}" id="toggle">
                ${(item.IsActive ? "Active" : "Inactive")}
              </button>
            </td>
        
            <td>
              <button onclick="editItem(${item.ContactId})" value="Edit" class="btn btn-warning">Edit</button>
            </td>
            <td>
            <a href="/ContactDetail/Index?contactid=${item.ContactId}">
              <button class="btn btn-info">Contact Details</button>
            </a>
            </td>
         </tr>`;

                $("#tblBody").append(row);
            });
        },
        error: function (err) {
            alert("Error loading contacts");
        }
    });
}

function addNewRecord(item) {
    $.ajax({
        url: "/ContactAjax/Create",
        type: "POST",
        data: item,
        success: function (success) {
            alert("Contact Added Successfully")
            loadContacts()
        },
        error: function (error) {
            alert("Error in addition of User")
        }
    })
}

//function createContact() {
//    var contact = {
//        FName: $("#FName").val(),
//        LName: $("#LName").val()
//    };

//    $.ajax({
//        url: "/ContactAjax/Create",
//        type: "POST",
//        data: contact,
//        success: function (data) {
//            loadContacts();
//            $("#contact-create").hide();
//            $("#contact-list").show();
//        },
//        error: function (err) {
//            alert("Error creating contact");
//        }
//    });
//}

function editItem(contactId) {
    $("#contact-list").slideUp(); // Hide contact list
    $.ajax({
        url: "/ContactAjax/Edit",  // This calls the GET Edit action
        type: "GET",
        data: { contactId: contactId },
        success: function (data) {
            $("#contact-edit").html(data);
            $("#contact-edit").slideDown(); // Show the edit form
        },
        error: function (err) {
            alert("Error loading contact for edit");
        }
    });
}


$(document).ready(function () {
    $("#btnAdd").click(function () {
        $("#contact-list").slideUp(); // Hide the contact list
        $.ajax({
            url: "/ContactAjax/Create",
            type: "GET",
            success: function (data) {
                $("#contact-create").html(data);
                $("#contact-create").slideDown(); // Show the create partial view
            }
        });
    });
    $("#contact-edit").on("submit", "#edit-contact-form", function (e) {
        e.preventDefault();  // Prevent default form submission

        $.ajax({
            url: "/ContactAjax/Edit",  // This calls the POST Edit action
            type: "POST",
            data: $(this).serialize(),  // Serialize form data
            success: function (data) {
                $("#contact-edit").slideUp();  // Hide the edit form
                $("#contact-list").slideDown();  // Show the contact list
                loadContacts();  // Reload the contacts
            },
            error: function (err) {
                alert("Error saving contact");
            }
        });
    });

    //$("#contact-create").on("click", "#btnCreate", function () {
    //    $.ajax({
    //        url: "/ContactAjax/Create",
    //        type: "POST",
    //        //data: $("#contact-create-form").serialize(),
    //        success: function (data) {
    //            $("#contact-create").slideUp(); // Hide the create partial view
    //            $("#contact-list").slideDown(); // Show the contact list again
    //            $.ajax({
    //                url: "/ContactAjax/GetContactList",
    //                type: "GET",
    //                success: function (data) {
    //                    $("#contact-list").html(data);
    //                }
    //            });
    //        }
    //    });
    //});


       
    
});

$(document).on('click', '.toggle-status-button', function () {
    var button = $(this); // Store the button element
    var contactId = button.data('userid'); // Get the contact ID from the data attribute
    var isActive = button.data('is-active'); // Get the current active status from the data attribute

    var newStatus = !isActive; // Toggle the active status

    $.ajax({
        url: '/ContactAjax/ToggleActiveStatus', // Adjust URL to match your controller/action
        type: 'POST',
        data: {
            contactId: contactId,
            isActive: newStatus
        },
        success: function (response) {
            if (response.success) {
                alert(response.message);
                // Update the button text and data attribute
                button.text(newStatus ? 'Active' : 'Inactive');
                button.data('is-active', newStatus);
                button.toggleClass('btn-danger btn-success'); // Change button class based on status
            } else {
                alert(response.message);
            }
        },
        error: function (xhr, status, error) {
            alert('An error occurred: ' + error);
        }
    });
});
