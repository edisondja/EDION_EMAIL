$('#formulario').ajaxForm({


    beforeSend: function () {

    },
    uploadProgress: function (evento, pos, total, progreso) {

        console.log(total);

        $('#progress').val(progreso); 

    },
    success: function () {

        console.log("exito");

    }
    complete: () => {

       
    }

});