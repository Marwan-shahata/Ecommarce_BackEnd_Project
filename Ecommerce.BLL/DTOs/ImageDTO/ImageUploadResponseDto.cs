using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.BLL
{
    public class ImageUploadResponseDto
    {
        public string FileName { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public string ContentType { get; set; } = string.Empty;
    }
}
