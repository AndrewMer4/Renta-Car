var dataTable;

$(function () {
    cargarDataTable();
});

function cargarDataTable() {
    dataTable = $("#tblClientes").DataTable({
        "ajax": {
            "url": "/Empleado/EmpleadoClientes/GetAll",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "nombres", "width": "15%" },
            { "data": "apellidos", "width": "15%" },
            { "data": "dui", "width": "15%" },
            { "data": "telefono", "width": "15%" },
            { "data": "edad", "width": "10%" },
            { "data": "direccion", "width": "20%" },
            {
                "data": "id",
                "width": "20%",
                "render": function (id) {
                    return `
                      <div class="d-flex justify-content-center gap-2">
                        <a href="/Empleado/EmpleadoClientes/Edit/${id}" class="btn btn-success btn-sm">
                          <i class="far fa-edit"></i> Editar
                        </a>
                        <a onclick="Delete('/Empleado/EmpleadoClientes/Delete/${id}')" class="btn btn-danger btn-sm">
                          <i class="far fa-trash-alt"></i> Borrar
                        </a>
                      </div>`;
                }
            }
        ],
        "language": {
            "emptyTable": "No hay registros",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ entradas",
            "infoEmpty": "Mostrando 0 a 0 de 0 entradas",
            "infoFiltered": "(filtrado de _MAX_ totales)",
            "lengthMenu": "Mostrar _MENU_ entradas",
            "loadingRecords": "Cargando...",
            "processing": "Procesando...",
            "search": "Buscar:",
            "zeroRecords": "Sin resultados encontrados",
            "paginate": {
                "first": "Primero",
                "last": "Último",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
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
