@section Scripts {
    @{ await Html.RenderPartialAsync("_ValidationScriptsPartial"); }

    <script>
        $(document).ready(function () {
            $('#ClassId').change(function () {
                var classId = $(this).val();
                var $timeRangeSelect = $('#timeRangeSelect');
                $timeRangeSelect.empty();
                $timeRangeSelect.append($('<option>', {
                    value: '',
                    text: '-- Cargando rangos de tiempo... --'
                }));

                if (classId) {
                    $.ajax({
                        url: '@Url.Action("GetTimeRangesByClassId", "ClassReservations")',
                        data: { classId: classId },
                        success: function (data) {
                            $timeRangeSelect.empty();
                            $timeRangeSelect.append($('<option>', {
                                value: '',
                                text: '-- Seleccione un rango de tiempo --'
                            }));
                            $.each(data, function (i, item) {
                                $timeRangeSelect.append($('<option>', {
                                    value: item.id,
                                    text: item.display
                                }));
                            });
                        },
                        error: function () {
                            $timeRangeSelect.empty();
                            $timeRangeSelect.append($('<option>', {
                                value: '',
                                text: '-- Error al cargar los rangos --'
                            }));
                        }
                    });
                } else {
                    $timeRangeSelect.empty();
                    $timeRangeSelect.append($('<option>', {
                        value: '',
                        text: '-- Seleccione una clase primero --'
                    }));
                }
            });
        });
    </script>
}
