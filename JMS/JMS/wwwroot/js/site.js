$('[data-toggle="tooltip"]').tooltip()

$('body').on('click', function (e) {
    if (!$('li.dropdown.mega-dropdown').is(e.target)
        && $('li.dropdown.mega-dropdown').has(e.target).length === 0
        && $('.open').has(e.target).length === 0
    ) {
        $('li.dropdown.mega-dropdown').removeClass('open');
    }
});
$('.phonenumber').mask('(000) 000-0000')
$('.issn').mask('0000-000x', {
    "translation": {
        0: { pattern: /[0-9]/ },
        x: { pattern: /[0-9X]/ }
    }
});

var summernotes = $(".summernote");
for (var i = 0; i < summernotes.length; i++) {
    var item = summernotes[i];
    $(item).summernote({
        toolbar: [
            ['style', ['style']],
            ['font', ['bold', 'italic', 'underline', 'clear']],
            ['fontname', ['fontname']],
            ['color', ['color']],
            ['para', ['ul', 'ol', 'paragraph']],
            ['table', ['table']],
            ['insert', ['link', 'picture', 'video']],
            ['view', ['fullscreen', 'codeview', 'help']],
            ['misc', ['clearAll']]
        ],

        buttons: {
            clearAll: function () {
                var ui = $.summernote.ui;
                // create button
                var button = ui.button({
                    contents: '<b>x</b>',
                    tooltip: 'Clear All',
                    click: function () {
                        $(this).parents(".note-editor").prev('textarea').summernote('code', '');
                        $(this).parents(".note-editor").prev('textarea').summernote({ focus: true });
                    }
                });
                return button.render();
            }
        },

    });
}
$('.datepicker').datepicker();
function ValidateDate(text) {    
    var dtRegex = new RegExp("^([0]?[1-9]|[1-2]\\d|3[0-1]) (JAN|FEB|MAR|APR|MAY|JUN|JULY|AUG|SEP|OCT|NOV|DEC) [1-2]\\d{3}$", 'i');
    return dtRegex.test(text);
}
var months=['JAN','FEB','MAR','APR','MAY','JUN','JULY','AUG','SEP','OCT','NOV','DEC'];