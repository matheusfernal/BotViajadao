﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BotViajadao.Model
{
    public class Localizacao
    {
        [JsonProperty("display_address")]
        public string[] DisplayAdress { get; set; }
    }
}