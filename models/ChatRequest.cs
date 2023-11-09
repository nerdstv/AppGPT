using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WinFormsApp3.models
{
    public class ChatRequest
    {
        public bool response_as_dict { get; set; }
        public bool attributes_as_list { get; set; }
        public bool show_original_response { get; set; }
        public int temperature { get; set; }
        public int max_tokens { get; set; }
        public string? providers { get; set; }
        public string? text { get; set; }
    }
}
