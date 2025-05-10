var dataTable;

$(function () {
    cargarDataTable();
});

function cargarDataTable() {
    dataTable = $("#tblVehiculos").DataTable({
        "ajax": {
            "url": "/Empleado/Vehiculos/GetAll",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "marca", "width": "10%" },
            { "data": "modelo", "width": "10%" },
            { "data": "anio", "width": "10%" },
            { "data": "kilometraje", "width": "10%" },
            { "data": "estado", "width": "10%" },
            {
                "data": "precioPorDia",
                "render": function (d) { return "$" + parseFloat(d).toFixed(2); },
                "width": "10%"
            },
            {
                "data": "urlImagen",
                "render": function (d) { return d ? `<img src="${d}" width="120"/>` : ""; },
                "width": "15%"
            }
        ],
        "language": { /* "language": {
    "emptyTable":      "No hay registros",
    "info":            "Mostrando _START_ a _END_ de _TOTAL_ entradas",
    "infoEmpty":       "Mostrando 0 a 0 de 0 entradas",
    "infoFiltered":    "(filtrado de _MAX_ totales)",
    "lengthMenu":      "Mostrar _MENU_ entradas",
    "loadingRecords":  "Cargando...",
    "processing":      "Procesando...",
    "search":          "Buscar:",
    "zeroRecords":     "Sin resultados encontrados",
    "paginate": {
        "first":    "Primero",
        "last":     "Último",
        "next":     "Siguiente",
        "previous": "Anterior"
    }
},     */ },
        "width": "100%"
    });
}

