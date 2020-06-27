var MyselectField = function (config) {
    jsGrid.Field.call(this, config);
};
MyselectField.prototype = new jsGrid.Field({

    css: "date-field",            // redefine general property 'css'
    align: "center",              // redefine general property 'align'

    myCustomProperty: "foo",      // custom property

    filterTemplate: function () {
        var select = $(`<select id=${this.id} multiple></select >`)
        for (var key in this.items) {
            select.append(`<option value="${key}">${this.items[key]}</option>`)
        }
        return select;
    },
    filterValue: function () {
        var grid = this._grid
        if (!$("#" + this.id).hasClass("select2-hidden-accessible")) {
            $("#" + this.id).select2();
            $("#" + this.id).on("change", function (e) { grid.search() });
        }
        return $("#" + this.id).val().join();
    }
});

jsGrid.fields.multiselect = MyselectField;

