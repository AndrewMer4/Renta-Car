var dataTable;

$(function () {
    cargarDataTable();
});

function cargarDataTable() {
    dataTable = $("#tblRentas").DataTable({
        ajax: {
            url: "/Empleado/EmpleadoRentas/GetAll",
            type: "GET",
            datatype: "json"
        },
        columns: [
            {
                data: "cliente",
                width: "20%"
            },
            {
                data: "vehiculo",
                width: "20%"
            },
            {
                data: "fechainicio",
                width: "15%"
            },
            {
                data: "fechafin",
                width: "15%"
            },
            {
                data: "total",
                render: function (data) {
                    return "$" + parseFloat(data).toFixed(2);
                },
                width: "10%"
            },
            {
                data: "id",
                render: function (id) {
                    return `
                      <div class="d-flex justify-content-center gap-2">
                        <a href="/Empleado/EmpleadoRentas/Edit/${id}" class="btn btn-success btn-sm">
                          <i class="far fa-edit"></i> Editar
                        </a>
                        <button onclick="deleteRentaEmpleado('/Empleado/EmpleadoRentas/Delete/${id}')" class="btn btn-danger btn-sm">
                          <i class="far fa-trash-alt"></i> Borrar
                        </button>
                      </div>`;
                },
                width: "20%"
            }
        ],
        language: {
            emptyTable: "No hay registros",
            info: "Mostrando _START_ a _END_ de _TOTAL_ entradas",
            infoEmpty: "Mostrando 0 a 0 de 0 entradas",
            infoFiltered: "(filtrado de _MAX_ totales)",
            lengthMenu: "Mostrar _MENU_ entradas",
            loadingRecords: "Cargando...",
            processing: "Procesando...",
            search: "Buscar:",
            zeroRecords: "Sin resultados encontrados",
            paginate: {
                first: "Primero",
                last: "Último",
                next: "Siguiente",
                previous: "Anterior"
            }
        },
        width: "100%"
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
