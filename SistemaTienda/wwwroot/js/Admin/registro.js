// wwwroot/js/Admin/registro.js
var dataTable;

$(function () {
    cargarDataTable();
});

function cargarDataTable() {
    dataTable = $("#tblRegistro").DataTable({
        ajax: {
            url: "/Admin/Registro/GetAll",
            type: "GET",
            dataType: "json"
        },
        columns: [
            { data: "nombre", width: "20%" },
            { data: "apellido", width: "20%" },
            { data: "email", width: "25%" },
            { data: "role", width: "15%" },
            {
                data: "id",
                width: "20%",
                orderable: false,
                render: function (id) {
                    return `
                      <div class="d-flex justify-content-center gap-2">
                        <a href="/Admin/Registro/Edit/${id}" class="btn btn-success btn-sm">
                          <i class="far fa-edit"></i> Editar
                        </a>
                        <button onclick="eliminarUsuario('${id}')" class="btn btn-danger btn-sm">
                          <i class="far fa-trash-alt"></i> Eliminar
                        </button>
                      </div>`;
                }
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

function eliminarUsuario(id) {
    swal({
        title: "¿Está seguro?",
        text: "¡No podrás deshacer esta acción!",
        icon: "warning",
        buttons: ["Cancelar", "Sí, borrar"],
        dangerMode: true
    })
        .then((willDelete) => {
            if (!willDelete) return;
            fetch(`/Admin/Registro/Delete/${id}`, {
                method: 'DELETE',
                headers: { 'Content-Type': 'application/json' }
            })
                .then(res => res.json())
                .then(data => {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                })
                .catch(() => {
                    toastr.error("Error en el servidor.");
                });
        });
}
