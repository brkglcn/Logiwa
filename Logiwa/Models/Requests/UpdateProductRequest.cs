﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Logiwa.Models.Requests
{
    public class UpdateProductRequest
    {
        public string id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string CategoryId { get; set; }
    }
}
