using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Html;
using Nop.Services.Localization;
using Nop.Services.Media;

namespace Nop.Services.Vendors
{
    /// <summary>
    /// Represents a vendor attribute formatter implementation
    /// </summary>
    public partial class VendorAttributeFormatter : IVendorAttributeFormatter
    {
        #region Fields

        private readonly IHtmlFormatter _htmlFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly IVendorAttributeParser _vendorAttributeParser;
        private readonly IVendorAttributeService _vendorAttributeService;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        private readonly IDownloadService _downloadService;

        #endregion

        #region Ctor

        public VendorAttributeFormatter(IHtmlFormatter htmlFormatter,
            ILocalizationService localizationService,
            IVendorAttributeParser vendorAttributeParser,
            IWebHelper webHelper,
            IDownloadService downloadService,
            IVendorAttributeService vendorAttributeService,
            IWorkContext workContext)
        {
            _htmlFormatter = htmlFormatter;
            _localizationService = localizationService;
            _vendorAttributeParser = vendorAttributeParser;
            _webHelper = webHelper;
            _downloadService = downloadService;
            _vendorAttributeService = vendorAttributeService;
            _workContext = workContext;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Format vendor attributes
        /// </summary>
        /// <param name="attributesXml">Attributes in XML format</param>
        /// <param name="separator">Separator</param>
        /// <param name="htmlEncode">A value indicating whether to encode (HTML) values</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the formatted attributes
        /// </returns>
        public virtual async Task<string> FormatAttributesAsync(string attributesXml, string separator = "<br />", bool htmlEncode = true,
            bool allowHyperlinks = true)
        {
            var result = new StringBuilder();
            var currentLanguage = await _workContext.GetWorkingLanguageAsync();
            var attributes = await _vendorAttributeParser.ParseVendorAttributesAsync(attributesXml);
            for (var i = 0; i < attributes.Count; i++)
            {
                var attribute = attributes[i];
                var valuesStr = _vendorAttributeParser.ParseValues(attributesXml, attribute.Id);
                for (var j = 0; j < valuesStr.Count; j++)
                {
                    var valueStr = valuesStr[j];
                    var formattedAttribute = string.Empty;
                    if (!attribute.ShouldHaveValues())
                    {
                        //no values
                        if (attribute.AttributeControlType == AttributeControlType.MultilineTextbox)
                        {
                            //multiline textbox
                            var attributeName = await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id);
                            //encode (if required)
                            if (htmlEncode)
                                attributeName = WebUtility.HtmlEncode(attributeName);
                            formattedAttribute = $"{attributeName}: {_htmlFormatter.FormatText(valueStr, false, true, false, false, false, false)}";
                            //we never encode multiline textbox input
                        }
                        else if (attribute.AttributeControlType == AttributeControlType.FileUpload)
                        {
                            //file upload
                            //file upload
                            _ = Guid.TryParse(valueStr, out var downloadGuid);
                            var download = await _downloadService.GetDownloadByGuidAsync(downloadGuid);
                            if (download != null)
                            {
                                string attributeText;
                                var fileName = $"{download.Filename ?? download.DownloadGuid.ToString()}{download.Extension}";
                                //encode (if required)
                                if (htmlEncode)
                                    fileName = WebUtility.HtmlEncode(fileName);
                                if (allowHyperlinks)
                                {
                                    //hyperlinks are allowed
                                    var downloadLink = $"{_webHelper.GetStoreLocation()}download/getfileupload/?downloadId={download.DownloadGuid}";
                                    attributeText = $"<a href=\"{downloadLink}\" class=\"fileuploadattribute\">{fileName}</a>";
                                }
                                else
                                {
                                    //hyperlinks aren't allowed
                                    attributeText = fileName;
                                }

                                var attributeName = await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id);
                                //encode (if required)
                                if (htmlEncode)
                                    attributeName = WebUtility.HtmlEncode(attributeName);
                                formattedAttribute = $"{attributeName}: {attributeText}";
                            }
                        }
                        else
                        {
                            //other attributes (textbox, datepicker)
                            formattedAttribute = $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {valueStr}";
                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                        }
                    }
                    else
                    {
                        if (int.TryParse(valueStr, out var attributeValueId))
                        {
                            var attributeValue = await _vendorAttributeService.GetVendorAttributeValueByIdAsync(attributeValueId);
                            if (attributeValue != null)
                            {
                                formattedAttribute = $"{await _localizationService.GetLocalizedAsync(attribute, a => a.Name, currentLanguage.Id)}: {await _localizationService.GetLocalizedAsync(attributeValue, a => a.Name, currentLanguage.Id)}";
                            }
                            //encode (if required)
                            if (htmlEncode)
                                formattedAttribute = WebUtility.HtmlEncode(formattedAttribute);
                        }
                    }

                    if (string.IsNullOrEmpty(formattedAttribute)) 
                        continue;

                    if (i != 0 || j != 0)
                        result.Append(separator);
                    result.Append(formattedAttribute);
                }
            }

            return result.ToString();
        }

        #endregion
    }
}