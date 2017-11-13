function saveRow($tr) {
    // new rows have no guid id, they get removed and replaced
    // all existing rows get replaced by return from server
    // if no return from server, they get marked in an error state

    $tr.addClass("danger")  // not working??
        .css("background-color", "red");
    console.log($tr);

    var row = {};
    $tr.find("input").each(function () {
        var val = $(this).val();
        if (this.type == "checkbox") val = this.checked;
        row[this.name] = val;
    });

    // next have an endpoint that loads the data, then display it in the html
    // and then have endpoints to receive additions, updates and deletes
    // of vague interest: http://todomvc.com/examples/jquery/#/all -- not really just jquery tho
    console.log("Saving!", row);
    $.get(
        "/",
        function(data) {
            $(".result").html(data);
        }
    );
}

function addRow(row) {
    $(".crons-table tbody").append(
        $("<tr>")
            .append(
                $("<input>", {
                    type: "hidden",
                    name: "id"
                }).val(row["id"])
            )
            .append(
                $("<td>").append(
                    $("<input>", {
                        type: "text",
                        name: "name"
                    }).val(row["name"])
                )
            )
            .append(
                $("<td>").append(
                    $("<input>", {
                        type: "checkbox",
                        name: "enabled",
                        value: "1"
                    }).prop("checked", row["enabled"])
                )
            )
            .append(
                $("<td>").append(
                    $("<div>Save</div>")
                        .addClass("btn btn-sm btn-primary")
                        .click(function () {
                            saveRow($(this).closest("tr"));
                        })
                )
            )
    );
}

$(document).ready(function(){
    // replace this with lookup for remote data
    [
        {
            id: "id1",
            name: "John",
            enabled: false,
        },
        {
            id: "id2",
            name: "Mary",
            enabled: true,
        },
        {
            id: "id3",
            name: "Sam",
            enabled: true,
        },
    ].forEach(addRow);
});
