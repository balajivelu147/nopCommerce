namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents an attribute control type
    /// </summary>
    public enum AttributeControlType
    {
        /// <summary>
        /// Dropdown list
        /// </summary>
        DropdownList = 1,

        /// <summary>
        /// Radio list
        /// </summary>
        RadioList = 2,

        /// <summary>
        /// Checkboxes
        /// </summary>
        Checkboxes = 3,

        /// <summary>
        /// TextBox
        /// </summary>
        TextBox = 4,

        /// <summary>
        /// Multiline textbox
        /// </summary>
        MultilineTextbox = 10,

        /// <summary>
        /// Datepicker
        /// </summary>
        Datepicker = 20,

        /// <summary>
        /// DatepickerCalendar
        /// </summary>
        DatepickerCalendar = 101,

        /// <summary>
        /// TimepickerCalendar
        /// </summary>
        TimepickerCalendar = 102,

        /// <summary>
        /// MultiOptionSelect
        /// </summary>
        MultiOptionSelectInputBox = 103,


        /// <summary>
        /// MultiOptionSelect
        /// </summary>
        MultiDatepickerCalendar = 104,

        /// <summary>
        /// File upload control
        /// </summary>
        FileUpload = 30,

        /// <summary>
        /// Color squares
        /// </summary>
        ColorSquares = 40,

        /// <summary>
        /// Image squares
        /// </summary>
        ImageSquares = 45,

        /// <summary>
        /// Read-only checkboxes
        /// </summary>
        ReadonlyCheckboxes = 50
    }
}