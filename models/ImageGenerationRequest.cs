using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp3.models
{
    public class ImageGenerationRequest
    {
        public bool response_as_dict { get; set; }
        public bool attributes_as_list { get; set; }
        public bool show_original_response { get; set; }
        public string? resolution { get; set; }
        public int num_images { get; set; }
        public string? providers { get; set; }
        public string? text { get; set; }

    }
}
