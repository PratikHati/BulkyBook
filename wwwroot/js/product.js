$(document).ready(function () {

    debugger;
    myfunc();
});


function myfunc() {
    mytable = $('#myTable').DataTable({
        "ajax": { url:'/admin/product/GetAll' },
        "columns": [
            { data: 'title', "width" : "15%" },
            { data: 'isbn', "width": "15%" },
            { data: 'listPrice', "width": "10%" },
            { data: 'author', "width": "15%" },
            { data: 'category.name', "width": "15%" }/*,
            {
                data: 'id',
                "width": "30%",
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                                 <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i> Edit</a>               
                                 <a class="btn btn-danger mx-2"> <i class="bi bi-trash-fill"></i> Delete</a>
                            </div>` ;
                }
            }*/
        ]
    });
}
