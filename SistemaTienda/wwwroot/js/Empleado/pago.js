var dataTable;

$(function () {
    cargarDataTable();
});

function cargarDataTable() {
    dataTable = $("#tblPagos").DataTable({
        "ajax": {
            "url": "/Empleado/EmpleadoPagos/GetAll",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "cliente", "width": "20%" },
            { "data": "vehiculo", "width": "20%" },
            {
                "data": "monto",
                "render": d => "$" + parseFloat(d).toFixed(2),
                "width": "10%"
            },
            { "data": "fechapago", "width": "15%" },
            { "data": "estado", "width": "10%" },
            {
                "data": "id",
                "width": "20%",
                "render": function (id) {
                    return `
                      <div class="d-flex justify-content-center gap-2">
                        <a href="/Empleado/EmpleadoPagos/Edit/${id}" class="btn btn-success btn-sm">
                          <i class="fas fa-edit"></i> Editar
                        </a>
                        <a onclick="Delete('/Empleado/EmpleadoPagos/Delete/${id}')" class="btn btn-danger btn-sm">
                          <i class="fas fa-trash-alt"></i> Borrar
                        </a>
                      </div>`;
                }
            }
        ],
        "language": { /* idéntico al de clientes.js */ },
        "width": "100%"
    });
}

function Delete(url) {
    swal({
        title: "¿Está seguro de borrar?",
        text: "¡Este contenido no se puede recuperar!",
        type: "warning",
        showCancelButton: true,
        confirmButtonText: "Sí, borrar!"
    }, function () {
        $.ajax({
            type: 'DELETE',
            url: url,
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    dataTable.ajax.reload();
                } else {
                    toastr.error(data.message);
                }
            },
            error: function () {
                toastr.error("Error en el servidor.");
            }
        });
    });
}
