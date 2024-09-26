$(document).ready(function () {
    $("#grid").jqGrid({
        url: "/ContactDetail/GetData",
        datatype: "json",
        colNames: ["ContactDetailId", "Email","Type" ],
        colModel: [
            { name: "ContactDetailId", key: true, hidden: true },
            //{ name: "ContactId", hidden: true }, // Hidden foreign key
            
            { name: "Email", index: "Email", editable: true },
            { name: "Type", index: "Type", editable: true }
        ],
        width: "500",
        height: "50%",
        caption: "Contact Details Records",
        pager: "#pager",
        rowNum: 5,
        rowList: [5, 10, 15],
        sortname: 'id',
        sortorder: 'asc',
        viewrecord: true,
        height: "250",

        gridComplete: function () {
            $("#grid").jqGrid('navGrid', '#pager',
                {
                    edit: true,
                    add: true,
                    del: true,
                    search: true,
                    refresh: true
                },
                {
                    url: "/ContactDetail/Edit",
                    editurl: "/ContactDetail/Edit",
                    closeAfterEdit: true,
                    width: "800",
                    serializeEditData: function (postdata) {
                        return {
                            detail: {
                                ContactDetailId: postdata.id,
                                Type: postdata.Type,
                                Email: postdata.Email,

                            }
                        };
                    },
                    afterSubmit: function (response, postdata) {
                        var result = JSON.parse(response.responseText);
                        if (result.success) {
                            alert(result.message);
                            return [true];
                        } else {
                            alert(result.message);
                            return [false];
                        }
                    }
                },
                {
                    url: "/ContactDetail/Add",
                    closeAfterAdd: true,
                    width: "600",
                    afterSubmit: function (response, postdata) {
                        var result = JSON.parse(response.responseText);
                        if (result.success) {
                            alert(result.message);
                            return [true];
                        } else {
                            alert(result.message);
                            return [false];
                        }
                    }
                },
                {
                    url: "/ContactDetail/Delete",
                    afterSubmit: function (response, postdata) {
                        var result = JSON.parse(response.responseText);
                        if (result.success) {
                            alert(result.message);
                            return [true];
                        } else {
                            alert(result.message);
                            return [false];
                        }
                    },
                },
                {
                    multipleSearch: false,
                    closeAfterSearch: true
                }

                    
            );
        }        
    })
})

